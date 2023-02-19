# C# Script for Multiple Measures
## produce multiple time intelligence measures for selected measure

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
var vReturnResult = __ + var_Result ;



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
string amt = "| AMT"                        ;       //-- amount
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
    var _affixName          = rfytdCml	                                ;       //-- ASSIGN DESIRED MEASURE HERE
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
    var _referencedDAXMeasure = "[" +vMeasureRenamed + amt+ "]" ;
    var _referencedDAXMeasureREM = "[" +vMeasureRenamed + rem+ "]" ;

   var _amt = __ +
             var_ + ts_Result + 
            "= " + vFullDAXObject	            
            + vReturnResult	                                    
            ;

    var _act = __ +
             var_ + ts_Result + 
            "= CALCULATE ( " +_referencedDAXMeasure+ ", " + "KEEPFILTERS( " + isBeforeToday + " )" + " )"	            
            + vReturnResult	                               
            ;

    var _ytd = __ +
             var_ + ts_Result + 
            "= CALCULATE ( " +_referencedDAXMeasure+ ", " + isBeforeToday + " )"	            
            + vReturnResult	                       
            ;

    var _cml = __ +
             var_maxDate + maxDimDate		
            + __ +  
             var_ + ts_Result + 
            "= CALCULATE ( " +_referencedDAXMeasure+ ", " + isBeforeMaxDate	 + " )"	            
            + vReturnResult                        
            ;

    var _ytdCml = __ +
             var_maxDateYTD + isMaxDateYTD		
            + __ +  
             var_ + ts_Result + 
            "= CALCULATE ( " +_referencedDAXMeasure+ ", " + isBeforeMaxDateYTD	 + " )"	            
            + vReturnResult                                    
            ;

    var _paytd = __ +

             var_ + ts_Result + 
            "= CALCULATE ( " +_referencedDAXMeasure+ ", " + isPreviousAY	 + " )"	            
            + vReturnResult                                    
            ;

    var _pfytd = __ +

             var_ + ts_Result + 
            "= CALCULATE ( " +_referencedDAXMeasure+ ", " + isPreviousFY	 + " )"	            
            + vReturnResult                                    
            ;

    var _caytd = __ +

             var_ + ts_Result + 
            "= CALCULATE ( " +_referencedDAXMeasure+ ", " + isCurrentAYTD	 + " )"	            
            + vReturnResult                                    
            ;

    var _cfytd = __ +

             var_ + ts_Result + 
            "= CALCULATE ( " +_referencedDAXMeasure+ ", " + isCurrentFYTD	 + " )"	            
            + vReturnResult	                                    
            ;

    var _rem = __ +
             var_ + ts_Result + 
            "= CALCULATE ( " +_referencedDAXMeasure+ " , " + isAfterToday	+ " )"	            
            + vReturnResult                                    
            ;
    
    var _remCml = __ +
             var_maxDate + maxDimDate		
            + __ +  
             var_ + ts_Result + 
            "= CALCULATE ( " +_referencedDAXMeasureREM+ ", " + isBeforeMaxDate	 + " )"	            
            + vReturnResult	                                   
            ;

    var _mFYTD = __ +

            // variable1
             var_maxDate + maxDimDate		
            + __ +  
 
            // result variable 
             var_ + ts_Result + 
            "= IF( " + _visibleDates + ",  TOTALYTD ( " +_referencedDAXMeasure+ ", " + col_DimDates	+ " , " + qt + _FYEnd + qt + " ) )"	            
            + __ +
             var_ + "_result1 " + 
            "= IF( " + _visibleDates + ",  CALCULATE ( " +_referencedDAXMeasure+ ", " + "KEEPFILTERS( DATESYTD( " + col_DimDates	+ " , " + qt + _FYEnd + qt + " ) ) ) )"
            + __ +
             var_ + "_KPIresult " + 
            "= IF( ISBLANK ( " + ts_Result + " ) , 0 , " + ts_Result + " ) -- return zero for KPI cards"
            + vReturnResult	                                    
            + __;
    
    var _mAYTD = __ +

            // variable1
             var_maxDate + maxDimDate		
            + __ +  
 
            // result variable 
             var_ + ts_Result + 
            "= IF( " + _visibleDates + ",  TOTALYTD ( " +_referencedDAXMeasure+ ", " + col_DimDates	+ " , " + qt + _AYEnd + qt + " ) )"	            
            + __ +
             var_ + "_result1 " + 
            "= IF( " + _visibleDates + ",  CALCULATE ( " +_referencedDAXMeasure+ ", " + "KEEPFILTERS( DATESYTD( " + col_DimDates	+ " , " + qt + _AYEnd + qt + " ) ) ) )"
            + __ +
             var_ + "_KPIresult " + 
            "= IF( ISBLANK ( " + ts_Result + " ) , 0 , " + ts_Result + " ) -- return zero for KPI cards"
            + vReturnResult	                                     
            + __;

    var _mFYTDCML = __ +

            // variable1
             var_maxDate + maxDimDate		
            + __ +  
 
            // result variable 
             var_ + ts_Result + 
            "= IF( " + _visibleDates + ",  TOTALYTD ( " +_referencedDAXMeasure+ ", " + col_DimDates	+ " , " + qt + _FYEnd + qt + " ) )"	            
            + __ +
             var_ + "_result1 " + 
            "= IF( " + _visibleDates + ",  CALCULATE ( " +_referencedDAXMeasure+ ", " + "DATESYTD( " + col_DimDates	+ " , " + qt + _FYEnd + qt + ") ) )"
            + __ +
             var_ + "_KPIresult " + 
            "= IF( ISBLANK ( " + ts_Result + " ) , 0 , " + ts_Result + " ) -- return zero for KPI cards"
            + vReturnResult	                                    
            + __;
    
    var _mAYTDCML = __ +

            // variable1
             var_maxDate + maxDimDate		
            + __ +  
 
            // result variable 
             var_ + ts_Result + 
            "= IF( " + _visibleDates + ",  TOTALYTD ( " +_referencedDAXMeasure+ ", " + col_DimDates	+ " , " + qt + _AYEnd + qt + " ) )"	            
            + __ +
             var_ + "_result1 " + 
            "= IF( " + _visibleDates + ",  CALCULATE ( " +_referencedDAXMeasure+ ", " + "DATESYTD( " + col_DimDates	+ " , " + qt + _AYEnd + qt + ") ) )"
            + __ +
             var_ + "_KPIresult " + 
            "= IF( ISBLANK ( " + ts_Result + " ) , 0 , " + ts_Result + " ) -- return zero for KPI cards"
            + vReturnResult	                                    
            + __;
    
    var _fytd = _mFYTD    ;
    var _aytd = _mAYTD    ;
    var _fytdCml = _mFYTDCML    ;
    var _aytdCml = _mAYTDCML    ;
    var _rfytdCml = _mFYTDCML    ;
    var _raytdCml = _mAYTDCML    ;
    var _pytd = ""    ;
