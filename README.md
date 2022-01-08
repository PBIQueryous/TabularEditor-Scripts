# TabularEditor-Scripts
Tabular Editor 2 Scripts for PBI

## Template

```c#

  
/*---------------------------------
| Title:                           |
| C# Tabular Editor - DAX template |
 ----------------------------------
| Author:                          |
| Imran Haq, PBI Queryous          |
| https://github.com/PBIQueryous   |
| STAY QUERYOUS PBIX CHAMPS!       |
 ---------------------------------*/

 /* SCRIPT NOTES 
 *--------------------------------------------------*
 | Inspiration and Credits:                         |
 | PowerBI.Tips Team    |   https://powerbi.tips/   |
 | Daniel Otykier       |   twitter.com/DOtykier    |
 | and endless more names from the PBI community,   |
 | Thank you all!                                   |
 *--------------------------------------------------*
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
var GBP0 = qt + "£" + qt + "#,0.0";
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
var rReturnRes = "RETURN" + '\n' + "_result";
var rReturn = "RETURN" + '\n';
var ifnotBlank = '\t' + "// IF(  NOT ISBLANK( ";
var thenResult = " ) ,  _result  )";
var rResult = '\t' + "_result";

// MeasureName Variables
var snap = " | SNAP";

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
        
        
        // Return Result
        + '\n' + vResult + m.DaxObjectName + '\n'
        + '\n' + rReturn
        + '\n' + rResult
        
        // optional in DAX
        // useful in cumulative measures - returns blank if no value exists for future dates
        + ifnotBlank + m.DaxObjectName + thenResult
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
        m1.FormatString = Currency2
        ;
// endSubScript
/**************************************** MeasureEnd **************************************/



}
/**** C# SCRIPT END ****/

```
