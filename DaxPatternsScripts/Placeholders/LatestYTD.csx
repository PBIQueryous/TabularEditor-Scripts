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
var __ = '\n';
var ___ = '\n' + '\t';
var _ = '\t';
String str = "'";

// Number Formatting Strings
var DateFormat = "dd/MM/yy";

// Var RETURN text strings
var ts_Return = "RETURN" + '\n';
var ts_Result = '\t' + "_result";

// TimeIntel Variable Filters
var var_ = "var ";
var col_DimDates = " DIM_Dates[Date] ";
var minDimDate = " MIN( " + col_DimDates	 + " ) ";
var maxDimDate = " MAX( " + col_DimDates	 + " ) ";
var var_minCalendarDate = "var _minCalendarDate = " + minDimDate;
var var_placeholderDateResult = "var _result = _minCalendarDate <= _maxAvailableDate ";

// Var Measure Folder
var newMeasureFolder = "@@DatePlaceholders";
var newMeasureSubFolder = "\\SubFolder";

// Script Variable: Creates a series of time intelligence measures for each selected measure in the Model

foreach(var c in Selected.Columns) 

/***************************************************************************** C# MeasureStart */
{
    var var_Name = "_maxDateYTD ";
    var _tableNameDefault = c.DaxTableName;
    var _mName = var_Name;
    var _mMeasureName = var_Name.Replace("_","@");
    var _result = _ + var_Name	;
    var _daxResult = ts_Return + _result;

/* SUBSCRIPT START *****************************************************************************/

//- Measure1 Title: Placeholder for Last Visible Date -----------------------------------------\\
/* Annotation Measure **************************************************************************/
    var _mAnnotation =
            __ + 
            " -- MaxDate - Year to Date (last complete day) "                            
            + __
            ;

/* Dax Expression ******************************************************************************/
    var _daxExpression = __ +
             var_ + var_Name + 
            "= CALCULATE (" +maxDimDate+ ", DIM_Dates[IsAfterToday] = FALSE )"            
            + __
            ;

/* Meta Description ****************************************************************************/
    var _mDescription =
            "Latest Complete Day YTD of " +col_DimDates+ ": " 
            + ___ + "• Last Visible Date"
            ;

/************************************************************************* DAX expression START */
    var newMeasure = c.Table.AddMeasure
    (                                 
        _mMeasureName,	            //-- MeasureName                                         
        _mAnnotation 	            //-- Annotation 
        + _daxExpression	        //-- Expression 
        + __ + _daxResult           //-- Return   
        
/* add optional code below *****************/  
        // " IF( ISBLANK (measure) , 0 , _variable ) "
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        newMeasure.DisplayFolder = newMeasureFolder;     //-- measure display folder
        newMeasure.Description = _mDescription; 	     //-- measure description   
        newMeasure.FormatString = DateFormat;	         //-- measure format
        newMeasure.IsHidden = true;                      // Hide the base column:
        

/* SUBSCRIPT END *********************************************************************************/
}
/********************************************************************************* C# MeasureEnd */
/**** C# SCRIPT END ****/

