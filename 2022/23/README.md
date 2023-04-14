# Advent of Code 2022 Day 23

This is my F# solution optimized for performance. It is using `HashSet` and `Dictionary` since these data structures provide higher performance over `Set` and `Map`.

A big improvement was to check each `Tile` in the `Set` with `contains` rather than calling `intersect` with many tiles.

I experimented using `int64` keys for storing `Tile`s in both sets and dicts to speed up lookups, but in this case there was a performance penalty, most likely because the computation `(int64 x <<< 32) + int64 y` underperformed compared to the default hash calculation. It should improve in cases where a record/struct has reference values.

Also tried converting records to structs, but this didn't boost performance either in this case, most likely for above justification.

Using `array` instead of `list` for fixed data saved some milliseconds, for example for iterating and cycling directions.

All in all, I am happy with the result having improved performance by 15x!

| Implementation | Time (s)|
|----------------|---------|
| With immutable data structures | 17 |
| With mutable data structures | 1.2 |

My conclusion here is that immutable data structures are good for use cases that proportionally have greater reads than writes and small data volume. `Map` internally store objects in a tree and therefore the best time complexity for a lookup is `O(log n)`, while for a `Dictionary` it is `O(1)`.
