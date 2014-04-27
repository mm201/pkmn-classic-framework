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
        public override GtsDatagram4 GtsDataForUser4(int pid)
        {
            using (MySqlConnection db = CreateConnection())
            {
                db.Open();
                MySqlDataReader reader = (MySqlDataReader)db.ExecuteReader("SELECT Data, Species, Gender, Level, " +
                    "RequestedSpecies, RequestedGender, RequestedMinLevel, RequestedMaxLevel, " +
                    "Unknown1, TrainerGender, Unknown2, TimeDeposited, TimeWithdrawn, pid, " +
                    "TrainerName, TrainerOT, TrainerCountry, TrainerRegion, TrainerClass, " +
                    "IsExchanged, TrainerVersion, TrainerLanguage FROM GtsPokemon4 WHERE pid = @pid",
                    new MySqlParameter("@pid", pid));

                if (!reader.Read()) return null;
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
#if DEBUG
                AssertHelper.Equals(result.PID, pid);
#endif
                reader.Close();
                return result;
            }
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

                /*
                MySqlDataReader reader = (MySqlDataReader)db.ExecuteReader("SELECT Data, Species, Gender, Level, " +
        "RequestedSpecies, RequestedGender, RequestedMinLevel, RequestedMaxLevel, " +
        "Unknown1, TrainerGender, Unknown2, TimeDeposited, TimeWithdrawn, pid, " +
        "TrainerName, TrainerOT, TrainerCountry, TrainerRegion, TrainerClass, " +
        "IsExchanged, TrainerVersion, TrainerLanguage FROM GtsPokemon4 WHERE pid = @pid",
        new MySqlParameter("@pid", pid));
                 * 
                 * */

                MySqlTransaction tran = db.BeginTransaction();

                int count = (int)tran.ExecuteScalar("SELECT Count(*) FROM GtsPokemon4 WHERE pid = @pid",
                    new MySqlParameter("@pid", datagram.PID));

                if (count > 0)
                {
                    // This player already has a pokemon in the system.
                    // we can possibly allow multiples under some future conditions
                    tran.Rollback();
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
                    new MySqlParameter("@Data", datagram.Data),
                    new MySqlParameter("@Species", datagram.Species),
                    new MySqlParameter("@Gender", (byte)datagram.Gender),
                    new MySqlParameter("@Level", datagram.Level),
                    new MySqlParameter("@RequestedSpecies", datagram.RequestedSpecies),
                    new MySqlParameter("@RequestedGender", (byte)datagram.RequestedGender),
                    new MySqlParameter("@RequestedMinLevel", datagram.RequestedMinLevel),
                    new MySqlParameter("@RequestedMaxLevel", datagram.RequestedMaxLevel),
                    new MySqlParameter("@Unknown1", datagram.Unknown1),
                    new MySqlParameter("@TrainerGender", (byte)datagram.TrainerGender),
                    new MySqlParameter("@Unknown2", datagram.Unknown2),
                    new MySqlParameter("@TimeDeposited", datagram.TimeDeposited),
                    new MySqlParameter("@TimeWithdrawn", datagram.TimeWithdrawn),
                    new MySqlParameter("@pid", datagram.PID),
                    new MySqlParameter("@TrainerName", datagram.TrainerName),
                    new MySqlParameter("@TrainerOT", datagram.TrainerOT),
                    new MySqlParameter("@TrainerCountry", datagram.TrainerCountry),
                    new MySqlParameter("@TrainerRegion", datagram.TrainerRegion),
                    new MySqlParameter("@TrainerClass", datagram.TrainerClass),
                    new MySqlParameter("@IsExchanged", datagram.IsExchanged),
                    new MySqlParameter("@TrainerVersion", datagram.TrainerVersion),
                    new MySqlParameter("@TrainerLanguage", datagram.TrainerLanguage));

                tran.Commit();

                return true;
            }
        }

        public override void GtsDeletePokemon4(int pid)
        {
        }

        public override void GtsTradePokemon4(int pidSrc, int pidDest)
        {
        }

        #endregion
    }
}
