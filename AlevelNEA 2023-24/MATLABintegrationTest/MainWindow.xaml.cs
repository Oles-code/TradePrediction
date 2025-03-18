using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Controls;
using System.Printing;
using System.Data.Entity;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using ScottPlot;
using ScottPlot.WPF;
using Color = System.Drawing.Color;
using ScottPlot.Plottable;
using System.Net.NetworkInformation;

namespace MATLABintegrationTest
{
    /// <summary>
    /// Interaction logic for all of the pages
    /// </summary>
    public partial class MainWindow
    {

        //defining variables global to this class

        string email = "";
        string password = "";
        string stockCodeG = "";
        string stockNameG = "";
        string[] hotStocks = new string[3];
        string[,] notifications = new string[4,2];

        public MainWindow()
        {
            InitializeComponent();

            DatabaseInteractions database = new DatabaseInteractions();

            //database.CreateTable(database.CreateConnection());
            //database.InsertStocks(database.CreateConnection());
            string[,] stockChanges = database.RelChangeInStockPrice(database.CreateConnection());
            database.UpdateUserPositions(database.CreateConnection(), stockChanges);


            MATLABinteractions MATLABloader = new MATLABinteractions();
            
            hotStocks = MATLABloader.FindHotStocks();
            TopLStock.Content = database.GetNameFromCode(database.CreateConnection(), hotStocks[0]);
            TopMStock.Content = database.GetNameFromCode(database.CreateConnection(), hotStocks[1]);
            TopRStock.Content = database.GetNameFromCode(database.CreateConnection(), hotStocks[2]);

            database.UpdateStockPrices(database.CreateConnection());

            TLPlot.Plot.Style(figureBackground: Color.Black);
            TLPlot.Plot.Style(dataBackground: Color.Black);
            TLPlot.Plot.Style(grid: Color.Black);
            TLPlot.Plot.Style(axisLabel: Color.WhiteSmoke);
            TLPlot.Plot.XAxis.Color(Color.WhiteSmoke);
            TLPlot.Plot.YAxis.Color(Color.WhiteSmoke);
            TLPlot.Plot.XLabel("Days");
            TLPlot.Plot.YLabel("Price/$");
            BMPlot.Refresh();

            TMPlot.Plot.Style(figureBackground: Color.Black);
            TMPlot.Plot.Style(dataBackground: Color.Black);
            TMPlot.Plot.Style(grid: Color.Black);
            TMPlot.Plot.Style(axisLabel: Color.WhiteSmoke);
            TMPlot.Plot.XAxis.Color(Color.WhiteSmoke);
            TMPlot.Plot.YAxis.Color(Color.WhiteSmoke);
            TMPlot.Plot.XLabel("Days");
            TMPlot.Plot.YLabel("Price/$");
            BMPlot.Refresh();

            TRPlot.Plot.Style(figureBackground: Color.Black);
            TRPlot.Plot.Style(dataBackground: Color.Black);
            TRPlot.Plot.Style(grid: Color.Black);
            TRPlot.Plot.Style(axisLabel: Color.WhiteSmoke);
            TRPlot.Plot.XAxis.Color(Color.WhiteSmoke);
            TRPlot.Plot.YAxis.Color(Color.WhiteSmoke);
            TRPlot.Plot.XLabel("Days");
            TRPlot.Plot.YLabel("Price/$");
            BMPlot.Refresh();

            BLPlot.Plot.Style(figureBackground: Color.Black);
            BLPlot.Plot.Style(dataBackground: Color.Black);
            BLPlot.Plot.Style(grid: Color.Black);
            BLPlot.Plot.Style(axisLabel: Color.WhiteSmoke);
            BLPlot.Plot.XAxis.Color(Color.WhiteSmoke);
            BLPlot.Plot.YAxis.Color(Color.WhiteSmoke);
            BLPlot.Plot.XLabel("Days");
            BLPlot.Plot.YLabel("Price/$");
            BMPlot.Refresh();

            BMPlot.Plot.Style(figureBackground: Color.Black);
            BMPlot.Plot.Style(dataBackground: Color.Black);
            BMPlot.Plot.Style(grid: Color.Black);
            BMPlot.Plot.Style(axisLabel: Color.WhiteSmoke);
            BMPlot.Plot.XAxis.Color(Color.WhiteSmoke);
            BMPlot.Plot.YAxis.Color(Color.WhiteSmoke);
            BMPlot.Plot.XLabel("Days");
            BMPlot.Plot.YLabel("Price/$");
            BMPlot.Refresh();

            BRPlot.Plot.Style(figureBackground: Color.Black);
            BRPlot.Plot.Style(dataBackground: Color.Black);
            BRPlot.Plot.Style(grid: Color.Black);
            BRPlot.Plot.Style(axisLabel: Color.WhiteSmoke);
            BRPlot.Plot.XAxis.Color(Color.WhiteSmoke);
            BRPlot.Plot.YAxis.Color(Color.WhiteSmoke);
            BRPlot.Plot.XLabel("Days");
            BRPlot.Plot.YLabel("Price/$");
            BMPlot.Refresh();

            StockScreenPlot.Plot.Style(figureBackground: Color.Black);
            StockScreenPlot.Plot.Style(dataBackground: Color.Black);
            StockScreenPlot.Plot.Style(grid: Color.Black);
            StockScreenPlot.Plot.Style(axisLabel: Color.WhiteSmoke);
            StockScreenPlot.Plot.XAxis.Color(Color.WhiteSmoke);
            StockScreenPlot.Plot.YAxis.Color(Color.WhiteSmoke);
            StockScreenPlot.Plot.XLabel("Days");
            StockScreenPlot.Plot.YLabel("Price/$");
            StockScreenPlot.Refresh();

            double[] priceForPlot = MATLABloader.PlotData(hotStocks[0]);
            priceForPlot = priceForPlot.Where(T => T != 0).ToArray();
            int[] numberOfDays = Enumerable.Range(1, priceForPlot.Length).ToArray();
            double[] days = numberOfDays.Select(x => (double)x).ToArray();

            TLPlot.Plot.AddScatter(days, priceForPlot);
            TLPlot.Refresh();

            priceForPlot = MATLABloader.PlotData(hotStocks[1]);
            priceForPlot = priceForPlot.Where(T => T != 0).ToArray();
            numberOfDays = Enumerable.Range(1, priceForPlot.Length).ToArray();
            days = numberOfDays.Select(x => (double)x).ToArray();

            TMPlot.Plot.AddScatter(days, priceForPlot);
            TMPlot.Refresh();

            priceForPlot = MATLABloader.PlotData(hotStocks[2]);
            priceForPlot = priceForPlot.Where(T => T != 0).ToArray();
            numberOfDays = Enumerable.Range(1, priceForPlot.Length).ToArray();
            days = numberOfDays.Select(x => (double)x).ToArray();

            TRPlot.Plot.AddScatter(days, priceForPlot);
            TRPlot.Refresh();
        } 

