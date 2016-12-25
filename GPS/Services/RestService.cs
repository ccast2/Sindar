using System;
using System.Collections.Generic;
using Android.Views;
using System.Threading.Tasks;
using Plugin.Connectivity;
using System.Net.Http;
using System.Net.Http.Headers;
using Android.Util;
using Android.Widget;

namespace GPS.Services
{
    class RestService
    {
        private string TAG = "X:" + typeof(RestService).Name;
        protected string BaseUrl { get; set; } = "http://restmision1a.azurewebsites.net/index.php/Users/";
        public string authKey;

        protected async Task<string> SendAsJson(string url, KeyValuePair<string, string>[] data)
        {
            var result = "";
            if (CrossConnectivity.Current.IsConnected)
            {   
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
            }
            else
            {
                Toast.MakeText(this, "Bottom toolbar tapped: " + e.Item.TitleFormatted, ToastLength.Short).Show();
                result = "";
            }

            return result;
        }
    }
}