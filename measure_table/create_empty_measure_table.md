
### step 1
```csx
// ATTENTION FOR TE2 Users: Script needs modification AND needs to be run in 3 steps

// First Step: Add Table
    var table = Model.AddCalculatedTable("Measure", "{0}"); 

// Second Step: JUST FOR TE2 Save Data Model Changes
```

### step 2
```csx
// Third Step: Hides the column, uncomment the next two lines and execute it separately to the previous creation
    var table = Model.Tables["Measure"]; 
    table.Columns[0].IsHidden = true;  
```
