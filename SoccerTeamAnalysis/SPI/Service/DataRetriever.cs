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
        private static void ExtractPlayerInfo(List<string> players)
        {
            int PlayerID, OVR, POT;
            string name, position;
            string[] stringSeparators = new string[] { "<td" };
            foreach (string player in players)
            { 
                string info = player.Replace("\n", string.Empty);
                info = info.Replace("</td>", string.Empty);
                string[] playerInfo = info.Split(stringSeparators, StringSplitOptions.None);
                string startPath = playerInfo[1].Substring(playerInfo[1].IndexOf("href") + 6);
                PlayerID = ParsePlayerID(startPath.Substring(0, startPath.IndexOf("\"")));

                //Parse rest of info and store in db (once I get a db set up)
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

        //Dan - 06/21/2018 - Reverse a string (used to parse Player ID)
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }

    
}