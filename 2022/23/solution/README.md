# Advent of Code 2022 Day 23

This is my F# solution optimized for performance. It is using `HashSet` and `Dictionary`, since these data structures provide higher performance over `Set` and `Map`.

I experimented using `int64` keys for tiles in both sets and dicts to speed up hash lookups, but in this case there was a performance penalty, most likely because the computation to map the key `(int64 x <<< 32) + int64 y` underperformed compared to the `Tile` default hash calculation as the record uses only two value types (x, y). For this reason converting records to structs didn't boost performance either.

All in all, I am happy with the result having improved performance by 15x!

| Implementation | Time (s)|
|----------------|---------|
| Functional: With immutable data structures     | 17      |
| Optimized: Functional with mutable data structures      | 1.2     |

My conclusion here is that immutable data structures are good for use cases that proportionally have greater reads than writes, and use small data volumes. `Map` internally store objects in a tree and therefore the best time complexity for a lookup is `O(log n)`, while for a `Dictionary` it is `O(1)`.
