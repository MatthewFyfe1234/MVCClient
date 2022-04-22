using System.Diagnostics;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Models;
using mvcClient.Services;
using Newtonsoft.Json;

namespace mvcClient.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITokenService _tokenService;
    public HomeController(ILogger<HomeController> logger, ITokenService tokenService)
    {
        _logger = logger;
        _tokenService = tokenService;
    }

    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public async Task<IActionResult> Weather()
    {
        var data = new List<WeatherData>();
        using (var client = new HttpClient())
        {
            var tokenResponse = await _tokenService.GetToken("weatherapi.read");
            client.SetBearerToken(tokenResponse.AccessToken);
            var result = client.GetAsync("https://localhost:5445/weatherforecast").Result;
            if (result.IsSuccessStatusCode)
            {
                var model = result.Content.ReadAsStringAsync().Result;
                data = JsonConvert.DeserializeObject<List<WeatherData>>(model);
                return View(data);
            }
            else
                throw new Exception("Unauthorised");
        };
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> WeatherAuthCode()
    {
        var data = new List<WeatherData>();
        using (var client = new HttpClient())
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            client.SetBearerToken(token);
            var result = client.GetAsync("https://localhost:5445/weatherforecast").Result;
            if (result.IsSuccessStatusCode)
            {
                var model = result.Content.ReadAsStringAsync().Result;
                data = JsonConvert.DeserializeObject<List<WeatherData>>(model);
                return View(data);
            }
            else
                throw new Exception("Unauthorised");
        };
    }
}
