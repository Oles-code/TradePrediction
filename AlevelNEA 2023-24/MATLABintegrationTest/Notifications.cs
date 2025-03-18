using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MATLABintegrationTest
{
    internal class Notifications
    {
        //returns any sell calls for owned stock, and any sell or buy calls for watchlist stocks
        public string[,] ReturnStockNotifications(string email)
        {
            string[,] notifications = new string[4, 2];

            DatabaseInteractions database = new DatabaseInteractions(); 
            MATLABinteractions MATLABloader = new MATLABinteractions();

            string[] ownedStockCodes = database.ReturnUserStocks(database.CreateConnection(), email);
            DoubleLinkList<string> ownedStockActions = new DoubleLinkList<string>();

            string[] watchlistStockCodes = database.ReturnUserWatchlist(database.CreateConnection(), email);
            DoubleLinkList<string> watchlistStockActions = new DoubleLinkList<string>();

            for (int x = 0; x < ownedStockCodes.Length; x++)
            {
                string entryExitOwned = MATLABloader.MACDCalculator(ownedStockCodes[x]);
                if (entryExitOwned == "Sell")
                {
                    ownedStockActions.AddBack(database.GetNameFromCode(database.CreateConnection(), ownedStockCodes[x]));
                    ownedStockActions.AddBack(entryExitOwned);
                }
            }

            
            for (int x = 0; x < watchlistStockCodes.Length; x++)
            {
                if (watchlistStockCodes[x] != " ")
                {
                    string entryExitWatchlist = MATLABloader.MACDCalculator(watchlistStockCodes[x]);
                    if ((entryExitWatchlist == "Sell" || entryExitWatchlist == "Buy") && ownedStockCodes.Contains(watchlistStockCodes[x]) == false)
                    {
                        watchlistStockActions.AddBack(database.GetNameFromCode(database.CreateConnection(), watchlistStockCodes[x]));
                        watchlistStockActions.AddBack(entryExitWatchlist);
                    }
                }
                    
            }


            int numOwnedNotifications = ((ownedStockActions.GetLength())/2)-1;
            int numWatchlistNotifications = ((ownedStockActions.GetLength()) / 2) - 1;
            if (numOwnedNotifications > 3) 
            {
                numOwnedNotifications= 3;
            }

            while (numOwnedNotifications >= 0)
            {
                notifications[numOwnedNotifications, 0] = ownedStockActions.GetNode((numOwnedNotifications)*2).DisplayNode();
                notifications[numOwnedNotifications, 1] = ownedStockActions.GetNode((numOwnedNotifications)*2+1).DisplayNode();
                numOwnedNotifications--;
            }
            int counter = 0;
            while (notifications.Length < 4 && numWatchlistNotifications>0)
            {
                notifications[counter, 0] = watchlistStockActions.GetNode((counter) * 2).DisplayNode();
                notifications[counter, 1] = watchlistStockActions.GetNode((counter) * 2 + 1).DisplayNode();
                counter++;
                numWatchlistNotifications--;
            }

            return notifications;
        }
    }
}
