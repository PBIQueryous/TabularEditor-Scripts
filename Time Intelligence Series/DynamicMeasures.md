# Dynamic Meausres
## Dynamic Measures to build time intelligence measures with corresponding annotation and descriptions

```c#

/*---------------------------------------------------
| DESCRIPTION:                                       |
| Create measures and meta dynamically               |
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
string _inc = __ + "INCLUDE:" ;
string _exc = __ + "EXCLUDE:" ;

// Number Formatting Strings
string DateFormat = "dd/MM/yy"                                 ;
string GBP0 =  "£#,0"                                          ;
string GBP2 = "£#,0.00"                                        ;
string posGBP = GBP0                                           ;
string negGBP = "-"+GBP0                                       ;
string neutGBP = GBP0                                          ;
string Whole = "#,0"                                           ;
string Percent = "0.0 %"                                       ;
string Decimal = "#,0.0"                                       ;
string Number = "#,0"                                          ;
string Currency0 = posGBP +";" + negGBP + ";" + neutGBP        ;
string Currency2 = GBP2+";" +"-"+GBP2+";" +GBP2                ;
string Deviation = "+"+Decimal+";" +"-"+Decimal+";"+ Decimal   ;

// Var Measure Folder
string _folder = null                                       ;
string _subfolder = null                                    ;

// assign format
string _typeCurrency = "£"                                  ;
string _typeCount = "#"                                     ;
string _typePercent = "%"                                   ;
string _mFormat = null	                                    ;   //-- format string


// Var Calendar Aspect
string _AY = "AY"                                              ;
string _FY = "FY"                                              ;
string _CY = "CY"                                              ;
string _Academic = "Academic"                                  ;
string _Fiscal = "Fiscal"                                      ;
string _Calendar = "Calendar"                                  ;
string _FYEnd = "31/3"                                         ;
string _AYEnd = "31/7"                                         ;
string _CYEnd = "31/12"                                        ;
string _EndPeriod = null                                    ;
string _PeriodDesc = null                                   ;

// Var RETURN text strings
string ts_Return = "RETURN" + '\n'             ;
string ts_Result = "_result "                  ;
var var_Result = ts_Return + _ + ts_Result	;



// TimeIntel Variable Filters
string var_ = "var "                                                                       ;
string var_maxDate = "var _maxDate = "                                                     ;
string var_minDate = "var _minDate = "                                                     ;
string var_maxDateYTD = "var _maxDateYTD = "                                               ;
string _maxDate = "_maxDate "                                                              ;
string _minDate = "_minDate "                                                              ;
string _maxDateYTD = "_maxDateYTD "                                                        ;
string col_DimDates = " DIM_Dates[Date] "                                                  ;

string minDimDate = " MIN( " + col_DimDates + " ) "                                        ;
string maxDimDate = " MAX( " + col_DimDates + " ) "                                        ;
string var_minCalendarDate = "var _minCalendarDate = " + minDimDate                        ;
string var_maxCalendarDate = "var _maxCalendarDate = " + maxDimDate                        ;
string var_placeholderDateResult = "var _result = _minCalendarDate <= _maxAvailableDate "  ;
string isAfterToday = "DIM_Dates[IsAfterToday] = TRUE"                                     ;
string isBeforeToday = "DIM_Dates[IsAfterToday] = FALSE"                                   ;
string isCurrentFY = " DIM_Dates[IsCurrentFY] "                                            ;
string isCurrentAY = " DIM_Dates[IsCurrentAY] "                                            ;
string isPreviousFY = " DIM_Dates[IsPreviousFY] "                                            ;
string isPreviousAY = " DIM_Dates[IsPreviousAY] "                                            ;
string isCurrentFYTD = " DIM_Dates[IsCurrentFY] , DIM_Dates[IsAfterToday] = FALSE "        ;
string isCurrentAYTD = " DIM_Dates[IsCurrentAY] , DIM_Dates[IsAfterToday] = FALSE "        ;
string isMaxDateYTD = " CALCULATE ( MAX(  DIM_Dates[Date]  ) , DIM_Dates[IsAfterToday] = FALSE ) "        ;
string isBeforeMaxDate = col_DimDates + " <= " + _maxDate                                  ;
string isBeforeMaxDateYTD = col_DimDates + " <= " + _maxDateYTD                                  ;
string _visibleDates = "[@VisibleDates]"                                                   ;


// Script Variable: Creates a series of time intelligence measures for each selected measure in the Model

foreach(var m in Selected.Measures) 

/***************************************************************************** C# MeasureStart */
{
// declare dax measure and measure object names
    var vSelectedMeasure    = m.Name	                                    ;           //-- selected Measure
    var var_mName           = vSelectedMeasure                              ;           //-- alternative variable for selected measure
    var vTableName          = m.DaxTableName                                ;           //-- table name of selected measure
    var vFullDAXObject      = m.DaxObjectFullName                           ;           //-- full measure name as object "[MeasureName]"
    var vDAXObject          = m.DaxObjectName	                            ;           //-- measure name without brackers "MeasureName"

// var _mMeasurename__     = vSelectedMeasure.Replace(" | BASE","")         ;
// rename selected measures with appropriate affix ( eg: [MeasureName | AFFIX] )
    string vMeasureString    = vSelectedMeasure                             ;           //-- selected measure to rename
    string _sp = " "                                                        ;           //-- space character string
    bool vCheckForBar = vMeasureString.Contains( "|" )                      ;           //-- check is selected measure contains separating bar
// remove string characters after "|"
    int _index                      = vMeasureString.IndexOf("|")           ;           //-- identify position of bar "|"
    if (_index >= 0) vMeasureString  = vMeasureString.Substring(0, _index)  ;           //-- replace text from and beyond position of bar "|"
    
    string vMeasureRenamed = null                                           ;           //-- declare empty variable for renamed measure
    if ( vCheckForBar ) vMeasureRenamed = vMeasureString                    ;           //-- if measure name contains "|" then return renamed string else insert a space
        else vMeasureRenamed = vMeasureString  + " ";


    // final return result variable
    var _result             = _ + vSelectedMeasure                          ;           //-- return measure name
    var _daxResult          = ts_Return + _ + _result                       ;           //-- shorthand for "RETURN + VariableName"

/* SUBSCRIPT START *****************************************************************************/

//- Measure1 Title: CUMULATIVE SUM FISCAL YTD: -----------------------------------------\\
/* Annotation Measure **************************************************************************/
// define affixes for each measure type
string act = "| ACT"                        ;       //-- actual
string ytd = "| YTD"                        ;       //-- year to date
string cml = "| CML"                        ;       //-- cumulatvive
string rem = "| REM"                        ;       //-- remaining (after ytd)
string ytdCml = "| YTD CML"                 ;       //-- cumulative year to date
string remCml = "| REM CML"                 ;       //-- cumulative remaining
string fytd = "| FYTD"                      ;       //-- fiscal year to date
string aytd = "| AYTD"                      ;       //-- academic year to date
string cfytd = "| CFYTD"               ;       //-- cumulative fiscal year to date
string caytd = "| CAYTD"               ;       //-- cumulative fiscal year to date
string fytdCml = "| FYTD CML"               ;       //-- cumulative fiscal year to date
string aytdCml = "| AYTD CML"               ;       //-- cumulative fiscal year to date
string rfytdCml = "| RFYTD CML"               ;       //-- cumulative fiscal year to date
string raytdCml = "| RAYTD CML"               ;       //-- cumulative fiscal year to date
string pytd = "| PYTD"                      ;       //-- previous year to date
string pfytd = "| PFYTD"                    ;       //-- previous fiscal year to date
string paytd = "| PAYTD"                    ;       //-- previous academic year to date
string pfytdCml = "| PFYTD CML"             ;       //-- cumulative previous fiscal year to date
string paytdCml = "| PAYTD CML"             ;       //-- cumulative previous academic year to date

    

    
// check format type for folder designation
    bool _checkCurrency = vSelectedMeasure.StartsWith( _typeCurrency ) ;
    bool _checkCount = vSelectedMeasure.StartsWith( _typeCount ) ;
    bool _checkPercent = vSelectedMeasure.StartsWith( _typePercent ) ;
    
 // test format
    if ( _checkCurrency ) _mFormat = Currency0          ;
        else if ( _checkCount ) _mFormat = Whole        ;   
        else if ( _checkPercent ) _mFormat = Percent    ;
        else _mFormat = Whole   ;

    if ( _checkCurrency ) _folder = _typeCurrency + " SUMs"             ;
        else if ( _checkCount ) _folder = _typeCount + " COUNTs"        ;   
        else if ( _checkPercent ) _folder = _typePercent + " PCTs"      ;
        else _folder = "_BASE"   ;

// assign folder and subfolder
    var newMeasureFolder = "__" + _folder                                  ; //-- display folder
    var newMeasureSubFolder = "\\" + _subfolder                         ;   //-- display subfolder


    




    


/* ***************************** SELECT ******************************     */
/* *************************  MEASURE TYPE ******************************  */
/* ************************ FROM THIS POINT ****************************** */

// all meta data and dax expressions are defined by the selected affix name below
    var _affixName          = aytd	                                ;       //-- ASSIGN DESIRED MEASURE HERE
    var vNew_MeasureName    = vMeasureRenamed + _affixName          ;       //-- new measure name + affix
    string _daxExpression   = null                                  ;       //-- empty dax expression assigned by test below

// assign relevant meta based on above affix selection
    var vAnnotationMetaData = _affixName            ;  //-- meta data attributes will correspond with selected measure affix
    string _mAnnotationStr = null                   ;  //-- declare empty annotation string
// test affixs to assign meta data later
    bool _testFiscal = vAnnotationMetaData.Contains( _FY )                      ;
    bool _testAcademic = vAnnotationMetaData.Contains( _AY )                    ;
    bool _testCalendar = vAnnotationMetaData.Contains( _CY )                    ;
// test affixs to organise measures into relevant folders    
    bool _testACT = vAnnotationMetaData.Contains( "ACT" )
            || vAnnotationMetaData.Contains( "REM" )                            ;
    bool _testYTD = vAnnotationMetaData.Contains( "YTD" )                       ;
    bool _testCML = vAnnotationMetaData.Contains( "YTD" ) 
            || vAnnotationMetaData.Contains( "CML" )                            ;
// run tests and assign folder names
    if ( _testACT ) _subfolder = "ACT"                                          ;
    if ( _testCML || _testYTD ) _subfolder = "CML" ; 
        else _subfolder = "ACT"                                                 ;   
    
    if ( _testYTD ) _mAnnotationStr = "YTD"                                     ; 
        else _mAnnotationStr = _subfolder                                       ;   //-- organise measures into folder


// assign measure meta data and description strings
 // set ytd period
    if ( _testFiscal ) _EndPeriod = _FYEnd              ; 
    else if ( _testAcademic ) _EndPeriod = _AYEnd       ; 
    else if ( _testCalendar ) _EndPeriod = _CYEnd       ; 
        else _EndPeriod = "31/12";

 // set relevant period 
    if ( _testFiscal ) _PeriodDesc = _Fiscal            ; 
    else if ( _testAcademic ) _PeriodDesc = _Academic   ; 
    else if ( _testCalendar ) _PeriodDesc = _Calendar   ; 
        else _PeriodDesc = "Actual"                     ;



/* Meta Description ****************************************************************************/
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
            + _bp + "N/A"
            ;
    var vMeasure_Annotation =
            __ + _annotationTXT 
            + __;

/* Dax Expressions contain in a variable *******************************************************/
    var _act = __ +
             var_ + ts_Result + 
            "= " +m.DaxObjectFullName	            
            + __
            ;

    var _ytd = __ +
             var_ + ts_Result + 
            "= CALCULATE ( " +m.DaxObjectFullName+ ", " + isBeforeToday + " )"	            
            + __
            ;

    var _cml = __ +
             var_maxDate + maxDimDate		
            + __ +  
             var_ + ts_Result + 
            "= CALCULATE ( " +vFullDAXObject+ ", " + isBeforeMaxDate	 + " )"	            
            + __
            ;

    var _ytdCml = __ +
             var_maxDateYTD + isMaxDateYTD		
            + __ +  
             var_ + ts_Result + 
            "= CALCULATE ( " +vFullDAXObject+ ", " + isBeforeMaxDateYTD	 + " )"	            
            + __
            ;

    var _caytd = __ +

             var_ + ts_Result + 
            "= CALCULATE ( " +vFullDAXObject+ ", " + isCurrentAYTD	 + " )"	            
            + __
            ;

    var _cfytd = __ +

             var_ + ts_Result + 
            "= CALCULATE ( " +vFullDAXObject+ ", " + isCurrentFYTD	 + " )"	            
            + __
            ;

    var _rem = __ +
             var_ + ts_Result + 
            "= CALCULATE ( " +m.DaxObjectFullName+ ", " + isAfterToday	+ " )"	            
            + __
            ;
    
    var _remCml = __ +
             var_maxDate + maxDimDate		
            + __ +  
             var_ + ts_Result + 
            "= CALCULATE ( " +vFullDAXObject+ ", " + isBeforeMaxDate	 + " )"	            
            + __
            ;
    
    var _mYTDCML = __ +

            // variable1
             var_maxDate + maxDimDate		
            + __ +  
 
            // result variable 
             var_ + ts_Result + 
            "= IF( " + _visibleDates + ",  TOTALYTD ( " +vFullDAXObject+ ", " + col_DimDates	+ " , " + qt + _EndPeriod + qt + " ) )"	            
            + __ +
             var_ + "_result1 " + 
            "= IF( " + _visibleDates + ",  CALCULATE ( " +vFullDAXObject+ ", " + "DATESYTD( " + col_DimDates	+ " , " + qt + _EndPeriod + qt + ") ) )"
            + __ +
             var_ + "_KPIresult " + 
            "= IF( ISBLANK ( " + ts_Result + " ) , 0 , " + ts_Result + " ) -- return zero for KPI cards"
            + __;

    
    var _fytd = _mYTDCML    ;
    var _aytd = _mYTDCML    ;
    var _fytdCml = _mYTDCML    ;
    var _aytdCml = _mYTDCML    ;
    var _rfytdCml = _mYTDCML    ;
    var _raytdCml = _mYTDCML    ;
    var _pytd = _mYTDCML    ;
    var _pfytd = ""    ;
    var _paytd = ""    ;
    var _pfytdCml = ""    ;
    var _paytdCml = ""    ;
        


// test to check affix and return the corresponding measure
    string vDAX_Expression = null ;

    if (_affixName.Equals(act))             vDAX_Expression = _act          ;
    else if (_affixName.Equals(ytd))        vDAX_Expression = _ytd          ;
    else if (_affixName.Equals(cml))        vDAX_Expression = _cml          ;
    else if (_affixName.Equals(rem))        vDAX_Expression = _rem          ;        
    else if (_affixName.Equals(ytdCml))     vDAX_Expression = _ytdCml       ;
    else if (_affixName.Equals(remCml))     vDAX_Expression = _remCml       ;
    else if (_affixName.Equals(fytd))       vDAX_Expression = _fytd         ;
    else if (_affixName.Equals(aytd))       vDAX_Expression = _aytd         ;
    else if (_affixName.Equals(cfytd))       vDAX_Expression = _cfytd         ;
    else if (_affixName.Equals(caytd))       vDAX_Expression = _caytd         ;
    else if (_affixName.Equals(fytdCml))    vDAX_Expression = _fytdCml      ;
    else if (_affixName.Equals(aytdCml))    vDAX_Expression = _aytdCml      ;
    else if (_affixName.Equals(rfytdCml))    vDAX_Expression = _fytdCml      ;
    else if (_affixName.Equals(raytdCml))    vDAX_Expression = _aytdCml      ;
    else if (_affixName.Equals(pytd))       vDAX_Expression = _pytd         ;
    else if (_affixName.Equals(pfytd))      vDAX_Expression = _pfytd        ;
    else if (_affixName.Equals(paytd))      vDAX_Expression = _paytd        ;
    else if (_affixName.Equals(paytdCml))   vDAX_Expression = _paytdCml     ;
    else if (_affixName.Equals(pfytdCml))   vDAX_Expression = _pfytdCml     ;
    
        else                                vDAX_Expression = _act          ; 

 
/* Meta Description ****************************************************************************/

    var _mDescription =
            _measureDescription + _bp                     //-- meta: measure description               
            + m.DaxObjectFullName                         //-- referenced measure name
            
            // describe filters (included and excluded)
            + __ + _calendarPeriod
            
            // what filter conditions INCLUDE
            + _filtersINCLUDE
 
            // what filter conditions EXCLUDE
            + _filtersEXCLUDE	
            ;

/************************************************************************* DAX expression START */

    var newMeasure          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vNew_MeasureName	                                ,   //-- MeasureName                                         
        vMeasure_Annotation 	                                //-- Annotation 
        + vDAX_Expression	                                    //-- Expression 
        + __ + var_Result	                                    //-- Return + Result   
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        newMeasure.DisplayFolder    = newMeasureFolder                                //-- measure display folder
                                        + newMeasureSubFolder	                 ;    //-- OPTIONAL (subfolder)   
        newMeasure.Description      = _mDescription                              ; 	  //-- measure description   
        newMeasure.FormatString     = _mFormat	                                 ;	  //-- measure format
        newMeasure.IsHidden = false                                              ;    //-- Hide the base column/measure:
        

/* SUBSCRIPT END *********************************************************************************/
}
/********************************************************************************* C# MeasureEnd */
/**** C# SCRIPT END ****/




```
