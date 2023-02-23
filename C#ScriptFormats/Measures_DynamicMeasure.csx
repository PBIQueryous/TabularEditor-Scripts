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
/* 
foreach( var m in Selected.Measures) 

{
  m.Table.AddMeasure(
      "MeasureName", 
      "Expression", 
      m.DisplayFolder)
      ;
} 
*/


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

// Var Measure Folder
string _folder = null                                       ;
string _subfolder = null                                    ;

// assign format
string _typeCurrency = "£"                                  ;
string _typeCount = "#"                                     ;
string _typePercent = "%"                                   ;
string _mFormat = null	                                    ;   //-- format string


// Var Calendar Aspect
var _AY = "AY"                                              ;
var _FY = "FY"                                              ;
var _CY = "CY"                                              ;
var _Academic = "Academic"                                  ;
var _Fiscal = "Fiscal"                                      ;
var _Calendar = "Calendar"                                  ;
var _FYEnd = "31/3"                                         ;
var _AYEnd = "31/7"                                         ;
var _CYEnd = "31/12"                                        ;
string _EndPeriod = null                                    ;
string _PeriodDesc = null                                   ;

// Var RETURN text strings
var ts_Return = "RETURN" + '\n'             ;
var ts_Result = "_result "                  ;
var var_Result = ts_Return + _ + ts_Result	;



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
var _visibleDates = "[@VisibleDates]"                                                   ;


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

   // var _mMeasurename__     = var_Name.Replace(" | BASE","")            ;
    // replace measure name text substring
    string _mMeasureName            = var_Name                             ;
    string _sp = " "                  ;
    bool _spTest = _mMeasureName.Contains( "|" ) ;
// _mMeasureName	+ _affix
    int _index                      = _mMeasureName.IndexOf("|")           ;
    if (_index >= 0) _mMeasureName  = _mMeasureName.Substring(0, _index)   ;
    
    string _mCleanMeasureName = null ;
    if ( _spTest ) _mCleanMeasureName = _mMeasureName ; 
        else _mCleanMeasureName = _mMeasureName  + " ";

    


 

    // final return result variable
    var _result             = _ + var_Name	                            ;
    var _daxResult          = ts_Return + _ + _result                   ;

/* SUBSCRIPT START *****************************************************************************/

//- Measure1 Title: CUMULATIVE SUM FISCAL YTD: -----------------------------------------\\
/* Annotation Measure **************************************************************************/
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

    
        // measure affix + annotation and formatting
    var _affix              = ytd	                         ;   //-- measure affix
    var _newMeasureName     = _mCleanMeasureName + _affix    ;       //-- new measure name + affix
    string _daxExpression = null ;



 // assign relevant meta based on affix
    var _calendarType = _affix ;         //-- choose calendar type
    string _mAnnotationStr = null ;
    string _mDAX ;
    bool _testFiscal = _calendarType.Contains( _FY ) ;
    bool _testAcademic = _calendarType.Contains( _AY ) ;
    bool _testCalendar = _calendarType.Contains( _CY ) ;
    bool _testACT = _calendarType.Contains( "ACT" ) ;
    bool _testYTD = _calendarType.Contains( "YTD" ) ;
    bool _testCML = _calendarType.Contains( "YTD" ) || _calendarType.Contains( "CML" ) ;
    if ( _testCML || _testYTD ) _subfolder = "CML" ; else _subfolder = "ACT" ;
    if ( _testYTD ) _mAnnotationStr = "YTD" ; 
        else _mAnnotationStr = _subfolder ;
// if (s.contains("a")||s.contains("b")||s.contains("c"))
    

    
// check format type for folder designation
    bool _checkCurrency = var_Name.StartsWith( _typeCurrency ) ;
    bool _checkCount = var_Name.StartsWith( _typeCount ) ;
    bool _checkPercent = var_Name.StartsWith( _typePercent ) ;
    
 // test format
    if ( _checkCurrency ) _mFormat = Currency0          ;
        else if ( _checkCount ) _mFormat = Whole        ;   
        else if ( _checkPercent ) _mFormat = Percent ;
        else _mFormat = Whole   ;

    if ( _checkCurrency ) _folder = _typeCurrency + " SUMs"          ;
        else if ( _checkCount ) _folder = _typeCount + " COUNTs"         ;   
        else if ( _checkPercent ) _folder = _typePercent + " PCTs" ;
        else _folder = "_BASE"   ;

