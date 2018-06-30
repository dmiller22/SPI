using SPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
            LeagueID = 39; //MLS
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

                if (content.Length < 10) MorePlayers = false;

                else
                {
                    ParsePlayerTable(content);
                    pageNum++;
                }
                //MorePlayers = false; //Temporary for debugging right now
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
                player.position = playerInfo[3].Substring(playerInfo[3].IndexOf(">") + 1);

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
                int skillStart = playerInfo[9].IndexOf(">");
                int skillEnd = playerInfo[9].Substring(skillStart).IndexOf("<") + skillStart;
                string skill = playerInfo[9].Substring(skillStart + 1, skillEnd - skillStart - 1).Trim();
                player.SkillMoves = Convert.ToInt32(skill);

                //Weak Foot
                int weakFootStart = playerInfo[10].IndexOf(">");
                int weakFootEnd = playerInfo[10].Substring(weakFootStart).IndexOf("<") + weakFootStart;
                string weakfoot = playerInfo[10].Substring(weakFootStart + 1, weakFootEnd - weakFootStart - 1).Trim();
                player.WeakFoot = Convert.ToInt32(weakfoot);

                //Work Rates
                string defWR = playerInfo[11].Substring(playerInfo[11].IndexOf("<b>") + 3);
                player.OffensiveWorkrate = playerInfo[11].Substring(playerInfo[11].IndexOf("<b>") + 3, 1);
                player.DefensiveWorkrate = defWR.Substring(defWR.IndexOf("<b>") + 3, 1);

                //Strong Foot
                player.StrongFoot = playerInfo[12].Substring(playerInfo[12].IndexOf(">") + 1);

                //Store in db
                bool IsNewPlayer = !IsPlayerStored(player.PlayerID);

                StoreNewPlayer(player, IsNewPlayer);
                
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

        private static bool IsPlayerStored(int PlayerID)
        {
            SqlConnection conn = new SqlConnection();
            DataTable dt = new DataTable(); ;
            ConnectionStringSettings mySetting = ConfigurationManager.ConnectionStrings["DefaultConnection"];
            string conString = mySetting.ConnectionString;
            string commandString = "Player_IsStored";
            SqlCommand command = new SqlCommand(commandString, conn);
            command.Parameters.Add(new SqlParameter("@PlayerID", PlayerID));
            command.CommandType = CommandType.StoredProcedure;
            conn.ConnectionString = conString;
            conn.Open();
            using (SqlDataReader dr = command.ExecuteReader())
            {
                dt.Load(dr);
            }
            conn.Close();

            return ((int)dt.Rows[0]["Player"] > 0);
        }

        private static void StoreNewPlayer(Player_Basic player, bool NewPlayer)
        {
            SqlConnection conn = new SqlConnection();
            ConnectionStringSettings mySetting = ConfigurationManager.ConnectionStrings["DefaultConnection"];
            string conString = mySetting.ConnectionString;
            string commandString = "PlayerBase_Modify";
            if (NewPlayer)
            {
                commandString = "PlayerBase_Store";
            }
            SqlCommand command = new SqlCommand(commandString, conn);
            command.Parameters.Add(new SqlParameter("@PlayerID", player.PlayerID));
            command.Parameters.Add(new SqlParameter("@Name", player.Name));
            command.Parameters.Add(new SqlParameter("@NationalTeamID", player.NationalTeamID));
            command.Parameters.Add(new SqlParameter("@ClubTeamID", player.ClubTeamID));
            command.Parameters.Add(new SqlParameter("@Position", player.position));
            command.Parameters.Add(new SqlParameter("@OVR", player.OVR));
            command.Parameters.Add(new SqlParameter("@POT", player.POT));
            command.Parameters.Add(new SqlParameter("@Age", player.Age));
            command.Parameters.Add(new SqlParameter("@ContractExp", player.ContractExpiration));
            command.Parameters.Add(new SqlParameter("@Skill", player.SkillMoves));
            command.Parameters.Add(new SqlParameter("@WeakFoot", player.WeakFoot));
            command.Parameters.Add(new SqlParameter("@OffWorkRate", player.OffensiveWorkrate));
            command.Parameters.Add(new SqlParameter("@DefWorkRate", player.DefensiveWorkrate));
            command.Parameters.Add(new SqlParameter("@StrongFoot", player.StrongFoot));
            command.CommandType = CommandType.StoredProcedure;
            conn.ConnectionString = conString;
            conn.Open();
            command.ExecuteNonQuery();
            conn.Close();
        }
    }
}