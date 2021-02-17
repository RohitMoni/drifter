# Drifter

Drifter is a framework for data migrations at scale.

## Requirements

Built using dotnet 5.

## Key Pillars:
1. Keep migration logic separate from regular code execution logic.

In large, long-running codebases with lots of state, adding migration logic in the middle of execution logic creates code bloat and makes the product harder to work in and reason about. It's important to keep migration code separate from the rest of the codebase to avoid this.

2. Think at scale: Lots of versions, expensive migrations, users using old / legacy versions.

Some products can have hundreds or thousands of versions and their state or models can morph a lot during that time. Users also use old, legacy versions very often and many expect to be able to go from old legacy versions to modern versions of the product with little to no hiccup.

3. Be independent of any particular data or serialization format.

There are lots of serialization formats out there. Sometimes new serialization formats are created that are better than old ones, but most of the time there are tradeoffs or specific circumstances that define the format a product uses. The idea of migrating data should be independent of any particular format while still allowing for specializations to take advantage of specific data formats.

## Getting Started

Take a look at the samples provided in Drifter.Samples.

## License

[The MIT License](https://opensource.org/licenses/MIT)