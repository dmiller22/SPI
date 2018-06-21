using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SPI.Service
{
    public static class DataRetriever
    {        
        public static string GetPlayerInfo(int LeagueID)
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

                MorePlayers = false; //Temporary for debugging right now
            }

            return "nichts";
        }
    }

    
}