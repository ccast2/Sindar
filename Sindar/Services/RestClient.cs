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
using Sindar.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Android.Util;

namespace Sindar.Services
{
    class RestClient
    {
        private string TAG = "X:" + typeof(MainActivity).Name;
        public async Task<IEnumerable<DeviceLocation>> UpdateDeviceLocations(IEnumerable<DeviceLocation> locations)
        {
            return await SendAsJson(locations);
        }
        protected string BaseUrl { get; set; } = "http://10.44.58.98/CTW/SindarRest/insertuser.php";
        protected async Task<IEnumerable<DeviceLocation>> SendAsJson(IEnumerable<DeviceLocation> locations)
        {
            var result = Enumerable.Empty<DeviceLocation>();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json")
                    );
                var oldContent = JsonConvert.SerializeObject(locations);
                var newcontent = new StringContent(oldContent, Encoding.UTF8, "application/json");
                var content = new FormUrlEncodedContent(new[]
                  {
                    new KeyValuePair<string, string>("locationJson", oldContent)
                });
                var response = await httpClient.PostAsync(BaseUrl,content).ConfigureAwait(false);
                Log.Debug(TAG, "request");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        result = await Task.Run(() =>
                        {
                            Log.Debug(TAG, "Respuesta Exitosa servicio");
                            return JsonConvert.DeserializeObject<IEnumerable<DeviceLocation>>(json);
                        }).ConfigureAwait(false);
                    }
                }
                else {
                    Log.Debug(TAG, "error");
                }
            }
            return result;

        }
    }
}