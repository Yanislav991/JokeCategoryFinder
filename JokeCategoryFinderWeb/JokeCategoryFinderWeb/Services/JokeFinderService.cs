using JokeCategoryFinderWeb.Models;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.ML;
using Newtonsoft.Json;
using System.Net;

namespace JokeCategoryFinderWeb.Services
{
    public class JokeFinderService : IJokeFinderService
    {
        private const string DataFilePath = "./wwwroot/jokes.json";
        private const string ModelFilePath = "./wwwroot/model.zip";
        public JokePrediction GetModelOutput(string joke)
        {
            var context = new MLContext();
            if (!File.Exists(ModelFilePath))
            {
                TrailModel(context);
            }
            var model = context.Model.Load(ModelFilePath, out _);
            var predictionEngine = context.Model.CreatePredictionEngine<JokeModel, JokePrediction>(model);
            var prediction = predictionEngine.Predict(new JokeModel { Joke = joke });
            return prediction;
        }

        private void TrailModel(MLContext context)
        {
            var data = GetData();
            IDataView trainingData = context.Data.LoadFromEnumerable<JokeModel>(data);
            var pipeline = context.Transforms.Conversion.MapValueToKey(inputColumnName: "Category", outputColumnName: "Label")
                .Append(context.Transforms.Text.FeaturizeText("Features", "Category"))
                .Append(context.MulticlassClassification.Trainers.LbfgsMaximumEntropy())
                .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var trainedModel = pipeline.Fit(trainingData);
            context.Model.Save(trainedModel, trainingData.Schema, ModelFilePath);
        }
        private IEnumerable<JokeModel> GetData()
        {
            var fileData = File.ReadAllText(DataFilePath);
            var deserializedObj = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(fileData)
                                .Where(x => x.Value.Count() > 30);
            var result = new List<JokeModel>();

            foreach (var category in deserializedObj)
            {
                foreach (var joke in category.Value)
                {
                    result.Add(new JokeModel()
                    {
                        Category = category.Key,
                        Joke = joke
                    });
                }
            }

            return result;
        }
    }
}
