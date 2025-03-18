function SD = NewSD(OldSumSq, NewMean, nChange, Num)

NewMeanSq = NewMean.^2;
NewSumSq = OldSumSq + (nChange.^2);

SD = sqrt(abs((NewSumSq/Num)-NewMeanSq));