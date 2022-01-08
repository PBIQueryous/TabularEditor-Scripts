# Conditonal Colour Format YTD and REM

```c#

var Red = '\u0022' + "#B20000" + '\u0022';
var Green = '\u0022' + "#299B00" + '\u0022';
const string qq = "\"";

// MeasureTemplate: Colour Conditional Formatting:
foreach(var m in Selected.Measures) {

   // ActualForecastColour:
    m.Table.AddMeasure(
    
   // Name
    "@ActualForecastColour", 
    
   // DAX expression
      '\n' + "VAR _YtD1 = LASTDATE( Dates[MTDAdd3] )"     
      + '\n' + "VAR _YtD = LASTDATE( Dates[LatestMTD] )" 
      + '\n' + "RETURN"
      + '\n' + "SWITCH( TRUE(), SELECTEDVALUE(Dates[MonthStart]) <  _YtD, " + qq + "Blue" + qq + ", SELECTEDVALUE(Dates[MonthStart]) > _YtD1," + qq + "Light Grey" + qq + "," + qq + "Dark Grey" + qq + ")",          
      
   // Display Folder +
      m.DisplayFolder                                                     
    );
```
