# Time Intelligence Series

```c#
/* SCRIPT NOTES
 * Title: 
 * Time-intelligence Measure Series
 * 
 * Inspiration and Credits:
 * ------------------------
 * PowerBI.Tips Team,
 * Daniel Otykier, twitter.com/DOtykier,
 * Tabular Editor 2/3 Documentation,
 * ------------------------
 * 
 * Author:
 * Imran Haq, PBI Queryous
 *
 * CustomActions Filepath:
 * C:\Users\UserName\AppData\Local\TabularEditor
 * 
 * Description:
 * This script, when executed, will loop through the currently selected columns,
 * creating a series of measure for each column and also hiding the column itself.
 *
 * C# measure template:
 * m.Table.AddMeasure( "MeasureName", "Expression", m.DisplayFolder);
 */

/* SET VARIABLES */

// Quotation Character - helpful for wrapping " " around a text string within the DAX code
const string qt = "\"";

// Number Formatting Strings
var GBP = qt + "£" + qt;
var Whole = "#,0";
var Percent = "0.0 %";
var Decimal = "#,0.0";
var Number = "#,0";
var Currency = GBP + "#,0; -" + GBP + "#,0;" + GBP + "#,0";

// Standard Date Variables - includes Date Columns and Text Strings
var dateColumn = "Dates[Date]";
var mtdColumn = "Dates[LatestMTD]";
var endDate = "31/3";
var endFY = qt + "31/3" + qt; 
var datesYTD = ", DATESYTD( " + dateColumn + ", " + endFY + " ) ";

// Fiscal Filter Date Variables
var calcVarMinMTDFY = "VAR _min = CALCULATE( MIN( Dates[LatestMTD] ) , Dates[IsCFY] = TRUE)";
var calcVarMaxMTDFY = "VAR _max = CALCULATE( MAX( Dates[LatestMTD] ) , Dates[IsCFY] = TRUE)";
var calcVarMaxDateFY = "VAR _max = CALCULATE( MAX( Dates[Date] ) , Dates[IsCFY] = TRUE)";
var calcVarMinMaxFY = calcVarMinMTDFY + '\n' + calcVarMaxMTDFY; 
var beforeVarMax = dateColumn + " <= _max ";
var beforeVarYtd = dateColumn + " <= _ytd ";
var betweenVarFY = dateColumn + " >= _min " + "&& " + dateColumn + " <= _max ";

// Filtered Date Variables
var calcMaxMTD = "CALCULATE( MAX( Dates[LatestMTD] ) , REMOVEFILTERS ()) ";
var calcVarMaxMTD = "VAR _ytd = CALCULATE( MAX( Dates[LatestMTD] ) /* , REMOVEFILTERS () */ ) ";
var calcVarMaxMTDremoveFilter = "VAR _YtD = CALCULATE( MAX( Dates[LatestMTD] ) , REMOVEFILTERS ()) ";
var calcVarMaxDate = "VAR _max = CALCULATE( MAX( Dates[Date] ) /* , Dates[IsCFY] = TRUE */ )";

var calcVarMaxYTD1 = "VAR _ytd1 = CALCULATE( MAX( Dates[MTDAdd1] ) , Dates[IsCFY] = TRUE)";
var calcVarMaxYTD2 = "VAR _ytd2 = CALCULATE( MAX( Dates[MTDAdd2] ) , Dates[IsCFY] = TRUE)";
var calcVarMaxYTD3 = "VAR _ytd3 = CALCULATE( MAX( Dates[MTDAdd3] ) , Dates[IsCFY] = TRUE)";

// Text Fillers for Measure Templates
var mMeasure = qt + "[MeasureName]" + qt;
var mActual = qt + "[Actual]" + qt;
var mPlan = qt + "[Plan]" + qt;

// MeasureName Variables
var snap = " | SNAP";
var ytdSnap = " | YTD SNAP";
var efy = " | EFY SNAP";
var efyCML = " | EFY CML";
var cml = " | CML";
var ytd = " | YTD CML";
var fytd = " | FYTD";
var Subfolder = "TimeInt";

// Var RETURN text strings
var rReturnRes = "RETURN" + '\n' + "_result";
var rReturn = "RETURN" + '\n';
var rResult = "_result";

// Script Variable
// Creates a series of time intelligence measures for each selected (base SUM) measure:
foreach(var m in Selected.Measures) 

    {   // SCRIPT START
    
/***************************************** MeasureStart ************************************/
// Measure1: SUM
    var m1 = m.Table.AddMeasure
    (                             
        // MeasureStart //
        // MeasureName
        m.Name + " | SUM",                               
    
        // Measure Descriptive Text
        '\n' + "// Base SUM "                           
        
        /***** DAX expression START *****/
        // DAX Start
        // DAX Variables
        + '\n' + '\n' + calcVarMaxDateFY                
        + '\n' + "VAR _result = CALCULATE( " + m.DaxObjectName + " ) " + '\n'
        
        // Return Result
        + '\n' + rReturn
        + '\n' + '\t' + "// IF(  NOT ISBLANK( " + m.DaxObjectName + " ) ,  _result  )"
        + '\n' + '\t' + rResult
    );
        /***** DAX expression END *****/
        
        // Display Folder (default - same folder as selected)
        m1.DisplayFolder 
        // Optional: new Folder name below
        = Subfolder
        ;      
    
        /***** Provide some documentation *****/
        m1.Description = "Derived from " + m.Name + ": " + 
        // Type metadata text here
        "Base Measure - End for Year, no filters."
        ;                             
        m1.FormatString = Currency
        ;                                                
/**************************************** MeasureEnd **************************************/
    
    


/***************************************** MeasureStart ************************************/
// Measure2: CY CML
    var m2 = m.Table.AddMeasure
    (                             
        
        // MeasureStart //
        // MeasureName
        m.Name + cml,                               
    
        // Measure Descriptive Text
        '\n' + "// Base CML "                           
        
        /***** DAX expression START *****/
        // DAX Start
        // DAX Variables
        + '\n' + '\n' + calcVarMaxDate
        + '\n' + "VAR _result = CALCULATE( [" + m.Name + " | CFY], " + beforeVarMax + " ) " + '\n'
        
        // Return Result
        + '\n' + rReturn
        + '\n' + '\t' + "// IF(  NOT ISBLANK( " + m.DaxObjectName + " ) ,  _result  )"
        + '\n' + '\t' + rResult
    );
        /***** DAX expression END *****/
        
        // Display Folder (default - same folder as selected)
        m2.DisplayFolder 
        // Optional: new Folder name below
        = Subfolder 
        ;      
    
        /***** Provide some documentation *****/
        m2.Description = "Derived from " + m.Name + ": " + 
        // Type metadata text here
        "Base Measure - End for Year, no filters."
        ;                             
        m2.FormatString = Currency
        ;                                                
/**************************************** MeasureEnd **************************************/
 


/***************************************** MeasureStart ************************************/
// Measure3: CFY
    var m3 = m.Table.AddMeasure
    (                             
        
        // MeasureStart //
        // MeasureName
        m.Name + " | CFY",                               
    
        // Measure Descriptive Text
        '\n' + "// Base SUM "                           
        
        /***** DAX expression START *****/
        // DAX Start
        // DAX Variables
        + '\n' + '\n' + calcVarMinMaxFY                  //  DAX Start
        + '\n' + "VAR _result = CALCULATE( " + m.DaxObjectName + ", " + betweenVarFY + " ) " + '\n'
        
        // Return Result
        + '\n' + rReturn
        + '\n' + '\t' + "// IF(  NOT ISBLANK( " + m.DaxObjectName + " ) ,  _result  )"
        + '\n' + '\t' + rResult
    );
        /***** DAX expression END *****/
        
        // Display Folder (default - same folder as selected)
        m3.DisplayFolder 
        // Optional: new Folder name below
        = Subfolder 
        ;      
    
        /***** Provide some documentation *****/
        m3.Description = "Derived from " + m.Name + ": " + 
        // Type metadata text here
        "Base Measure - End for Year, no filters."
        ;                             
        m3.FormatString = Currency
        ;
/**************************************** MeasureEnd **************************************/

        
/***************************************** MeasureStart ************************************/
// Measure3: YTD
    var m4 = m.Table.AddMeasure
    (                             
        
        // MeasureStart //
        // MeasureName
        m.Name + ytd,                               
    
        // Measure Descriptive Text
        '\n' + "// Base SUM "                           
        
        /***** DAX expression START *****/
        // DAX Start
        // DAX Variables
        + '\n' + '\n' + calcVarMaxDateFY                
        + '\n' + "VAR _result = CALCULATE( " + m.DaxObjectName + datesYTD + " ) " + '\n'
        
        // Return Result
        + '\n' + rReturn
        + '\n' + '\t' + "// IF(  NOT ISBLANK( " + m.DaxObjectName + " ) ,  _result  )"
        + '\n' + '\t' + rResult
    );
        /***** DAX expression END *****/
        
        // Display Folder (default - same folder as selected)
        m4.DisplayFolder 
        // Optional: new Folder name below
        = Subfolder 
        ;      
    
        /***** Provide some documentation *****/
        m4.Description = "Derived from " + m.Name + ": " + 
        // Type metadata text here
        "Base Measure - End for Year, no filters."
        ;                             
        m4.FormatString = Currency
        ;                                                
/**************************************** MeasureEnd **************************************/
 
    
    } 
// SCRIPT END                                                    
```
