using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using GamestatsBase;
using PkmnFoundations.Data;
using PkmnFoundations.Structures;
using PkmnFoundations.Wfc;

namespace PkmnFoundations.GTS
{
    /// <summary>
    /// Summary description for pokemondpds_web
    /// </summary>
    public class pokemondpds_web : GamestatsHandler
    {
        public pokemondpds_web()
            : base("uLMOGEiiJogofchScpXb000244fd00006015100000005b440e7epokemondpds",
            GamestatsRequestVersions.Version3, GamestatsResponseVersions.Version2, true, true)
        {

        }

        public override void ProcessGamestatsRequest(byte[] request, MemoryStream response, string url, int pid, HttpContext context, GamestatsSession session)
        {
            switch (url)
            {
                default:
                    SessionManager.Remove(session);

                    // unrecognized page url
                    ShowError(context, 404);
                    return;

                case "/pokemondpds/web/enc/lobby/checkProfile.asp":
                {
                    if (request.Length != 168)
                    {
                        ShowError(context, 400);
                        return;
                    }

                    byte[] gamestatsHeader = new byte[20];
                    byte[] requestData = new byte[148];

                    Array.Copy(request, 0, gamestatsHeader, 0, 20);
                    Array.Copy(request, 20, requestData, 0, 148);

                    TrainerProfilePlaza requestProfile = new TrainerProfilePlaza(gamestatsHeader, requestData);
                    Database.Instance.PlazaSetProfile(requestProfile);

                    TrainerProfilePlaza responseProfile = Database.Instance.PlazaGetProfile(requestProfile.PID);
                    response.Write(responseProfile.Data, 0, 152);

                } break;

                case "/pokemondpds/web/enc/lobby/getSchedule.asp":
                {
                        // note(mythra): this response CAN be overwritten by the PEERCHAT server.
                        //
                        // Effectively the client fetches what it needs if it wants to create a new room,
                        // joins or creates a channel, and checks the `b_lib_c_lobby` channel key has been set.
                        // if it has, it loads that room data. If not it loads this response.
                        byte[] serializedSchedule = PlazaSchedule.Generate().Save();
                        // The 'status code' if we succeeded. We just always write success.
                        response.Write(new byte[] { 0x0, 0x0, 0x0, 0x0 }, 0, 4);
                        response.Write(serializedSchedule, 0, serializedSchedule.Length);
                } break;

                case "/pokemondpds/web/enc/lobby/getVIP.asp":
                {
                    // Status Code.
                    response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, 4);
                    // VIPs.
                    foreach (var id in VIPIds)
                    {
                        response.Write(BitConverter.GetBytes(id), 0, 4);
                        response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0, 4);
                    }
                }
                break;

                case "/pokemondpds/web/enc/lobby/getQuestionnaire.asp":
                {
                    response.Write(new byte[] { 0x0, 0x0, 0x0, 0x0 }, 0, 4);
                    response.Write(staticQuestionnaire, 0, staticQuestionnaire.Length);

                } break;

                case "/pokemondpds/web/enc/lobby/submitQuestionnaire.asp":
                {
                        // One day we could parse as 'SubmittedQuestionnaire', and save in a DB somewhere.
                        // that'd be cool!
                        //
                        // literally 'thx' in ascii... lol
                        response.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x74, 0x68, 0x78, 0x00 }, 0, 8);

                } break;
            }
        }

        /// <summary>
        /// The list of "VIPs".
        ///
        /// Being a VIP in a lobby just gives you a golden trainer card, and upgrades your 'Touch Toy' to the highest
        /// level right away. So far we've just been giving this to the developers who signed up for it.
        /// </summary>
        private static int[] VIPIds = new int[] { 600403373, 601315647, 601988829 };

        /// <summary>
        /// A static questionnaire, who's id is not above 1k so it doesn't load the custom question text.
        ///
        /// The last weeks results is still taken.
        /// And the footer of unknown data is copied from static responses.
        /// </summary>
        private static byte[] staticQuestionnaire = new PlazaQuestionnaire(
            new PlazaQuestion(730, "Not used", new string[] { "N/A", "N/A", "N/A" }, new byte[12], false),
            new PlazaQuestion(729, "Not used", new string[] { "N/A", "N/A", "N/A" }, new byte[12], false),
            new int[] { 69, 420, 100 },
            new byte[] { 0x64, 0x01, 0x00, 0x00, 0x11, 0x01, 0x00, 0x00, 0x83, 0x00, 0x00, 0x00 }).Save();
    }
}
