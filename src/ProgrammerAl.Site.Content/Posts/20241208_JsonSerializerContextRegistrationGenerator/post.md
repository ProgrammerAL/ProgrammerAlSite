Title: "New Project: C# Json Serializer Context Registrations Generator"
Published: 2024/12/08
Tags: 
- Project
- C#
---

## New Project: C# Json Serializer Context Registrations Generator

Do you use the built-in System.Text.Json Source Generator to generate code to serialize/deserialize JSON objects without using reflection? If so, you know it can be a hassle to remember to register all of the classes with the code generator. I know I think it's a hassle, and easy to forget.

That's why I made this project to make the developer experience of registering those classes a little easier. Instead of adding them all above a single class, you can add a custom attribute above each class that should be registered, and then a .NET Tool will generate a class with all of the registrations in the single file for you. Then then built-in System.Text.Json Source Generator will run off that.

The full details are in the project README at: https://github.com/ProgrammerAL/json-serializer-context-registration-generator. It also includes instructions on how to setup your code to use this.

## NuGet Packages

There are 2 NuGet packages. One for the attributes you add to your code, and the other for a .NET Tool to generate code files based on how the attributes were used. 

The attributes are in package `ProgrammerAL.JsonSerializerRegistrationGenerator.Attributes`, and the .NET Tool is at `ProgrammerAL.JsonSerializerRegistrationGenerator.Runner`

