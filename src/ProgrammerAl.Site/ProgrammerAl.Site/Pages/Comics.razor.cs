using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;

using Microsoft.AspNetCore.Components;

using ProgrammerAl.Site.DataProviders;

namespace ProgrammerAl.Site.Pages;

public partial class Comics : ComponentBase
{
    [Inject]
    private PostSummariesProvider PostSummariesProvider { get; set; }

    [Inject]
    private NavigationManager NavManager { get; set; }

    protected override async Task OnInitializedAsync()
    {

        await base.OnInitializedAsync();
    }
}
