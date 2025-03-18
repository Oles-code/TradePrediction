%function which returns the three most bought stocks in last day,
%the if statements are necessary as there is no data on weekends

function hotStocks = HotStocksFinder(stockCodes)

startDate = datetime('today') - 1;
pRaw = [];

for x = 1:length(stockCodes)
    stockData = getMarketDataViaYahoo(stockCodes{x}, startDate, datetime('today'), '1d');
    pRaw = [pRaw; stockData];
    if isempty(pRaw) == true
        startDate = startDate - 1;
        stockData = getMarketDataViaYahoo(stockCodes{x}, startDate, datetime('today'), '1d');
        pRaw = [pRaw; stockData];
        if isempty(pRaw) == true
            startDate = startDate - 1;
            stockData = getMarketDataViaYahoo(stockCodes{x}, startDate, datetime('today'), '1d');
            pRaw = [pRaw; stockData];
        end
    end
end

[~, sortIndex] = sort(pRaw.Volume, 'descend');
sortedStockCodes = stockCodes(sortIndex);
% return 3 highest volume stocks
hotStocks = sortedStockCodes(1:3);
