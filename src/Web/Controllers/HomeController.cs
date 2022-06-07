using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Web.Models;

namespace Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index(string q)
    {
        string endpoint = _configuration.GetValue<string>("CosmosDbEndpoint");
        string key = _configuration.GetValue<string>("AuthorizationKey");

        CosmosClient cosmosClient;
        if (!string.IsNullOrEmpty(_configuration.GetValue<string>("ServerRegion")))
        {
            var serverRegion = _configuration.GetValue<string>("ServerRegion");

            CosmosClientOptions options = new CosmosClientOptions();
            options.ApplicationName = "App-Workshop";

            if (serverRegion == "JapanEast")
            {
                options.ApplicationRegion = Regions.JapanEast;
            }
            else if (serverRegion == "EastUS")
            {
                options.ApplicationRegion = Regions.EastUS;
            }

            cosmosClient = new CosmosClient(endpoint, key, options);
        }
        else
        {
            cosmosClient = new CosmosClient(endpoint, key);
        }

        Container container = cosmosClient.GetContainer("AdventureWorks", "Products");

        var queryString = "SELECT c.productId, c.productName, c.color, c.listPrice, c.category, c.sellStartDate FROM c";

        // <iframe width="100%" height="166" scrolling="no" frameborder="no" allow="autoplay" src="https://w.soundcloud.com/player/?url=https%3A//api.soundcloud.com/tracks/771984076&color=%23ff5500&auto_play=true&hide_related=false&show_comments=true&show_user=true&show_reposts=false&show_teaser=true"></iframe>

        if (string.IsNullOrEmpty(q))
        {
            ViewData["Message"] = "すべてを表示";
        }
        else
        {
            ViewData["Message"] = $"検索結果 - {q}";

            queryString += $" WHERE c.productName LIKE '%{q}%'";
        }

        var query = container.GetItemQueryIterator<Product>(new QueryDefinition(queryString));
        List<Product> results = new List<Product>();

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();

            results.AddRange(response.ToList());
        }

        return View(results);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
