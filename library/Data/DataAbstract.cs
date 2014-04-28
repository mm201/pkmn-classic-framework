using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PokeFoundations.Structures;

namespace PokeFoundations.Data
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
        public abstract GtsDatagram4 GtsDataForUser4(int pid);

        public abstract bool GtsDepositPokemon4(GtsDatagram4 datagram);

        public abstract bool GtsDeletePokemon4(int pid);

        public abstract bool GtsTradePokemon4(int pidSrc, int pidDest);
        public abstract bool GtsTradePokemon4(GtsDatagram4 upload, GtsDatagram4 result);

        public abstract GtsDatagram4[] GtsSearch4(ushort species, Genders gender, byte minLevel, byte maxLevel, byte country, int count);

        #endregion
    }
}
