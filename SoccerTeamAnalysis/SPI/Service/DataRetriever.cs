using SPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SPI.Service
{
    public static class DataRetriever
    {        
        public static void GetPlayerInfo(int LeagueID)
        {
            WebClient client = new WebClient();

            /* https://www.futwiz.com/en/fifa18/career-mode/players?minrating=40&maxrating=99&leagues[]=19&page=0 */

            LeagueID = 19; /* Bundesliga */
            int pageNum = 0;
            bool MorePlayers = true;

            while (MorePlayers)
            {
                string pageURL = string.Format("http://www.futwiz.com/en/fifa18/career-mode/players?minrating=40&maxrating=99&leagues[]={0}&page={1}", LeagueID, pageNum);
                string content = client.DownloadString(pageURL);

                int tableLocation = content.IndexOf("id=\"results\"");
                content = content.Substring(tableLocation);
                int tbodyStart = content.IndexOf("<tbody>");
                int tbodyEnd = content.IndexOf("</tbody>");

                content = content.Substring(tbodyStart, tbodyEnd - tbodyStart);

                pageNum++;

                if (content.Length < 10) MorePlayers = false;

                else
                {
                    ParsePlayerTable(content);
                }
                MorePlayers = false; //Temporary for debugging right now
            }
        }

        //Dan - 06/21/2018 - Breaks up the table of multiple players into a list containing a string for each player's info
        private static void ParsePlayerTable(string content)
        {
            List<string> players = new List<string>();

            while (content.Length > 0)
            {
                int trStart = content.IndexOf("<tr>");
                int trEnd = content.IndexOf("</tr>");

                string playerInfo = content.Substring(trStart, trEnd - trStart);
                players.Add(playerInfo);
                content = content.Substring(trEnd + 6);
            }

            ExtractPlayerInfo(players);
        }

        //Dan - {COMPLETION DATE HERE} - Extracts all base player info from the table on the page read in
        private static void ExtractPlayerInfo(List<string> playersHtml)
        {
            string[] stringSeparators = new string[] { "<td" };
            foreach (string playerHtml in playersHtml)
            {
                Player_Basic player = new Player_Basic();
                string info = playerHtml.Replace("\n", string.Empty);
                info = info.Replace("</td>", string.Empty);
                string[] playerInfo = info.Split(stringSeparators, StringSplitOptions.None);

                //PlayerID
                string startPath = playerInfo[1].Substring(playerInfo[1].IndexOf("href") + 6);
                player.PlayerID = ParsePlayerID(startPath.Substring(0, startPath.IndexOf("\"")));

                //Name
                string nameHtml = playerInfo[2].Substring(playerInfo[2].IndexOf("<b>") + 3);
                player.Name = nameHtml.Substring(0, nameHtml.IndexOf("<"));

                //National Team
                string nationalTeamHtml = playerInfo[2].Substring(playerInfo[2].IndexOf("flags/") + 6);
                player.NationalTeamID = Convert.ToInt32(nationalTeamHtml.Substring(0, nationalTeamHtml.IndexOf(".")));
                player.NationalTeam = (NationalTeam)player.NationalTeamID;

                //Club Team
                string clubTeamHtml = playerInfo[2].Substring(playerInfo[2].IndexOf("all/all/") + 8);
                player.ClubTeamID = Convert.ToInt32(clubTeamHtml.Substring(0, clubTeamHtml.IndexOf("/")));
                player.ClubTeam = (ClubTeam)player.ClubTeamID;
                
                //Position
                player.position = playerInfo[3].Substring(playerInfo[3].IndexOf(">"));

                //OVR
                string ovrHtml = playerInfo[4].Substring(playerInfo[4].IndexOf("<div class=") + 1);
                int ovrStart = ovrHtml.IndexOf(">");
                player.OVR = Convert.ToInt32(ovrHtml.Substring(ovrStart + 1, 2));

                //POT
                string potHtml = playerInfo[5].Substring(playerInfo[5].IndexOf("<div class=") + 1);
                int potStart = potHtml.IndexOf(">");
                player.POT = Convert.ToInt32(potHtml.Substring(potStart + 1, 2));

                //AGE
                player.Age = Convert.ToInt32(playerInfo[7].Substring(playerInfo[7].IndexOf(">") + 1, 2));

                //Contract
                player.ContractExpiration = Convert.ToInt32(playerInfo[8].Substring(playerInfo[8].IndexOf(">") + 1));

                //SkillMoves
                player.SkillMoves = Convert.ToInt32(playerInfo[9].Substring(playerInfo[9].IndexOf(">") + 1, 1));

                //Weak Foot
                player.WeakFoot = Convert.ToInt32(playerInfo[10].Substring(playerInfo[10].IndexOf(">") + 1, 1));

                //Work Rates
                string defWR = playerInfo[11].Substring(playerInfo[11].IndexOf("<b>") + 3);
                player.OffensiveWorkrate = playerInfo[11].Substring(playerInfo[11].IndexOf("<b>") + 3, 1);
                player.DefensiveWorkrate = defWR.Substring(defWR.IndexOf("<b>") + 3, 1);

                //Strong Foot
                player.StrongFoot = playerInfo[12].Substring(playerInfo[12].IndexOf(">") + 1);

                //Store in db (once I get a db set up)
            }
        }

        //Dan - 06/21/2018 - Parses the player ID from their link on the page
        private static int ParsePlayerID(string path)
        {
            string reversed_path = Reverse(path);

            string id_Reversed = reversed_path.Substring(0, reversed_path.IndexOf('/'));
            int ID = Convert.ToInt32(Reverse(id_Reversed));

            return ID;
        }

        //Dan - 06/21/2018 - Reverses a string
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }

    
}