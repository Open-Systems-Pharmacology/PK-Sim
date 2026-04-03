## Bug fixes
When fixing bugs, follow a red-green approach:
1. **Write a failing test first** that reproduces the bug
2. **Verify the test fails** (red)
3. **Implement the fix**
4. **Verify the test passes** (green)

## New features
For every model and presentation change, write corresponding unit tests.

## Conventions
- Tests use BDDHelper with `ContextSpecification<T>`, `Context/Because/[Observation]` pattern and FakeItEasy for mocking
- Async tests use `ContextSpecificationAsync<T>`
- Test class names follow `When_<scenario>` pattern
- Observation method names follow `should_<expected_behavior>` pattern
