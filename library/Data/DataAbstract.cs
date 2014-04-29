using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PkmnFoundations.Structures;

namespace PkmnFoundations.Data
{
    public abstract class DataAbstract
    {
        #region Initialization
        private static DataAbstract m_instance;

        public static DataAbstract Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new DataMysql();
                }
                return m_instance;
            }
        }
        #endregion

        #region Utility
        #endregion

        #region GTS
        public abstract GtsRecord4 GtsDataForUser4(int pid);

        public abstract bool GtsDepositPokemon4(GtsRecord4 record);

        public abstract bool GtsDeletePokemon4(int pid);

        public abstract bool GtsTradePokemon4(int pidSrc, int pidDest);
        public abstract bool GtsTradePokemon4(GtsRecord4 upload, GtsRecord4 result);

        public abstract GtsRecord4[] GtsSearch4(int pid, ushort species, Genders gender, byte minLevel, byte maxLevel, byte country, int count);

        #endregion
    }
}