//    var _pfytd = ""    ;
//    var _paytd = ""    ;
    var _pfytdCml = ""    ;
    var _paytdCml = ""    ;
        


// test to check affix and return the corresponding measure
    string vDAX_Expression = null ;

    if (amt.Equals(amt))             vDAX_Expression = _amt          ;
    else if (act.Equals(act))             vDAX_Expression = _act          ;
    else if (ytd.Equals(ytd))        vDAX_Expression = _ytd          ;
    else if (cml.Equals(cml))        vDAX_Expression = _cml          ;
    else if (rem.Equals(rem))        vDAX_Expression = _rem          ; 
    else if (remCml.Equals(remCml))        vDAX_Expression = _remCml          ; 
    else if (ytdCml.Equals(ytdCml))     vDAX_Expression = _ytdCml       ;
    else if (remCml.Equals(remCml))     vDAX_Expression = _remCml       ;
    else if (fytd.Equals(fytd))       vDAX_Expression = _fytd         ;
    else if (aytd.Equals(aytd))       vDAX_Expression = _aytd         ;
    else if (cfytd.Equals(cfytd))       vDAX_Expression = _cfytd         ;
    else if (caytd.Equals(caytd))       vDAX_Expression = _caytd         ;
    else if (fytdCml.Equals(fytdCml))    vDAX_Expression = _fytdCml      ;
    else if (aytdCml.Equals(aytdCml))    vDAX_Expression = _aytdCml      ;
    else if (rfytdCml.Equals(rfytdCml))    vDAX_Expression = _fytdCml      ;
    else if (raytdCml.Equals(raytdCml))    vDAX_Expression = _aytdCml      ;
    else if (pytd.Equals(pytd))       vDAX_Expression = _pytd         ;
    else if (pfytd.Equals(pfytd))      vDAX_Expression = _pfytd        ;
    else if (paytd.Equals(paytd))      vDAX_Expression = _paytd        ;
    else if (paytdCml.Equals(paytdCml))   vDAX_Expression = _paytdCml     ;
    else if (pfytdCml.Equals(pfytdCml))   vDAX_Expression = _pfytdCml     ;
    
        else                                vDAX_Expression = _amt          ; 

 
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
var m1_desc = "// Total Amount (AMT)" ;
    var m1          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + amt	                                ,   //-- MeasureName                                         
        m1_desc 	                                //-- Annotation 
        + _amt	                                    //-- Expression 
  
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m1.DisplayFolder    = newMeasureFolder   ;                             //-- measure display folder
                                       // + newMeasureSubFolder	                 ;    //-- OPTIONAL (subfolder)   
        m1.Description      = m1_desc                              ; 	  //-- measure description   
        m1.FormatString     = _mFormat	                                 ;	  //-- measure format
        m1.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m1a_desc = "// Actual (ACT)"  ;
