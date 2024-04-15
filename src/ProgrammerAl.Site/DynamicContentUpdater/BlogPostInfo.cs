using ProgrammerAl.Site.Utilities.Entities;

using System;

namespace ProgrammerAl.Site.DynamicContentUpdater;

public record BlogPostInfo(string PostName, DateOnly PostDate, PostEntry Entry);
