using ProgrammerAl.Site.Utilities.Entities;

using System;

namespace ProgrammerAl.Site.DynamicContentUpdater;

public record PostInfo(
    string PostName,
    DateOnly PostDate,
    ParsedEntry Entry);
