using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Android.Util;

namespace GPS.Services
{
    class RestService
    {
        private string TAG = "X:" + typeof(RestService).Name;
        protected string BaseUrl { get; set; } = "http://192.168.1.7/index.php/";
        public string authKey = "";

        public RestService()
        {
            
            PreferencesService ap = new PreferencesService(Android.App.Application.Context);
            authKey = ap.getAccessKey();
        }

        public async Task<string> SendAsJson(string url, KeyValuePair<string, string>[] data)
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
                var response = await httpClient.PostAsync(new Uri(BaseUrl + url), content).ConfigureAwait(false);

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
    }
}