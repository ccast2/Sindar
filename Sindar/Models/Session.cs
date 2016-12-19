using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Android.Util;
using Newtonsoft.Json;
using Sindar.Activities;

namespace Sindar.Models
{
    class Session
    {
        private string TAG = "X:" + typeof(MainActivity).Name;
        protected string BaseUrl { get; set; } = "http://restmision1a.azurewebsites.net/index.php/Users/";
        public string authKey;

        public Session(string key)
        {
            authKey = key;
        }
        public async Task<User> ValidateKey()
        {
            var data = new[]{
                new KeyValuePair<string, string>("default","1")
             };
            var url = "";
            string result = await SendAsJson(url,data);
            User response = new Models.User();
            if (result != "")
            {
                response = JsonConvert.DeserializeObject<User>(result);
            }
            return response;

        }
        protected async Task<string> SendAsJson(string url,KeyValuePair<string, string>[] data)
        {
            var result = "";
            HttpClientHandler hch = new HttpClientHandler();
            hch.Proxy = null;
            hch.UseProxy = false;
            using (var httpClient = new HttpClient(hch))
            {
             
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json")
                    );
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authKey);
                var content = new FormUrlEncodedContent(data);
                Log.Debug(TAG, "request");
                var response = await httpClient.PostAsync(new Uri(BaseUrl + url), content).ConfigureAwait(false);
                Log.Debug(TAG, "response");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        result = json;
                    }
                }
                else
                {
                    Log.Debug(TAG, "error");
                }
            }

            return result;
        }
        internal async Task<User> Login(string UserName, string Password)
        {
            
            var data = new[]{
                new KeyValuePair<string, string>("Plate",UserName),
                new KeyValuePair<string, string>("Password",Password)
             };
            var url = "login";
            string result = await SendAsJson(url,data);
            User response = new Models.User();
            if (result != "")
            {
                response = JsonConvert.DeserializeObject<User>(result);
            }
            
            return response;
        }
    }
}