        //Welcome Screen

        //performs verification of login details and loads all user specific metrics 
        private void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            DatabaseInteractions database = new DatabaseInteractions();
            MATLABinteractions MATLABloader = new MATLABinteractions();
            Notifications getNotifications = new Notifications();

            email = WelcomeEmailBox.Text;
            password = PasswordLogin.Password;
           
            
            if (database.VerifyLoginData(database.CreateConnection(), email, password) == true)
            {
                WelcomeScreen.Visibility = Visibility.Collapsed;
                HomeScreen.Visibility = Visibility.Visible;

                AccountScreenEmail.Text = email;
                AccountScreenPassword.Text = password;

                SearchedStock.Visibility = Visibility.Collapsed;

                string[] userTopSymbols = database.ReturnUserTopStocks(database.CreateConnection(), email);
                int counter = 0;
                string[] userTopNames = new string[3]; 

                while (counter < 3 && (userTopSymbols[counter] != null || userTopSymbols[counter] != ""))
                {
                    userTopNames[counter] = database.GetNameFromCode(database.CreateConnection(), userTopSymbols[counter]);
                    counter++;
                }
                BottomLStock.Content = userTopNames[0];
                BottomMStock.Content = userTopNames[1];
                BottomRStock.Content = userTopNames[2];
                YourStock1.Content = userTopNames[0];
                YourStock2.Content = userTopNames[1];
                YourStock3.Content = userTopNames[2];

                double[] priceForPlot;
                int[] numberOfDays;
                double[] days;

                if (userTopSymbols[0] != null)
                {
                    priceForPlot = MATLABloader.PlotData(userTopSymbols[0]);
                    priceForPlot = priceForPlot.Where(T => T != 0).ToArray();
                    numberOfDays = Enumerable.Range(1, priceForPlot.Length).ToArray();
                    days = numberOfDays.Select(x => (double)x).ToArray();

                    BLPlot.Plot.AddScatter(days, priceForPlot);
                    BLPlot.Refresh();
                }
                

                if (userTopSymbols[1] != null) 
                {
                    priceForPlot = MATLABloader.PlotData(userTopSymbols[1]);
                    priceForPlot = priceForPlot.Where(T => T != 0).ToArray();
                    numberOfDays = Enumerable.Range(1, priceForPlot.Length).ToArray();
                    days = numberOfDays.Select(x => (double)x).ToArray();

                    BMPlot.Plot.AddScatter(days, priceForPlot);
                    BMPlot.Refresh();
                }
                

                if (userTopSymbols[2] != null)
                {
                    priceForPlot = MATLABloader.PlotData(userTopSymbols[2]);
                    priceForPlot = priceForPlot.Where(T => T != 0).ToArray();
                    numberOfDays = Enumerable.Range(1, priceForPlot.Length).ToArray();
                    days = numberOfDays.Select(x => (double)x).ToArray();

                    BRPlot.Plot.AddScatter(days, priceForPlot);
                    BRPlot.Refresh();
                }

                notifications = getNotifications.ReturnStockNotifications(email);

                FirstNotification.Content = String.Format("{0} - {1}", notifications[0,0], notifications[0,1]);
                SecondNotification.Content = String.Format("{0} - {1}", notifications[1, 0], notifications[1, 1]);
                ThirdNotification.Content = String.Format("{0} - {1}", notifications[2, 0], notifications[2, 1]);
                FourthNotification.Content = String.Format("{0} - {1}", notifications[3, 0], notifications[3, 1]);

                if (notifications[0,0] == null || notifications[0,0] == "" || notifications[0,0] == " ")
                {
                    FirstNotification.IsEnabled = false;
                }
                else
                {
                    FirstNotification.IsEnabled = true;
                }
                if (notifications[1, 0] == null || notifications[1, 0] == "" || notifications[1, 0] == " ")
                {
                    SecondNotification.IsEnabled = false;
                }
                else
                {
                    SecondNotification.IsEnabled = true;
                }
                if (notifications[2, 0] == null || notifications[2, 0] == "" || notifications[2, 0] == " ")
                {
                    ThirdNotification.IsEnabled = false;
                }
                else
                {
                    ThirdNotification.IsEnabled = true;
                }
                if (notifications[3, 0] == null || notifications[3, 0] == "" || notifications[3, 0] == " ")
                {
                    FourthNotification.IsEnabled = false;
                }
                else
                {
                    FourthNotification.IsEnabled = true;
                }

                string[] watchlist = database.ReturnUserWatchlist(database.CreateConnection(), email);
                WatchListStock1.Content = database.GetNameFromCode(database.CreateConnection(), watchlist[0]);
                WatchListStock2.Content = database.GetNameFromCode(database.CreateConnection(), watchlist[1]);
                WatchListStock3.Content = database.GetNameFromCode(database.CreateConnection(), watchlist[2]);
                if (watchlist[0] == null || watchlist[0] == "" || watchlist[0] == " ")
                {
                    WatchListStock1.IsEnabled = false;
                }
                else
                {
                    WatchListStock1.IsEnabled = true;
                }
                if (watchlist[1] == null || watchlist[1] == "" || watchlist[1] == " ")
                {
                    WatchListStock2.IsEnabled = false;
                }
                else
                {
                    WatchListStock2.IsEnabled = true;
                }
                if (watchlist[2] == null || watchlist[2] == "" || watchlist[2] == " ")
                {
                    WatchListStock3.IsEnabled = false;
                }
                else
                {
                    WatchListStock3.IsEnabled = true;
                }

                double pandL = database.ReturnUserPandL(database.CreateConnection(), email);
                AccountPandL.Text = String.Format("${0:.00}", pandL);
            }
            else
            {
                InvalidLogin.Visibility = Visibility.Visible;
            }
           
        }

