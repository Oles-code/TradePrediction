%function which returns most recent value of a share of a specified stock

function price = PriceReturner(stockCode)

startDate = datetime('today')-4;
stockData = getMarketDataViaYahoo(stockCode, startDate, datetime('today'), '1d');
if isempty(stockData)
    price = -1;
else
    price = stockData.AdjClose(end);
end
