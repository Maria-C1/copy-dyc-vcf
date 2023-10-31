﻿using Childrens_Social_Care_CPD.Core.Resources;
using Childrens_Social_Care_CPD.Models;
using Microsoft.AspNetCore.Mvc;

namespace Childrens_Social_Care_CPD.Controllers;

public class ResourcesQuery
{
    public string[] Tags { get; set; }
    public int Page { get; set; } = 1;

    public ResourcesQuery()
    {
        Tags = Array.Empty<string>();
    }
}

public class ResourcesController : Controller
{
    private readonly IResourcesSearchStrategy _strategy;

    public ResourcesController(IResourcesSearchStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(strategy);

        _strategy = strategy;
    }

    [Route("resources", Name = "Resource")]
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] ResourcesQuery query, bool preferencesSet = false, CancellationToken cancellationToken = default)
    {
        var contextModel = new ContextModel(string.Empty, "Resources", "Resources", "Resources", true, preferencesSet);
        ViewData["ContextModel"] = contextModel;

        var viewModel = await _strategy.SearchAsync(query, cancellationToken);
        return View(viewModel);
    }
}
