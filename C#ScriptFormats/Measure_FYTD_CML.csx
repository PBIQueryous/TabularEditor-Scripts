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
var GBP0 =  "£#,0"                                          ;
var GBP2 = "£#,0.00"                                        ;
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
var act = "| ACT"                    ;
var ytd = "| YTD"                      ;
var cml = "| CML"                      ;
var rem = "| REM"                      ;
var ytdCml = "| YTD CML"               ;
var remCml = "| REM CML"               ;
var fytd = "| FYTD"                    ;
var aytd = "| AYTD"                    ;
var fytdCml = "| FYTD CML"             ;
var aytdCml = "| AYTD CML"             ;
var pytd = "| PYTD"                    ;
var pfytd = "| PFYTD"                  ;
var paytd = "| PAYTD"                  ;
var pfytdCml = "| PFYTD CML"           ;
var paytdCml = "| PAYTD CML"           ;

// TimeIntel Variable Filters
var var_ = "var "                                                                       ;
var var_maxDate = "var _maxDate = "                                                     ;
var _maxDate = "_maxDate "                                                              ;
var col_DimDates = " DIM_Dates[Date] "                                                  ;
var minDimDate = " MIN( " + col_DimDates + " ) "                                        ;
var maxDimDate = " MAX( " + col_DimDates + " ) "                                        ;
var var_minCalendarDate = "var _minCalendarDate = " + minDimDate                        ;
var var_placeholderDateResult = "var _result = _minCalendarDate <= _maxAvailableDate "  ;
var isAfterToday = "DIM_Dates[IsAfterToday] = TRUE"                                     ;
var isBeforeToday = "DIM_Dates[IsAfterToday] = FALSE"                                   ;
var isBeforeMaxDate = col_DimDates + " <= " + _maxDate                                  ;



// Script Variable: Creates a series of time intelligence measures for each selected measure in the Model

foreach(var m in Selected.Measures) 

/***************************************************************************** C# MeasureStart */
{
    // measure name variable
    var var_Name            = m.Name	                                   ;
    var _mName              = var_Name                                     ;
    var _tableNameDefault   = m.DaxTableName                               ;
    var var_FullDaxObject   = m.DaxObjectFullName                          ;
    var var_DaxObject       = m.DaxObjectName	                           ;

    // replace measure name text substring
    string _mMeasureName            = var_Name                             ;
    int _index                      = _mMeasureName.IndexOf("|")           ;
    if (_index >= 0) _mMeasureName  = _mMeasureName.Substring(0, _index)   ;

    


    // var _mMeasurename__     = var_Name.Replace(" | BASE","")            ;

    // final return result variable
    var _result             = _ + var_Name	                            ;
    var _daxResult          = ts_Return + _ + _result                   ;

/* SUBSCRIPT START *****************************************************************************/

//- Measure1 Title: CUMULATIVE SUM FISCAL YTD: -----------------------------------------\\
/* Annotation Measure **************************************************************************/
    
    // Var Measure Folder
    var newMeasureFolder = "__£ SUMS"                                  ;   //-- display folder
    var newMeasureSubFolder = "\\FiscalPeriod"                         ;   //-- display subfolder
    
    // measure affix + annotation and formatting
    var _affix                   = fytdCml	                             ;   //-- measure affix
    var _mFormat                 = Currency0	                         ;   //-- format string
    var _annotationTXT           = " -- Fiscal YTD Cumulative "          ;   //-- annotation
    var _measureDescription      = "CUMULATIVE SUM FISCAL YTD: "         ;   //-- calculation type
    var _filtersINCLUDE =             
            __ + _inc
            + _bp + "Cumulative sum across Fiscal Period ( year end 31/3 )"
            + _bp + "Cumulative sum each fiscal year"
            ;
    var _filtersEXCLUDE =
            __ + _exc
            + _bp + "n/a"
            ;
    var _mAnnotation =
            __ + _annotationTXT 
            + __;

/* Dax Expression ******************************************************************************/
    var _FYEnd = "31/3";
    var _AYEnd = "31/7";
    var _EndPeriod = _FYEnd;
    var _daxExpression = __ +

            // variable1
             var_maxDate + maxDimDate		
            + __ +  
 
            // result variable 
             var_ + ts_Result + 
            "= TOTALYTD ( " +var_FullDaxObject+ ", " + col_DimDates	+ ", " + qt + _EndPeriod + qt + " )"	            
            
            + __;

/* Meta Description ****************************************************************************/
    var _mDescription =
            _measureDescription                                 //-- meta: measure description               
            + m.DaxObjectFullName                               //-- referenced measure name
            // describe filters (included and excluded)
            // include
            + _filtersINCLUDE
 
            // exclude
            + _filtersEXCLUDE	
            ;

/************************************************************************* DAX expression START */
    // new measure name + affix

    var _newMeasureName     = _mMeasureName	+ _affix    ;       //-- new measure name + affix
    var newMeasure          = m.Table.AddMeasure                //-- create measure function
    (                                 
        _newMeasureName	                                ,       //-- MeasureName                                         
        _mAnnotation 	                                        //-- Annotation 
        + _daxExpression	                                    //-- Expression 
        + __ + var_Result	                                    //-- Return + Result   
        
/* add optional code below *****************/  
        // " IF( ISBLANK (measure) , 0 , _variable ) "
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        newMeasure.DisplayFolder    = newMeasureFolder + newMeasureSubFolder	 ;    //-- measure display folder
        newMeasure.Description      = _mDescription                              ; 	  //-- measure description   
        newMeasure.FormatString     = _mFormat	                                 ;	  //-- measure format
        newMeasure.IsHidden = false                                              ;    // Hide the base column:
        

/* SUBSCRIPT END *********************************************************************************/
}
/********************************************************************************* C# MeasureEnd */
/**** C# SCRIPT END ****/

