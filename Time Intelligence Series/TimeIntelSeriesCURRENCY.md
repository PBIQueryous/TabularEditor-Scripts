# Time Intelligence Series

- [x] InDEV
- [ ] Complete


**NOTE: Script is entirely dependent on my `Fiscal Calendar` table created in Power Query.  
You can find the complete M-code for my `Dates Table` here:** [Dates Table](https://github.com/PBIQueryous/M-Code/tree/main/Calendars)  

This C# script for Tabular Editor, when executed, will produce the following set of time-intelligence measures:

<br/>

#### Measure List

| Abbreviation | Full Name |
| --- | --- |
| SNAP | Snapshot |
| CML | Cumulative |
| CYTD | Current Year to Date |
| CFYTD | Current Fiscal Year to Date |
| CYTD CML | Current Year to Date Cumulative |
| CFYTD CML | Current Fiscal Year to Date Cumulative |
| REM | Remaining Future Values |
| Actual & Forecast YTD | as named Year to Date |
| Actual & Forecast CML | as named Cumulative |
| Actual & Forecast FYTD CML | as named Fiscal Year to Date Cumulative |


<br/>

## C# Script
**TL;DR**\
**The following script is formatted for CURRENCY**\
**Bonus metadata options: in-code comments, display folder, measure description & format string** 
**C# script variables allow for DAX code customisation according to your requirements and _hopefully_ makes for an easier reading experience... hopefully :P**
```c#

  
  


  
/*---------------------------------------------------
| TITLE:                                             |
| Time Intelligence Series                           |
| Tabular Editor Advanced Script                     |
 ----------------------------------------------------
| AUTHOR:                                            |
| Imran Haq, PBI Queryous                            |
| https://github.com/PBIQueryous                     |
| STAY QUERYOUS PBIX CHAMPS!                         |
 ---------------------------------------------------*/

 /* SCRIPT NOTES 
 *---------------------------------------------------*
 | Inspiration and Credits:                          |
 | PowerBI.Tips Team    |   https://powerbi.tips/    |
 | Daniel Otykier       |   twitter.com/DOtykier     |
 | and endless more names from the PBI community,    |
 | Thank you all!                                    |
 *---------------------------------------------------*
 */

 /* DESCRIPTION
 * -----------------------------------
 * This script, when executed, will loop through the currently selected measure(s),
 * creating a series of measure(s) declared in the script below.
 * 
 * e.g., from Model select [Measure] where Measure = SUM( tbl[column] ) or COUNT( tbl[column] )
 * -----------------------------------
 */

 // C# measure formula template:
 // m.Table.AddMeasure( "MeasureName", "Expression", m.DisplayFolder);

/**** C# SCRIPT START ****/

// SET VARIABLES
// Quotation Character - helpful for wrapping " " around a text string within the DAX code
const string qt = "\"";

// Number Formatting Strings
var GBP0 = qt + "£" + qt + "#,0";
var GBP2 = qt + "£" + qt + "#,0.00";
var Whole = "#,0";
var Percent = "0.0 %";
var Decimal = "#,0.0";
var Number = "#,0";
var Currency0 = GBP0+";" +"-"+GBP0+";" +GBP0;
var Currency2 = GBP2+";" +"-"+GBP2+";" +GBP2;
var Deviation = "+"+Decimal+";" +"-"+Decimal+";"+ Decimal;

// Var RETURN text strings
var vResult = "var _result = ";
var rReturnResult = "RETURN" + '\n' + '\t' + "_result";
var rReturn = "RETURN" + '\n';
var ifnotBlank = '\t' + "// IF(  NOT ISBLANK( ";
var thenResult = " ) ,  _result  )";
var rResult = '\t' + "_result";

// MeasureName Variables
var snap = " | SNAP";
var cml = " | CML";
var cytd = " | CYTD";
var cfytd = " | CFYTD";
var cytdCml = " | CYTD CML";
var cfytdCml = " | CFYTD CML";
var rem = " | REM";



// TimeIntel Variable Filters
var datesDate = "Dates[Date]";
var datesMTD = "Dates[LatestMTD]";
var isCFY = "Dates[IsCFY] = TRUE";
var isCYTD = "Dates[IsCYTD] = TRUE";
var maxDate = "_maxDate";
var curDate = "_curDate";
var mtdDate = "_ytd";
var vardatesDate = "var " +maxDate+ " = MAX( " + datesDate + " )";
var varlatestMTD = "var "+mtdDate+ " = CALCULATE( MAX( " +datesMTD+ " ), REMOVEFILTERS())";
var varmaxdatesCFY = "var " +maxDate+ " = CALCULATE( MAX( " +datesDate+ "), " + isCFY + " )";
var fiscalyear = qt+"31/3"+qt;
var datesFiscal = "DATESYTD (" + datesDate + "," + fiscalyear + " )";
// Var Measure Folder
var subFolder = "_Measures\\SubFolder";

// Script Variable: Creates a series of time intelligence measures for each selected measure in the Model
foreach(var m in Selected.Measures) 
{
 

 
/***************************************** MeasureStart ************************************/
// Measure1: SUM
    var m1 = m.Table.AddMeasure
    (                             

// startSubScript
        // MeasureName
        m.Name + snap,                               
    
        // DAX comment string
        '\n' + "// snapshot - basic sum "                           
        
/* DAX expression START */
        // DAX Variables               
        
        
        // Result Expression Variable
        + '\n' + vResult + m.DaxObjectName + '\n'
        
        // Return Expression
        + '\n' + rReturn
        + '\n' + rResult
        
        // optional in DAX
        // useful in cumulative measures - returns blank if no value exists for future dates
        + '\n' + ifnotBlank + m.DaxObjectName + thenResult
        );
/* DAX expression END */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m1.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m1.Description = "From: " + m.Name + " - " + '\n' +
        // Type metadata text here
        "base measure; basic sum; forms the reference to subsequent time-intelligence measures"
        ;                             
        m1.FormatString = Currency0
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/


/***************************************** MeasureStart ************************************/
// Measure2: CML
    var m2 = m.Table.AddMeasure
    (                             

// startSubScript
        // MeasureName
        m.Name + cml,                               
    
        // DAX comment string
        '\n' + "// cumulative of " + m.Name + snap + '\n'
        
/* DAX expression START */
        // DAX Variables               
        + '\n' + vardatesDate
        
        // Result Expression Variable
        + '\n' + vResult + 
        
        // DAX Expression
        "CALCULATE( [" + m.Name + snap + "], "          // calculate
        // filter context
        + datesDate + " <= " + maxDate + " )" + '\n'     // filter
        
        // Return Expression
        + '\n' + rReturnResult
        
        // optional in DAX
        // useful in cumulative measures - returns blank if no value exists for future dates
        + '\n' + ifnotBlank + m.DaxObjectName + thenResult
        );
/* DAX expression END */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m2.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m2.Description = "From: " + m.Name + " - " + '\n' +
        // Type metadata text here
        "cumulative measure; continuous until max calendar date"
        ;                             
        m2.FormatString = Currency0
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/


/***************************************** MeasureStart ************************************/
// Measure3: CYTD
    var m3 = m.Table.AddMeasure
    (                             

// startSubScript
        // MeasureName
        m.Name + cytd,                               
    
        // DAX comment string
        '\n' + "// current year to date of " + m.Name + snap + '\n'
        
/* DAX expression START */
        // DAX Variables               
        
        
        // Result Expression Variable
        + '\n' + vResult + 
        
        // DAX Expression
        "CALCULATE( [" + m.Name + snap + "], "          // calculate
        // filter context
        + isCYTD + " )" + '\n'                          // filter
        
        // Return Expression
        + '\n' + rReturnResult
        
        // optional in DAX
        // useful in cumulative measures - returns blank if no value exists for future dates
        + '\n' + ifnotBlank + m.DaxObjectName + thenResult
        );
/* DAX expression END */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m3.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m3.Description = "From: " + m.Name + " - " + '\n' +
        // Type metadata text here
        "current year to date, latest YTD is up to date today"
        ;                             
        m3.FormatString = Currency0
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/


/***************************************** MeasureStart ************************************/
// Measure4: CYTD
    var m4 = m.Table.AddMeasure
    (                             

// startSubScript
        // MeasureName
        m.Name + cfytd,                               
    
        // DAX comment string
        '\n' + "// current fiscal year to date of " + m.Name + snap + '\n'
        
/* DAX expression START */
        // DAX Variables               
        + '\n' + varlatestMTD
        
        // Result Expression Variable
        + '\n' + vResult + 
        
        // DAX Expression
        "CALCULATE( [" + m.Name + snap + "], "          // calculate
        // filter context
        + "KEEPFILTERS( " + datesDate+ " <= " + mtdDate + " ), " + isCFY + " )" // filter
        
        // Return Expression
        + '\n' + rReturnResult
        
        // optional in DAX
        // useful in cumulative measures - returns blank if no value exists for future dates
        + '\n' + ifnotBlank + m.DaxObjectName + thenResult
        );
/* DAX expression END */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m4.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m4.Description = "From: " + m.Name + " - " + '\n' +
        // Type metadata text here
        "current fiscal year to date, latest YTD is up to date today"
        ;                             
        m4.FormatString = Currency0
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/


/***************************************** MeasureStart ************************************/
// Measure5: CYTD CML
    var m5 = m.Table.AddMeasure
    (                             

// startSubScript
        // MeasureName
        m.Name + cytdCml,                               
    
        // DAX comment string
        '\n' + "// cumulative current ytd of " + m.Name + cytd + '\n'
        
/* DAX expression START */
        // DAX Variables               
        + '\n' + vardatesDate
        
        // Result Expression Variable
        + '\n' + vResult + 
        
        // DAX Expression
        "CALCULATE( [" + m.Name + cytd + "], "          // calculate
        // filter context
        + datesDate + " <= " + maxDate + " )" + '\n'     // filter
        
        // Return Expression
        + '\n' + rReturnResult
        
        // optional in DAX
        // useful in cumulative measures - returns blank if no value exists for future dates
        + '\n' + ifnotBlank + m.DaxObjectName + thenResult
        );
/* DAX expression END */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m5.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m5.Description = "From: " + m.Name + " - " + '\n' +
        // Type metadata text here
        "cumulative measure; continuous until max calendar date"
        ;                             
        m5.FormatString = Currency0
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/

/***************************************** MeasureStart ************************************/
// Measure6: CFYTD CML
    var m6 = m.Table.AddMeasure
    (                             

// startSubScript
        // MeasureName
        m.Name + cfytdCml,                               
    
        // DAX comment string
        '\n' + "// current fiscal year to date of " + m.Name + cfytd + '\n'
        
/* DAX expression START */
        // DAX Variables               
        + varmaxdatesCFY 
        
        // Result Expression Variable
        + '\n' + vResult + '\n' + 
        
        // DAX Expression
        "CALCULATE( [" + m.Name + snap + "], "                              // calculate
        // filter context
        + '\n' + '\t' + datesFiscal + ", " + '\n' +                         // filter
        "// optional filter:" +'\n' +
        "/* turn on = upto current fiscal year only"+ '\n' +
        " * turn off = all fiscal years */"
        + '\n' + '\t' + datesDate + " <= " + maxDate + ")"
        
        // Return Expression
        + '\n' + rReturnResult
        
        // optional in DAX
        // useful in cumulative measures - returns blank if no value exists for future dates
        + '\n' + ifnotBlank + m.DaxObjectName + thenResult
        );
/* DAX expression END */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m6.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m6.Description = "From: " + m.Name + " - " + '\n' +
        // Type metadata text here
        "current fiscal year to date, latest YTD is up to date today"
        ;                             
        m6.FormatString = Currency0
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/


/***************************************** MeasureStart ************************************/
// Measure7: REM
    var m7 = m.Table.AddMeasure
    (                             

// startSubScript
        // MeasureName
        m.Name + rem,                               
    
        // DAX comment string
        '\n' + "// remaining future values " + m.Name + snap + '\n'
        
/* DAX expression START */
        // DAX Variables               
        + varlatestMTD 
        
        // Result Expression Variable
        + '\n' + vResult + '\n' + 
        
        // DAX Expression
        "CALCULATE( [" + m.Name + snap + "], "                              // calculate
        // filter context
        + '\n' + '\t' + 
        "KEEPFILTERS( " + datesDate + " > " + mtdDate + "))"               // filter
        + '\n'
        
        // Return Expression
        + '\n' + rReturnResult
        );
/* DAX expression END */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m7.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m7.Description = "From: " + m.Name + " - " + '\n' +
        // Type metadata text here
        "remaining future values beyond the latest complete MTD"
        ;                             
        m7.FormatString = Currency0
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/


/***************************************** MeasureStart ************************************/
// Measure8: Actual and Future
    var m8 = m.Table.AddMeasure
    (                             

// startSubScript
        // MeasureName
        m.Name + " & Forecast | YTD",                               
    
        // DAX comment string
        '\n' + 
        "// actual YTD and remaining future values " 
        + '\n'
        
/* DAX expression START */
        // DAX Variables               
        + varlatestMTD 
        + '\n' + 
        "var _actual = ["  + m.Name + snap + "]"
        + '\n' + 
        "var _forecast = " 
        + '\n' + '\t' +
        "CALCULATE( [forecastMEASURE], -- eg: Forecast, Budget, Plan etc"
        + '\n' + '\t' + '\t' +
        "KEEPFILTERS( " + datesDate + " > " + mtdDate + "))"
        // Result Expression Variable
        + '\n' + 
        vResult
         + '\n' + 
        
        // DAX Result Expression
        " _actual + _forecast"                                              // calculate
        + '\n'
        
        // Return Expression
        + '\n' + rReturnResult
        );
/* DAX expression END */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m8.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m8.Description = "From: " + m.Name + " - " + '\n' +
        // Type metadata text here
        "actual YTD and future forecast"
        ;                             
        m8.FormatString = Currency0
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/


/***************************************** MeasureStart ************************************/
// Measure9: Actual & Forecast CML
    var m9 = m.Table.AddMeasure
    (                             

// startSubScript
        // MeasureName
        m.Name + " & Forecast | CML",                               
    
        // DAX comment string
        '\n' + 
        "// actual YTD and remaining future values cumulative totals " 
        + '\n'
        
/* DAX expression START */
        // DAX Variables               
        + '\n' + vardatesDate
        
        // Result Expression Variable
        + '\n' + vResult + 
        
        // DAX Expression
        "CALCULATE( [" + m.Name + " & Forecast | YTD" + "], "          // calculate
        // filter context
        + datesDate + " <= " + maxDate + " )"                           // filter
         + '\n'     
        
        // Return Expression
        + '\n' + rReturnResult
        
        // optional in DAX
        // useful in cumulative measures - returns blank if no value exists for future dates
        + '\n' + ifnotBlank + m.DaxObjectName + thenResult
        );
/* DAX expression END */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m9.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m9.Description = "From: " + m.Name + " - " + '\n' +
        // Type metadata text here
        "cumulative measure; continuous until max calendar date"
        ;                             
        m9.FormatString = Currency0
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/


/***************************************** MeasureStart ************************************/
// Measure10: Actual & Forecast CFYTD CML
    var m10 = m.Table.AddMeasure
    (                             

// startSubScript
        // MeasureName
        m.Name + " & Forecast | CFYTD CML",                               
    
        // DAX comment string
        '\n' + "// current fiscal year to date of " + m.Name + " & Forecast | YTD" + '\n'
        
/* DAX expression START */
        // DAX Variables               
        + varmaxdatesCFY 
        
        // Result Expression Variable
        + '\n' + vResult + '\n' + 
        
        // DAX Expression
        "CALCULATE( [" + m.Name + " & Forecast | YTD" + "], "                              // calculate
        // filter context
        + '\n' + '\t' + datesFiscal + ", " + '\n' +                         // filter
        "// optional filter:" +'\n' +
        "/* turn on = upto current fiscal year only"+ '\n' +
        " * turn off = all fiscal years */"
        + '\n' + '\t' + datesDate + " <= " + maxDate + ")"
        
        // Return Expression
        + '\n' + rReturnResult
        
        // optional in DAX
        // useful in cumulative measures - returns blank if no value exists for future dates
        + '\n' + ifnotBlank + m.DaxObjectName + thenResult
        );
/* DAX expression END */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m10.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m10.Description = "From: " + m.Name + " & Forecast | YTD" + " - " + '\n' +
        // Type metadata text here
        "current fiscal year to date, latest YTD is up to date today"
        ;                             
        m10.FormatString = Currency0
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/


}
/**** C# SCRIPT END ****/


```
