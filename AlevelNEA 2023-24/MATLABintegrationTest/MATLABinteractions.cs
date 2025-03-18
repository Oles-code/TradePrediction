 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MATLABintegrationTest
{

    //This class deals with all usage of MATLAB functions in the C# code
    internal class MATLABinteractions
    {

        //re-runs my machine learning algorithm to produce updated predictor fits
        public void RunML()
        {
            MLApp.MLApp matlab = new MLApp.MLApp();
            DatabaseInteractions Database = new DatabaseInteractions();

            List<string> StockCodes = Database.ReturnStockCodes(Database.CreateConnection());
            object objStockCodes = StockCodes.Select(x => x as object).ToArray();

            matlab.Execute(@"cd C:\Users\olesc\OneDrive\Documents\MATLAB\");
            matlab.Feval("StockfitML", 0, out object nullObj);


            matlab.Quit();
        }
        
        //returns the three most bought stocks from the last open market day
        public string[] FindHotStocks()
        {
            MLApp.MLApp matlab = new MLApp.MLApp();
            DatabaseInteractions Database = new DatabaseInteractions();
            string[] threeStocks = new string[3];

            List<string> StockCodes = Database.ReturnStockCodes(Database.CreateConnection());
            object objStockCodes = StockCodes.Select(x => x as object).ToArray();

            matlab.Execute(@"cd C:\Users\olesc\OneDrive\Documents\MATLAB\");
            matlab.Feval("HotStocksFinder", 1, out object stocksOut, objStockCodes);
            try
            {
                object[]? hotStocks = stocksOut as object[];
                object[,] hotStockTest = hotStocks[0] as object[,];

                threeStocks[0] = hotStockTest[0,0].ToString();
                threeStocks[1] = hotStockTest[1,0].ToString();
                threeStocks[2] = hotStockTest[2,0].ToString();
            }
            catch
            {
               threeStocks = null;
            }
            matlab.Quit();


            return threeStocks;
        }

        //runs prediction on inputted stock, if custom alogrithm is unable to produce a prediction then
        //MACD calculator used as a substitute
        public string Predict(string stockCode)
        {
            string prediction = "";

            MLApp.MLApp matlab = new MLApp.MLApp();

            matlab.Execute(@"cd C:\Users\olesc\OneDrive\Documents\MATLAB\");
            matlab.Feval("PricePredictor", 1, out object predictionObj, stockCode);
            try
            {
                object[]? predictionObjArray = predictionObj as object[];
                prediction = (string)predictionObjArray[0];
            }
            catch
            {
            }

            if (prediction == "No consistent predictions from algorithm" || prediction == "")
            {
                matlab.Execute(@"cd C:\Users\olesc\OneDrive\Documents\MATLAB\");
                matlab.Feval("MACDCalculator", 1, out object predictionMACDObj, stockCode);
                try
                {
                    object[]? predictionMACDObjArray = predictionMACDObj as object[];
                    prediction = (string)predictionMACDObjArray[0];
                }
                catch
                {
                }
            }
            if (prediction == "" || prediction == null)
            {
                prediction = "Not available";
            }
            
            return prediction;
        }

        //returns the last available price of the desired stock
        public double Price(string stockCode)
        {
            MLApp.MLApp matlab = new MLApp.MLApp();
            double price = 0;

            matlab.Execute(@"cd C:\Users\olesc\OneDrive\Documents\MATLAB\");
            matlab.Feval("PriceReturner", 1, out object priceObj, stockCode);
            try
            {
                object[]? priceObjArray = priceObj as object[];
                price = Convert.ToDouble(priceObjArray[0]);
            }
            catch
            {
            }
            matlab.Quit();

            return price;
        }


        //loads new live data into local spreadsheet to be used for prediction algorithm
        public void UpdateData()
        {
            MLApp.MLApp matlab = new MLApp.MLApp();

            matlab.Execute(@"cd C:\Users\olesc\OneDrive\Documents\MATLAB\");
            matlab.Feval("StockDataLoader", 0, out object nullReturn);

            matlab.Quit();
        }

        //loads last 30 days of data to be used for plotting stock graphs
        public double[] PlotData(string stockCode)
        {
            double[] data = new double[31];
            MLApp.MLApp matlab = new MLApp.MLApp();

            matlab.Execute(@"cd C:\Users\olesc\OneDrive\Documents\MATLAB\");
            matlab.Feval("ReturnPlotData", 1, out object stockData, stockCode);

            object[]? priceObjArray = stockData as object[];
            double[,] priceArray = (double[,])priceObjArray[0];
            
            for (int i = 0; i < priceArray.Length; i++)
            {
                data[i] = Convert.ToDouble(priceArray[i,0]);
            }
            
            matlab.Quit();
            return data;
        }


        //runs the MACD calculator on the desired stock 
        public string MACDCalculator(string stockCode)
        {
            string entryExit = "";

            MLApp.MLApp matlab = new MLApp.MLApp();

            matlab.Execute(@"cd C:\Users\olesc\OneDrive\Documents\MATLAB\");
            matlab.Feval("MACDCalculator", 1, out object priceObj, stockCode);
            try
            {
                object[]? priceObjArray = priceObj as object[];
                entryExit = (string)priceObjArray[0];
            }
            catch
            {
            }
            matlab.Quit();

            return entryExit;
        }

    }
}
