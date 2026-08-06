When working with DevExpress TablePanel layouts in PK-Sim views:

## Row visibility
- Use `tablePanel.RowFor(control).Visible = value` to show/hide rows
- After changing row visibility, **always** call `layoutControlItem.AdjustTablePanelHeight(tablePanel, layoutControl)` to recalculate the table height and eliminate gaps from hidden rows
- See `IndividualSettingsView` and `RandomPopulationSettingsView` for reference implementations

## Adding new rows to a TablePanel
1. Add the row definition in the Designer's `Rows.AddRange` array
2. Add label (column 0) and control (column 1) with matching `SetRow` indices
3. Add `BeginInit`/`EndInit` pairs for any new controls with `.Properties`
4. Add field declarations at the bottom of the Designer file
5. Wire up bindings in `InitializeBinding()` — but do NOT call presenter methods here as the presenter may not be fully initialized yet
6. Populate dynamic combo/MRU items in `BindTo()` instead of `InitializeBinding()`

## Validation rules
- Domain validation uses `CreateRule.For<T>()` from `OSPSuite.Utility.Validation`
- Add rules in the model class constructor via `Rules.Add(rule)`
- See `SchemaItemRules` for shared rules across `ISchemaItem` implementations
- Error message constants go in `PKSimConstants.Error`
