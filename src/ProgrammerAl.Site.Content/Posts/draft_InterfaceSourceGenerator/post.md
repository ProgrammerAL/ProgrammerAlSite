Title: New Project: Public Interface Generator
Published: 2024/05/06
Tags: 
- Blog
- Project
---

## New Project: Public Interface Generator

As developers we like to automat things. Anything we can automate is one less thing we waste time on. Another thing that's getting easier to do, easier than it used to be at least, is to codify our patterns. One common pattern in C# is to cerate an interface that exists just to make unit test mocking easier. So why not generate the interface code instead of writing it? 

Well that's the project. There's code and a NuGet package and everything. The full details are in the project README at: https://github.com/ProgrammerAL/public-interface-generator

## Quick Overview: What it does

The purpose of this project is to make a C# Source Generator to create Interfaces at compile time. The Source Generator will inspect all classes with the provided [GenerateInterfaceAttribute] attribute and generate an Interface of all public Methods, Properties, and Events.

## NuGet Package

The NuGet package is hosted on nuget.org. You can get it from: https://www.nuget.org/packages/PublicInterfaceGenerator

