function NWSmbs = AreStockSymbolsWorking(StockSymbols)
CharCodes = [];
for x = 1:200%size(StockSymbols, 1)
    Newraw = getMarketDataViaYahoo(StockSymbols.Symbol{x}, '23-Mar-2023', '23-May-2023' , '1d');
    if isempty(Newraw)
        CharCodes = [CharCodes; string(StockSymbols.Symbol{x})];
    end
end
NWSmbs = CharCodes;