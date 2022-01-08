# Conditional Colour Formatter (Green/Red - Pos/Neg)
Conditional colour format for negative = red, positive = green

*(copy and paste into tabular editor, select any measure and run script)*
```c#
/*# Start here #*/
var Red = '\u0022' + "#B20000" + '\u0022';
var Green = '\u0022' + "#299B00" + '\u0022';
const string qq = "\"";

// MeasureTemplate: Sophisticated Variance Column Chart:
foreach(var m in Selected.Measures) 

{
    // VarColour:
    m.Table.AddMeasure(
    
    // Name
    "@VARColour", 
    
    // DAX expression
      '\n' + "SWITCH ( TRUE (), "     
      + '\n' + m.DaxObjectName + " < 0, " +  Red + ", -- if Revenue DECREASE then RED"
      + '\n' + m.DaxObjectName + " > 0, " +  Green + " -- if Revenue DECREASE then RED"
      + '\n' + ")",          
    
    // Display Folder
    m.DisplayFolder // = FolderName                                                    
    );
}
```
