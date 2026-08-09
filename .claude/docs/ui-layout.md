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

## View height
Heights are always derived from content, never from hardcoded margin or padding constants.

- A resizable view exposes `OptimalHeight` and raises `HeightChanged` when its content changes.
- A view that **hosts** another view must subscribe to the hosted view's `HeightChanged` and push the new height into the layout:
  `view.HeightChanged += (o, e) => OnEvent(() => layoutItem.AdjustControlHeight(e.Height, layoutControl));`
- `BaseContainerUserControl.AddViewTo` already does this for you. If you host a view with `panel.FillWith(view)` instead, you own the subscription — forgetting it means the layout free-splits the available height and the hosted grid grows a scrollbar.
- If a grid is a few pixels short, the missing subscription is the cause. Do not paper over it by adding `Margin.Vertical` or `Padding.Height` to the total.

## Hiding a layout item
- Default a layout item to `LayoutVisibility.Never` in the Designer and let its setter turn it on, rather than defaulting to visible and hiding it later — a setter that is never called leaves an empty control on screen.
- Exclude hidden items from any height calculation.

## Validation rules
- Domain validation uses `CreateRule.For<T>()` from `OSPSuite.Utility.Validation`
- Add rules in the model class constructor via `Rules.Add(rule)`
- See `SchemaItemRules` for shared rules across `ISchemaItem` implementations
- Error message constants go in `PKSimConstants.Error`
