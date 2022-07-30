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

        public static Database CreateInstance(string connStr, string provider)
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

        internal static DateTime DateLerp(DateTime first, DateTime second, double weight)
        {
            TimeSpan diff = second - first;
            return first + new TimeSpan((long)(diff.Ticks * weight));
        }

        internal const double HYPE_DECAY_DAYS = 7.0d;
        internal const double HYPE_DECAY_RATE = -0.09902102579427790134531887449403; // -ln(2)/HYPE_DECAY_DAYS
        internal const double HYPE_NEW_VIDEO = 5.0d;
        internal const double HYPE_WATCHED_VIDEO = 1.0d;
        internal const double HYPE_SAVED_VIDEO = 1.0d; // Note that when the client wants to save, it will call both Get and FlagSaved

        /// <summary>
        /// Calculates how much Hype has changed between two times. This could be 
        /// </summary>
        /// <param name="oldHype"></param>
        /// <param name="oldDate"></param>
        /// <param name="newDate"></param>
        /// <returns></returns>
        internal static double HypeDecay(double oldHype, DateTime oldDate, DateTime newDate)
        {
            TimeSpan ts = newDate - oldDate;
            double decays = HYPE_DECAY_RATE * ts.Ticks / TimeSpan.FromDays(1).Ticks;
            return Math.Exp(decays) * oldHype; // e^(days*-ln(2)/7), should decay by half each week.
        }

        /// <summary>
        /// Gets the desired date to use for hype ratings. Changes exactly once a week.
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        internal static DateTime GetActiveHypeDate(DateTime now)
        {
            DateTime dateNow = now.Date;
            return dateNow.AddDays(-(int)dateNow.DayOfWeek);
        }

        internal const double GTS_LOCK_DURATION = 60.0d;

        #endregion

        #region GTS 4
        public const int GTS_VERSION_4 = 0;

        public abstract GtsRecord4 GtsDataForUser4(Pokedex.Pokedex pokedex, int pid);
        public abstract GtsRecord4 GtsGetRecord4(Pokedex.Pokedex pokedex, long tradeId, bool isExchanged, bool allowHistory);

        public abstract bool GtsDepositPokemon4(GtsRecord4 record);

        public abstract bool GtsDeletePokemon4(int pid);

        public abstract bool GtsLockPokemon4(ulong tradeId, int partner_pid);
        public abstract bool GtsCheckLockStatus4(ulong tradeId, int partner_pid);

        public abstract bool GtsTradePokemon4(int pidSrc, int pidDest);
        public abstract bool GtsTradePokemon4(GtsRecord4 upload, GtsRecord4 result, int partner_pid);

        public abstract GtsRecord4[] GtsSearch4(Pokedex.Pokedex pokedex, int pid, ushort species, Genders gender, byte minLevel, byte maxLevel, byte country, int count);
        public abstract int GtsAvailablePokemon4();

        public abstract void GtsSetLastSearch4(int pid);
        public abstract DateTime? GtsGetLastSearch4(int pid);
        #endregion

        #region Battle Tower 4
        public abstract ulong BattleTowerUpdateRecord4(BattleTowerRecord4 record);
        public abstract ulong BattleTowerAddLeader4(BattleTowerRecord4 record);
        public abstract BattleTowerRecord4[] BattleTowerGetOpponents4(Pokedex.Pokedex pokedex, int pid, byte rank, byte roomNum);
        public abstract BattleTowerProfile4[] BattleTowerGetLeaders4(Pokedex.Pokedex pokedex, byte rank, byte roomNum);
        #endregion

        #region Wi-fi Plaza
        public abstract TrainerProfilePlaza PlazaGetProfile(int pid);
        public abstract bool PlazaSetProfile(TrainerProfilePlaza profile);
        #endregion

        #region Other Gamestats 4
        public abstract bool GamestatsBumpProfile4(int pid, string ip_address);
        public abstract bool GamestatsSetProfile4(TrainerProfile4 profile);
        public abstract TrainerProfile4 GamestatsGetProfile4(int pid);
        #endregion

        #region Bans
        public abstract BanStatus CheckBanStatus(int pid);
        public abstract BanStatus CheckBanStatus(byte[] mac_address);
        public abstract BanStatus CheckBanStatus(string ip_address);
        public abstract BanStatus CheckBanStatus(TrainerProfileBase profile);
        public abstract BanStatus CheckBanStatus(uint ip_address);

        public abstract void AddBan(int pid, BanStatus status);
        public abstract void AddBan(byte[] mac_address, BanStatus status);
        public abstract void AddBan(string ip_address, BanStatus status);
        #endregion

        #region GTS 5
        public const int GTS_VERSION_5 = 0;

        public abstract GtsRecord5 GtsDataForUser5(Pokedex.Pokedex pokedex, int pid);
        public abstract GtsRecord5 GtsGetRecord5(Pokedex.Pokedex pokedex, long tradeId, bool isExchanged, bool allowHistory);

        public abstract bool GtsDepositPokemon5(GtsRecord5 record);

        public abstract bool GtsDeletePokemon5(int pid);

        public abstract bool GtsLockPokemon5(ulong tradeId, int partner_pid);
        public abstract bool GtsCheckLockStatus5(ulong tradeId, int partner_pid);

        public abstract bool GtsTradePokemon5(int pidSrc, int pidDest);
        public abstract bool GtsTradePokemon5(GtsRecord5 upload, GtsRecord5 result, int partner_pid);

        public abstract GtsRecord5[] GtsSearch5(Pokedex.Pokedex pokedex, int pid, ushort species, Genders gender, byte minLevel, byte maxLevel, byte country, int count);
        public abstract int GtsAvailablePokemon5();

        public abstract void GtsSetLastSearch5(int pid);
        public abstract DateTime ? GtsGetLastSearch5(int pid);
        #endregion

        #region Other Gamestats 5
        public abstract bool GamestatsSetProfile5(TrainerProfile5 profile);
        public abstract TrainerProfile5 GamestatsGetProfile5(int pid);
        #endregion

        #region Battle Subway 5
        public abstract ulong BattleSubwayUpdateRecord5(BattleSubwayRecord5 record);
        public abstract ulong BattleSubwayAddLeader5(BattleSubwayRecord5 record);
        public abstract BattleSubwayRecord5[] BattleSubwayGetOpponents5(Pokedex.Pokedex pokedex, int pid, byte rank, byte roomNum);
        public abstract BattleSubwayProfile5[] BattleSubwayGetLeaders5(Pokedex.Pokedex pokedex, byte rank, byte roomNum);
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

        /// <summary>
        /// Instructs the database that the provided datetime is now active.
        /// If the new datetime is outside the active leaderboard's datetime
        /// range, a new leaderboard should be initialized.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>True if it began a new leaderboard</returns>
        public abstract bool TrainerRankingsPerformRollover();

        /// <summary>
        /// Gets the three record types being collected for the active leaderboard.
        /// </summary>
        /// <returns>RecordTypes</returns>
        public abstract IList<TrainerRankingsRecordTypes> TrainerRankingsGetActiveRecordTypes();

        /// <summary>
        /// Submits trainer rankings data for one player and populates the active leaderboard with it.
        /// </summary>
        /// <param name="submission"></param>
        public abstract void TrainerRankingsSubmit(TrainerRankingsSubmission submission);

        /// <summary>
        /// Gets past reports, sorted descending, falling within a specified date range.
        /// </summary>
        /// <param name="start">Datetime during which the oldest returned leaderboard was active</param>
        /// <param name="end">Datetime during which the newest returned leaderboard was active</param>
        /// <param name="limit">Limit on the number of results or less than 1 for unlimited</param>
        /// <returns>Reports</returns>
        public abstract TrainerRankingsReport[] TrainerRankingsGetReport(DateTime start, DateTime end, int limit);

        /// <summary>
        /// Gets past reports, sorted descending, falling within a specified date range.
        /// </summary>
        /// <param name="start">Datetime during which the oldest returned leaderboard was active</param>
        /// <param name="end">Datetime during which the newest returned leaderboard was active</param>
        /// <returns>Reports</returns>
        public TrainerRankingsReport[] TrainerRankingsGetReport(DateTime start, DateTime end)
        {
            return TrainerRankingsGetReport(start, end, 0);
        }

        /// <summary>
        /// Gets the single report which was active during the specified date.
        /// </summary>
        /// <param name="during"></param>
        /// <returns>Report</returns>
        public TrainerRankingsReport TrainerRankingsGetReport(DateTime during)
        {
            return TrainerRankingsGetReport(during, during, 1).FirstOrDefault();
        }

        /// <summary>
        /// Gets the most recently finished report
        /// </summary>
        /// <returns>Report</returns>
        public TrainerRankingsReport TrainerRankingsGetReport()
        {
            return TrainerRankingsGetReport(DateTime.MinValue, DateTime.UtcNow, 1).FirstOrDefault();
        }

        /// <summary>
        /// Gets the currently active, incomplete report.
        /// </summary>
        /// <returns></returns>
        public abstract TrainerRankingsReport TrainerRankingsGetPendingReport();

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
        public abstract void PokedexInsertRibbon(Ribbon r);

        public abstract void PokedexInsertRegion(Region r);
        public abstract void PokedexInsertLocation(Location l);

        #endregion

        #region Pokedex retrieval
        public abstract List<Species> PokedexGetAllSpecies(Pokedex.Pokedex pokedex);
        public abstract List<Form> PokedexGetAllForms(Pokedex.Pokedex pokedex);
        public abstract List<FormStats> PokedexGetAllFormStats(Pokedex.Pokedex pokedex);
        public abstract List<Family> PokedexGetAllFamilies(Pokedex.Pokedex pokedex);
        public abstract List<Evolution> PokedexGetAllEvolutions(Pokedex.Pokedex pokedex);

        public abstract List<Pokedex.Type> PokedexGetAllTypes(Pokedex.Pokedex pokedex);
        public abstract List<Item> PokedexGetAllItems(Pokedex.Pokedex pokedex);
        public abstract List<Move> PokedexGetAllMoves(Pokedex.Pokedex pokedex);
        public abstract List<Ability> PokedexGetAllAbilities(Pokedex.Pokedex pokedex);
        public abstract List<Ribbon> PokedexGetAllRibbons(Pokedex.Pokedex pokedex);

        public abstract List<Region> PokedexGetAllRegions(Pokedex.Pokedex pokedex);
        public abstract List<Location> PokedexGetAllLocations(Pokedex.Pokedex pokedex);
        #endregion
    }
}
