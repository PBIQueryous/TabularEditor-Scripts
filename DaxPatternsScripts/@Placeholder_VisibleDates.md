# @Placeholder_VisibleDates
## Placeholder to show only visible dates

```c#

/*---------------------------------------------------
| DESCRIPTION:                                       |
| Create SUM from Selected Columns                   |
| Tabular Editor Advanced Script                     |
 ----------------------------------------------------
| AUTHOR:                                            |
| Imran Haq | PBI Queryous                           |
| https://github.com/PBIQueryous                     |
| STAY QUERYOUS!                                     |
 ---------------------------------------------------*/


//--- C# measure formula template ---\\
/* m.Table.AddMeasure( "MeasureName", "Expression", m.DisplayFolder); */


 /* DESCRIPTION
 * -----------------------------------
 * This script, when executed, will loop through the currently selected measure(s),
 * creating a series of measure(s) declared in the script below.
 * 
 * e.g., from Model select [Measure] where Measure = SUM( tbl[column] ) or COUNT( tbl[column] )
 * -----------------------------------
 */

/**** C# SCRIPT START ****/

///--- SET VARIABLES ---\\\

//-- Quotation Character - helpful for wrapping " " around a text string within the DAX code
const string qt = "\"";
var lf = '\n';


// Number Formatting Strings
var DateFormat = "dd/MM/yy";

// Var RETURN text strings
var ts_Return = "RETURN" + '\n';
var ts_Result = '\t' + "_result";

// TimeIntel Variable Filters
var col_DimDates = "DIM_Dates[Date]";
var minDimDate = "MIN( " + col_DimDates	 + " )";
var maxDimDate = "MAX( " + col_DimDates	 + " )";
var var_minCalendarDate = "var _minCalendarDate = " + minDimDate;
var var_placeholderDateResult = "var _result = _minCalendarDate <= _maxAvailableDate";

// Var Measure Folder
var newMeasureFolder = "@@Formatting";
var newMeasureSubFolder = "\\SubFolder";

// Script Variable: Creates a series of time intelligence measures for each selected measure in the Model
foreach(var c in Selected.Columns) 
{
 

 
/***************************************** MeasureStart ************************************/
//-- Measure1: Placeholder for Last Visible Date --\\

    var _maxAvailableDate = "var _maxAvailableDate = CALCULATE ( MAX ( " + c.DaxObjectFullName + " ) , REMOVEFILTERS() )";
    var newMeasure = c.Table.AddMeasure
    (                             

// startSubScript
        //-- MeasureName
        "@MaxVisibleDate" + c.DaxTableName	,                             
    
        //-- DAX comment string
        lf + 
        "-- get latest visible date from table "                            
        + lf

/* DAX expression START */              
        
        //-- Result Expression Variable
        + lf 
        + _maxAvailableDate  
        + lf
        + var_minCalendarDate	
        + lf
        + var_placeholderDateResult 
        // Return Expression
        + lf 
        + ts_Return
        + ts_Result
        
        //-- add optional code below
        //-- eg: IF( ISBLANK (measure) , 0 , _variable )
        );
/* DAX expression END */
        
//-- Metadata
        //-- Display Folder (default - same folder as selected)
        newMeasure.DisplayFolder 
        //-- Optional: new Folder name below
        = newMeasureFolder	
        ;      
    
//-- Provide some documentation
        newMeasure.Description = 
        "Last Date of " 
        + c.DaxObjectFullName  + ": " 
        + lf +
        //-- Type metadata text here
        "• Last Visible Date"
        ;                             
        newMeasure.FormatString = DateFormat	
        ;

        // Hide the base column:
        newMeasure.IsHidden = true;
//-- endSubScript
/**************************************** MeasureEnd **************************************/


}
/**** C# SCRIPT END ****/


```
