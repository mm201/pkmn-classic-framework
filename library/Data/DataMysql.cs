using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using PokeFoundations.Structures;
using PokeFoundations.Support;

namespace PokeFoundations.Data
{
    public class DataMysql : DataAbstract
    {
        #region Initialization
        private MySqlConnection CreateConnection()
        {
            // hard-coded for now, will need to add configuration later
            return new MySqlConnection("Server=10.211.55.2;Database=gts;User ID=gts;Password=gts;Pooling=true;");
        }
        #endregion

        #region Utility
        public static String SqlSanitize(String s)
        {
            return SqlSanitize(s, "");
        }

        public static String SqlSanitize(String s, String newChar)
        {
            String result = s.Replace("\'", newChar).Replace("[", newChar).Replace("]", newChar).Replace("`", newChar);
            int x = result.IndexOf("--");
            if (x != -1) result = result.Substring(0, x);
            return result;
        }
        #endregion

        #region GTS
        public GtsDatagram4 GtsDataForUser4(MySqlTransaction tran, int pid)
        {
            MySqlDataReader reader = (MySqlDataReader)tran.ExecuteReader("SELECT Data, Species, Gender, Level, " +
                "RequestedSpecies, RequestedGender, RequestedMinLevel, RequestedMaxLevel, " +
                "Unknown1, TrainerGender, Unknown2, TimeDeposited, TimeWithdrawn, pid, " +
                "TrainerName, TrainerOT, TrainerCountry, TrainerRegion, TrainerClass, " +
                "IsExchanged, TrainerVersion, TrainerLanguage FROM GtsPokemon4 WHERE pid = @pid",
                new MySqlParameter("@pid", pid));

            if (!reader.Read())
            {
                reader.Close();
                return null;
            }
            GtsDatagram4 result = Datagram4FromReader(reader);
#if DEBUG
            AssertHelper.Equals(result.PID, pid);
#endif
            reader.Close();
            return result;
        }

        public override GtsDatagram4 GtsDataForUser4(int pid)
        {
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                MySqlTransaction tran = db.BeginTransaction();
                GtsDatagram4 result = GtsDataForUser4(tran, pid);
                tran.Commit();
                return result;
            }
        }

        public bool GtsDepositPokemon4(MySqlTransaction tran, GtsDatagram4 datagram)
        {
            if (datagram.Data.Length != 236) throw new FormatException("pkm data must be 236 bytes.");
            if (datagram.TrainerName.Length != 16) throw new FormatException("Trainer name must be 16 bytes.");
            // note that IsTraded being true in the datagram is not an error condition
            // since it might have use later on. You should check for this in the upload handler.

            long count = (long)tran.ExecuteScalar("SELECT Count(*) FROM GtsPokemon4 WHERE pid = @pid",
                new MySqlParameter("@pid", datagram.PID));

            if (count > 0)
            {
                // This player already has a pokemon in the system.
                // we can possibly allow multiples under some future conditions
                return false;
            }

            tran.ExecuteNonQuery("INSERT INTO GtsPokemon4 " +
                "(Data, Species, Gender, Level, RequestedSpecies, RequestedGender, " +
                "RequestedMinLevel, RequestedMaxLevel, Unknown1, TrainerGender, " +
                "Unknown2, TimeDeposited, TimeWithdrawn, pid, TrainerName, TrainerOT, " +
                "TrainerCountry, TrainerRegion, TrainerClass, IsExchanged, TrainerVersion, " +
                "TrainerLanguage) " +
                "VALUES (@Data, @Species, @Gender, @Level, @RequestedSpecies, " +
                "@RequestedGender, @RequestedMinLevel, @RequestedMaxLevel, @Unknown1, " +
                "@TrainerGender, @Unknown2, @TimeDeposited, @TimeWithdrawn, @pid, " +
                "@TrainerName, @TrainerOT, @TrainerCountry, @TrainerRegion, @TrainerClass, " +
                "@IsExchanged, @TrainerVersion, @TrainerLanguage)",
                ParamsFromDatagram4(datagram));

            return true;
        }

