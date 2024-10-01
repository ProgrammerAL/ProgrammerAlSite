Title: New Project: Public Interface Generator
Published: 2024/09/30
Tags: 
- Blog
- Project
- Source Generator
- C#
---

## New Project: Public Interface Generator

As developers we like to automate things. Anything we can automate is one less thing we waste time on later. Another thing that's getting easier to do, easier than it used to be at least, is to codify our patterns. Enforcing code patterns with code. One common pattern in C# is to create an interface that exists just to make unit test mocking easier. So why not generate the interface code instead of writing it? 

Well that's the project. There's code and a NuGet package and everything. The full details are in the project README at: https://github.com/ProgrammerAL/public-interface-generator

## Quick Overview: What it does

The purpose of the project is to make a C# Source Generator to create interfaces at compile time. The Source Generator will inspect all classes with the provided [GenerateInterfaceAttribute] attribute and generate an Interface of all public Methods, Properties, and Events.

## NuGet Package

The NuGet package is hosted on nuget.org. You can get it from: https://www.nuget.org/packages/PublicInterfaceGenerator

