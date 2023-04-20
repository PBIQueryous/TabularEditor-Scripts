```C#

/*---------------------------------------------------
| DESCRIPTION:                                       |
| Generate KPI-card friendly Measures                |
| Tabular Editor Advanced Script                     |
 ----------------------------------------------------
| AUTHOR:                                            |
| Imran Haq | PBI Queryous                           |
| https://github.com/PBIQueryous                     |
| STAY QUERYOUS!                                     |
 ---------------------------------------------------*/


//--- C# measure formula template ---\\
/* 
 * m.Table.AddMeasure( "MeasureName", "Expression", m.DisplayFolder);
 */

/* Creates a SUM measure for every currently selected column(s)
 *
 * Author: Mike Carlo, https://powerbi.tips
 *
 * Select Columns using Control + Left Click
 * Script will create one measure for each column selected
 * Then Change the formatting of the column, adds a description,
 * and places measures in a named folder.
 *
 */


/**** C# SCRIPT START ****/

///--- SET VARIABLES ---\\\
//-- Quotation Character - helpful for wrapping " " around a text string within the DAX code
const string qt = "\"";
var newMeasureFolder = "__Measures";
var newMeasureSubFolder = "\\_KPIs";

var GBP0 = qt + "£" + qt + "#,0";

//-- Loop through the list of selected columns
foreach(var m in Selected.Measures)
{
    var newMeasure = m.Table.AddMeasure(
    m.Name + " (KPI)",             // Name
        
        
    "var _m =" + m.DaxObjectFullName +
    "var _result = IF( ISBLANK( _m ) , 0 , _m ) "   // take selected measure - if blank return zero
    + " RETURN" + " _result"

    );
    
    //-- Set the format string on the new measure:
    newMeasure.FormatString = "#,0";

    //-- Provide some documentation:
    newMeasure.Description = "KPI of: " + m.DaxObjectFullName;

    //-- Create all measures within a Named Folder
    newMeasure.DisplayFolder = 
    newMeasureFolder + newMeasureSubFolder
        // + newMeasureSubFolder
        ;
}

```