        public override bool GtsDepositPokemon4(GtsDatagram4 datagram)
        {
            if (datagram.Data.Length != 236) throw new FormatException("pkm data must be 236 bytes.");
            if (datagram.TrainerName.Length != 16) throw new FormatException("Trainer name must be 16 bytes.");
            // note that IsTraded being true in the datagram is not an error condition
            // since it might have use later on. You should check for this in the upload handler.

            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                MySqlTransaction tran = db.BeginTransaction();

                if (!GtsDepositPokemon4(tran, datagram))
                {
                    tran.Rollback();
                    return false;
                }

                tran.Commit();
                return true;
            }
        }

        public int GtsGetDepositId(int pid, MySqlTransaction tran)
        {
            object o = tran.ExecuteScalar("SELECT Top(1) id FROM GtsPokemon4 WHERE pid = @pid " +
                "ORDER BY IsExchanged DESC, TimeWithdrawn, TimeDeposited",
                new MySqlParameter("@pid", pid));
            if (o == null || o == DBNull.Value) return 0;
            return (int)o;
        }

        public bool GtsDeletePokemon4(MySqlTransaction tran, int pid)
        {
            int pkmnId = GtsGetDepositId(pid, tran);
            if (pkmnId == 0) return false;

            tran.ExecuteNonQuery("DELETE FROM GtsPokemon4 WHERE id = @id",
                new MySqlParameter("@id", pkmnId));
            return true;
        }

        public override bool GtsDeletePokemon4(int pid)
        {
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                MySqlTransaction tran = db.BeginTransaction();

                if (!GtsDeletePokemon4(tran, pid))
                {
                    tran.Rollback();
                    return false;
                }

                tran.Commit();
                return true;
            }
        }

        public override bool GtsTradePokemon4(int pidSrc, int pidDest)
        {
            // not needed yet.
            return false;
        }

        public override bool GtsTradePokemon4(GtsDatagram4 upload, GtsDatagram4 result)
        {
            GtsDatagram4 traded = upload.Clone();
            traded.FlagTraded(result);

            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                MySqlTransaction tran = db.BeginTransaction();

                GtsDatagram4 resultOrig = GtsDataForUser4(tran, result.PID);
                if (resultOrig == null || resultOrig != result)
                {
                    // looks like the pokemon was ninja'd between the Exchange and Exchange_finish
                    tran.Rollback();
                    return false;
                }

                if (!GtsDeletePokemon4(tran, result.PID))
                {
                    tran.Rollback();
                    return false;
                }

                if (!GtsDepositPokemon4(tran, traded))
                {
                    tran.Rollback();
                    return false;
                }

                tran.Commit();
                return true;
            }
        }

        public override GtsDatagram4[] GtsSearch4(ushort species, Genders gender, byte minLevel, byte maxLevel, byte country, int count)
        {
            using (MySqlConnection db = CreateConnection())
            {
                List<MySqlParameter> _params = new List<MySqlParameter>();
                String where = "WHERE Species = @species";
                _params.Add(new MySqlParameter("@species", species));

                if (gender != Genders.Either)
                {
                    where += " AND Gender = @gender";
                    _params.Add(new MySqlParameter("@gender", (byte)gender));
                }

                if (minLevel > 0 && maxLevel > 0)
                {
                    where += " AND Level BETWEEN @min_level AND @max_level";
                    _params.Add(new MySqlParameter("@min_level", minLevel));
                    _params.Add(new MySqlParameter("@max_level", maxLevel));
                }
                else if (minLevel > 0)
                {
                    where += " AND Level >= @min_level";
                    _params.Add(new MySqlParameter("@min_level", minLevel));
                }
                else if (maxLevel > 0)
                {
                    where += " AND Level <= @max_level";
                    _params.Add(new MySqlParameter("@max_level", maxLevel));
                }

                if (country > 0)
                {
                    where += " AND TrainerCountry = @country";
                    _params.Add(new MySqlParameter("@country", country));
                }

                _params.Add(new MySqlParameter("@count", count));

                db.Open();
                // todo: sort me in creative ways
                MySqlDataReader reader = (MySqlDataReader)db.ExecuteReader("SELECT Data, Species, Gender, Level, " +
                    "RequestedSpecies, RequestedGender, RequestedMinLevel, RequestedMaxLevel, " +
                    "Unknown1, TrainerGender, Unknown2, TimeDeposited, TimeWithdrawn, pid, " +
                    "TrainerName, TrainerOT, TrainerCountry, TrainerRegion, TrainerClass, " +
                    "IsExchanged, TrainerVersion, TrainerLanguage FROM GtsPokemon4 " + where +
                    " ORDER BY TimeDeposited DESC LIMIT @count",
                    _params.ToArray());

                List<GtsDatagram4> datagrams = new List<GtsDatagram4>(count);

                while (reader.Read())
                {
                    datagrams.Add(Datagram4FromReader(reader));
                }

                return datagrams.ToArray();
            }
        }

        private static GtsDatagram4 Datagram4FromReader(MySqlDataReader reader)
        {
            GtsDatagram4 result = new GtsDatagram4();

            byte[] data = new byte[236];
            reader.GetBytes(0, 0, data, 0, 236);
            result.Data = data;
            data = null;

            result.Species = reader.GetUInt16(1);
            result.Gender = (Genders)reader.GetByte(2);
            result.Level = reader.GetByte(3);
            result.RequestedSpecies = reader.GetUInt16(4);
            result.RequestedGender = (Genders)reader.GetByte(5);
            result.RequestedMinLevel = reader.GetByte(6);
            result.RequestedMaxLevel = reader.GetByte(7);
            result.Unknown1 = reader.GetByte(8);
            result.TrainerGender = (GtsTrainerGenders)reader.GetByte(9);
            result.Unknown2 = reader.GetByte(10);
            result.TimeDeposited = reader.GetDateTime(11);
            result.TimeWithdrawn = reader.GetDateTime(12);
            result.PID = reader.GetInt32(13);

            data = new byte[16];
            reader.GetBytes(14, 0, data, 0, 16);
            result.TrainerName = data;
            data = null;

            result.TrainerOT = reader.GetUInt16(15);
            result.TrainerCountry = reader.GetByte(16);
            result.TrainerRegion = reader.GetByte(17);
            result.TrainerClass = reader.GetByte(18);
            result.IsExchanged = reader.GetByte(19);
            result.TrainerVersion = reader.GetByte(20);
            result.TrainerLanguage = reader.GetByte(21);

            return result;
        }

        private static MySqlParameter[] ParamsFromDatagram4(GtsDatagram4 datagram)
        {
            MySqlParameter[] result = new MySqlParameter[22];

            result[0] = new MySqlParameter("@Data", datagram.Data);
            result[1] = new MySqlParameter("@Species", datagram.Species);
            result[2] = new MySqlParameter("@Gender", (byte)datagram.Gender);
            result[3] = new MySqlParameter("@Level", datagram.Level);
            result[4] = new MySqlParameter("@RequestedSpecies", datagram.RequestedSpecies);
            result[5] = new MySqlParameter("@RequestedGender", (byte)datagram.RequestedGender);
            result[6] = new MySqlParameter("@RequestedMinLevel", datagram.RequestedMinLevel);
            result[7] = new MySqlParameter("@RequestedMaxLevel", datagram.RequestedMaxLevel);
            result[8] = new MySqlParameter("@Unknown1", datagram.Unknown1);
            result[9] = new MySqlParameter("@TrainerGender", (byte)datagram.TrainerGender);
            result[10] = new MySqlParameter("@Unknown2", datagram.Unknown2);
            result[11] = new MySqlParameter("@TimeDeposited", datagram.TimeDeposited);
            result[12] = new MySqlParameter("@TimeWithdrawn", datagram.TimeWithdrawn);
            result[13] = new MySqlParameter("@pid", datagram.PID);
            result[14] = new MySqlParameter("@TrainerName", datagram.TrainerName);
            result[15] = new MySqlParameter("@TrainerOT", datagram.TrainerOT);
            result[16] = new MySqlParameter("@TrainerCountry", datagram.TrainerCountry);
            result[17] = new MySqlParameter("@TrainerRegion", datagram.TrainerRegion);
            result[18] = new MySqlParameter("@TrainerClass", datagram.TrainerClass);
            result[19] = new MySqlParameter("@IsExchanged", datagram.IsExchanged);
            result[20] = new MySqlParameter("@TrainerVersion", datagram.TrainerVersion);
            result[21] = new MySqlParameter("@TrainerLanguage", datagram.TrainerLanguage);

            return result;
        }

        #endregion
    }
}
