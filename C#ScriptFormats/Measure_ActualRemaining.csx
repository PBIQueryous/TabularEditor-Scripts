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
const string qt = "\""      ;
var __ = '\n'               ;
var _lf = '\n' + '\t'       ;
var _ = '\t'                ;
String str = "'"            ;
var _bp = __ + "     • "    ;
var _inc = __ + "INCLUDE:" ;
var _exc = __ + "EXCLUDE:" ;

// Number Formatting Strings
var DateFormat = "dd/MM/yy"                                 ;
var GBP0 = qt + "£" + qt + "#,0"                            ;
var GBP2 = qt + "£" + qt + "#,0.00"                         ;
var posGBP = GBP0                                           ;
var negGBP = "-"+GBP0                                       ;
var neutGBP = GBP0                                          ;
var Whole = "#,0"                                           ;
var Percent = "0.0 %"                                       ;
var Decimal = "#,0.0"                                       ;
var Number = "#,0"                                          ;
var Currency0 = posGBP +";" + negGBP + ";" + neutGBP        ;
var Currency2 = GBP2+";" +"-"+GBP2+";" +GBP2                ;
var Deviation = "+"+Decimal+";" +"-"+Decimal+";"+ Decimal   ;

// Var RETURN text strings
var ts_Return = "RETURN" + '\n'             ;
var ts_Result = "_result "                  ;
var var_Result = ts_Return + _ + ts_Result	;

// MeasureName Variables
var act = " | ACT"                    ;
var ytd = " | YTD"                      ;
var cml = " | CML"                      ;
var rem = " | REM"                      ;
var ytdCml = " | YTD CML"               ;
var remCml = " | REM CML"               ;
var fytd = " | FYTD"                    ;
var aytd = " | AYTD"                    ;
var fytdCml = " | FYTD CML"             ;
var aytdCml = " | AYTD CML"             ;
var pytd = " | PYTD"                    ;
var pfytd = " | PFYTD"                  ;
var paytd = " | PAYTD"                  ;
var pfytdCml = " | PFYTD CML"           ;
var paytdCml = " | PAYTD CML"           ;

// TimeIntel Variable Filters
var var_ = "var "                                                                       ;
var col_DimDates = " DIM_Dates[Date] "                                                  ;
var minDimDate = " MIN( " + col_DimDates	 + " ) "                                    ;
var maxDimDate = " MAX( " + col_DimDates	 + " ) "                                    ;
var var_minCalendarDate = "var _minCalendarDate = " + minDimDate                        ;
var var_placeholderDateResult = "var _result = _minCalendarDate <= _maxAvailableDate "  ;
var isAfterToday = "DIM_Dates[IsAfterToday] = TRUE"                             ;
var isBeforeToday = "DIM_Dates[IsAfterToday] = FALSE"                          ;

// Var Measure Folder
var newMeasureFolder = "__£ Values"          ;
var newMeasureSubFolder = "\\Actual"         ;

// Script Variable: Creates a series of time intelligence measures for each selected measure in the Model

foreach(var m in Selected.Measures) 

/***************************************************************************** C# MeasureStart */
{
    var var_Name = m.Name	                            ;
    var _mName = var_Name                               ;
    var _mMeasureName = var_Name.Replace(" | BASE","")  ;
    var _newMeasureName = _mMeasureName + rem           ;
    var _result = _ + var_Name	                        ;
    var _daxResult = ts_Return + _ + _result            ;

/* SUBSCRIPT START *****************************************************************************/

//- Measure1 Title: Placeholder for Last Visible Date -----------------------------------------\\
/* Annotation Measure **************************************************************************/
    var _mAnnotation =
            __ + 
            " -- Actual Values later than or to equal to TODAY "                            
            + __
            ;

/* Dax Expression ******************************************************************************/
    var _daxExpression = __ +
             var_ + ts_Result + 
            "= CALCULATE ( " +m.DaxObjectFullName+ ", " + isAfterToday	+ " )"	            
            + __
            ;

/* Meta Description ****************************************************************************/
    var _mDescription =
            "SUM: " + m.DaxObjectFullName 	  
            + __ + _inc
            + _bp + "Values upto TODAY (equals Actual YTD)"
            + __ + _exc
            ;

/************************************************************************* DAX expression START */
    var newMeasure = m.Table.AddMeasure
    (                                 
        _newMeasureName ,	        //-- MeasureName                                         
        _mAnnotation 	            //-- Annotation 
        + _daxExpression	        //-- Expression 
        + __ + var_Result	        //-- Return   
        
/* add optional code below *****************/  
        // " IF( ISBLANK (measure) , 0 , _variable ) "
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        newMeasure.DisplayFolder = newMeasureFolder + newMeasureSubFolder	 ;    //-- measure display folder
        newMeasure.Description = _mDescription                               ; 	  //-- measure description   
        newMeasure.FormatString = GBP0	                                     ;	  //-- measure format
        newMeasure.IsHidden = false                                          ;    // Hide the base column:
        

/* SUBSCRIPT END *********************************************************************************/
}
/********************************************************************************* C# MeasureEnd */
/**** C# SCRIPT END ****/

