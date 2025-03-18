%function which clears and found polynomial fits from my algorithm that are
%not consistent enough, e.g. do not change enough from original price to be
%worth buying/selling or are too volatile in their predicted price

function SucFits = CleanMatches(potentialFits, minChange, maxrVol, polyOrder)
fitChangeVol = [];
if (isempty(potentialFits) == false)
    %stores all the found repeated fits
    requiredCols = [potentialFits(:,1:(polyOrder+1))];
    requiredCols = [requiredCols, potentialFits(:,6:7)];
    fitChangeVol = requiredCols;
    sucIndex1 = abs(fitChangeVol(:,5))>minChange;
    sucIndex2 = abs(fitChangeVol(:,5)*maxrVol)>fitChangeVol(:,6);
    sucIndex = and(sucIndex1, sucIndex2);
    fitChangeVol(sucIndex,:) = [];
end

SucFits = fitChangeVol;
