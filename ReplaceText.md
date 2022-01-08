# Modify text (find and replace) in all Measures


## All measures
```c#
/* 
 * Warning!  Take a backup copy first.  
 * This script will operate over every measure in the model. It is 
 * essential that your FromString and ToString are set to change only the
 * specific usage of the string that you need to change across the entire model.
*/

var FromString = "OLD TEXT";
var ToString = "NEW TEXT";

foreach (var m in Model.AllMeasures)
    {
        /* Cycle over all text in all measures in model and replaces the FromString with the ToString */
        m.Expression = m.Expression.Replace(FromString,ToString);
        
    }
```

## Select measures
```c#


var FromString = "OLD TEXT";
var ToString = "NEW TEXT";

foreach (var m in Selected.Measures)
    {
        /* Cycle over all text in all selected measures and replaces the FromString with the ToString */
        m.Expression = m.Expression.Replace(FromString,ToString);
        
    }
```
