%function which uses moving average convergence/divergence (MACD) in order
%to give entry or exit recommendation for stock

function enterExit = MACDCalculator(stockCode)

period26 = 26;
period12 = 12;
signalPeriod = 9;

numEMAdays = 5;

EMA26 = [];
EMA12 = [];
signalEMA = [];

%get data up to period26 + 22 back, to give 14 data points of EMA, 
%enough for 5 days of signal EMA to be calculated, and then the most
%recent 5 days of MACD and signal compared to check for entry/exit
%+30 as 5 days of signal, 9 days needed for each signal data point
%and excess 16 to account for some days not having data i.e. weekend

%getting current data for the stock
startDate = datetime('today') - days(period26+(numEMAdays+signalPeriod+20));
endDate = datetime('today');
stockDataAll= getMarketDataViaYahoo(stockCode, string(startDate), string(endDate), '1d');
stockDataAll = stockDataAll.AdjClose;
stockDataAll = flip(stockDataAll,1); 

for x = 1:(numEMAdays + signalPeriod)
    StockData26 = stockDataAll(x:x+period26-2,:);
    EMA26(x) = EMACalculator(StockData26,1);

    StockData12 = stockDataAll(x:x+period12-1,:);
    EMA12(x) = EMACalculator(StockData12,1);
end

MACD = EMA12(:) - EMA26(:);

for x = 1:numEMAdays
   signalEMA(x) = EMACalculator(MACD, x);
   signalEMA = signalEMA(:);
end

MACDSignalDif = MACD(1:numEMAdays,:) - signalEMA;
%Is there change in market, and if so buy or sell
%0 in first column is no change, 1 is change
%0 in second is no buy or sell, -1 is sell, 1 is buy
changeInMarket = [0,0];

for x = 1:numEMAdays-1
    sumofDif = abs(MACDSignalDif(x,1)+MACDSignalDif(x+1,1));
    MagnitudeDif = abs(MACDSignalDif(x,1)) + abs(MACDSignalDif(x+1,1));
    if sumofDif<MagnitudeDif
        changeInMarket(1,1) = 1;
        if MACDSignalDif(x,1) < 0
           changeInMarket(1,2) = 1; 
        else
            changeInMarket(1,2) = -1;
        end
    end
end

if changeInMarket(1,1) == 1
    if changeInMarket(1,2) == -1
        enterExit = "Sell";
    elseif changeInMarket(1,2) == 1
        enterExit = "Buy";
    else
        enterExit = "Hold";
    end
else
    enterExit = "Hold";
end
