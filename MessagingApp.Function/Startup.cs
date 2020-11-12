using System;
using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using MessagingApp;
using MessagingAppML.Model.DataModels;
using MessagingApp.Function;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MessagingApp.Function
{
    public class Startup : FunctionsStartup
    {
        private readonly string _environment;
        private readonly string _modelPath;

        public Startup()
        {
            _environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT");

            if (_environment == "Development")
            {
                _modelPath = Path.Combine(Environment.CurrentDirectory, "MLModel.zip");
            }
            else
            {
                string deploymentPath = @"D:\home\site\wwwroot\";
                _modelPath = Path.Combine(deploymentPath, "MLModels", "MLModel.zip");
            }
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddPredictionEnginePool<ModelInput, ModelOutput>()
                .FromFile(modelName: "MLModel", filePath: _modelPath, watchForChanges: true);
            /* .FromUri(
                    modelName: "MLModel",
                    uri:"*.zip",
                    period: TimeSpan.FromMinutes(1));*/
        }
    }
}
