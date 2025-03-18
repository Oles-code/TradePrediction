%custom machine learning function which is used to predict 
%buying or selling of stock
%(passed from c#) model ReRun
function StockfitML()

dataDir = 'C:\Users\olesc\OneDrive\Documents\MATLAB\MATLABData\';
dataFile = 'RawStockData';
StockData = readtable([dataDir, dataFile]);

%Splitting data into individual stocks
%Split stocks is an 1xn matrix with each cell having 
% one table of data corresponding to a stock's price
splitStocks = StockDataSplitter(StockData);


%Time period for which prediction is made
timeExtrap = 30;
%misc variables
polyOrder = 3;
polyTol = 0.1; %changed

%number of extra columns required to contain extra data about fit
%1st is number of matching fits, 2nd is predicted relative change
%after x time
%3rd is volatility, 4th is sum of increase squared (needed for sd)
XtraCol = 4;

%number of comanies included in simulation 
numIter = size(splitStocks,1);

%periodicity of fits in days (must be even as data taken bi-periodically)
numDays = 30;

%%Change here - random 20% of data taken for testing - but this 
% 20% must be of 30 day blocks
%split all data into 30 day blocks (for each stock) and then 
%take a random 20% of this for test
%Variable containing a table of a 30 day period of data in each cell 
testBlocks = [];

%Loop which splits each different stock ticker into 30 day periods
% of data
for c = 1:numIter
    %Determines current stock and how many blocks that ticker can
    % be split into
    currentStock = splitStocks{c,1}; 
    numTestBlocks = fix(size(currentStock,1)/numDays)*2;
    lastRow = 1;

    %Iterated around the current ticker placing each block of 30
    % days into a separate cell in TestBlocks
    %The blocks of data overlap 15 days each, to avoid potential loss
    % of interesting fits in periods across 30 day blocks
    for i = 1:(numTestBlocks-1)
        currentStockCell = {currentStock(lastRow:(lastRow+(numDays-1)),:)};
        testBlocks = [testBlocks; currentStockCell];
        lastRow = lastRow+(numDays/2);
    end
end

randCols = randi(10,size(testBlocks,1),1);
testingIndex = find(randCols>8);
trainingIndex = find(randCols<=8);
testData = {testBlocks{testingIndex, :}};

%minimum relative change in price for a stock to be valid when predicting
minChange = 0.1;

%maximum volatility relative to growth
maxrVol = 0.5;

%final found fits with their fit, change, volatility and order of
%polynomial
successFits = [];

poly3Found = ones(1,XtraCol+polyOrder+1);
convergedFits = [];

%loop to adjust data block
for d = 1:size(trainingIndex,1)
    
    %defining new data which is going to be used for training
    newData = testBlocks{trainingIndex(d),:};
    currentStock = string(newData.symbolsForMerge(1));

    %defining boundary
    lastRowIndex = size(newData,1);
    endDate = newData.Date(lastRowIndex,1);
    endDate = DateFormatter(endDate);
    
    %normsalises all stock price
    normalisedNewPrice = newData.AdjClose ./ newData.AdjClose(1);
    newdays = 1:size(normalisedNewPrice,1);
    newdays = newdays(:);
    %polynomial fit of the new stock
    pNew = polyfit(newdays, normalisedNewPrice, polyOrder);
    %fNew = polyval(pNew, Newdays); %only neccessary if want to see fit

    %tolerance of fit for comparing to previously identified trends
    %mean is formatted to dimensions of previously found fits matrix
    %and has absolute values taken
    %plot(Newdays, fNew);
    %hold on
    formattedMean = abs(pNew(ones(size(poly3Found, 1),1),:));
    tol = polyTol .* formattedMean;
   
    %new prediction for price change, using new fit
    nChangeP = FuturePriceChange(endDate, timeExtrap, currentStock); 
   
    %error check if there is extrapolated data found
    if nChangeP == 'no data'
        continue;
    end
   
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
           nMean = rdivide(plus(nChangeP, poly3Found(index, 6).*(newFound-1)), newFound);
           
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
        %%nChangeP = FuturePriceChange(startDate, timeExtrap, StockSymbols.Symbol{x}); 
        nSD = 0; 
        nSumsq = nChangeP.^2;
        poly3Found = [poly3Found; pNew, 1 , nChangeP, nSD, nSumsq];
    end 
end

  
%Tests any found fits and writes the successful fits to a file
minChange = 0.05;
maxrVol = 0.5;
polyOrder = 3;
%tested successful fits should be used to write to a database of prediction
successFits = CleanMatches(poly3Found, minChange, maxrVol, polyOrder);

%tests the successful fits
postTestSuc = StockFitTesting(successFits, testData, polyOrder);
postTestSuc = sortrows(postTestSuc,size(postTestSuc,2));
%only keeps actual polynomial data, and change + s.d.
postTestSuc = postTestSuc(2:end,1:6);

homedir = cd;
flnm = 'Poly3Found.csv';   
fid = fopen(flnm,'wt+'); 
writematrix(postTestSuc, flnm)
fclose(fid);
cd(homedir); 

