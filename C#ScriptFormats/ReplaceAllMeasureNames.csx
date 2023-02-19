/* PBI Queryous */

//-- Replace a string in a Measure Name

var SearchString = "r";      // set the search string that you want to replace
var ReplaceString = "_";            // set the replace string
 
// iterate through selected measures
// All Measures: Model.AllMeasures
// Selected Measures: Selected.Measures
foreach (var m in Model.AllMeasures)
    {
        m.Name = m.Name.Replace(SearchString, ReplaceString);
    }
