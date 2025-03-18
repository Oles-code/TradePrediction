%% Testing fits and predictions
%returns martix with the found fits and with one extra column indicating
%accuracy of the fit when predicting, if 0 is in accuracy column then
%the fit was not found in the test data

function fits = StockFitTesting(sucFits, testData, polyOrder)

newCol = zeros(size(sucFits,1),2);
sucFits = [sucFits, newCol];
timeExtrap = 30;
tol = 0.1;
%counter to see if there are issues with returning stock codes
counter = 0;


for x = 1:size(testData,1)
    
    currentDataCell = testData(1,x);
    currentData = currentDataCell{1,1};
    currentStock = currentData.symbolsForMerge(1);
    currentStock = currentStock{1,1};

    %normalises stock price
    normalisedPrice = currentData.AdjClose ./ currentData.AdjClose(1);
    newdays = 1:size(normalisedPrice,1);
    newdays = newdays(:);

    %polynomial fit of the new stock
    pNew = polyfit(newdays, normalisedPrice, polyOrder);

    %gets the price change to use for comparison
    testEnd = datetime(currentData.Date(end));
    nChangeP = FuturePriceChange(testEnd, timeExtrap, currentStock);

    %error check if there is extrapolated data found
    if nChangeP == 'no data'
      continue;
    end

    %checks if there is a fit that has been found that matches
    %the identified trend
    difference = sucFits(:,1:(size(pNew, 2))) - pNew(ones(1, size(sucFits,1)),:);
    withinTol = abs(difference) <= tol; 
    isPresent = sum(withinTol, 2);

    %sees if a similar fit has been logged
    replaceIndex = find(isPresent == size(pNew,2));
    if size(replaceIndex,1) >=1
        for m = 1:size(replaceIndex, 1) 
           %identifies correct index to compare
           index = replaceIndex(m,1); 
           %finds the difference between newly found change and old
           %mean change for this particular fit
           changeSimi = sucFits(index,6)-nChangeP;
           %finds Z-Value for this fit
           ZVal = abs(changeSimi/sucFits(index,6));
           if(sucFits(index,end) == 0)
               %logs new Z value and adds that a test fit was used
               sucFits(index, end) = ZVal;
               sucFits(index,size(sucFits,2)-1) = sucFits(index,size(sucFits,2)-1)+1;
           else
               %updates mean Z val and adds that another test fit 
               %was used
               sucFits(index, end) = plus(sucFits(index,end),ZVal)/2;
               sucFits(index,size(sucFits,2)-1) = sucFits(index,size(sucFits,2)-1)+1;
           end
        end
    end
    %drops out of program if data is consistently returning erros
    if counter>100
        fits = sucFits;
        return;
    end
end    


fits = sucFits;
