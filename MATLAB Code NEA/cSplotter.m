function []=cSplotter()
pRaw = getMarketDataViaYahoo('A', '26-Mar-2023', '26-May-2023', '1d');
nD  ays = 1:size(pRaw,1);
nDays = nDays(:);

fig = figure('visible', 'off');
plot(nDays, pRaw.AdjClose(:));
Frame = getframe(fig);
%creates bitmap image of the plot
image = Frame.cdata;
% Rearranges the bitmap data to RGB sequence pixel by pixel, row by row
im1 = flip(image, 3);         % flip the color data from BGR to RGB
im2 = permute(im1, [3,2,1]);  % put dimensions in this order: color, width, height
imFlat = reshape(im2, 1,[]);  % flatten to one-dimensional array

imSize = size(image);