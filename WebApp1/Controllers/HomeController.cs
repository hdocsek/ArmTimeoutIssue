using Microsoft.AspNetCore.Mvc;

namespace WebApp1.Controllers;

[Route("[controller]")]
[ApiController]
public class HomeController: ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var currentUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}";
        return Content(@$"<html><head><title>Instructions</title></head><body>
<h1>INSTRUCTIONS</h1> 
<ol>
    <li>
        Copy ConsoleApp1 to an ARM machine
    </li>
    <li>
        Set the targetUrl in Program.Main to: {currentUrl} 
    </li>
    <li>
        Execute Console App1: This will send a POST to {currentUrl} <br> 
        ==> Fails on ARM machines with a timeout exception 
    </li>
<ol>
</body></html>", "text/html");

    }

    [HttpPost]
    public IActionResult Post(HomeInput input)
    {
        if (input?.Data == null)
            return BadRequest("Input.Data was empty");
        // just return what was posted 
        return Ok(input.Data);
    }
}

public class HomeInput
{
    public string? Data { get; set; }
}