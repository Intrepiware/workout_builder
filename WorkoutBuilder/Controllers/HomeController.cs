﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WorkoutBuilder.Models;

namespace WorkoutBuilder.Controllers
{
    public class HomeController : Controller
    {
        //public required ITestService TestService { protected get; init; }

        public IActionResult Index()
        {
            //return Content(TestService.GetMessage());
            return Ok();
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
    }
}