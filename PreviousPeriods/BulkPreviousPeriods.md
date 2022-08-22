```C#


  
/*---------------------------------------------------
| TITLE:                                             |
| Time Intelligence Series                           |
| Tabular Editor Advanced Script                     |
 ----------------------------------------------------
| AUTHOR:                                            |
| Imran Haq, PBI Queryous                            |
| https://github.com/PBIQueryous                     |
| STAY QUERYOUS PBI CHAMPS!                          |
 ---------------------------------------------------*/
 
 /* DESCRIPTION
 * -----------------------------------
 * This script, when executed, will loop through the currently selected measure(s),
 * creating a series of measure(s) declared in the script below.
 * 
 * e.g., from Model select [Measure] where Measure = SUM( tbl[column] ) or COUNT( tbl[column] )
 * -----------------------------------
 */

 // C# measure formula template:
 // m.Table.AddMeasure( "MeasureName", "Expression", m.DisplayFolder);

/**** C# SCRIPT START ****/

// SET VARIABLES
// Quotation Character - helpful for wrapping " " around a text string within the DAX code
const string qt = "\"";

// Number Formatting Strings
var GBP0 = qt + "£" + qt + "#,0";
var GBP2 = qt + "£" + qt + "#,0.00";
var Whole = "#,0";
var Percent = "0.0 %";
var Decimal = "#,0.0";
var Number = "#,0";
var Currency0 = GBP0+";" +"-"+GBP0+";" +GBP0;
var Currency2 = GBP2+";" +"-"+GBP2+";" +GBP2;
var Deviation = "+"+Decimal+";" +"-"+Decimal+";"+ Decimal;
var Variance = "▲ #,0; ▼ #,0; ∓ #,0";
var Change = "↗ 0.0%;↘ 0.0%; 0.0%"; 

// Var RETURN text strings
var vResult = "var _result = ";
var rReturnResult = "RETURN" + '\n' + '\t' + "_result";
var rReturn = "RETURN" + '\n';
var ifnotBlank = '\t' + "// IF(  NOT ISBLANK( ";
var thenResult = " ) ,  _result  )";
var rResult = '\t' + "_result";

// MeasureName Variables
var snap = " | SNAP";
var cml = " | CML";
var cytd = " | CYTD";
var cfytd = " | CFYTD";
var cytdCml = " | CYTD CML";
var cfytdCml = " | CFYTD CML";
var rem = " | REM";



// TimeIntel Variable Filters
var datesDate = "_Dates[Date]";
var datesMTD = "_Dates[LatestMTD]";
var isCFY = "_Dates[IsCFY] = TRUE";
var isCYTD = "_Dates[IsCYTD] = TRUE";
var maxDate = "_maxDate";
var curDate = "_curDate";
var mtdDate = "_ytd";
var vardatesDate = "var " +maxDate+ " = MAX( " + datesDate + " )";
var varlatestMTD = "var "+mtdDate+ " = CALCULATE( MAX( " +datesMTD+ " ), REMOVEFILTERS())";
var varmaxdatesCFY = "var " +maxDate+ " = CALCULATE( MAX( " +datesDate+ "), " + isCFY + " )";
var fiscalyear = qt+"31/3"+qt;
var datesFiscal = "DATESYTD (" + datesDate + "," + fiscalyear + " )";
// Var Measure Folder
var subFolder = "_zSpend\\_Calcs";

// Script Variable: Creates a series of time intelligence measures for each selected measure in the Model
foreach(var m in Selected.Measures) 
{
 

 
/***************************************** MeasureStart ************************************/
    // Previous Year ##:
    m.Table.AddMeasure(
    m.Name + " | SPPY #", // Name
        "var _calc = CALCULATE(" + m.DaxObjectName + ", SAMEPERIODLASTYEAR(" + datesDate + "))" + // DAX expression
        '\n' + "RETURN" +
        '\n' + '\t' + "IF( NOT ISBLANK( " + m.DaxObjectName + "), _calc )",
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string
        

/***************************************** MeasureStart ************************************/
    // Previous Year Change %%:
    m.Table.AddMeasure(
    m.Name + " | PY %", // Name
        "var _current = " + m.DaxObjectName + '\n' +
        "var _previous = IF ( NOT ISBLANK ( _current ) , [" + m.Name + " | SPPY #] )" + '\n' +
        "var _diff = ( _current - _previous )" + '\n' +
        "var _calc = DIVIDE( _diff, _previous , BLANK() )" + '\n' +
        "RETURN" + '\n' +
        '\t' + "IF( NOT ISBLANK( _current ), _calc )", // DAX expression
        m.DisplayFolder // Display Folder
        ).FormatString = Change; // Set format string


/***************************************** MeasureStart ************************************/
    // Previous Quarter ##:
    m.Table.AddMeasure(
    m.Name + " | SPPQ #", // Name
    "var _calc = CALCULATE(" + m.DaxObjectName + ", PREVIOUSQUARTER(" + datesDate + "))" + // DAX expression
        '\n' + "RETURN" +
        '\n' + '\t' + "IF( NOT ISBLANK( " + m.DaxObjectName + "), _calc )",
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string

/***************************************** MeasureStart ************************************/
    // Previous Month ##:
    m.Table.AddMeasure(
    m.Name + " | SPPM #", // Name
    "var _calc = CALCULATE(" + m.DaxObjectName + ", PREVIOUSMONTH(" + datesDate + "))" + // DAX expression
        '\n' + "RETURN" +
        '\n' + '\t' + "IF( NOT ISBLANK( " + m.DaxObjectName + "), _calc )",
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string        
        
/***************************************** MeasureStart ************************************/
    // Previous Year Change %%:
    m.Table.AddMeasure(
    m.Name + " | SPPQ %", // Name
        "var _current = " + m.DaxObjectName + '\n' +
        "var _previous = IF ( NOT ISBLANK ( _current ) , [" + m.Name + " | SPPQ #] )" + '\n' +
        "var _diff = ( _current - _previous )" + '\n' +
        "var _calc = DIVIDE( _diff, _previous , BLANK() )" + '\n' +
        "RETURN" + '\n' +
        '\t' + "IF( NOT ISBLANK( _current ), _calc )", // DAX expression
        m.DisplayFolder // Display Folder
        ).FormatString = Change; // Set format string
  

        
/***************************************** MeasureStart ************************************/
    // Previous Month Change ##:
    m.Table.AddMeasure(
    m.Name + " | Change PM #", // Name
        '\n' + "var _current = [" + m.Name + " | CM]" + 
        '\n' + "var _previous = [" + m.Name + " | PM]" +
        '\n' + "var _result = _current - _previous" + 
        '\n' + rReturnResult, // DAX expression
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string        


/***************************************** MeasureStart ************************************/
    // Previous PM Change %%:
    m.Table.AddMeasure(
    m.Name + " | Diff PM %", // Name
    '\n' + "var _current = [" + m.Name + " | CM]" + 
    '\n' + "var _previous = IF ( NOT ISBLANK ( _current ) , [" + m.Name + " | PM] )" +
    '\n' + "var _diff = ( _current - _previous )" + '\n' +
    '\n' + "var _calc = DIVIDE( _diff, _previous , BLANK() )" +
    '\n' + "RETURN" +
    '\n' + '\t' + "IF( NOT ISBLANK( _current ), _calc )", // DAX expression
        m.DisplayFolder // Display Folder
        ).FormatString = Change; // Set format string

/***************************************** MeasureStart ************************************/
    // Previous Month Change ##:
    m.Table.AddMeasure(
    m.Name + " | Change PQ #", // Name
    '\n' + "var _current = [" + m.Name + " | CQ]" + 
    '\n' + "var _previous = [" + m.Name + " | PQ]" +
        '\n' + "var _result = _current - _previous" + 
        '\n' + rReturnResult, // DAX expression
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string        


/***************************************** MeasureStart ************************************/
    // Previous PM Change %%:
    m.Table.AddMeasure(
    m.Name + " | Diff PQ %", // Name
    '\n' + "var _current = [" + m.Name + " | CQ]" + 
    '\n' + "var _previous = IF ( NOT ISBLANK ( _current ) , [" + m.Name + " | PQ] )" +
    '\n' + "var _diff = ( _current - _previous )" + '\n' +
    '\n' + "var _calc = DIVIDE( _diff, _previous , BLANK() )" +
    '\n' + "RETURN" +
    '\n' + '\t' + "IF( NOT ISBLANK( _current ), _calc )", // DAX expression
        m.DisplayFolder // Display Folder
        ).FormatString = Change; // Set format string

/***************************************** MeasureStart ************************************/
    // Previous Month Change ##:
    m.Table.AddMeasure(
    m.Name + " | Change PY #", // Name
    '\n' + "var _current = [" + m.Name + " | CY]" + 
    '\n' + "var _previous = [" + m.Name + " | PY]" +
        '\n' + "var _result = _current - _previous" + 
        '\n' + rReturnResult, // DAX expression
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string        


/***************************************** MeasureStart ************************************/
    // Previous PM Change %%:
    m.Table.AddMeasure(
    m.Name + " | Diff PY %", // Name
    '\n' + "var _current = [" + m.Name + " | CY]" + 
    '\n' + "var _previous = IF ( NOT ISBLANK ( _current ) , [" + m.Name + " | PY] )" +
    '\n' + "var _diff = ( _current - _previous )" + '\n' +
    '\n' + "var _calc = DIVIDE( _diff, _previous , BLANK() )" +
    '\n' + "RETURN" +
    '\n' + '\t' + "IF( NOT ISBLANK( _current ), _calc )", // DAX expression
        m.DisplayFolder // Display Folder
        ).FormatString = Change; // Set format string

        
/***************************************** MeasureStart ************************************/
    // Previous Quarter ##:
    m.Table.AddMeasure(
    m.Name + " | CQ", // Name
    "CALCULATE(" + m.DaxObjectName + ", _Dates[QuarterOffset] = 0 )",
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string
  
  
/***************************************** MeasureStart ************************************/
    // Previous Quarter ##:
    m.Table.AddMeasure(
    m.Name + " | PQ", // Name
    "CALCULATE(" + m.DaxObjectName + ", _Dates[QuarterOffset] = -1 )",
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string  

/***************************************** MeasureStart ************************************/
    // Previous Quarter Variance ##:
    m.Table.AddMeasure(
    m.Name + " | PQ Variance #", // Name
    '\n' + "var _curr = " + m.DaxObjectName +
    '\n' + "var _prev = [" + m.Name + " | SPPQ #]" +
    '\n' + "var _calc = _curr - _prev" +
    '\n' + "var _result = IF( NOT ISBLANK ( _prev ) , _calc )" +
    rReturnResult,
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string    

      
/***************************************** MeasureStart ************************************/
    // Previous Year Variance ##:
    m.Table.AddMeasure(
    m.Name + " | PY Variance #", // Name
    '\n' + "var _curr = " + m.DaxObjectName +
    '\n' + "var _prev = [" + m.Name + " | SPPY #]" +
    '\n' + "var _calc = _curr - _prev" +
    '\n' + "var _result = IF( NOT ISBLANK ( _prev ) , _calc )" +
    rReturnResult,
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string          
        
/***************************************** MeasureStart ************************************/
    // Previous Month ##:
    m.Table.AddMeasure(
    m.Name + " | CM", // Name
    "CALCULATE(" + m.DaxObjectName + ", _Dates[MonthOffset] = 0 )",
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string  
  
  
/***************************************** MeasureStart ************************************/
    // Previous Month ##:
    m.Table.AddMeasure(
    m.Name + " | PM", // Name
    "CALCULATE(" + m.DaxObjectName + ", _Dates[MonthOffset] = -1 )",
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string    

/***************************************** MeasureStart ************************************/
    // Previous Month Variance ##:
    m.Table.AddMeasure(
    m.Name + " | PM Variance #", // Name
    '\n' + "var _curr = " + m.DaxObjectName +
    '\n' + "var _prev = [" + m.Name + " | SPPM #]" +
    '\n' + "var _calc = _curr - _prev" +
    '\n' + "var _result = IF( NOT ISBLANK ( _prev ) , _calc )" +
    rReturnResult,
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string          
                
        
/***************************************** MeasureStart ************************************/
    // Previous Year ##:
    m.Table.AddMeasure(
    m.Name + " | CY", // Name
    "CALCULATE(" + m.DaxObjectName + ", _Dates[YearOffset] = 0 )",
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string  
  
  
/***************************************** MeasureStart ************************************/
    // Previous Year ##:
    m.Table.AddMeasure(
    m.Name + " | PY", // Name
    "CALCULATE(" + m.DaxObjectName + ", _Dates[YearOffset] = -1 )",
        m.DisplayFolder // Display Folder
        ).FormatString = Number; // Set format string    
        

        
}
/**** C# SCRIPT END ****/




```
