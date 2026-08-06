When adding a new property to a domain model class, update all three locations:
1. The property declaration on the class
2. `UpdatePropertiesFrom` — copy every settable property from the source
3. The XML serializer's `PerformMapping` — add `Map(x => x.NewProperty)`

Missing any of these causes silent data loss on clone/save-load.
