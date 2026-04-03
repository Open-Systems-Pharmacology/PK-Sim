## C# Coding Style

### Expression-bodied members
Use expression-bodied members (`=>`) whenever a method, property, or accessor contains a single expression.

```csharp
// Wrong
public IEnumerable<string> AllEventKeys()
{
   return _eventKeyRepository.All();
}

// Right
public IEnumerable<string> AllEventKeys() => _eventKeyRepository.All();
```
