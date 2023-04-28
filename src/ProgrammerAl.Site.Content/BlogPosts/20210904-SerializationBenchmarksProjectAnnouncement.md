Title: Introducing: .NET Serialization Benchmarks
Published: 2021/09/04
Tags: 
- Performance
- Benchmark
---

## New OSS
Every so often I'll see someone talk about the performance of different serialization/deserialization protocols. Of course, anyone making the case for choosing one over the other will use benchmarks that show their tool of choice in the best light. And I don't mean that in an insulting way. It's very easy for us all to only talk about where our choice of tools excel (like I do in any conversation about .NET).

I've been wanting something that benchmarks these libraries at different entity sizes. Something with only a few properties, all the way up to very large objects with a lot of properties, with multiple levels of children.

It is nowhere near complete yet, but the repo that compares a handful of these serialization protocols using different libraries. My goal is to add to it over time to include a lot more scenarios and a lot more serialization/deserialization strategies. But as of right now I feel the project shows off enough info to announce.

## Announcing: .NET Serialization Benchmarks

It's on GitHub: [https://github.com/ProgrammerAl/SerializationBenchmarks](https://github.com/ProgrammerAl/SerializationBenchmarks)

