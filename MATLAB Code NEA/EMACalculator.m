%function which calculates the exponential moving average (EMA) of a
%stock's price

function EMA = EMACalculator(stockData, currentDay)

%the weighted multiplier used in each period
WMultiplier = 2/(size(stockData,1) + 1);

if currentDay == size(stockData,1)
   EMA = stockData(currentDay) * WMultiplier;
else
   EMA = stockData(currentDay)* WMultiplier + (1-WMultiplier)*EMACalculator(stockData, currentDay+1);
end
