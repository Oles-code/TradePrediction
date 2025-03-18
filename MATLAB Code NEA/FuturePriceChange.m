%finds the relative change in price after given time 

function CHNG = FuturePriceChange(startDate, TimeL,stockCode)
endDate = startDate + caldays(TimeL);
endDate = string(endDate);
startDate = string(startDate);
stockCode = convertStringsToChars(stockCode);
pRaw = getMarketDataViaYahoo(stockCode, startDate, endDate, '1d');
%need to normalise
if isempty(pRaw) ~= true
    relativeChange = pRaw.AdjClose(end)/pRaw.AdjClose(1);
    relativeGrowth = relativeChange-1;
    CHNG = relativeGrowth;
else
    CHNG = 'no data';
end