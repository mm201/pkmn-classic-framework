using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PkmnFoundations.Structures;
using System.Configuration;
using PkmnFoundations.Pokedex;

namespace PkmnFoundations.Data
{
    public abstract class Database
    {
        #region Initialization
        private static Database m_instance;

        public static Database Instance
        {
            get
            {
                // fixme: this is not thread safe
                if (m_instance == null)
                {
                    m_instance = CreateInstance();
                }
                return m_instance;
            }
        }

        public static Database CreateInstance()
        {
            ConnectionStringSettings connStr = ConfigurationManager.ConnectionStrings["pkmnFoundationsConnectionString"];
            if (connStr == null) throw new NotSupportedException("No database connection string provided. Please add one in web.config or app.config.");

            return CreateInstance(connStr);
        }

        public static Database CreateInstance(ConnectionStringSettings connStr)
        {
            if (connStr == null) throw new ArgumentNullException("connStr");

            return CreateInstance(connStr.ConnectionString, connStr.ProviderName);
        }

        public static Database CreateInstance(String connStr, String provider)
        {
            if (connStr == null) throw new ArgumentNullException("connStr");
            if (provider == null) throw new ArgumentNullException("provider");

            switch (provider)
            {
                case "MySql.Data.MySqlClient":
                    return new DataMysql(connStr);
                default:
                    throw new NotSupportedException("Database provider not supported.");
            }
        }
        #endregion

        #region Utility
        #endregion

        #region GTS 4
        public const int GTS_VERSION_4 = 0;

        public abstract GtsRecord4 GtsDataForUser4(int pid);

        public abstract bool GtsDepositPokemon4(GtsRecord4 record);

        public abstract bool GtsDeletePokemon4(int pid);

        public abstract bool GtsTradePokemon4(int pidSrc, int pidDest);
        public abstract bool GtsTradePokemon4(GtsRecord4 upload, GtsRecord4 result);

        public abstract GtsRecord4[] GtsSearch4(int pid, ushort species, Genders gender, byte minLevel, byte maxLevel, byte country, int count);
        public abstract int GtsAvailablePokemon4();

        public abstract void GtsLogTrade4(GtsRecord4 record, DateTime ? timeWithdrawn);
        #endregion

        #region Battle Tower 4
        public abstract ulong BattleTowerUpdateRecord4(BattleTowerRecord4 record);
        public abstract ulong BattleTowerAddLeader4(BattleTowerRecord4 record);
        public abstract BattleTowerRecord4[] BattleTowerGetOpponents4(int pid, byte rank, byte roomNum);
        public abstract BattleTowerProfile4[] BattleTowerGetLeaders4(byte rank, byte roomNum);
        #endregion

        #region Other Gamestats 4
        public abstract bool GamestatsSetProfile4(TrainerProfile4 profile);
        #endregion

        #region GTS 5
        public const int GTS_VERSION_5 = 0;

        public abstract GtsRecord5 GtsDataForUser5(int pid);

        public abstract bool GtsDepositPokemon5(GtsRecord5 record);

        public abstract bool GtsDeletePokemon5(int pid);

        public abstract bool GtsTradePokemon5(int pidSrc, int pidDest);
        public abstract bool GtsTradePokemon5(GtsRecord5 upload, GtsRecord5 result);

        public abstract GtsRecord5[] GtsSearch5(int pid, ushort species, Genders gender, byte minLevel, byte maxLevel, byte country, int count);
        public abstract int GtsAvailablePokemon5();

        public abstract void GtsLogTrade5(GtsRecord5 record, DateTime ? timeWithdrawn);
        #endregion

        #region Other Gamestats 5
        public abstract bool GamestatsSetProfile5(TrainerProfile5 profile);
        #endregion

        #region Battle Subway 5
        public abstract ulong BattleSubwayUpdateRecord5(BattleSubwayRecord5 record);
        public abstract ulong BattleSubwayAddLeader5(BattleSubwayRecord5 record);
        public abstract BattleSubwayRecord5[] BattleSubwayGetOpponents5(int pid, byte rank, byte roomNum);
        public abstract BattleSubwayProfile5[] BattleSubwayGetLeaders5(byte rank, byte roomNum);
        #endregion

        #region Global Terminal 4
        public const int DRESSUP_VERSION_4 = 1;
        public const int BOX_VERSION_4 = 1;
        public const int BATTLEVIDEO_VERSION_4 = 1;

        public abstract ulong DressupUpload4(DressupRecord4 record);
        public abstract DressupRecord4[] DressupSearch4(ushort species, int count);

        public abstract ulong BoxUpload4(BoxRecord4 record);
        public abstract BoxRecord4[] BoxSearch4(BoxLabels4 label, int count);

        public abstract ulong BattleVideoUpload4(BattleVideoRecord4 record);
        public abstract BattleVideoHeader4[] BattleVideoSearch4(ushort species, BattleVideoRankings4 ranking, BattleVideoMetagames4 metagame, byte country, byte region, int count);
        public abstract BattleVideoRecord4 BattleVideoGet4(ulong serial, bool incrementViews = false);
        public abstract bool BattleVideoFlagSaved4(ulong serial);

        public abstract ulong BattleVideoCount4();
        #endregion

        #region Global Terminal 5
        public const int MUSICAL_VERSION_5 = 1;
        public const int BATTLEVIDEO_VERSION_5 = 1;

        public abstract ulong MusicalUpload5(MusicalRecord5 record);
        public abstract MusicalRecord5[] MusicalSearch5(ushort species, int count);

        public abstract ulong BattleVideoUpload5(BattleVideoRecord5 record);
        public abstract BattleVideoHeader5[] BattleVideoSearch5(ushort species, BattleVideoRankings5 ranking, BattleVideoMetagames5 metagame, byte country, byte region, int count);
        public abstract BattleVideoRecord5 BattleVideoGet5(ulong serial, bool incrementViews = false);
        public abstract bool BattleVideoFlagSaved5(ulong serial);

        public abstract ulong BattleVideoCount5();
        #endregion

        #region Pokedex creation
        public abstract void PokedexInsertSpecies(Species s);
        public abstract void PokedexInsertForm(Form f);
        public abstract void PokedexInsertFormStats(FormStats f);
        public abstract void PokedexInsertFamily(Family f);
        public abstract void PokedexInsertEvolution(Evolution f);

        public abstract void PokedexInsertType(PkmnFoundations.Pokedex.Type t);
        public abstract void PokedexInsertItem(Item i);
        public abstract void PokedexInsertMove(Move m);
        public abstract void PokedexInsertAbility(Ability a);

        #endregion
    }
}
