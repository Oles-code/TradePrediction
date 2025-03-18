using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Data.Sqlite;
using System.Data.SQLite;
using System.Windows.Documents;
using System.IO;
using System.Reflection;
using Microsoft.Data.Sqlite;
using System.Windows.Media.Animation;
using System.Net;
using System.Linq.Expressions;
using System.Xml.Schema;
using System.Windows.Input;

namespace MATLABintegrationTest
{

    //This class contains all the interactions with the database
    internal class DatabaseInteractions
    {

        //Creates the database (only used on first running of program)
        public void CreateDatabaseTest()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();
            CreateTable(sqlite_conn);
        }

        //creates connection to the database
        public SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            // sqlite_conn = new SqliteConnection(@"Data Source=file:C:\\Users\\olesc\\source\\repos\\AlevelNEA 2023-24\\Database\\Database.db;");
            sqlite_conn = new SQLiteConnection("Data Source=database.db");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("NOO");
            }
            return sqlite_conn;
        }

        //creates table in the database (only used in first running of program)
        public void CreateTable(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            //string Createsql = "CREATE TABLE LoginTable(Email VARCHAR, Password VARCHAR, TotalPandL DECIMAL, WatchStock1 VARCHAR, WatchStock2 VARCHAR, WatchStock3 VARCHAR)";
            //string Createsql = "CREATE TABLE StockCodesTable(Symbol VARCHAR, Name VARCHAR, Price DECIMAL)";
            string Createsql = "CREATE TABLE UserPositionsTable(Email VARCHAR, StockSymbol VARCHAR, Value DECIMAL, LastUpdate DATE)";
            
            //string Createsql = "DROP TABLE StockCodesTable";

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            conn.Close();
        }

        //finds all the stock symbols owned by a particular user
        public string[] ReturnUserStocks(SQLiteConnection conn, string email)
        {
            List<string> stockCodes = new List<string>();

            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = String.Format("SELECT StockSymbol FROM UserPositionsTable WHERE Email = '{0}';", email);
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                stockCodes.Add(sqlite_datareader.GetString(0));
            }
            sqlite_datareader.Close();
            conn.Close();

            string[] userCodes = stockCodes.Select(i => i.ToString()).ToArray();

            return userCodes;
        } 

        //inserts all stock codes/names into table (only used in first running of program)
        public void InsertStocks(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();

            string[,] companyCodes = LoadStockCode();
            int length = companyCodes.GetLength(1);

            for (int x = 0; x<length; x++)
            {
                string Symbol = companyCodes[0,x];
                string Name = companyCodes[1,x];

                string Command = String.Format("INSERT INTO StockCodesTable (Symbol, Name, Price) VALUES('{0}', '{1}', '0');", Symbol, Name);
                sqlite_cmd.CommandText = Command;
                sqlite_cmd.ExecuteNonQuery();

            }

            conn.Close();
        }

        //compares last recorded to current stock price to return their relative change in order to calculate
        //user PandL
        public string[,] RelChangeInStockPrice(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            DatabaseInteractions databaseAccess = new DatabaseInteractions();
            MATLABinteractions MATLABprices = new MATLABinteractions();
            double newPrice = -1;
            double oldPrice = -1;

            List<string> companyCodes = databaseAccess.ReturnStockCodes(databaseAccess.CreateConnection());
            int length = companyCodes.Count();
            double[] relativeChanges = new double[length];
            string[,] codesAndChanges = new string[2, length];

            for (int x = 0; x<length; x++)
            {
                SQLiteDataReader sqlite_datareader;
                try
                {
                    newPrice = MATLABprices.Price(companyCodes[x]);
                }
                catch
                {
                    newPrice = -1;
                }
                string Command = String.Format("SELECT Price FROM StockCodesTable WHERE Symbol = '{0}';", companyCodes[x]);
                sqlite_cmd.CommandText = Command;
                sqlite_datareader = sqlite_cmd.ExecuteReader();
                try
                {
                    while (sqlite_datareader.Read())
                    {
                        oldPrice = sqlite_datareader.GetDouble(0);
                    }
                }
                catch (Exception ex)
                {
                    oldPrice = -1;
                }
                sqlite_datareader.Close();

                relativeChanges[x] = newPrice / oldPrice;
                codesAndChanges[0,x] = companyCodes[x];
                codesAndChanges[1,x] = Convert.ToString(relativeChanges[x]);
            }
            conn.Close();
            return codesAndChanges;
        }

        //updates the value of each position owned by users based of the relative change in stock price
        public void UpdateUserPositions(SQLiteConnection conn, string[,] stockChanges) 
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            SQLiteDataReader sqlite_datareader;
            DoubleLinkList<string> emails = new DoubleLinkList<string>();
            DoubleLinkList<string> boughtSymbols = new DoubleLinkList<string>();
            DoubleLinkList<double> values = new DoubleLinkList<double>();
            DoubleLinkList<string> codes = new DoubleLinkList<string>();
            DoubleLinkList<double> changes = new DoubleLinkList<double>();
            DateTime MyDateTime = DateTime.Now;
            string sqlFormattedDate = MyDateTime.ToString("yyyy-MM-dd");

            for (int x = 0; x < stockChanges.GetLength(1); x++)
            {
                codes.AddBack(stockChanges[0,x]);
                changes.AddBack(Convert.ToDouble(stockChanges[1,x]));
            }

            string Command = String.Format("SELECT Email, StockSymbol, Value FROM UserPositionsTable;");
            sqlite_cmd.CommandText = Command;
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            try
            {
                while (sqlite_datareader.Read())
                {
                    emails.AddBack(sqlite_datareader.GetString(0));
                    boughtSymbols.AddBack(sqlite_datareader.GetString(1));
                    values.AddBack(sqlite_datareader.GetDouble(2));
                }
            }
            catch (Exception ex)
            {
            }
            sqlite_datareader.Close();
            SQLiteDataReader sqlite_datareader1;

            for (int x = 0; x < emails.GetLength(); x++)
            {
                int codeIndex = codes.IndexOf(boughtSymbols.GetNode(x).DisplayNode());
                double stockChange = changes.GetNode(x).DisplayNode();
                double newValue = values.GetNode(x).DisplayNode() * stockChange;
                double currentPandL = 0;

                Command = String.Format("SELECT TotalPandL FROM LoginTable WHERE Email = '{0}';", emails.GetNode(x).DisplayNode());
                sqlite_cmd.CommandText = Command;
                sqlite_datareader1 = sqlite_cmd.ExecuteReader();
                try
                {
                    while (sqlite_datareader1.Read())
                    {
                        currentPandL = sqlite_datareader1.GetDouble(0);
                    }
                }
                catch (Exception ex)
                {
                }
                sqlite_datareader1.Close();

                double newPandL = currentPandL + newValue - values.GetNode(x).DisplayNode();

                Command = String.Format("UPDATE LoginTable SET TotalPandL = '{0}' WHERE Email = '{1}';", newPandL, emails.GetNode(x).DisplayNode());
                sqlite_cmd.CommandText = Command;
                sqlite_cmd.ExecuteNonQuery();

                Command = String.Format("UPDATE UserPositionsTable SET Value = '{0}', LastUpdate = '{1}' WHERE Email = '{2}' AND StockSymbol = '{3}';", newValue, sqlFormattedDate, emails.GetNode(x).DisplayNode(), boughtSymbols.GetNode(x).DisplayNode());
                sqlite_cmd.CommandText = Command;
                sqlite_cmd.ExecuteNonQuery();
            }

            conn.Close();
        }

        //returns a specified user's PandL
        public double ReturnUserPandL(SQLiteConnection conn, string email)
        {
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = String.Format("SELECT TotalPandL FROM LoginTable WHERE EMAIL = '{0}';", email);

            double pandL = 0;
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            try
            {
                while (sqlite_datareader.Read())
                {
                    pandL = sqlite_datareader.GetDouble(0);
                }
            }
            catch (Exception ex)
            {
                pandL = 0;
            }
            sqlite_datareader.Close();
            conn.Close();
            return pandL;
        }

        //updates stock table to include most recent prices
        public void UpdateStockPrices(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            MATLABinteractions MATLABprices = new MATLABinteractions();
            DatabaseInteractions databaseAccess = new DatabaseInteractions();
            double price;

            List<string> companyCodes = databaseAccess.ReturnStockCodes(databaseAccess.CreateConnection());
            int length = companyCodes.Count();

            for (int x = 0; x < length; x++)
            {
                try
                {
                    price = MATLABprices.Price(companyCodes[x]);
                }
                catch
                {
                    price = -1;
                }
                string Command = String.Format("UPDATE StockCodesTable SET Price = '{0}' WHERE Symbol = '{1}';", price, companyCodes[x]);
                sqlite_cmd.CommandText = Command;
                sqlite_cmd.ExecuteNonQuery();
            }

            conn.Close();
        }

        //adds the details of a new user to the LoginTable table
        public void InsertLoginData(SQLiteConnection conn, string email, string password)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();

            string Command = String.Format("INSERT INTO LoginTable (Email, Password, WatchStock1, WatchStock2, WatchStock3, TotalPandL) VALUES('{0}', '{1}', '{3}','{3}', '{3}','{2}');", email, password, 0, " ");

            sqlite_cmd.CommandText = Command;
            sqlite_cmd.ExecuteNonQuery();

            conn.Close();
        }

        //updates UserPositionsTable table after a user buys a stock
        public void InsertBuy(SQLiteConnection conn, string email, string stockCode, double volume, double currentValue)    
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            string Command = "";

            DateTime myDateTime = DateTime.Now;
            string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd");

            double newValue = currentValue + volume;
            if (currentValue == 0)
            {
                Command = String.Format("INSERT INTO UserPositionsTable (Email, StockSymbol, Value, LastUpdate) VALUES('{0}', '{1}', '{2}', '{3}');", email, stockCode, newValue, sqlFormattedDate);
            }
            else
            {
                Command = String.Format("UPDATE UserPositionsTable SET Email = '{0}', StockSymbol = '{1}', Value = '{2}', LastUpdate = '{3}' WHERE Email = '{0}' AND StockSymbol = '{1}';",email, stockCode, newValue, sqlFormattedDate);
            }

            sqlite_cmd.CommandText = Command;
            sqlite_cmd.ExecuteNonQuery();

            conn.Close();
        }

        //updates UserPositionsTable table after a user sells a stock
        public void InsertSell(SQLiteConnection conn, string email, string stockCode, double volume, double currentValue)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();

            DateTime MyDateTime = DateTime.Now;
            string sqlFormattedDate = MyDateTime.ToString("yyyy-MM-dd");

            double newValue = currentValue - volume;

            if (newValue < 0) 
            {
                newValue = 0;
            }

            string Command = String.Format("UPDATE UserPositionsTable SET Email = '{0}', StockSymbol = '{1}', Value = '{2}', LastUpdate = '{3}' WHERE Email = '{0}' AND StockSymbol = '{1}';", email, stockCode, newValue, sqlFormattedDate);

            sqlite_cmd.CommandText = Command;
            sqlite_cmd.ExecuteNonQuery();

            conn.Close();
        }

        //finds the value of a user's position for a specified stock
        public double ReturnValue(SQLiteConnection conn, string email, string StockCode)
        {
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = String.Format("SELECT Value FROM UserPositionsTable WHERE EMAIL = '{0}' AND StockSymbol = '{1}'", email, StockCode);

            double value = 0;
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            try
            {  
                while (sqlite_datareader.Read())
                {
                    value = sqlite_datareader.GetDouble(0);
                }  
            }
            catch (Exception ex)
            {
                value = 0;
            }
            sqlite_datareader.Close();
            conn.Close();
            return value;
        }

        //returns most recent recorded stock price to be used on stock screen
        public double ReturnPrice(SQLiteConnection conn, string stockCode)
        {
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader sqlite_datareader;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = String.Format("SELECT Price FROM StockCodesTable WHERE Symbol = '{0}'", stockCode);

            double price = 0;
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            try
            {
                while (sqlite_datareader.Read())
                {
                    price = sqlite_datareader.GetDouble(0);
                }
            }
            catch (Exception ex)
            {
                price = -1;
            }
            sqlite_datareader.Close();
            conn.Close();
            return price;
        }

        //compares entered details with those in the LoginTable table in order to allow or deny user access
        public bool VerifyLoginData(SQLiteConnection conn, string email, string password)
        {
            bool validLogin = false;

            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT Email, Password FROM LoginTable";

            DoubleLinkList<string> emails = new DoubleLinkList<string>();
            DoubleLinkList<string> passwords = new DoubleLinkList<string>();
            int positionEmail = -1;
            int positionPassword = -1;

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                emails.AddBack(sqlite_datareader.GetString(0));
                passwords.AddBack(sqlite_datareader.GetString(1));
            }

            try
            {
                positionEmail = emails.IndexOf(email);
                positionPassword = passwords.IndexOf(password);
                if (positionEmail == positionPassword && (positionEmail !=-1 || positionPassword!=-1)) 
                {
                    validLogin = true;
                }
            }
            catch (Exception ex)
            {
                validLogin=false;
            }
            sqlite_datareader.Close();
            conn.Close();

            return validLogin;
        }

        //verifies that an email is not already in use
        public bool CheckValidEmail(SQLiteConnection conn, string email)
        {
            bool validEmail = false;

            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT Email FROM LoginTable";

            DoubleLinkList<string> emails = new DoubleLinkList<string>();

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                emails.AddBack(sqlite_datareader.GetString(0));
            }

            if (emails.Contains(email) == true)
            {
                validEmail = false;
            }
            else
            {
                validEmail=true;
            }

            sqlite_datareader.Close();
            conn.Close();
            return validEmail;
        }

        //edits the LoginTable table after a user updates their email
        public void EditEmail(SQLiteConnection conn, string emailNew, string emailOld)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();

            string command = String.Format("UPDATE LoginTable SET Email = '{0}' WHERE Email = '{1}';", emailNew, emailOld);

            sqlite_cmd.CommandText = command;
            sqlite_cmd.ExecuteNonQuery();

            conn.Close();
        }

        //edits the LoginTable table after a user updates their password
        public void EditPassword(SQLiteConnection conn, string password, string email)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();

            string Command = String.Format("UPDATE LoginTable SET Password = '{0}' WHERE Email = '{1}';", password, email);

            sqlite_cmd.CommandText = Command;
            sqlite_cmd.ExecuteNonQuery();

            conn.Close();
        }

        //searches for any matches of stock to those entered by user
        public string StockSearch(SQLiteConnection conn, string stockToFind)
        {
            string stockCode = "";
            DoubleLinkList<string> stockCodes = new DoubleLinkList<string>();

            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = String.Format("SELECT Symbol FROM StockCodesTable WHERE Name LIKE '{0}%' OR Symbol LIKE '{0}%'", stockToFind);
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                stockCodes.AddBack(sqlite_datareader.GetString(0));
            }

            try
            {
                stockCode = stockCodes.GetNode(0).DisplayNode();
            }
            catch (Exception e)
            {
                stockCode = "";
            }
            sqlite_datareader.Close();
            conn.Close();

            return stockCode;
        }

        //converts from the code of a stock to the name of the company
        public string GetNameFromCode(SQLiteConnection conn, string code)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();

            sqlite_cmd.CommandText = String.Format("SELECT Name FROM StockCodesTable WHERE Symbol = '{0}';", code);
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            DoubleLinkList <string> names = new DoubleLinkList<string>();
            string stockName = "";


            while (sqlite_datareader.Read())
            {
                names.AddBack(sqlite_datareader.GetString(0));
            }

            try
            {
                stockName = names.GetNode(0).DisplayNode();
            }
            catch (Exception e)
            {
                stockName = "";
            }
            sqlite_datareader.Close();
            conn.Close();

            return stockName;
        }

        //converts from the name of a company to its stock code
        public string GetCodeFromName(SQLiteConnection conn, string name)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();

            sqlite_cmd.CommandText = String.Format("SELECT Symbol FROM StockCodesTable WHERE Name = '{0}';", name);
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            DoubleLinkList<string> codes = new DoubleLinkList<string>();
            string stockCode = "";


            while (sqlite_datareader.Read())
            {
                codes.AddBack(sqlite_datareader.GetString(0));
            }

            try
            {
                stockCode = codes.GetNode(0).DisplayNode();
            }
            catch (Exception e)
            {
                stockCode = "";
            }
            sqlite_datareader.Close();
            conn.Close();

            return stockCode;
        }

        //returns the 3 most valuable positions currently held by a user
        public string[] ReturnUserTopStocks(SQLiteConnection conn, string email)
        {
            string[] stockSymbols = new string[3];
            DoubleLinkList<string> stockCodes = new DoubleLinkList<string>();

            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = String.Format("SELECT StockSymbol FROM UserPositionsTable WHERE Email = '{0}' ORDER BY VALUE DESC", email);
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                stockCodes.AddBack(sqlite_datareader.GetString(0));
            }

            try
            {
                stockSymbols[0] = stockCodes.GetNode(0).DisplayNode();
                try
                {
                    stockSymbols[1] = stockCodes.GetNode(1).DisplayNode();
                    try
                    {
                        stockSymbols[2] = stockCodes.GetNode(2).DisplayNode();
                    }
                    catch { }
                }
                catch { }
            }
            catch (Exception e)
            {
            }
            sqlite_datareader.Close();
            conn.Close();

            return stockSymbols;
        }

        //adds a stock to the users watchlist
        public string AddWatchlist(SQLiteConnection conn, string stockCode, string email)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;

            DoubleLinkList<string> stockCodes = new DoubleLinkList<string>();

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = String.Format("SELECT WatchStock1 FROM LoginTable WHERE Email = '{0}';", email);
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            try
            {
                while (sqlite_datareader.Read())
                {
                    stockCodes.AddBack(sqlite_datareader.GetString(0));
                }
                if (stockCodes.GetNode(0).DisplayNode() == " " || stockCodes.GetNode(0).DisplayNode() == "" || stockCodes.GetNode(0).DisplayNode() == null)
                {
                    sqlite_cmd = conn.CreateCommand();
                    sqlite_cmd.CommandText = String.Format("UPDATE LoginTable SET WatchStock1 = '{0}' WHERE Email = '{1}';", stockCode, email);
                    sqlite_datareader = sqlite_cmd.ExecuteReader();
                    sqlite_datareader.Close();
                    conn.Close();
                    return "WatchStock1";
                }
                else
                {
                    sqlite_cmd = conn.CreateCommand();
                    sqlite_cmd.CommandText = String.Format("SELECT WatchStock2 FROM LoginTable WHERE Email = '{0}';", email);
                    sqlite_datareader = sqlite_cmd.ExecuteReader();

                    try
                    {
                        while (sqlite_datareader.Read())
                        {
                            stockCodes.AddBack(sqlite_datareader.GetString(0));
                        }

                        if (stockCodes.GetNode(1).DisplayNode() == " " || stockCodes.GetNode(1).DisplayNode() == "" || stockCodes.GetNode(1).DisplayNode() == null)
                        {
                            sqlite_cmd = conn.CreateCommand();
                            sqlite_cmd.CommandText = String.Format("UPDATE LoginTable SET WatchStock2 = '{0}' WHERE Email = '{1}';", stockCode, email);
                            sqlite_datareader = sqlite_cmd.ExecuteReader();
                            sqlite_datareader.Close();
                            conn.Close();
                            return "WatchStock2";
                        }
                        else
                        {
                            sqlite_cmd = conn.CreateCommand();
                            sqlite_cmd.CommandText = String.Format("SELECT WatchStock3 FROM LoginTable WHERE Email = '{0}';", email);
                            sqlite_datareader = sqlite_cmd.ExecuteReader();

                            try
                            {
                                while (sqlite_datareader.Read())
                                {
                                    stockCodes.AddBack(sqlite_datareader.GetString(0));
                                }
                                if (stockCodes.GetNode(2).DisplayNode() == " " || stockCodes.GetNode(2).DisplayNode() == "" || stockCodes.GetNode(2).DisplayNode() == null)
                                {
                                    sqlite_cmd = conn.CreateCommand();
                                    sqlite_cmd.CommandText = String.Format("UPDATE LoginTable SET WatchStock3 = '{0}' WHERE Email = '{1}';", stockCode, email);
                                    sqlite_datareader = sqlite_cmd.ExecuteReader();
                                    sqlite_datareader.Close();
                                    conn.Close();
                                    return "WatchStock3";
                                }
                                else
                                {
                                    return "Watchlist Full";
                                }
                            }
                            catch
                            {
                                return "Watchlist Full";
                            }
                        }
                    }
                    catch
                    {
                        return "Watchlist Full";
                    }
                }
            }
            catch (Exception e)
            {
                return "Watchlist Full";
            }
        }

        //returns a users watchlist
        public string[] ReturnUserWatchlist(SQLiteConnection conn, string email)
        {
            string[] codes = new string[3];

            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = String.Format("SELECT WatchStock1 FROM LoginTable WHERE Email = '{0}';", email);
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                codes[0] = sqlite_datareader.GetString(0);
            }

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = String.Format("SELECT WatchStock2 FROM LoginTable WHERE Email = '{0}';", email);
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                codes[1] = sqlite_datareader.GetString(0);
            }

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = String.Format("SELECT WatchStock3 FROM LoginTable WHERE Email = '{0}';", email);
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                codes[2] = sqlite_datareader.GetString(0);
            }

            sqlite_datareader.Close();
            conn.Close();
            return codes;
        }

        //returns all the stock codes
        public List<string> ReturnStockCodes(SQLiteConnection conn)
        {
            List<string> stockCodes = new List<string>();

            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = String.Format("SELECT Symbol FROM StockCodesTable;");
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
               stockCodes.Add(sqlite_datareader.GetString(0)); 
            }
            sqlite_datareader.Close();
            conn.Close();

            return stockCodes;
        }

        //returns all the names of companies and their corresponding stock codes
        static string[,] LoadStockCode()
        {
            List<string> StockCodes = new List<string>();
            List<string> StockNames = new List<string>();
            string filePath =
            @"C:\Users\olesc\OneDrive\Documents\MATLAB\MATLABData\CondensedStockCodes.csv";
            StreamReader reader = null;
            int counter = 0;

            if (File.Exists(filePath))
            {
                reader = new StreamReader(File.OpenRead(filePath));
 
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    foreach (var item in values)
                    {
                        if (counter%2 == 0)
                        {
                            StockCodes.Add(item);
                        }
                        else
                        {
                            StockNames.Add(item);
                        }
                        counter++;
                    }
                }
            }
            else
            {
                Console.WriteLine("File doesn't exist");
            }

            counter = 0;
            string[,] codesAndNames = new string[2,StockCodes.Count];

            foreach (var item in StockCodes)
            {
                codesAndNames[0, counter] = StockCodes[counter];
                codesAndNames[1,counter] = StockNames[counter];
                counter++;
            }

            return codesAndNames;
        }


    }
}
