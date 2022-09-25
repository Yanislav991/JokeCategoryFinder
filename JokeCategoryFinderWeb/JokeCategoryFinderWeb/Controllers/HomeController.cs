using JokeCategoryFinderWeb.Models;
using JokeCategoryFinderWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace JokeCategoryFinderWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJokeFinderService _jokeFinderService;

        public HomeController(IJokeFinderService jokeFinderService)
        {
            this._jokeFinderService = jokeFinderService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index([FromBody] JokeResponseModel jokeResponseModel)
        {
            var result = _jokeFinderService.GetModelOutput(jokeResponseModel.joke);
            return Json(new { Category = result.Category, Percent = String.Format("Value: {0:P2}.", result.Score.Max()) });
        }

    }
}