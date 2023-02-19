/* Author: PBI Queryous */

var FromString = "OLD TEXT";        // old text to replace
var ToString = "NEW TEXT";          // new text

/* 
All Measures: Model.AllMeasures
Selected Measures: Selected.Measures
*/
foreach (var m in Selected.Measures)
    {
        /* Cycle over all text in all measures in model and replaces the FromString with the ToString */
        m.Expression = m.Expression.Replace(FromString,ToString);
        
    }
