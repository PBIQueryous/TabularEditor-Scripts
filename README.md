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

// Quotation Character - helpful for wrapping " " around a text string within the DAX code
const string qt = "\"";

// Var Measure Display Folder
var subFolder = "TimeInt";

// Number Formatting Strings
var GBP = qt + "£" + qt;
var Whole = "#,0";
var Percent = "0.0 %";
var Decimal = "#,0.0";
var Number = "#,0";
var Currency = GBP + "#,0; -" + GBP + "#,0;" + GBP + "#,0";

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
