using MessagingAppML.Model.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
            var text = MessageBox.Text;
            var recipient = RecipientBox.Text;

            if (text == "" || recipient == "")
            {
                return;
            }

            var prediction = TestSinglePrediction(text).Result;
            switch (prediction)
            {
                case "Negative":
                    {
                        var sendText = await DisplayAlert("Warning", "The message you're about to send might be rude and/or offensive. Are you sure you want to send it?", "Yes", "No");
                        if (sendText)
                            await SendSms(text, recipient);
                        break;
                    }
                case "Positive":
                    await SendSms(text, recipient);
                    break;
                default:
                    break;
            }
        }

        private async Task<string> TestSinglePrediction(string text)
        {
            var req = new Request();
            req.Comment = text;

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                try
                {
                    var serializedText = JsonConvert.SerializeObject(req);
                    var content = new StringContent(serializedText, Encoding.UTF8, "application/json");
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri("");

                    var response = await client.PostAsync(request.RequestUri, content).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsStringAsync().Result;
                    }
                    return response.StatusCode.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

    }
}