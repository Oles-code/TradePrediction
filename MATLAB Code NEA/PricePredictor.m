%function which uses data from my algorithm in order to give 
%a buy or sell prediction

function price = PricePredictor(stockCode)

dataDir = 'C:\Users\olesc\OneDrive\Documents\MATLAB\';
dataFile = 'Poly3Found';
poly3Data = readtable([dataDir, dataFile]);

poly3Coefs = poly3Data{:, 1:4};
poly3ChangeSD = poly3Data{:, 5:6};
chngToSD = poly3ChangeSD(:,1)./poly3ChangeSD(:,2);


startDate = datetime('today') - days(30);
endDate = datetime('today');

polyOrder = 3;
polyTol = 0.1;
maxRangeOfChange = 0.2;

%Contains fit of each order of polynomial, 3,4,5 respectively in each cell
fits = cell(1,3);

%Getting data from Yahoo Finance and normalising the price
pRaw = getMarketDataViaYahoo(stockCode, string(startDate), string(endDate), '1d');

if isempty(pRaw) ~= true
    normalisedPrice = pRaw.AdjClose./pRaw.AdjClose(1);
else
    price = 'no data available for this stock';
end

Newdays = 1:size(normalisedPrice,1);
Newdays = Newdays(:);
for x = 1:3
    pNew = {polyfit(Newdays, normalisedPrice, polyOrder)};
    fits{1,x} = pNew;
    polyOrder = polyOrder + 1;
end

%tolerance of fit for comparing to previously identified trends
%mean is formatted to dimensions of previously found fits matrix
new3rdCoefs = fits{1,1};
%This weird line needs to be added as each cell in Fits is itself stored as
%a cell
new3rdCoefs = new3rdCoefs{1};
formattedMean = abs(new3rdCoefs(ones(size(poly3Coefs, 1),1),:));
tol = polyTol .* formattedMean;

%checks if there is a fit that has been found that match        
%the identified trend
difference = poly3Coefs(:,1:(size(pNew, 2))) - new3rdCoefs(ones(1, size(poly3Coefs,1)),:);
withinTol = abs(difference) <= tol;         
isPresent = sum(withinTol, 2);

%sees if a similar fit has already been logged
replaceIndex = find(isPresent == length(pNew));

potentialPredicts = poly3ChangeSD(replaceIndex,:);
potentialRatios = chngToSD(replaceIndex,:);

removeRatioIndex = find(abs(potentialRatios) == Inf);
removeRatioIndex = [removeRatioIndex; find(abs(potentialRatios) < 0.5)];

keepIndex = true(size(potentialRatios, 1), 1);
keepIndex(removeRatioIndex) = false;


potentialPredicts = potentialPredicts(keepIndex,:);
potentialPredicts = sortrows(potentialPredicts,1);
if size(potentialPredicts, 1) ~= 0
    if size(potentialPredicts, 1) == 1
        price = round(pRaw{1,"AdjClose"}*potentialPredicts(1,1),2);
        if price> 0
            price = "Buy";
        elseif price == 0
            price = "Hold";
        else
            price = "Sell";
        end 
    elseif (potentialPredicts(end,1)-potentialPredicts(1,1))>maxRangeOfChange
        price = "No consistent predictions from algorithm";
    else
        price = mean(potentialPredicts,1);
    end
else
    price = "No consistent predictions from algorithm";
end