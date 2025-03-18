%function to update the data stored on the stocks

function StockDataLoader()

dataDir = 'C:\Users\olesc\OneDrive\Documents\MATLAB\MATLABData\';
dataFile = 'RawStockData';
PrevData = readtable([dataDir, dataFile]);

subTables = StockDataSplitter(PrevData);
uniqueStocks = unique(PrevData.symbolsForMerge);
startDate = datetime(subTables{1}.Date(end));

rawData = [];

for x = 1:length(uniqueStocks)
    newRaw = getMarketDataViaYahoo(uniqueStocks{x}, string(startDate), string(datetime('today')), '1d');
    if (isempty(newRaw))
        %pause(1);
        continue;
    end
    newRawClean = removevars(newRaw,{'Open','Close', 'High','Low','Volume'});
    currentSymbol = uniqueStocks{x};
    symbolsForMerge = repmat({currentSymbol}, size(newRawClean, 1),1);
    symbolsForMerge = table(symbolsForMerge);
    readyData = horzcat(symbolsForMerge,newRawClean);
    readyData = vertcat(subTables{x},readyData);
    rawData = vertcat(rawData,readyData);
end

%writes the stock data to a csv
folderPath = 'C:\Users\olesc\OneDrive\Documents\MATLAB\MATLABData';
homedir = pwd;
cd(folderPath);
flnm = 'RawStockData.csv';   
fid = fopen(flnm,'wt+'); 
writetable(rawData, flnm)
fclose(fid);
cd(homedir); 