        //loads the create account screen
        private void Button_Click_CreateAccountScreen(object sender, RoutedEventArgs e)
        {
            WelcomeScreen.Visibility = Visibility.Collapsed;
            AccountCreation.Visibility = Visibility.Visible;
        }

        //hides password message
        private void passwordBoxLogin_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordLogin.Password))
                PassWordTextLogin.Visibility = Visibility.Visible;
            else
                PassWordTextLogin.Visibility = Visibility.Collapsed;
        }

        //closes program
        private void CloseProgramButtonWelcomeScreen_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        //Create Account

        //returns to login page from account creation page
        private void Button_Click_BackToLogin(object sender, RoutedEventArgs e)
        {
            WelcomeScreen.Visibility = Visibility.Visible;
            AccountCreation.Visibility = Visibility.Collapsed;
        }

        //adds account details to database
        private void Button_Click_CreateAccount(object sender, RoutedEventArgs e)
        {
            DatabaseInteractions Database = new DatabaseInteractions();
            email = EmailCreateAccountBox.Text;
            password = PasswordCreateAccountBox.Password;

            if (Database.CheckValidEmail(Database.CreateConnection(), email) == true)
            {
                AccountCreation.Visibility = Visibility.Collapsed;
                HomeScreen.Visibility = Visibility.Visible;

                AccountScreenEmail.Text = email;
                AccountScreenPassword.Text = password;

                FirstNotification.Content = "";
                FirstNotification.IsEnabled = false;
                SecondNotification.Content = "";
                SecondNotification.IsEnabled = false;
                ThirdNotification.Content = "";
                ThirdNotification.IsEnabled = false;
                FourthNotification.Content = "";
                FourthNotification.IsEnabled = false;

                SearchedStock.Visibility = Visibility.Collapsed;

                Database.InsertLoginData(Database.CreateConnection(), email, password);
            }
            else
            {
                EmailUsedMessage.Visibility = Visibility.Visible;
            }

            WatchListStock1.IsEnabled = false;
            WatchListStock2.IsEnabled = false;
            WatchListStock3.IsEnabled = false;

            FirstNotification.IsEnabled = false;
            SecondNotification.IsEnabled = false;
            ThirdNotification.IsEnabled = false;
            FourthNotification.IsEnabled = false;
        }

        //hides password message
        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordCreateAccountBox.Password))
                PassWordText.Visibility = Visibility.Visible;
            else
                PassWordText.Visibility = Visibility.Collapsed;
        }

        //closes program
        private void CloseProgramButtonAccountScreen_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        //Home Screen

        //opens notifications page
        private void Button_Click_Notifications(object sender, RoutedEventArgs e)
        {
            NotificationScreen.Visibility = Visibility.Visible;
        }

        //closes program
        private void CloseProgramButtonHomeScreen_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //opens top left stock (hot stock 1)
        private void TopLStock_Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(Convert.ToString(TopLStock.Content));
        }

        //opens top middle stock (hot stock 2)
        private void TopMStock_Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(Convert.ToString(TopMStock.Content));
        }

        //opens top right stock (hot stock 3)
        private void TopRStock_Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(Convert.ToString(TopRStock.Content));
        }

        //opens bottom left stock (user's most valuable stock)
        private void BottomLStock_Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(Convert.ToString(BottomLStock.Content));
        }

        //opens bottom middle stock (user's 2nd most valuable stock)
        private void BottomMStock_Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(Convert.ToString(BottomMStock.Content));
        }

        //opens bottom right stock (user's 3rd most valuable stock)
        private void BottomRStock_Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(Convert.ToString(BottomRStock.Content));
        }

        //hides search bar message
        private void SearchBarTextChangedEventHandler(object sender, TextChangedEventArgs args)
        {
            if (string.IsNullOrEmpty(SearchBar.Text))
            {
                SearchBarText.Visibility = Visibility.Visible;
                SearchButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                SearchBarText.Visibility = Visibility.Collapsed;
                SearchButton.Visibility = Visibility.Visible;
            }
        }

        //triggers stock search sequence
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string stockToFind = SearchBar.Text;

            DatabaseInteractions Database = new DatabaseInteractions();
            string foundCode = Database.StockSearch(Database.CreateConnection(), stockToFind);
            stockCodeG = foundCode;

            stockNameG = Database.GetNameFromCode(Database.CreateConnection(), stockCodeG);

            if (foundCode == "" || foundCode == null)
            {
                SearchedStock.Content = "No found stock";
                SearchedStock.Visibility = Visibility.Visible;
                SearchedStock.IsEnabled = false;
            }
            else
            {
                //convert code to name
                SearchedStock.IsEnabled = true;
                SearchedStock.Content = foundCode;
                SearchedStock.Visibility = Visibility.Visible;
            }
        }

        //opens stock page of the searched stock
        public void SearchedStock_Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(stockNameG);
            SearchedStock.Visibility = Visibility.Collapsed;
        }

        //opens account page
        private void ButtonToAccountScreen_Click(object sender, RoutedEventArgs e)
        {
            HomeScreen.Visibility = Visibility.Collapsed;
            AccountScreen.Visibility = Visibility.Visible;
        }

        //opens first stock notification
        private void Noti1Click(object sender, RoutedEventArgs e)
        {
            //add if statement to check if company notification or stock notification
            OpenStockScreen(notifications[0, 0]);
        }

        //opens second stock notification
        private void Noti2Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(notifications[1,0]);
        }

        //opens third stock notification
        private void Noti3Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(notifications[2, 0]);
        }

        //opens fourth stock notification
        private void Noti4Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(notifications[3, 0]);
        }

        //Notification screen
        
        //closes the notification pop-down
        private void CloseNotifications_Click(object sender, RoutedEventArgs e)
        {
            NotificationScreen.Visibility = Visibility.Collapsed;
        }

        //Stock Screen

        //returns to home screen from stock screen
        private void BackButtonToHomeScreen_Click(object sender, RoutedEventArgs e)
        {
            HomeScreen.Visibility = Visibility.Visible;
            StockScreen.Visibility = Visibility.Collapsed;
            BuyGrid.Visibility = Visibility.Collapsed;
            SellGrid.Visibility = Visibility.Collapsed;
            SearchedStock.Visibility = Visibility.Collapsed;

            DatabaseInteractions Database = new DatabaseInteractions();

            double currentValue = Database.ReturnValue(Database.CreateConnection(), email, stockCodeG);
            CurrentPositionBox.Text = String.Format("${0:0.00}", currentValue);
        }

        //adds the current stock to the user's watchlist, if watchlist is not full
        private void AddToWatchlist_Click(object sender, RoutedEventArgs e)
        {
            DatabaseInteractions database = new DatabaseInteractions();
            string watchlistPos = database.AddWatchlist(database.CreateConnection(), stockCodeG, email);

            if (watchlistPos == "WatchStock1") 
            {
                WatchListStock1.Content = stockNameG;
                WatchListStock1.IsEnabled = true;
            }
            else if(watchlistPos == "WatchStock2")
            {
                WatchListStock2.Content = stockNameG;
                WatchListStock1.IsEnabled = true;
            }
            else if (watchlistPos == "WatchStock3")
            {
                WatchListStock3.Content = stockNameG;
                WatchListStock1.IsEnabled = true;
            }
            else
            {
                WatchlistError.Visibility = Visibility.Visible;
            }
        }

        //opens sell stock pop-down
        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            SellGrid.Visibility = Visibility.Visible;
        }

        //opens buy stock pop-down
        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            BuyGrid.Visibility = Visibility.Visible;
        }

        //writes the user's sell order to database
        private void ConfirmSell_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double SellVol = Convert.ToDouble(SellAmount.Text);

                DatabaseInteractions Database = new DatabaseInteractions();

                double OldValue = Database.ReturnValue(Database.CreateConnection(), email, stockCodeG);

                Database.InsertSell(Database.CreateConnection(), email, stockCodeG, SellVol, OldValue);

                double currentValue = Database.ReturnValue(Database.CreateConnection(), email, stockCodeG);
                CurrentPositionBox.Text = String.Format("${0:0.00}", currentValue);
            }
            catch
            {

            }

        }

        //writes the user's buy order to database
        private void ConfirmBuy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double BuyVol = Convert.ToDouble(BuyAmount.Text);

                DatabaseInteractions Database = new DatabaseInteractions();

                double OldValue = Database.ReturnValue(Database.CreateConnection(), email, stockCodeG);

                Database.InsertBuy(Database.CreateConnection(), email, stockCodeG, BuyVol, OldValue);

                double currentValue = Database.ReturnValue(Database.CreateConnection(), email, stockCodeG);
                CurrentPositionBox.Text = String.Format("${0:0.00}", currentValue);
            }
            catch
            {

            }
        }

        //closes user's buy pop-down
        private void CloseBuyGrid_Click(object sender, RoutedEventArgs e)
        {
            BuyGrid.Visibility= Visibility.Collapsed;
            BuyAmount.Text = "";
        }

        //closes user's sell pop-down
        private void CloseSellGrid_Click(object sender, RoutedEventArgs e)
        {
            SellGrid.Visibility = Visibility.Collapsed;
            SellAmount.Text = "";
        }

        //closes program
        private void CloseProgramButtonStockScreen_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Account screen

        //enables email editing
        private void EditEmailBox_Click(object sender, RoutedEventArgs e)
        {
            AccountScreenEmail.IsReadOnly = false;
            SaveEmail.Visibility = Visibility.Visible;
            AccountScreenEmailError.Visibility = Visibility.Collapsed;
        }

        //enables password editing
        private void EditPasswordBox_Click(object sender, RoutedEventArgs e)
        {
            AccountScreenPassword.IsReadOnly = false;
            SavePassword.Visibility = Visibility.Visible;
        }

        //writes new email to database
        private void SaveEmail_Click(object sender, RoutedEventArgs e)
        {
            AccountScreenEmail.IsReadOnly = true;
            SaveEmail.Visibility = Visibility.Collapsed;

            DatabaseInteractions Database = new DatabaseInteractions();

            bool validEmail = Database.CheckValidEmail(Database.CreateConnection(), AccountScreenEmail.Text);

            if (validEmail == true)
            {
                Database.EditEmail(Database.CreateConnection(), AccountScreenEmail.Text, email);
                email = AccountScreenEmail.Text;
                AccountScreenEmailError.Visibility = Visibility.Collapsed;
            }
            else
            {
                AccountScreenEmailError.Visibility = Visibility.Visible;
                AccountScreenEmail.Text = email;
            }
        }

        //writes new password to database
        private void SavePassword_Click(object sender, RoutedEventArgs e)
        {
            AccountScreenPassword.IsReadOnly = true;
            SavePassword.Visibility= Visibility.Collapsed;

            DatabaseInteractions Database = new DatabaseInteractions();

            password = AccountScreenPassword.Text;
            Database.EditPassword(Database.CreateConnection(), password, email);
        }

        //opens employee management pop-down
        private void ManageEmployees_Click(object sender, EventArgs e)
        {

        }

        //opens stock screens for watchlist stocks
        private void WatchList1_Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(Convert.ToString(WatchListStock1.Content));
        }

        private void WatchList2_Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(Convert.ToString(WatchListStock2.Content));
        }

        private void WatchList3_Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(Convert.ToString(WatchListStock3.Content));
        }

        //opens stock screens for user's most bought stocks
        private void YourStock1_Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(Convert.ToString(YourStock1.Content));
        }

        private void YourStock2_Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(Convert.ToString(YourStock2.Content));
        }

        private void YourStock3_Click(object sender, RoutedEventArgs e)
        {
            OpenStockScreen(Convert.ToString(YourStock3.Content));
        }

        //returns to home screen from account screen
        private void BackButtonToHomeScreenAccount_Click(object sender, RoutedEventArgs e)
        {
            HomeScreen.Visibility = Visibility.Visible;
            AccountScreen.Visibility = Visibility.Collapsed;
            SearchedStock.Visibility = Visibility.Collapsed;
        }

        //logs user out and returns to login page
        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            AccountScreen.Visibility= Visibility.Collapsed;
            WelcomeScreen.Visibility = Visibility.Visible;

            PasswordLogin.Password = "";
            WelcomeEmailBox.Text = "Enter Email";
            InvalidLogin.Visibility = Visibility.Collapsed;
            EmailCreateAccountBox.Text = "Enter Email";
            PasswordCreateAccountBox.Password = "";
            EmailUsedMessage.Visibility = Visibility.Collapsed;
            AccountScreenEmailError.Visibility = Visibility.Collapsed;

            BottomLStock.Content = "";
            BottomMStock.Content = "";
            BottomRStock.Content = "";
            YourStock1.Content = "";
            YourStock2.Content = "";
            YourStock3.Content = "";

            FirstNotification.Content = "";
            SecondNotification.Content = "";
            ThirdNotification.Content = "";
            FourthNotification.Content = "";

            WatchListStock1.IsEnabled = false;
            WatchListStock2.IsEnabled = false;
            WatchListStock3.IsEnabled = false;

            AccountPandL.Text = "";

            BLPlot.Plot.Clear();
            BLPlot.Refresh();
            BMPlot.Plot.Clear();
            BMPlot.Refresh();
            BRPlot.Plot.Clear();
            BRPlot.Refresh();
        }

        //re-runs machine learning algorithm in MATLAB
        private void RunML_Click(object sender, RoutedEventArgs e)
        {
            MATLABinteractions MATLABloader = new MATLABinteractions();
            MATLABloader.RunML();
        }

        //updates the data used in the prediction algorithms
        private void UpdateData_Click(object sender, RoutedEventArgs e)
        {
            MATLABinteractions matlab = new MATLABinteractions();
            matlab.UpdateData();
        }

        //General

        //general sequence for opening stock page
        private void OpenStockScreen(string stockButton)
        {
            HomeScreen.Visibility = Visibility.Collapsed;
            AccountScreen.Visibility = Visibility.Collapsed;
            NotificationScreen.Visibility = Visibility.Collapsed;
            StockScreen.Visibility = Visibility.Visible;
            WatchlistError.Visibility = Visibility.Collapsed;
            StockScreenTitle.Text = Convert.ToString(TopLStock.Content);
            stockNameG = Convert.ToString(stockButton);
            StockScreenTitle.Text = stockNameG;

            DatabaseInteractions database = new DatabaseInteractions();
            MATLABinteractions MATLABloader = new MATLABinteractions();

            stockCodeG = database.GetCodeFromName(database.CreateConnection(), stockNameG);

            double stockPrice = database.ReturnPrice(database.CreateConnection(), stockCodeG);
            CurrentPriceBox.Text = String.Format("${0:0.00}", stockPrice);

            double currentValue = database.ReturnValue(database.CreateConnection(), email, stockCodeG);
            CurrentPositionBox.Text = String.Format("${0:0.00}", currentValue);

            double[] priceForPlot = MATLABloader.PlotData(stockCodeG);
            priceForPlot = priceForPlot.Where(T => T != 0).ToArray();
            int[] numberOfDays = Enumerable.Range(1, priceForPlot.Length).ToArray();
            double[] days = numberOfDays.Select(x => (double)x).ToArray();

            StockScreenPlot.Plot.Clear();
            StockScreenPlot.Refresh();
            StockScreenPlot.Plot.AddScatter(days, priceForPlot);
            StockScreenPlot.Refresh();

            //run program to determine buy or sell
            string prediction = MATLABloader.Predict(stockCodeG);
            OurRecommendationBox.Text = prediction;
        }

    }
}
