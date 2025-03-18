%function which returns the last 30 days of data to be used for graph
%plotting

function stockData = ReturnPlotData(stockCode)

startDate = datetime('today') - 31;

rawStockData = getMarketDataViaYahoo(stockCode, startDate, datetime('today'), '1d');
try
   newRawClean = removevars(rawStockData,{'Open','Close', 'High','Low','Volume', 'Date'});
   newRawClean = table2array(newRawClean);
catch
    newRawClean = {};
end


stockData = newRawClean;