using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using PkmnFoundations.Structures;
using PkmnFoundations.Support;
using System.Security.Cryptography;

namespace PkmnFoundations.Data
{
    public class DataMysql : DataAbstract
    {
        #region Initialization
        public DataMysql(String connString)
        {
            ConnectionString = connString;
        }

        public String ConnectionString { get; set; }

        private MySqlConnection CreateConnection()
        {
            return new MySqlConnection(ConnectionString);
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

        #region GTS 4
        public GtsRecord4 GtsDataForUser4(MySqlTransaction tran, int pid)
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
            GtsRecord4 result = Record4FromReader(reader);
#if DEBUG
            AssertHelper.Equals(result.PID, pid);
#endif
            reader.Close();
            return result;
        }

        public override GtsRecord4 GtsDataForUser4(int pid)
        {
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                using (MySqlTransaction tran = db.BeginTransaction())
                {
                    GtsRecord4 result = GtsDataForUser4(tran, pid);
                    tran.Commit();
                    return result;
                }
            }
        }

        public bool GtsDepositPokemon4(MySqlTransaction tran, GtsRecord4 record)
        {
            if (record.Data.Length != 236) throw new FormatException("pkm data must be 236 bytes.");
            if (record.TrainerName.RawData.Length != 16) throw new FormatException("Trainer name must be 16 bytes.");
            // note that IsTraded being true in the record is not an error condition
            // since it might have use later on. You should check for this in the upload handler.

            long count = (long)tran.ExecuteScalar("SELECT Count(*) FROM GtsPokemon4 WHERE pid = @pid",
                new MySqlParameter("@pid", record.PID));

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
                ParamsFromRecord4(record));

