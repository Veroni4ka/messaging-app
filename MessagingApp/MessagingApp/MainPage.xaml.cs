using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Microsoft.ML;
using Microsoft.ML.Data;
using MessagingAppML.Model.DataModels;
using System.IO;

namespace MessagingApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private const string MODEL_FILEPATH = @"MLModel.zip";

        public MainPage()
        {
            InitializeComponent();
        }

        public async Task SendSms(string messageText, string recipient)
        {
            try
            {
                var message = new SmsMessage(messageText, new[] { recipient });
                await Sms.ComposeAsync(message);
                await DisplayAlert("Success", "Your message has been successfully sent.", "OK");
            }
            catch (FeatureNotSupportedException ex)
            {
                await DisplayAlert("Error", "Sms is not supported on this device.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void SendBtn_Clicked(object sender, EventArgs e)
        {
            var mlContext = new MLContext();
            var text = MessageBox.Text;
            var recipient = RecipientBox.Text;

            if (text == "" || recipient == "")
            {
                return;
            }

            var prediction = TestSinglePrediction(mlContext, text);
            if (prediction)
            {
                var sendText = await DisplayAlert("Warning", "The message you're about to send might be rude and/or offensive. Are you sure you want to send it?", "Yes", "No");
                if (sendText)
                    await SendSms(text, recipient);
            }
            await SendSms(text, recipient);
        }

        private static bool TestSinglePrediction(MLContext mlContext, string text)
        {
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
            FileInfo _dataRoot = new FileInfo(typeof(App).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }
    }
}
