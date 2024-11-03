Title: "New Project: Code Updater"
Published: 2024/11/03
Tags: 
- Blog
- Project
- C#
---

## New Project: Code Updater

Quick, when was the last time you updated all dependencies on all your projects? Not recently enough I bet. The release of .NET 9 is right around the corner. How long will it take you to update your projects to use it? After all, it's just a single character change in the csproj file. In every csproj file. Plus you have to make sure the project still copmiles....it can take some time.

When we get around to updating, there are always manual tasks we do, if we get the time to do it. So why not automate that? I created a .NET Tool to run updates on code in a local directory. The full details are in the project README at: https://github.com/ProgrammerAL/code-updater

## Quick Overview: What it does

The purpose of this project is to update specific aspects of code within a directory. This is useful when you have a large number of projects that you want to update all at once. This application is assumed to run on a local machine, and then the changes are manually committed to source control. This allows for a more controlled update process, because a developer is meant to manually check the changes before committing them.

The ReadMe on GitHub (linked above) goes into a lot more details on how to use it. Give it a whirl on a project you have.

## .NET Tool

The NuGet package is hosted on nuget.org. You can view the NuGet at: https://www.nuget.org/packages/ProgrammerAL.Tools.CodeUpdater

You can install the tool locally using: `dotnet tool install --global ProgrammerAL.Tools.CodeUpdater --version 1.0.0.47`

