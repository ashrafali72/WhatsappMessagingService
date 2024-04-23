using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;

namespace WhatsappService
{
    public class WhatsappHelper
    {
        #region Private Fields
        private static readonly HttpClient _httpClient;
        private static readonly string _Instance = ConfigurationManager.AppSettings["InstanceID"];
        private static readonly string _AccessToken = ConfigurationManager.AppSettings["WhatsAppAccessToken"];
        private static readonly string _PhoneNumberPrefix = "91";
        private static readonly string _PhoneNumberSuffix = "@c.us";
        #endregion

        #region Static constructor
        static WhatsappHelper()
        {
            _httpClient = new HttpClient();
        }
        #endregion

        #region Public Method for sending WhatsApp
        public static bool SendWhatsapp(string number, string message, string fileUrl = null, string fileCaption = null)
        {
            bool IsSuccess;
            if (!string.IsNullOrEmpty(fileUrl))
            {
                IsSuccess = SendMediaAsync(number, message, fileUrl, fileCaption);
            }
            else
            {
                IsSuccess = SendMessageAsync(number, message);
            }
            return IsSuccess;
        }
        #endregion

        #region Send Message
        private static bool SendMessageAsync(string number, string msg)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://waapi.app/api/v1/instances/"+ _Instance + "/client/action/send-message");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", "Bearer " + _AccessToken);
            number = _PhoneNumberPrefix + number + _PhoneNumberSuffix;
            string jsonContent = JsonConvert.SerializeObject(new { chatId = number, message = msg });
            var content = new StringContent(jsonContent, null, "application/json");
            request.Content = content;
            var response = _httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            return true;
        }
        #endregion

        #region Send Media
        private static bool SendMediaAsync(string number, string msg, string fileUrl, string fileCaption)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://waapi.app/api/v1/instances/"+ _Instance + "/client/action/send-media");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", "Bearer " + _AccessToken);
            number = _PhoneNumberPrefix + number + _PhoneNumberSuffix;
            string jsonContent = JsonConvert.SerializeObject(new { chatId = number, message = msg, mediaUrl = fileUrl, mediaCaption = fileCaption });
            var content = new StringContent(jsonContent, null, "application/json");
            request.Content = content;
            var response = _httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            return true;
        }
        #endregion

    }
}
