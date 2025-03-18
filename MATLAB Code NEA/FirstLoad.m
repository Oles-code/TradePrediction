%function to load intial stock prices before running of full program

function FirstLoad(stockCodes)


StartDate = datetime('today') - 730;

RawData = [];
for x = 1:length(stockCodes)
    Newraw = getMarketDataViaYahoo(stockCodes{x}, string(StartDate), string(datetime('today')-30), '1d');
    if (isempty(Newraw))
        %pause(1);
        continue;
    end
    NewrawClean = removevars(Newraw,{'Open','Close', 'High','Low','Volume'});
    CurrentSymbol = stockCodes{x};
    symbolsForMerge = repmat({CurrentSymbol}, size(NewrawClean, 1),1);
    symbolsForMerge = table(symbolsForMerge);
    ReadyData = horzcat(symbolsForMerge,NewrawClean);
    %ReadyData = vertcat(subTables{x},ReadyData);
    RawData = vertcat(RawData,ReadyData);  
end


folderPath = 'C:\Users\olesc\OneDrive\Documents\MATLAB\MATLABData';
homedir = pwd;
cd(folderPath);
flnm = 'RawStockData.csv';   
fid = fopen(flnm,'wt+'); 
writetable(RawData, flnm)
fclose(fid);
cd(homedir);