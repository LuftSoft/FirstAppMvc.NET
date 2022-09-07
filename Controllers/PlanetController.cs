using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NETMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace App.Controllers
{
    public class PlanetController : Controller
    {
        private readonly PlanetService _planetService;
        private readonly ILogger<PlanetService> _logger;

        public PlanetController(PlanetService service, ILogger<PlanetService> logger)
        {
            _planetService = service;
            _logger = logger;
        }

        [Route("danh-sach-cac-hanh-tinh.html")]
        public IActionResult Index()
        {
            return View();
        }
        [BindProperty(SupportsGet = true, Name = "action")]
        public string Name { set; get; }
        // public IActionResult Mars()
        // {
        //     var planet = _planetService.Where(p => p.Name == this.Name).FirstOrDefault();
        //     return View("Detail", planet);
        // }
        public IActionResult Planet_detail(string name)
        {
            var planet = _planetService.Where(p => p.Name == name).FirstOrDefault();
            return View("Detail", planet);
        }
    }
}