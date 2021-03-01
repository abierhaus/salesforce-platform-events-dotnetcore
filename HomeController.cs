using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace salesforce_platform_events_dotnetcore
{
    public class HomeController : Controller
    {
        public SalesforceEventService SalesforceEventService { get; set; }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, SalesforceEventService salesforceEventService)
        {
            _logger = logger;
            SalesforceEventService = salesforceEventService;
        }


        public async Task<IActionResult> Index()
        {

            var result = await SalesforceEventService.PublishEventAsync();

            _logger.LogInformation(result);

            return new OkObjectResult(result);
        }
    }
}