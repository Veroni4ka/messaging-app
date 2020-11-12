using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.ML;
using MessagingAppML.Model.DataModels;
using Microsoft.ML;

namespace MessagingApp.Function
{
    public class CallMLModel
    {
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEnginePool;
        private const string MODEL_FILEPATH = @"MLModel.zip";

        public CallMLModel(PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        [FunctionName("CallMLModel")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //Parse HTTP Request Body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ModelInput data = JsonConvert.DeserializeObject<ModelInput>(requestBody);
            
            //Make Prediction
            ModelOutput prediction = _predictionEnginePool.Predict("MLModel", example: data);
            
            //var mlContext = new MLContext();
            //ModelInput sampleStatement = new ModelInput { Comment_text = data.Comment_text };

            //ITransformer trainedModel = mlContext.Model.Load(GetAbsolutePath(MODEL_FILEPATH), out var modelInputSchema);

            // Create prediction engine related to the loaded trained model
            //var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(trainedModel);

            // Score
            //var predictedResult = predEngine.Predict(sampleStatement);
            //Convert prediction to string
            string sentiment = prediction.Prediction == "0" ? "Positive" : "Negative";

            //Return Prediction
            return (ActionResult)new OkObjectResult(sentiment);
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(CallMLModel).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.Parent.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }
    }
}
