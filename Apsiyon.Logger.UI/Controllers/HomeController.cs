using System;
using System.Threading;
using Apsiyon.Logger.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Apsiyon.Logger.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;

        public HomeController(ILogger logger)
        {
            _logger = logger;
        }

        public string Index()
        {
            _logger.Add("test", DateTime.Now);
            _logger.Add("test2", Environment.CurrentDirectory);
            _logger.Add("test3", Environment.OSVersion);
            _logger.Add("test4", null);
            return "Apsiyon.Logger - .NET Core 3.1 MVC Demo";
        }
    }
}