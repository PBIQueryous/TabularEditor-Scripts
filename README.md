# TabularEditor-Scripts
Tabular Editor 2 Scripts for PBI

## Template
```c#
/* SCRIPT NOTES
 *  ----------------------------------
 * | Title:                           |
 * | Time-intelligence Measure Series |
 *  ----------------------------------
 * | Author:                          |
 * | Imran Haq, PBI Queryous          |
 *  ----------------------------------
 * -----------------------------------
 * Inspiration and Credits:           
 * PowerBI.Tips Team, https://powerbi.tips/
 * Daniel Otykier, twitter.com/DOtykier,
 * and endless more names from the PBI community, thank you!
 * -----------------------------------
 * Description:
 * This script, when executed, will loop through the currently selected columns,
 * creating a series of measure for each column and also hiding the column itself.
 * -----------------------------------
 * C# measure formula template:
 * m.Table.AddMeasure( "MeasureName", "Expression", m.DisplayFolder);
 */

/* SET VARIABLES */

// Quotation Character - helpful for wrapping " " around a text string within the DAX code
const string qt = "\"";

// Number Formatting Strings
var GBP = qt + "£" + qt;
var Whole = "#,0";
var Percent = "0.0 %";
var Decimal = "#,0.0";
var Number = "#,0";
var Currency = GBP + "#,0; -" + GBP + "#,0;" + GBP + "#,0";

// Standard Date Variables - includes Date Columns and Text Strings
var dateColumn = "Dates[Date]";
var mtdColumn = "Dates[LatestMTD]";
var endDate = "31/3";
var endFY = qt + "31/3" + qt; 
var datesYTD = ", DATESYTD( " + dateColumn + ", " + endFY + " ) ";

// Fiscal Filter Date Variables
var calcVarMinMTDFY = "VAR _min = CALCULATE( MIN( Dates[LatestMTD] ) , Dates[IsCFY] = TRUE)";
var calcVarMaxMTDFY = "VAR _max = CALCULATE( MAX( Dates[LatestMTD] ) , Dates[IsCFY] = TRUE)";
var calcVarMaxDateFY = "VAR _max = CALCULATE( MAX( Dates[Date] ) , Dates[IsCFY] = TRUE)";
var calcVarMinMaxFY = calcVarMinMTDFY + '\n' + calcVarMaxMTDFY; 
var beforeVarMax = dateColumn + " <= _max ";
var beforeVarYtd = dateColumn + " <= _ytd ";
var betweenVarFY = dateColumn + " >= _min " + "&& " + dateColumn + " <= _max ";

// Filtered Date Variables
var calcMaxMTD = "CALCULATE( MAX( Dates[LatestMTD] ) , REMOVEFILTERS ()) ";
var calcVarMaxMTD = "VAR _ytd = CALCULATE( MAX( Dates[LatestMTD] ) /* , REMOVEFILTERS () */ ) ";
var calcVarMaxMTDremoveFilter = "VAR _YtD = CALCULATE( MAX( Dates[LatestMTD] ) , REMOVEFILTERS ()) ";
var calcVarMaxDate = "VAR _max = CALCULATE( MAX( Dates[Date] ) /* , Dates[IsCFY] = TRUE */ )";

var calcVarMaxYTD1 = "VAR _ytd1 = CALCULATE( MAX( Dates[MTDAdd1] ) , Dates[IsCFY] = TRUE)";
var calcVarMaxYTD2 = "VAR _ytd2 = CALCULATE( MAX( Dates[MTDAdd2] ) , Dates[IsCFY] = TRUE)";
var calcVarMaxYTD3 = "VAR _ytd3 = CALCULATE( MAX( Dates[MTDAdd3] ) , Dates[IsCFY] = TRUE)";

// Text Fillers for Measure Templates
var mMeasure = qt + "[MeasureName]" + qt;
var mActual = qt + "[Actual]" + qt;
var mPlan = qt + "[Plan]" + qt;

// MeasureName Variables
var sum = " | SUM";
var snap = " | SNAP";
var ytdSnap = " | YTD SNAP";
var efy = " | EFY SNAP";
var efyCML = " | EFY CML";
var cml = " | CML";
var fytdCML = " | YTD CML";
var fytd = " | FYTD";
var rem = " | REM";

// Var RETURN text strings
var rReturnRes = "RETURN" + '\n' + "_result";
var rReturn = "RETURN" + '\n';
var rResult = "_result";

// Var Measure Folder
var subFolder = "TimeInt";

// Script Variable
// Creates a series of time intelligence measures for each selected (base SUM) measure:
foreach(var m in Selected.Measures) 

{   // SCRIPT START
    
/***************************************** MeasureStart ************************************/
// Measure1: SUM
    var m1 = m.Table.AddMeasure
    (                             

// startSubScript
        // MeasureName
        m.Name + sum,                               
    
        // DAX comment string
        '\n' + "// Base SUM "                           
        
/* DAX expression START */
        // DAX Variables
        + '\n' + '\n' + calcVarMaxDateFY                
        + '\n' + "VAR _result = CALCULATE( " + m.DaxObjectName + " ) " + '\n'
        
        // Return Result
        + '\n' + rReturn
        + '\n' + '\t' + "// IF(  NOT ISBLANK( " + m.DaxObjectName + " ) ,  _result  )"
        + '\n' + '\t' + rResult
        );
/* DAX expression END */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m1.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m1.Description = "Derived from " + m.Name + ": " + 
        // Type metadata text here
        "Base Measure - End for Year, no filters."
        ;                             
        m1.FormatString = Currency
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/
}     // SCRIPT END
```
