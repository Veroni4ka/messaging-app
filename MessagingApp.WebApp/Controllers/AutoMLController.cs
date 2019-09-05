using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MessagingAppML.Model.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;

namespace MessagingApp.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoMLController : ControllerBase
    {
        private const string MODEL_FILEPATH = @"MLModel.zip";
        [HttpGet("{text}")]
        public bool Get(string text)
        {
            var mlContext = new MLContext();
            ModelInput sampleStatement = new ModelInput { Content = text };

            ITransformer trainedModel = mlContext.Model.Load(GetAbsolutePath(MODEL_FILEPATH), out var modelInputSchema);

            // Create prediction engine related to the loaded trained model
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(trainedModel);

            // Score
            var predictedResult = predEngine.Predict(sampleStatement);

            return Convert.ToBoolean(predictedResult.Prediction);
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(AutoMLController).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }
    }
}
