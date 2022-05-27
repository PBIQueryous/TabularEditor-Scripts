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



// Script Variable: Creates a series of time intelligence measures for each selected measure in the Model
foreach(var m in Selected.Measures) 
{
 
// Expression Strings
var selectedMeasure = m.Name;
var measureName = m.DaxObjectName;
var resultLine = '\n' + vResult + measureName + '\n';
var returnLineA = '\n' + rReturn + '\n' + "// " + rResult + '\n' + ifnotBlank + measureName + thenResult;
var returnLineB = '\n' + rReturn + '\n' + rResult + '\n' + "// " + ifnotBlank + measureName + thenResult;


// Metadata Strings
var descriptionString = "From: " + selectedMeasure + " - " + '\n';
var subFolder = "_Measures\\SubFolder";
 
/***************************************** MeasureStart ************************************/
// Measure1: SUM
    var m1 = m.Table.AddMeasure
    (                             

// startSubScript
        
        // MeasureName
        selectedMeasure + snap,                               
        '\n' + 
        
        // DAX comment string
        "// snapshot - basic sum "                           
        
/* -- DAX expression START -- */
        
        // Result Expression Variable
        + resultLine
        
        // Return Expression
        + returnVariableA
        
        );
        
/* --  DAX expression END -- */
        
// Metadata
        // Display Folder (default - same folder as selected)
        m1.DisplayFolder 
        // Optional: new Folder name below
        = subFolder
        ;      
    
// Provide some documentation
        m1.Description = descriptionString +
        // Type metadata text here
        " Base Measure; SUM (Snapshot) "
        ;                             
        m1.FormatString = Decimal
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/
}
/**** C# SCRIPT END ****/
```
