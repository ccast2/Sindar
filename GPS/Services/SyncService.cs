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
using Newtonsoft.Json;
using System.Net.Http;
using GPS.Models;
using Android.Util;

namespace GPS.Services
{
    class SyncService
    {
        DatabaseService db = new DatabaseService();
        private readonly string TAG = "X:" + typeof(SyncService).Name;
        public SyncService()
        {
        }

        private void pullEvents()
        {
            throw new NotImplementedException();
        }

        private void pullServices()
        {
            throw new NotImplementedException();
        }

        public async void pushLocations()
        {
            var locations = db.getLocations();
            var list = locations.ToList();
            Log.Debug(TAG, "Ubicaciones: " + list.Count);
            if (list.Count > 10)
            {
                var locationsJson = JsonConvert.SerializeObject(locations);

                var data = new[]{
                    new KeyValuePair<string, string>("locationJson",locationsJson)
                 };
                var url = "/Locations";
                RestService rest = new RestService();
                string result = await rest.SendAsJson(url, data);
                if (result != "")
                {
                    var serverLocations = JsonConvert.DeserializeObject<IEnumerable<DeviceLocation>>(result);
                    db.deleteLocations(serverLocations.ToList());
                }
            }

        }
    }
}