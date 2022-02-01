using Microsoft.AspNetCore.Mvc;
using RebelCmsTemplate.Models;
using System.Diagnostics;
using RebelCmsTemplate.Util;

namespace RebelCmsTemplate.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Route("")]
    public IActionResult Login()
    {
        return View();
    }

    [Route("Main")]
    public IActionResult Main()
    {
        return View();
    }

    

    [Route("LogOut")]
    public IActionResult LogOut()
    {
        SharedUtil sharedUtil = new(_httpContextAccessor);
        sharedUtil.GetRemoveSession();
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}