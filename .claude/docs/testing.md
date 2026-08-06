## Bug fixes
When fixing bugs, follow a red-green approach:
1. **Write a failing test first** that reproduces the bug
2. **Verify the test fails** (red)
3. **Implement the fix**
4. **Verify the test passes** (green)

## New features
For every model and presentation change, write corresponding unit tests.

## Test coverage expectations
Every new addition must have tests at all layers except UI (views are difficult to test):
- **Domain model** (Core): new classes, properties, methods, clone/update behavior
- **Serialization** (Infrastructure): XML round-trip, snapshot mapper round-trip
- **Presentation**: presenters, DTOs, mappers, validation rules
- **Integration**: end-to-end scenarios for simulation creation workflows

Do NOT skip domain model tests — even simple POCOs like mappings need specs for clone, equality, and collection behavior.

## Conventions
- Tests use BDDHelper with `ContextSpecification<T>`, `Context/Because/[Observation]` pattern and FakeItEasy for mocking
- Async tests use `ContextSpecificationAsync<T>`
- Test class names follow `When_<scenario>` pattern
- Observation method names follow `should_<expected_behavior>` pattern
