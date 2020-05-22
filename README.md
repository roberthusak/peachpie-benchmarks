# Peachpie Benchmarks

This repository contains benchmarks to evaluate the performance of Peachpie and its comparison with PHP:

- `microbenchmarks`: Runs small code snippets in a custom benchmarking engine, uses Docker to run under both Peachpie and PHP.
- `transformations`: Uses BenchmarkDotNet to evaluate the performance benefits of the transformation phase in Peachpie compiler.