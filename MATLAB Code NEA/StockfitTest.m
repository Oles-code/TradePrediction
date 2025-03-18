%% Generic prediction model
%% take user input for how long in the future they would like?
%  to predict for (pass from c#) for model ReRun
clear
close all

%load all stock code
dataDir = 'C:\MATLABdata\';
dataFile = 'stocks-list';
StockSymbols = readtable([dataDir, dataFile]);

%time period for which user wants prediction
timeExtrap = 30;
%misc variables
polyOrder = 3;
polyTol = 0.1; %changed
%potentially look at turning points rather than just gof?

%number of extra columns required to contain extra data about fit
%1st is number of matching fits, 2nd is predicted relative change
%after x time
%3rd is volatility, 4th is sum of increase squared (needed for sd)
XtraCol = 4;
%number of iterations in simulation
numIter = 50;
%minimum proportion of iterations that a fit must have for it to
%be useful
minProportion = 0.04;
%fits that are found to be frequently repeated
%as well as their mean change and volatility
FitChangeVol = [];
%minimum relative change in price for a stock to be valid when predicting
%adaptive for different lengths of extrapolation?
minChange = 0.1;
%maximum volatility relative to growth
maxrVol = 0.5;

poly3Found = ones(1,XtraCol+polyOrder+1);
convergedFits = [];

%put another loop to adjust dates?
startDate = datetime('5-Mar-2023');
endDate = datetime('5-May-2023');
%new is the next company being added in machine learning
for x = 1:numIter
    Newraw = getMarketDataViaYahoo(StockSymbols.Symbol{x}, string(startDate), string(endDate), '1d');

 if isempty(Newraw)
    continue;
 end
 Newdays = 1:size(Newraw,1);
 Newdays = Newdays(:);
 normalisedNewPrice = Newraw.AdjClose ./ Newraw.AdjClose(1);

    %polynomial fit of the new stock
    pNew = polyfit(Newdays, normalisedNewPrice, polyOrder);
    %fNew = polyval(pNew, Newdays); %only neccessary if want to see fit


    if (polyOrder==3)
    %tolerance of fit for comparing to previously identified trends
    %mean is formatted to dimensions of previously found fits matrix
    %and has absolute values taken
    %plot(Newdays, fNew);
    hold on
    formattedMean = abs(pNew(ones(size(poly3Found, 1),1),:));
    tol = polyTol .* formattedMean;
    %checks if there is a fit that has been found that matches
    %the identified trend
    difference = poly3Found(:,1:(size(pNew, 2))) - pNew(ones(1, size(poly3Found,1)),:);
    withinTol = abs(difference) <= tol; 
    isPresent = sum(withinTol, 2);
    %sees if a similar fit has already been logged
    replaceIndex = find(isPresent == length(pNew));
    if size(replaceIndex,1) >=1
        for c = 1:size(replaceIndex, 1) 
           %identifies correct index to replace
           index = replaceIndex(c,1); 
           %edits logged mean fit to incorporate newly found regression       
           matchedFits = poly3Found(index, 1:(size(pNew, 2)));
           replacement = rdivide(plus(matchedFits, pNew), 2);
           poly3Found(index, 1:(size(pNew, 2))) = replacement;
           %new number of this fit found
           newFound = plus(poly3Found(index, 5),1);
           %new prediction for price change, using new fit
           nChangeP = FuturePriceChange(endDate, timeExtrap, StockSymbols.Symbol{x}); 
           nMean = rdivide(plus(nChangeP, poly3Found(index, 6)), 2);
           %new prediction for volatility (one standard deviation)
           nSD = NewSD(poly3Found(index,8), nMean, nChangeP, newFound);
           %increases number of this fit found by 1
           poly3Found(index, 5) = newFound;
           %adds new mean, s.d., and sum of changes squared
           poly3Found(index, 6) = nMean;
           poly3Found(index, 7) = nSD;
           poly3Found(index,8) = plus(poly3Found(index,8), (nChangeP.^2));
        end
        if size(replaceIndex(:,1)) > 1
           convergedFits = [convergedFits,-1,replaceIndex];
        end
    else
        nChangeP = FuturePriceChange(startDate, timeExtrap, StockSymbols.Symbol{x}); 
        nSD = 0; 
        nSumsq = nChangeP.^2;
        poly3Found = [poly3Found; pNew, 1 , nChangeP, nSD, nSumsq];
    end
   end
end


%do these statements as separate functions?

%determines what fits have been found enough times for them to be valid

potentialFits = find(poly3Found(:,5)>=(minProportion*numIter));

if (isempty(potentialFits) == false)
    %loop just for visualising
    for c = 1:size(potentialFits, 1)
        fFound = polyval(poly3Found(potentialFits(c,1),1:4), Newdays(:));
        plot(Newdays(:),fFound);
        hold on
    end
    %stores all the found repeated fits
    requiredCols = [poly3Found(potentialFits,1:(size(pNew,2)))];
    requiredCols = [requiredCols, poly3Found(potentialFits,6:7)];
    FitChangeVol = [FitChangeVol;requiredCols];
end

%cleans the found fits that have a large volatility, and therefore
%large uncertainty in prediction

if (abs(FitChangeVol(:,5))>minChange) && (abs(FitChangeVol(:,5)*maxrVol)>FitChangeVol(:,6))
    
end
  

