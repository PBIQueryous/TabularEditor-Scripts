# DAX Formatter

## Long line
```c#
/*  Cycle over all measures in model format
using DAX Formatter with Long Lines, 
then add 1 line feed to the start of the measure */

Model.AllMeasures.FormatDax();
foreach (var m in Model.AllMeasures)
    {
        m.Expression = '\n' + m.Expression ;
    }
```
## Short line
```c#
FormatDax(Model.AllMeasures, true);
foreach (var m in Model.AllMeasures)
    {
        m.Expression = '\n' + m.Expression;
    }
```