            return true;
        }

        public override bool GtsDepositPokemon4(GtsRecord4 record)
        {
            if (record.Data.Length != 236) throw new FormatException("pkm data must be 236 bytes.");
            if (record.TrainerName.RawData.Length != 16) throw new FormatException("Trainer name must be 16 bytes.");
            // note that IsTraded being true in the record is not an error condition
            // since it might have use later on. You should check for this in the upload handler.

            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                using (MySqlTransaction tran = db.BeginTransaction())
                {
                    if (!GtsDepositPokemon4(tran, record))
                    {
                        tran.Rollback();
                        return false;
                    }

                    tran.Commit();
                    return true;
                }
            }
        }

        public int GtsGetDepositId4(int pid, MySqlTransaction tran)
        {
            object o = tran.ExecuteScalar("SELECT id FROM GtsPokemon4 WHERE pid = @pid " +
                "ORDER BY IsExchanged DESC, TimeWithdrawn, TimeDeposited LIMIT 1",
                new MySqlParameter("@pid", pid));
            if (o == null || o == DBNull.Value) return 0;
            return (int)((uint)o);
        }

        public bool GtsDeletePokemon4(MySqlTransaction tran, int pid)
        {
            int pkmnId = GtsGetDepositId4(pid, tran);
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
                using (MySqlTransaction tran = db.BeginTransaction())
                {
                    if (!GtsDeletePokemon4(tran, pid))
                    {
                        tran.Rollback();
                        return false;
                    }

                    tran.Commit();
                    return true;
                }
            }
        }

        public override bool GtsTradePokemon4(int pidSrc, int pidDest)
        {
            // not needed yet.
            return false;
        }

        public override bool GtsTradePokemon4(GtsRecord4 upload, GtsRecord4 result)
        {
            GtsRecord4 traded = upload.Clone();
            traded.FlagTraded(result);

            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                using (MySqlTransaction tran = db.BeginTransaction())
                {
                    GtsRecord4 resultOrig = GtsDataForUser4(tran, result.PID);
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
        }

        public override GtsRecord4[] GtsSearch4(int pid, ushort species, Genders gender, byte minLevel, byte maxLevel, byte country, int count)
        {
            using (MySqlConnection db = CreateConnection())
            {
                List<MySqlParameter> _params = new List<MySqlParameter>();
                String where = "WHERE pid != @pid AND IsExchanged = 0";
                _params.Add(new MySqlParameter("@pid", pid));

                if (species > 0)
                {
                    where += " AND Species = @species";
                    _params.Add(new MySqlParameter("@species", species));
                }

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

                String limit = "";
                if (count > 0)
                {
                    _params.Add(new MySqlParameter("@count", count));
                    limit = " LIMIT @count";
                }

                db.Open();
                // todo: sort me in creative ways
                MySqlDataReader reader = (MySqlDataReader)db.ExecuteReader("SELECT Data, Species, Gender, Level, " +
                    "RequestedSpecies, RequestedGender, RequestedMinLevel, RequestedMaxLevel, " +
                    "Unknown1, TrainerGender, Unknown2, TimeDeposited, TimeWithdrawn, pid, " +
                    "TrainerName, TrainerOT, TrainerCountry, TrainerRegion, TrainerClass, " +
                    "IsExchanged, TrainerVersion, TrainerLanguage FROM GtsPokemon4 " + where +
                    " ORDER BY TimeDeposited DESC" + limit,
                    _params.ToArray());

                List<GtsRecord4> records;
                if (count > 0) records = new List<GtsRecord4>(count);
                else records = new List<GtsRecord4>();

                while (reader.Read())
                {
                    records.Add(Record4FromReader(reader));
                }

                return records.ToArray();
            }
        }

        private static GtsRecord4 Record4FromReader(MySqlDataReader reader)
        {
            GtsRecord4 result = new GtsRecord4();

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
            if (reader.IsDBNull(11)) result.TimeDeposited = null;
            else result.TimeDeposited = reader.GetDateTime(11);
            if (reader.IsDBNull(12)) result.TimeWithdrawn = null;
            else result.TimeWithdrawn = reader.GetDateTime(12);
            result.PID = reader.GetInt32(13);

            data = new byte[16];
            reader.GetBytes(14, 0, data, 0, 16);
            result.TrainerName = new EncodedString4(data);
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

        private static MySqlParameter[] ParamsFromRecord4(GtsRecord4 record)
        {
            MySqlParameter[] result = new MySqlParameter[22];

            result[0] = new MySqlParameter("@Data", record.Data);
            result[1] = new MySqlParameter("@Species", record.Species);
            result[2] = new MySqlParameter("@Gender", (byte)record.Gender);
            result[3] = new MySqlParameter("@Level", record.Level);
            result[4] = new MySqlParameter("@RequestedSpecies", record.RequestedSpecies);
            result[5] = new MySqlParameter("@RequestedGender", (byte)record.RequestedGender);
            result[6] = new MySqlParameter("@RequestedMinLevel", record.RequestedMinLevel);
            result[7] = new MySqlParameter("@RequestedMaxLevel", record.RequestedMaxLevel);
            result[8] = new MySqlParameter("@Unknown1", record.Unknown1);
            result[9] = new MySqlParameter("@TrainerGender", (byte)record.TrainerGender);
            result[10] = new MySqlParameter("@Unknown2", record.Unknown2);
            result[11] = new MySqlParameter("@TimeDeposited", record.TimeDeposited);
            result[12] = new MySqlParameter("@TimeWithdrawn", record.TimeWithdrawn);
            result[13] = new MySqlParameter("@pid", record.PID);
            result[14] = new MySqlParameter("@TrainerName", record.TrainerName.RawData);
            result[15] = new MySqlParameter("@TrainerOT", record.TrainerOT);
            result[16] = new MySqlParameter("@TrainerCountry", record.TrainerCountry);
            result[17] = new MySqlParameter("@TrainerRegion", record.TrainerRegion);
            result[18] = new MySqlParameter("@TrainerClass", record.TrainerClass);
            result[19] = new MySqlParameter("@IsExchanged", record.IsExchanged);
            result[20] = new MySqlParameter("@TrainerVersion", record.TrainerVersion);
            result[21] = new MySqlParameter("@TrainerLanguage", record.TrainerLanguage);

            return result;
        }

        public override int GtsAvailablePokemon4()
        {
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                return (int)(long)db.ExecuteScalar("SELECT Count(*) FROM GtsPokemon4 WHERE IsExchanged = 0");
            }
        }
        #endregion

        #region GTS 5
        public GtsRecord5 GtsDataForUser5(MySqlTransaction tran, int pid)
        {
            MySqlDataReader reader = (MySqlDataReader)tran.ExecuteReader("SELECT Data, Unknown0, " +
                "Species, Gender, Level, " +
                "RequestedSpecies, RequestedGender, RequestedMinLevel, RequestedMaxLevel, " +
                "Unknown1, TrainerGender, Unknown2, TimeDeposited, TimeWithdrawn, pid, " +
                "TrainerOT, TrainerName, TrainerCountry, TrainerRegion, TrainerClass, " +
                "IsExchanged, TrainerVersion, TrainerLanguage, TrainerBadges, TrainerUnityTower " +
                "FROM GtsPokemon5 WHERE pid = @pid",
                new MySqlParameter("@pid", pid));

            if (!reader.Read())
            {
                reader.Close();
                return null;
            }
            GtsRecord5 result = Record5FromReader(reader);
#if DEBUG
            AssertHelper.Equals(result.PID, pid);
#endif
            reader.Close();
            return result;
        }

        public override GtsRecord5 GtsDataForUser5(int pid)
        {
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                using (MySqlTransaction tran = db.BeginTransaction())
                {
                    GtsRecord5 result = GtsDataForUser5(tran, pid);
                    tran.Commit();
                    return result;
                }
            }
        }

        public bool GtsDepositPokemon5(MySqlTransaction tran, GtsRecord5 record)
        {
            if (record.Data.Length != 220) throw new FormatException("pkm data must be 220 bytes.");
            if (record.Unknown0.Length != 16) throw new FormatException("pkm padding must be 16 bytes.");
            if (record.TrainerName.RawData.Length != 16) throw new FormatException("Trainer name must be 16 bytes.");
            // note that IsTraded being true in the record is not an error condition
            // since it might have use later on. You should check for this in the upload handler.

            long count = (long)tran.ExecuteScalar("SELECT Count(*) FROM GtsPokemon5 WHERE pid = @pid",
                new MySqlParameter("@pid", record.PID));

            if (count > 0)
            {
                // This player already has a pokemon in the system.
                // we can possibly allow multiples under some future conditions
                return false;
            }

            tran.ExecuteNonQuery("INSERT INTO GtsPokemon5 " +
                "(Data, Unknown0, Species, Gender, Level, RequestedSpecies, RequestedGender, " +
                "RequestedMinLevel, RequestedMaxLevel, Unknown1, TrainerGender, " +
                "Unknown2, TimeDeposited, TimeWithdrawn, pid, TrainerOT, TrainerName, " +
                "TrainerCountry, TrainerRegion, TrainerClass, IsExchanged, TrainerVersion, " +
                "TrainerLanguage, TrainerBadges, TrainerUnityTower) " +
                "VALUES (@Data, @Unknown0, @Species, @Gender, @Level, @RequestedSpecies, " +
                "@RequestedGender, @RequestedMinLevel, @RequestedMaxLevel, @Unknown1, " +
                "@TrainerGender, @Unknown2, @TimeDeposited, @TimeWithdrawn, @pid, " +
                "@TrainerOT, @TrainerName, @TrainerCountry, @TrainerRegion, @TrainerClass, " +
                "@IsExchanged, @TrainerVersion, @TrainerLanguage, @TrainerBadges, @TrainerUnityTower)",
                ParamsFromRecord5(record));

            return true;
        }

        public override bool GtsDepositPokemon5(GtsRecord5 record)
        {
            if (record.Data.Length != 220) throw new FormatException("pkm data must be 220 bytes.");
            if (record.Unknown0.Length != 16) throw new FormatException("pkm padding must be 16 bytes.");
            if (record.TrainerName.RawData.Length != 16) throw new FormatException("Trainer name must be 16 bytes.");
            // note that IsTraded being true in the record is not an error condition
            // since it might have use later on. You should check for this in the upload handler.

            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                using (MySqlTransaction tran = db.BeginTransaction())
                {
                    if (!GtsDepositPokemon5(tran, record))
                    {
                        tran.Rollback();
                        return false;
                    }

                    tran.Commit();
                    return true;
                }
            }
        }

        public int GtsGetDepositId5(int pid, MySqlTransaction tran)
        {
            object o = tran.ExecuteScalar("SELECT id FROM GtsPokemon5 WHERE pid = @pid " +
                "ORDER BY IsExchanged DESC, TimeWithdrawn, TimeDeposited LIMIT 1",
                new MySqlParameter("@pid", pid));
            if (o == null || o == DBNull.Value) return 0;
            return (int)((uint)o);
        }

        public bool GtsDeletePokemon5(MySqlTransaction tran, int pid)
        {
            int pkmnId = GtsGetDepositId5(pid, tran);
            if (pkmnId == 0) return false;

            tran.ExecuteNonQuery("DELETE FROM GtsPokemon5 WHERE id = @id",
                new MySqlParameter("@id", pkmnId));
            return true;
        }

        public override bool GtsDeletePokemon5(int pid)
        {
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                using (MySqlTransaction tran = db.BeginTransaction())
                {
                    if (!GtsDeletePokemon5(tran, pid))
                    {
                        tran.Rollback();
                        return false;
                    }

                    tran.Commit();
                    return true;
                }
            }
        }

        public override bool GtsTradePokemon5(int pidSrc, int pidDest)
        {
            // not needed yet.
            return false;
        }

        public override bool GtsTradePokemon5(GtsRecord5 upload, GtsRecord5 result)
        {
            GtsRecord5 traded = upload.Clone();
            traded.FlagTraded(result);

            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                using (MySqlTransaction tran = db.BeginTransaction())
                {
                    GtsRecord5 resultOrig = GtsDataForUser5(tran, result.PID);
                    if (resultOrig == null || resultOrig != result)
                    {
                        // looks like the pokemon was ninja'd between the Exchange and Exchange_finish
                        tran.Rollback();
                        return false;
                    }

                    if (!GtsDeletePokemon5(tran, result.PID))
                    {
                        tran.Rollback();
                        return false;
                    }

                    if (!GtsDepositPokemon5(tran, traded))
                    {
                        tran.Rollback();
                        return false;
                    }

                    tran.Commit();
                    return true;
                }
            }
        }

        public override GtsRecord5[] GtsSearch5(int pid, ushort species, Genders gender, byte minLevel, byte maxLevel, byte country, int count)
        {
            using (MySqlConnection db = CreateConnection())
            {
                List<MySqlParameter> _params = new List<MySqlParameter>();
                String where = "WHERE pid != @pid AND IsExchanged = 0";
                _params.Add(new MySqlParameter("@pid", pid));

                if (species > 0)
                {
                    where += " AND Species = @species";
                    _params.Add(new MySqlParameter("@species", species));
                }

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

                String limit = "";
                if (count > 0)
                {
                    _params.Add(new MySqlParameter("@count", count));
                    limit = " LIMIT @count";
                }

                db.Open();
                // todo: sort me in creative ways
                MySqlDataReader reader = (MySqlDataReader)db.ExecuteReader("SELECT Data, Unknown0, " +
                    "Species, Gender, Level, " +
                    "RequestedSpecies, RequestedGender, RequestedMinLevel, RequestedMaxLevel, " +
                    "Unknown1, TrainerGender, Unknown2, TimeDeposited, TimeWithdrawn, pid, " +
                    "TrainerOT, TrainerName, TrainerCountry, TrainerRegion, TrainerClass, " +
                    "IsExchanged, TrainerVersion, TrainerLanguage, TrainerBadges, TrainerUnityTower " +
                    "FROM GtsPokemon5 " + where +
                    " ORDER BY TimeDeposited DESC" + limit,
                    _params.ToArray());

                List<GtsRecord5> records;
                if (count > 0) records = new List<GtsRecord5>(count);
                else records = new List<GtsRecord5>();

                while (reader.Read())
                {
                    records.Add(Record5FromReader(reader));
                }

                return records.ToArray();
            }
        }

        private static GtsRecord5 Record5FromReader(MySqlDataReader reader)
        {
            GtsRecord5 result = new GtsRecord5();

            byte[] data = new byte[220];
            reader.GetBytes(0, 0, data, 0, 220);
            result.Data = data;
            data = null;

            data = new byte[16];
            reader.GetBytes(1, 0, data, 0, 16);
            result.Unknown0 = data;
            data = null;

            result.Species = reader.GetUInt16(2);
            result.Gender = (Genders)reader.GetByte(3);
            result.Level = reader.GetByte(4);
            result.RequestedSpecies = reader.GetUInt16(5);
            result.RequestedGender = (Genders)reader.GetByte(6);
            result.RequestedMinLevel = reader.GetByte(7);
            result.RequestedMaxLevel = reader.GetByte(8);
            result.Unknown1 = reader.GetByte(9);
            result.TrainerGender = (GtsTrainerGenders)reader.GetByte(10);
            result.Unknown2 = reader.GetByte(11);
            if (reader.IsDBNull(12)) result.TimeDeposited = null;
            else result.TimeDeposited = reader.GetDateTime(12);
            if (reader.IsDBNull(13)) result.TimeWithdrawn = null;
            else result.TimeWithdrawn = reader.GetDateTime(13);
            result.PID = reader.GetInt32(14);
            result.TrainerOT = reader.GetUInt32(15);

            data = new byte[16];
            reader.GetBytes(16, 0, data, 0, 16);
            result.TrainerName = new EncodedString5(data);
            data = null;

            result.TrainerCountry = reader.GetByte(17);
            result.TrainerRegion = reader.GetByte(18);
            result.TrainerClass = reader.GetByte(19);
            result.IsExchanged = reader.GetByte(20);
            result.TrainerVersion = reader.GetByte(21);
            result.TrainerLanguage = reader.GetByte(22);
            result.TrainerBadges = reader.GetByte(23);
            result.TrainerUnityTower = reader.GetByte(24);

            return result;
        }

        private static MySqlParameter[] ParamsFromRecord5(GtsRecord5 record)
        {
            MySqlParameter[] result = new MySqlParameter[25];

            result[0] = new MySqlParameter("@Data", record.Data);
            result[1] = new MySqlParameter("@Unknown0", record.Unknown0);
            result[2] = new MySqlParameter("@Species", record.Species);
            result[3] = new MySqlParameter("@Gender", (byte)record.Gender);
            result[4] = new MySqlParameter("@Level", record.Level);
            result[5] = new MySqlParameter("@RequestedSpecies", record.RequestedSpecies);
            result[6] = new MySqlParameter("@RequestedGender", (byte)record.RequestedGender);
            result[7] = new MySqlParameter("@RequestedMinLevel", record.RequestedMinLevel);
            result[8] = new MySqlParameter("@RequestedMaxLevel", record.RequestedMaxLevel);
            result[9] = new MySqlParameter("@Unknown1", record.Unknown1);
            result[10] = new MySqlParameter("@TrainerGender", (byte)record.TrainerGender);
            result[11] = new MySqlParameter("@Unknown2", record.Unknown2);
            result[12] = new MySqlParameter("@TimeDeposited", record.TimeDeposited);
            result[13] = new MySqlParameter("@TimeWithdrawn", record.TimeWithdrawn);
            result[14] = new MySqlParameter("@pid", record.PID);
            result[15] = new MySqlParameter("@TrainerOT", record.TrainerOT);
            result[16] = new MySqlParameter("@TrainerName", record.TrainerName.RawData);
            result[17] = new MySqlParameter("@TrainerCountry", record.TrainerCountry);
            result[18] = new MySqlParameter("@TrainerRegion", record.TrainerRegion);
            result[19] = new MySqlParameter("@TrainerClass", record.TrainerClass);
            result[20] = new MySqlParameter("@IsExchanged", record.IsExchanged);
            result[21] = new MySqlParameter("@TrainerVersion", record.TrainerVersion);
            result[22] = new MySqlParameter("@TrainerLanguage", record.TrainerLanguage);
            result[23] = new MySqlParameter("@TrainerBadges", record.TrainerBadges);
            result[24] = new MySqlParameter("@TrainerUnityTower", record.TrainerUnityTower);

            return result;
        }

        public override int GtsAvailablePokemon5()
        {
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                return (int)(long)db.ExecuteScalar("SELECT Count(*) FROM GtsPokemon5 WHERE IsExchanged = 0");
            }
        }
        #endregion

        #region Global Terminal 4
        public override long DressupUpload4(DressupRecord4 record)
        {
            if (record.Data.Length != 224) throw new ArgumentException("Dressup data must be 224 bytes.");
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                using (MySqlTransaction tran = db.BeginTransaction())
                {
                    long exists = (long)tran.ExecuteScalar("SELECT EXISTS(SELECT * FROM TerminalDressup4 WHERE md5 = unhex(md5(@data)) AND Data = @data)", new MySqlParameter("@data", record.Data));
                    if (exists != 0) return 0;

                    if (record.SerialNumber == 0)
                    {
                        long serial = (long)tran.ExecuteScalar("INSERT INTO TerminalDressup4 (pid, " +
                            "Data, md5, TimeAdded, ParseVersion, Species) VALUES (@pid, @data, " +
                            "unhex(md5(@data)), UTC_TIMESTAMP(), 1, @species); SELECT LAST_INSERT_ID()",
                            new MySqlParameter("@pid", record.PID),
                            new MySqlParameter("@data", record.Data),
                            new MySqlParameter("@species", record.Species));
                        tran.Commit();
                        return serial;
                    }
                    else
                    {
                        int rows = tran.ExecuteNonQuery("INSERT INTO TerminalDressup4 (pid, SerialNumber, " +
                            "Data, md5, TimeAdded, ParseVersion, Species) VALUES (@pid, @serial, @data, " +
                            "unhex(md5(@data)), UTC_TIMESTAMP(), 1, @species)",
                            new MySqlParameter("@pid", record.PID),
                            new MySqlParameter("@serial", record.SerialNumber),
                            new MySqlParameter("@data", record.Data),
                            new MySqlParameter("@species", record.Species));
                        tran.Commit();

                        return rows > 0 ? record.SerialNumber : 0;
                    }
                }
            }
        }

        public override DressupRecord4[] DressupSearch4(ushort species, int count)
        {
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();

                List<DressupRecord4> results = new List<DressupRecord4>(count);
                MySqlDataReader reader = (MySqlDataReader)db.ExecuteReader("SELECT pid, " +
                    "SerialNumber, Data FROM TerminalDressup4 WHERE Species = @species " +
                    "ORDER BY TimeAdded DESC LIMIT @count", 
                    new MySqlParameter("@species", species), 
                    new MySqlParameter("@count", count));
                while (reader.Read())
                {
                    results.Add(Dressup4FromReader(reader));
                }

                reader.Close();
                db.Close();
                return results.ToArray();
            }
        }

        private DressupRecord4 Dressup4FromReader(MySqlDataReader reader)
        {
            byte[] data = new byte[224];
            reader.GetBytes(2, 0, data, 0, 224);

            return new DressupRecord4(reader.GetInt32(0), reader.GetInt64(1), data);
        }

        public override long BoxUpload4(BoxRecord4 record)
        {
            if (record.Data.Length != 540) throw new ArgumentException("Box data must be 540 bytes.");
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                using (MySqlTransaction tran = db.BeginTransaction())
                {
                    long exists = (long)tran.ExecuteScalar("SELECT EXISTS(SELECT * FROM TerminalBoxes4 WHERE md5 = unhex(md5(@data)) AND Data = @data)", new MySqlParameter("@data", record.Data));
                    if (exists != 0) return 0;

                    if (record.SerialNumber == 0)
                    {
                        long serial = (long)tran.ExecuteScalar("INSERT INTO TerminalBoxes4 (pid, " +
                            "Data, md5, TimeAdded, ParseVersion, Label) VALUES (@pid, @data, " +
                            "unhex(md5(@data)), UTC_TIMESTAMP(), 1, @label); SELECT LAST_INSERT_ID()",
                            new MySqlParameter("@pid", record.PID),
                            new MySqlParameter("@data", record.Data),
                            new MySqlParameter("@label", (int)record.Label));
                        tran.Commit();
                        return serial;
                    }
                    else
                    {
                        int rows = tran.ExecuteNonQuery("INSERT INTO TerminalBoxes4 (pid, SerialNumber, " +
                            "Data, md5, TimeAdded, ParseVersion, Label) VALUES (@pid, @serial, @data, " +
                            "unhex(md5(@data)), UTC_TIMESTAMP(), 1, @label)",
                            new MySqlParameter("@pid", record.PID),
                            new MySqlParameter("@serial", record.SerialNumber),
                            new MySqlParameter("@data", record.Data),
                            new MySqlParameter("@label", (int)record.Label));
                        tran.Commit();

                        return rows > 0 ? record.SerialNumber : 0;
                    }
                }
            }
        }

        public override BoxRecord4[] BoxSearch4(BoxLabels4 label, int count)
        {
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();

                List<BoxRecord4> results = new List<BoxRecord4>(count);
                MySqlDataReader reader = (MySqlDataReader)db.ExecuteReader("SELECT pid, " +
                    "Label, SerialNumber, Data FROM TerminalBoxes4 WHERE Label = @label " +
                    "ORDER BY TimeAdded DESC LIMIT @count",
                    new MySqlParameter("@label", (int)label),
                    new MySqlParameter("@count", count));
                while (reader.Read())
                {
                    results.Add(Box4FromReader(reader));
                }

                reader.Close();
                db.Close();
                return results.ToArray();
            }
        }

        private BoxRecord4 Box4FromReader(MySqlDataReader reader)
        {
            byte[] data = new byte[540];
            reader.GetBytes(3, 0, data, 0, 540);

            return new BoxRecord4(reader.GetInt32(0), (BoxLabels4)reader.GetInt32(1), reader.GetInt64(2), data);
        }

        public override long BattleVideoUpload4(BattleVideoRecord4 record)
        {
            if (record.Data.Length != 7272) throw new ArgumentException();
            if (record.Header.Data.Length != 228) throw new ArgumentException();

            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                using (MySqlTransaction tran = db.BeginTransaction())
                {
                    long exists = (long)tran.ExecuteScalar("SELECT EXISTS(SELECT * " +
                        "FROM TerminalBattleVideos4 WHERE md5 = unhex(md5(CONCAT(@header, @data))) " +
                        "AND Data = @data AND Header = @header)", 
                        new MySqlParameter("@header", record.Header.Data), 
                        new MySqlParameter("@data", record.Data));
                    if (exists != 0) return 0;

                    if (record.SerialNumber == 0)
                    {
                        long key = (long)tran.ExecuteScalar("INSERT INTO TerminalBattleVideos4 " +
                            "(pid, Header, Data, md5, TimeAdded, ParseVersion, TrainerName, " +
                            "Metagame, Country, Region) " +
                            "VALUES (@pid, @header, @data, unhex(md5(CONCAT(@header, @data))), " +
                            "UTC_TIMESTAMP(), 1, @trainer, @metagame, @country, @region); " +
                            "SELECT LAST_INSERT_ID()",
                            new MySqlParameter("@pid", record.PID),
                            new MySqlParameter("@header", record.Header.Data),
                            new MySqlParameter("@data", record.Data),
                            new MySqlParameter("@trainer", record.Header.TrainerName),
                            new MySqlParameter("@metagame", (byte)record.Header.Metagame),
                            new MySqlParameter("@country", (byte)record.Header.Country),
                            new MySqlParameter("@region", (byte)record.Header.Region)
                            );
                        long serial = BattleVideoHeader4.KeyToSerial((long)key);

                        tran.ExecuteNonQuery("UPDATE TerminalBattleVideos4 SET " +
                            "SerialNumber = @serial WHERE id = @key", 
                            new MySqlParameter("@serial", serial), 
                            new MySqlParameter("@key", key));

                        // todo: make a proc to insert both video and party.
                        InsertBattleVideoParty(record.Header, (ulong)key, tran);

                        tran.Commit();
                        return serial;
                    }
                    else
                    {
                        ulong key = (ulong)BattleVideoHeader4.SerialToKey(record.SerialNumber);

                        int rows = tran.ExecuteNonQuery("INSERT INTO TerminalBattleVideos4 " +
                            "(id, pid, SerialNumber, Header, Data, md5, TimeAdded, ParseVersion, TrainerName, " +
                            "Metagame, Country, Region) " +
                            "VALUES (@key, @pid, @serial, @header, @data, unhex(md5(CONCAT(@header, @data))), " +
                            "UTC_TIMESTAMP(), 1, @trainer, @metagame, @country, @region)",
                            new MySqlParameter("@key", key),
                            new MySqlParameter("@pid", record.PID),
                            new MySqlParameter("@serial", record.SerialNumber),
                            new MySqlParameter("@header", record.Header.Data),
                            new MySqlParameter("@data", record.Data),
                            new MySqlParameter("@trainer", record.Header.TrainerName),
                            new MySqlParameter("@metagame", (byte)record.Header.Metagame),
                            new MySqlParameter("@country", (byte)record.Header.Country),
                            new MySqlParameter("@region", (byte)record.Header.Region)
                            );

                        if (rows == 0) return 0;

                        InsertBattleVideoParty(record.Header, key, tran);

                        tran.Commit();
                        return record.SerialNumber;
                    }
                }
            }
        }

        private void InsertBattleVideoParty(BattleVideoHeader4 header, ulong key, MySqlTransaction tran)
        {
            MySqlCommand cmd = new MySqlCommand("INSERT INTO " +
            "TerminalBattleVideoPokemon4 (video_id, Slot, Species) VALUES " +
            "(@key, @slot, @species)", tran.Connection, tran);
            cmd.Parameters.Add("@key", MySqlDbType.UInt64).Value = key;
            cmd.Parameters.Add("@slot", MySqlDbType.UByte);
            cmd.Parameters.Add("@species", MySqlDbType.UInt16);

            ushort[] party = header.Party;
            for (byte x = 0; x < 12; x++)
            {
                ushort species = party[x];
                if (species == 0) continue;
                cmd.Parameters["@slot"].Value = x;
                cmd.Parameters["@species"].Value = species;
                cmd.ExecuteNonQuery();
            }
        }

        public override BattleVideoHeader4[] BattleVideoSearch4(ushort species, BattleVideoMetagames4 metagame, byte country, byte region, int count)
        {
            using (MySqlConnection db = CreateConnection())
            {
                List<MySqlParameter> _params = new List<MySqlParameter>();
                String where = "";
                bool hasSearch = false;

                if (species != 0xffff)
                {
                    where += " WHERE EXISTS(SELECT * FROM TerminalBattleVideoPokemon4 " +
                        "WHERE video_id = TerminalBattleVideos4.id AND Species = @species)";
                    _params.Add(new MySqlParameter("@species", species));
                    hasSearch = true;
                }

                if (metagame == BattleVideoMetagames4.SearchColosseumSingleNoRestrictions)
                    metagame = BattleVideoMetagames4.ColosseumSingleNoRestrictions;
                if (metagame == BattleVideoMetagames4.SearchColosseumDoubleNoRestrictions)
                    metagame = BattleVideoMetagames4.ColosseumDoubleNoRestrictions;

                if (metagame == BattleVideoMetagames4.SearchColosseumSingleCupMatch)
                {
                    where += (hasSearch ? " AND " : " WHERE ") + "Metagame BETWEEN 1 AND 6";
                    hasSearch = true;
                }
                else if (metagame == BattleVideoMetagames4.SearchColosseumDoubleCupMatch)
                {
                    where += (hasSearch ? " AND " : " WHERE ") + "Metagame BETWEEN 8 AND 13";
                    hasSearch = true;
                }
                else if (metagame != BattleVideoMetagames4.SearchLatest30)
                {
                    where += (hasSearch ? " AND " : " WHERE ") + "Metagame = @metagame";
                    _params.Add(new MySqlParameter("@metagame", (byte)metagame));
                    hasSearch = true;
                }

                if (country != 0xff)
                {
                    where += (hasSearch ? " AND " : " WHERE ") + "Country = @country";
                    _params.Add(new MySqlParameter("@country", country));
                    hasSearch = true;
                }

                if (region != 0xff)
                {
                    where += (hasSearch ? " AND " : " WHERE ") + "Region = @region";
                    _params.Add(new MySqlParameter("@region", region));
                }

                _params.Add(new MySqlParameter("@count", count));

                db.Open();

                List<BattleVideoHeader4> results = new List<BattleVideoHeader4>(count);
                MySqlDataReader reader = (MySqlDataReader)db.ExecuteReader("SELECT pid, " +
                    "SerialNumber, Header FROM TerminalBattleVideos4" + where +
                    " ORDER BY TimeAdded DESC LIMIT @count",
                    _params.ToArray());
                while (reader.Read())
                {
                    results.Add(BattleVideoHeader4FromReader(reader));
                }

                reader.Close();
                db.Close();
                return results.ToArray();
            }
        }

        private BattleVideoHeader4 BattleVideoHeader4FromReader(MySqlDataReader reader)
        {
            byte[] data = new byte[228];
            reader.GetBytes(2, 0, data, 0, 228);

            return new BattleVideoHeader4(reader.GetInt32(0), reader.GetInt64(1), data);
        }

        public override BattleVideoRecord4 BattleVideoGet4(long serial)
        {
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                MySqlDataReader reader = (MySqlDataReader)db.ExecuteReader("SELECT pid, " +
                    "SerialNumber, Header, Data FROM TerminalBattleVideos4 " +
                    "WHERE SerialNumber = @serial", 
                    new MySqlParameter("@serial", serial));

                if (reader.Read())
                    return BattleVideo4FromReader(reader);
                else return null;
            }
        }

        private BattleVideoRecord4 BattleVideo4FromReader(MySqlDataReader reader)
        {
            byte[] data = new byte[7272];
            reader.GetBytes(3, 0, data, 0, 7272);
            BattleVideoHeader4 header = BattleVideoHeader4FromReader(reader);

            return new BattleVideoRecord4(header.PID, header.SerialNumber, header, data);
        }

        #endregion
    }
}