// assign folder and subfolder
    var newMeasureFolder = "__" + _folder                                  ; //-- display folder
    var newMeasureSubFolder = "\\" + _subfolder                         ;   //-- display subfolder


    
 // set ytd period
    if ( _testFiscal ) _EndPeriod = _FYEnd; 
    else if ( _testAcademic ) _EndPeriod = _AYEnd ; 
    else if ( _testCalendar ) _EndPeriod = _CYEnd ; else _EndPeriod = "31/12";

 // set relevant period 
    if ( _testFiscal ) _PeriodDesc = _Fiscal; 
    else if ( _testAcademic ) _PeriodDesc = _Academic ; 
    else if ( _testCalendar ) _PeriodDesc = _Calendar ; else _PeriodDesc = "Actual";


    var _annotationTXT           = " -- " + _PeriodDesc + " " + _mAnnotationStr          ;   //-- annotation
    var _measureDescription      = _folder + " " + _mAnnotationStr + ": "         ;   //-- calculation type

    var _calendarPeriod = __ + "CALENDAR: " + _bp + _PeriodDesc.ToUpper() ;

   var _filtersINCLUDE =             
            __ + _inc
            + _bp +  _PeriodDesc + " Period ( year end "+ _EndPeriod +" )"
            + _bp +  _PeriodDesc + " year"
            ;
    var _filtersEXCLUDE =
            __ + _exc
            + _bp + "n/a"
            ;
    var _mAnnotation =
            __ + _annotationTXT 
            + __;

/* Dax Expressions ******************************************************************************/
    var _mYTD = __ +
             var_ + ts_Result + 
            "= CALCULATE ( " +m.DaxObjectFullName+ ", " + isBeforeToday + " )"	            
            + __
            ;

    var _mCML = __ +
             var_maxDate + maxDimDate		
            + __ +  
             var_ + ts_Result + 
            "= CALCULATE ( " +var_FullDaxObject+ ", " + isBeforeMaxDate	 + " )"	            
            + __
            ;

    var _mREM = __ +
             var_ + ts_Result + 
            "= CALCULATE ( " +m.DaxObjectFullName+ ", " + isAfterToday	+ " )"	            
            + __
            ;

    var _mYTDCML = __ +

            // variable1
             var_maxDate + maxDimDate		
            + __ +  
 
            // result variable 
             var_ + ts_Result + 
            "= IF( " + _visibleDates + ",  TOTALYTD ( " +var_FullDaxObject+ ", " + col_DimDates	+ " , " + qt + _EndPeriod + qt + " ) )"	            
            + __ +
             var_ + "_result1 " + 
            "= IF( " + _visibleDates + ",  CALCULATE ( " +var_FullDaxObject+ ", " + "DATESYTD( " + col_DimDates	+ " , " + qt + _EndPeriod + qt + ") ) )"
            + __ +
             var_ + "_KPIresult " + 
            "= IF( ISBLANK ( " + ts_Result + " ) , 0 , " + ts_Result + " ) -- return zero for KPI cards"
            + __;

/* Meta Description ****************************************************************************/
    var _mDescription =
            _measureDescription + _bp                                 //-- meta: measure description               
            + m.DaxObjectFullName                               //-- referenced measure name
            // describe filters (included and excluded)
            
            + __ + _calendarPeriod
            // include
            + _filtersINCLUDE
 
            // exclude
            + _filtersEXCLUDE	
            ;

/************************************************************************* DAX expression START */
    // new measure name + affix

    if (_newMeasureName.Contains(rem)) _daxExpression = _mREM; 
    else if (_newMeasureName.Contains(ytd)) _daxExpression = _mYTD;
    else if (_newMeasureName.Contains(cml)) _daxExpression = _mCML;

    var newMeasure          = m.Table.AddMeasure                //-- create measure function
    (                                 
        _newMeasureName	                                ,       //-- MeasureName                                         
        _mAnnotation 	                                        //-- Annotation 
        + _daxExpression	                                    //-- Expression 
        + __ + var_Result	                                    //-- Return + Result   
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        newMeasure.DisplayFolder    = newMeasureFolder                           //-- measure display folder
                                        + newMeasureSubFolder	 ;   
        newMeasure.Description      = _mDescription                              ; 	  //-- measure description   
        newMeasure.FormatString     = _mFormat	                                 ;	  //-- measure format
        newMeasure.IsHidden = false                                              ;    // Hide the base column:
        

/* SUBSCRIPT END *********************************************************************************/
}
/********************************************************************************* C# MeasureEnd */
/**** C# SCRIPT END ****/

