function date  = DateFormatter(InpDate)

InpDate = string(InpDate);
Year = extractAfter(InpDate,6);
Month = extractBefore(extractAfter(InpDate,3),3);
Day = extractBefore(InpDate,3);

if Month == "01"
    Month = "Jan";
elseif Month == "02"
    Month = "Feb";
elseif Month == "03"
    Month = "Mar";   
elseif Month == "04"
    Month = "Apr";
elseif Month == "05"
    Month = "May";
elseif Month == "06"
    Month = "Jun";   
elseif Month == "07"
    Month = "Jul";
elseif Month == "08"
    Month = "Aug";   
elseif Month == "09"
    Month = "Sep";
elseif Month == "10"
    Month = "Oct";
elseif Month == "11"
    Month = "Nov";   
elseif Month == "12"
    Month = "Dec";
end

date = datetime(Day + '-' + Month + '-' + Year);