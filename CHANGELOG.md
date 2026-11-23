# Changelog

## 3.0.0

### Fixed
- Instance cache is now scoped to each mapping call instead of ambient process/thread state. Fixes corrupted one-to-many mappings, overwritten objects and exceptions under concurrency (#90, #81).

### Added
- `InstanceCache` class and `Map` / `MapDynamic` overloads that accept it, to merge instances across calls explicitly.
- Targets: netstandard2.0, net48, net6.0, net8.0, net9.0, net10.0 (netstandard2.1 and net47 retained).

### Changed (breaking behavior)
- `keepCache` is ignored; instances are no longer kept between calls. Pass an `InstanceCache` to get the previous behavior.
- `Cache.ClearInstanceCache()` is obsolete and does nothing; `Cache.ClearAllCaches()` only clears type metadata.

## 2.0.5
- Memory improvement (#83).

