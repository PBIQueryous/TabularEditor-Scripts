# TimeIntel Measures (£)

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
 *-----------------------------------------------------*
 | Inspiration and Credits:                            |
 | PowerBI.Tips Team    |   powerbi.tips/              |
 | Mike Carlo           |   twitter.com/Mike_R_Carlo   |
 | Seth Bauer           |   twitter.com/Seth_C_Bauer   |
 | Tommy Puglia         |   twitter.com/tommypuglia    |
 | Daniel Otykier       |   twitter.com/DOtykier       |
 | Enterprise DNA       |   twitter.com/_enterprisedna |
 | Imke Feldmann        |   twitter.com/thebiccountant |
 | and endless more names from the PBI community,      |
 | Thank you all!                                      |
 *-----------------------------------------------------*
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
var rResult = '\t' + "_result";
var rReturnResult = "RETURN" + '\n' + '\t' + "_result";
var rReturn = "RETURN" + '\n';
var rReturnA1 = rReturn + '\n' + "// " + rResult;
var rReturnA2 = rReturn + '\n' + rResult;
var ifnotBlank = '\t' + "IF(  NOT ISBLANK( ";
var thenResult = " ) ,  _result  )";


// MeasureName Variables
var snap = " | SNAP";
var cml = " | CML";
var ytd = " | YTD";
var ytdCml = " | YTD CML";
var cytd = " | CYTD";
var cfytd = " | CFYTD";
var cytdCml = " | CYTD CML";
var cfytdCml = " | CFYTD CML";
var rem = " | REM";
var calculate = "CALCULATE( ";


// TimeIntel Variable Filters
var datesDate = "_Dates[Date]";
var datesMTD = "_Dates[LatestMTD]";
var isCFY = "_Dates[IsCFY] = TRUE";
var isCYTD = "_Dates[IsCYTD] = TRUE";
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
        + '\n' + "// " + rResult
        
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
        " Base Measure; SUM (Snapshot) "
        ;                             
        m1.FormatString = Decimal
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
        calculate + "[" + m.Name + snap + "], "          // calculate
        // filter context
        + datesDate + " <= " + maxDate + " )" + '\n'     // filter
        
        // Return Expression
        + '\n' + rReturnA1
        
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
        "cumulative measure; until MAX date variable"
        ;                             
        m2.FormatString = Decimal
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
        calculate + "[" + m.Name + snap + "] "          // calculate
        // filter context
        + " /* , " + isCYTD + " */" + " )" + '\n'                          // filter
        
        // Return Expression
        + '\n' + rReturnA1
        
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
        m3.FormatString = Decimal
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/


/***************************************** MeasureStart ************************************/
// Measure4: CFYTD
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
        calculate + "[" + m.Name + snap + "], "          // calculate
        // filter context
        + "KEEPFILTERS( " + datesDate+ " <= " + mtdDate + " ), " + isCFY + " )" // filter
        
        // Return Expression
        + '\n' + rReturnA1
        
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
        m4.FormatString = Decimal
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
        calculate + "[" + m.Name + cytd + "], "          // calculate
        // filter context
        + datesDate + " <= " + maxDate + " )" + '\n'     // filter
        
        // Return Expression
        + '\n' + rReturnA1
        
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
        m5.FormatString = Decimal
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
        calculate + "[" + m.Name + snap + "], "                              // calculate
        // filter context
        + '\n' + '\t' + datesFiscal + ", " + '\n' +                         // filter
        "// optional filter:" +'\n' +
        "/* turn on = upto current fiscal year only"+ '\n' +
        " * turn off = all fiscal years */"
        + '\n' + '\t' + datesDate + " <= " + maxDate + ")"
        
        // Return Expression
        + '\n' + rReturnA1
        
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
        m6.FormatString = Decimal
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
        calculate + "[" + m.Name + snap + "], "                              // calculate
        // filter context
        + '\n' + '\t' + 
        "KEEPFILTERS( " + datesDate + " > " + mtdDate + "))"               // filter
        + '\n'
        
        // Return Expression
        + '\n' + rReturnA2
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
        m7.FormatString = Decimal
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
        + '\n' + rReturnA1
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
        m8.FormatString = Decimal
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
        calculate + "[" + m.Name + " & Forecast | YTD" + "], "          // calculate
        // filter context
        + datesDate + " <= " + maxDate + " )"                           // filter
         + '\n'     
        
        // Return Expression
        + '\n' + rReturnA1
        
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
        m9.FormatString = Decimal
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
        calculate + "[" + m.Name + " & Forecast | YTD" + "], "                              // calculate
        // filter context
        + '\n' + '\t' + datesFiscal + ", " + '\n' +                         // filter
        "// optional filter:" +'\n' +
        "/* turn on = upto current fiscal year only"+ '\n' +
        " * turn off = all fiscal years */"
        + '\n' + '\t' + datesDate + " <= " + maxDate + ")"
        
        // Return Expression
        + '\n' + rReturnA1
        
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
        m10.FormatString = Decimal
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/

/***************************************** MeasureStart ************************************/
// Measure11: YTD
    var m11 = m.Table.AddMeasure
    (                             

// startSubScript
        // MeasureName
        m.Name + ytd,                               
    
        // DAX comment string
        '\n' + "// current year to date of " + m.Name + snap + '\n'
        
/* DAX expression START */
        // DAX Variables               
        
        
        // Result Expression Variable
        + '\n' + vResult + 
        
        // DAX Expression
        calculate + "[" + m.Name + snap + "] "          // calculate
        // filter context
        + "/* , " + isCYTD + " */" + " )" + '\n'                          // filter
        
        // Return Expression
        + '\n' + rReturnA1
        
        // optional in DAX
        // useful in cumulative measures - returns blank if no value exists for future dates
        + '\n' + ifnotBlank + m.DaxObjectName + thenResult
        );
/* DAX expression END */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m11.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m11.Description = "From: " + m.Name + " - " + '\n' +
        // Type metadata text here
        "current year to date, latest YTD is up to date today"
        ;                             
        m11.FormatString = Decimal
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/

/***************************************** MeasureStart ************************************/
// Measure12: YTD CML
    var m12 = m.Table.AddMeasure
    (                             

// startSubScript
        // MeasureName
        m.Name + ytdCml,                               
    
        // DAX comment string
        '\n' + "// cumulative current ytd of [" + m.Name + ytd + "]" + '\n'
        
/* DAX expression START */
        // DAX Variables               
        + '\n' + vardatesDate
        
        // Result Expression Variable
        + '\n' + vResult + 
        
        // DAX Expression
        calculate + "[" + m.Name + ytd + "], "          // calculate
        // filter context
        + datesDate + " <= " + maxDate + " )" + '\n'     // filter
        
        // Return Expression
        + '\n' + rReturnA1
        
        // optional in DAX
        // useful in cumulative measures - returns blank if no value exists for future dates
        + '\n' + ifnotBlank + m.DaxObjectName + thenResult
        );
/* DAX expression END */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m12.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m12.Description = "From: " + m.Name + " - " + '\n' +
        // Type metadata text here
        "cumulative measure; "
        ;                             
        m12.FormatString = Decimal
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/
}
/**** C# SCRIPT END ****/

```