var m1a          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + act ,                     //-- MeasureName                                         
        m1a_desc	                            //-- Annotation 
        + _act	                                    //-- Expression 
  
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m1a.DisplayFolder    = newMeasureFolder   ;                             //-- measure display folder
                                       // + newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m1a.Description      = m1a_desc                              ; 	  //-- measure description   
        m1a.FormatString     = _mFormat	                                 ;	  //-- measure format
        m1a.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/
        
/************************************************************************* DAX expression START */
var m2_desc = "// Remaining (REM)" ;
var m2          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + rem	                                ,   //-- MeasureName                                         
        m2_desc 	                                //-- Annotation 
        + _rem	                                    //-- Expression 
  
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "
    )
    ;

/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m2.DisplayFolder    = newMeasureFolder    ;                            //-- measure display folder
                                       // + newMeasureSubFolder	                 ;    //-- OPTIONAL (subfolder)   
        m2.Description      = m2_desc                              ; 	  //-- measure description   
        m2.FormatString     = _mFormat	                                 ;	  //-- measure format
        m2.IsHidden = false                                              ;    //-- Hide the base column/measure:


/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m3_desc =  "// Actual Year To Date (YTD)"   ;
var m3          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + ytd ,                     //-- MeasureName                                         
        m3_desc	                            //-- Annotation 
        + _ytd	                                    //-- Expression 
  
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m3.DisplayFolder    = newMeasureFolder     ;                           //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m3.Description      = m3_desc                              ; 	  //-- measure description   
        m3.FormatString     = _mFormat	                                 ;	  //-- measure format
        m3.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/

/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m4_desc = "// Actual Year to Date Cumulative (YTD CML)" ;
var m4          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + ytdCml ,                     //-- MeasureName                                         
        m4_desc 	                            //-- Annotation 
        + _ytdCml	                                    //-- Expression 
   
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m4.DisplayFolder    = newMeasureFolder      ;                          //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m4.Description      = m4_desc                              ; 	  //-- measure description   
        m4.FormatString     = _mFormat	                                 ;	  //-- measure format
        m4.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/

/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m5_desc = "// Remaing Year to Date Cumulative (REM CML)"  ;
var m5          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + remCml ,                     //-- MeasureName                                         
        m5_desc	                            //-- Annotation 
        + _remCml	                                    //-- Expression 
  
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m5.DisplayFolder    = newMeasureFolder   ;                             //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m5.Description      = m5_desc                              ; 	  //-- measure description   
        m5.FormatString     = _mFormat	                                 ;	  //-- measure format
        m5.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/


/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m6_desc = "// Fiscal Year to Date"  ;
var m6          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + fytd ,                     //-- MeasureName                                         
        	                            //-- Annotation 
         _fytd	                                    //-- Expression 
   
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m6.DisplayFolder    = newMeasureFolder        ;                        //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m6.Description      = m6_desc                              ; 	  //-- measure description   
        m6.FormatString     = _mFormat	                                 ;	  //-- measure format
        m6.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/


/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m7_desc = "// Fiscal Year to Date Cumulative (FYTD CML)" ;
var m7          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + fytdCml ,                     //-- MeasureName                                         
        m7_desc 	                            //-- Annotation 
        + _fytdCml	                                    //-- Expression 
   
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m7.DisplayFolder    = newMeasureFolder         ;                       //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m7.Description      = m7_desc                              ; 	  //-- measure description   
        m7.FormatString     = _mFormat	                                 ;	  //-- measure format
        m7.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/


/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m8_desc = "// Academic Year to Date (AYTD)"  ;
var m8          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + aytd ,                     //-- MeasureName                                         
        m8_desc	                            //-- Annotation 
        + _aytd	                                    //-- Expression 
  
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m8.DisplayFolder    = newMeasureFolder         ;                       //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m8.Description      = m8_desc                              ; 	  //-- measure description   
        m8.FormatString     = _mFormat	                                 ;	  //-- measure format
        m8.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/


