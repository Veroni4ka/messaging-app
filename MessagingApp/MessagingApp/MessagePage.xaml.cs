using MessagingAppML.Model.DataModels;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MessagingApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagePage : ContentPage
    {
        private const string MODEL_FILEPATH = @"MLModel.zip";

        public MessagePage()
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

        public static Stream GetAbsolutePath(string relativePath)
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("MessagingApp.MLModel.zip");

            //FileInfo _dataRoot = new FileInfo(typeof(App).Assembly.Location);
            //string assemblyFolderPath = _dataRoot.Directory.FullName;

            //string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return stream;
        }
    }
}