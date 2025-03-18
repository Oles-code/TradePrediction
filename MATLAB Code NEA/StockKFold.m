%dataDir = 'C:\Users\olesc\OneDrive\Documents\MATLAB\MATLABData\';%path
%dataFile = '**file name**';
%StockData = readtable([dataDir, dataFile]);
%getmarketdataviayahoo also allowed as in correct folder

%other random useful stuff;
%heatmap - correlation matrix plot
%A(any(sum(isnan(A), 2),2),:) = [] - how to remove nan rows

%returns column vector of cells, each cell one fold of data
%entry data should be arranged row wise, ie new row = new data
function Folds = StockKFold(numFolds, data) %data should be matrix

foldSize = fix(size(data,1)/numFolds);
foldsForReturn = [];

for x = 1:numFolds
    currentFold = {data((x-1)*foldSize+1:x*foldSize,:)};
    foldsForReturn = [foldsForReturn; currentFold];
end

Folds = foldsForReturn;