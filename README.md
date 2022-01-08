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
 * PowerBI.Tips Team, 
 * Daniel Otykier, twitter.com/DOtykier,
 * -----------------------------------
 * Description:
 * This script, when executed, will loop through the currently selected columns,
 * creating a series of measure for each column and also hiding the column itself.
 * -----------------------------------
 * C# measure formula template:
 * m.Table.AddMeasure( "MeasureName", "Expression", m.DisplayFolder);
 */

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
