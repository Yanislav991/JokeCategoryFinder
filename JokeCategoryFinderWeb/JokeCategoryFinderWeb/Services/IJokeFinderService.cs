using JokeCategoryFinderWeb.Models;

namespace JokeCategoryFinderWeb.Services
{
    public interface IJokeFinderService
    {
        public JokePrediction GetModelOutput(string joke);
    }
}