/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m9_desc =  "// Fiscal Year to Date Cumulative (AYTD CML)"  ;
var m9          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + aytdCml ,                     //-- MeasureName                                         
       	m9_desc                            //-- Annotation 
        + _aytdCml	                                    //-- Expression 
  
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m9.DisplayFolder    = newMeasureFolder         ;                       //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m9.Description      = m9_desc                              ; 	  //-- measure description   
        m9.FormatString     = _mFormat	                                 ;	  //-- measure format
        m9.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/


/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m10_desc = "// Current Academic Year (CAYTD)" ;
var m10          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + caytd ,                     //-- MeasureName                                         
        m10_desc 	                            //-- Annotation 
        + _caytd	                                    //-- Expression 
  
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m10.DisplayFolder    = newMeasureFolder          ;                      //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m10.Description      = m10_desc                              ; 	  //-- measure description   
        m10.FormatString     = _mFormat	                                 ;	  //-- measure format
        m10.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/


/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m11_desc = "// Current Fiscal Year (CFYTD)" ;
var m11          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + cfytd ,                     //-- MeasureName                                         
        m11_desc	                            //-- Annotation 
        + _cfytd	                                    //-- Expression 
   
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m11.DisplayFolder    = newMeasureFolder           ;                     //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m11.Description      = m11_desc                              ; 	  //-- measure description   
        m11.FormatString     = _mFormat	                                 ;	  //-- measure format
        m11.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/


/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m12_desc = "// Remaining Academic Year To Date Cumulative" ;
var m12          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + raytdCml ,                     //-- MeasureName                                         
        m12_desc 	                            //-- Annotation 
        + _raytdCml	                                    //-- Expression 
   
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m12.DisplayFolder    = newMeasureFolder           ;                     //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m12.Description      = m12_desc                              ; 	  //-- measure description   
        m12.FormatString     = _mFormat	                                 ;	  //-- measure format
        m12.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/


/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m13_desc = "// Remaining Fiscal Year to Date Cumulative"  ;
var m13          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + rfytdCml ,                     //-- MeasureName                                         
        m13_desc	                            //-- Annotation 
        + _rfytdCml	                                    //-- Expression 
  
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m13.DisplayFolder    = newMeasureFolder             ;                   //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m13.Description      = m13_desc                              ; 	  //-- measure description   
        m13.FormatString     = _mFormat	                                 ;	  //-- measure format
        m13.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/


/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m14_desc = "// Previous Year to Date" ;
var m14          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + pytd ,                     //-- MeasureName                                         
        m14_desc                            //-- Annotation 
        + _pytd	                                    //-- Expression 
   
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m14.DisplayFolder    = newMeasureFolder             ;                   //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m14.Description      = m14_desc                              ; 	  //-- measure description   
        m14.FormatString     = _mFormat	                                 ;	  //-- measure format
        m14.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/

/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m15_desc = "// Previous Academic Year to Date" ;
var m15          = m.Table.AddMeasure                //-- create measure function
    (                                 
        vMeasureRenamed + paytd ,                     //-- MeasureName                                         
        m15_desc 	      //-- Annotation 
        + _paytd	                                  //-- Expression 
   
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m15.DisplayFolder    = newMeasureFolder             ;                   //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m15.Description      = m15_desc                              ; 	  //-- measure description   
        m15.FormatString     = _mFormat	                                 ;	  //-- measure format
        m15.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/

/* SUBSCRIPT START *********************************************************************************/
/************************************************************************* DAX expression START */
var m16_desc = "// Previous Fiscal Year to Date" ;
var m16          = m.Table.AddMeasure                //-- create measure function
    (                                 
        
        vMeasureRenamed + pfytd ,                     //-- MeasureName                                         
        m16_desc 	          //-- Annotation 
        + _pfytd	                                  //-- Expression 
   
        
/* add optional code below *****************/  
        + __ +  "// bonus description / annotation / code "     // bonus annotation or notes
    )
    ;
/*************************************************************************** DAX expression END */
        
/* Metadata *************************************************************************************/
        m16.DisplayFolder    = newMeasureFolder             ;                   //-- measure display folder
                                        //+ newMeasureSubFolder	         ;    //-- OPTIONAL (subfolder)   
        m16.Description      = m16_desc                             ; 	  //-- measure description   
        m16.FormatString     = _mFormat	                                 ;	  //-- measure format
        m16.IsHidden = false                                              ;    //-- Hide the base column/measure:

/* SUBSCRIPT END *********************************************************************************/


}
/********************************************************************************* C# MeasureEnd */
/**** C# SCRIPT END ****/



```
