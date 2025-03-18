%function to split the raw data into data for each individual stock

function splitStocks = StockDataSplitter(StockData)

uniqueStocks = unique(StockData.symbolsForMerge);
subTables = cell(length(uniqueStocks), 1);

for i = 1:length(uniqueStocks)
    stock = uniqueStocks{i};

    % Logical indexing to filter rows
    filteredRows = strcmp(StockData.symbolsForMerge, stock);

    % Create a sub-table based on filtered rows
    subTables{i} = StockData(filteredRows, :);
end

splitStocks = subTables;