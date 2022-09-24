using Microsoft.ML.Data;

namespace JokeCategoryFinderWeb.Models
{
    public class JokePrediction
    {
        [ColumnName("PredictedLabel")]
        public string Category { get; set; }
        public float[] Score { get; set; }
    }
}
