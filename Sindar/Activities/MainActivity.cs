using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Sindar.Services;
using System;
using Android.Util;
using System.IO;
using SQLite;
using Sindar.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sindar
{
    [Activity(Label = "Sindar", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private LocationManager _locationManager;
        private string TAG = "X:" + typeof(MainActivity).Name;
        private IEnumerable<DeviceLocation> savedLocations;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            FindViewById<TextView>(Resource.Id.getLocation).Click += getLocationClick;
            FindViewById<TextView>(Resource.Id.syncLocation).Click += syncAllLocations;
        }

        private async void syncAllLocations(object sender, EventArgs e)
        {
            IEnumerable<DeviceLocation> locations;
            locations = new DeviceLocation[] { };
            var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            path = Path.Combine(path, "baseSindar.db3");
            var conn = new SQLiteConnection(path);
            var localLocations = from s in conn.Table<DeviceLocation>()
                                 select s;

            List<DeviceLocation> list = locations.ToList();
            foreach (var fila in localLocations)
            {
                DeviceLocation dev = new DeviceLocation();
                dev.Latitude = fila.Latitude;
                dev.Longitude = fila.Longitude;
                dev.Accuracy = fila.Accuracy;
                dev.SavedDate = fila.SavedDate;
                dev.Id = fila.Id;

                list.Add(dev);
            }
            var items = (IEnumerable<DeviceLocation>)list;

            RestClient Rest = new RestClient();
            savedLocations = await Rest.UpdateDeviceLocations(items);
            List<DeviceLocation> listSaved = savedLocations.ToList();
            if (listSaved.Count > 0)
            {
                foreach (var item in listSaved)
                {
                    if (item.Saved)
                    {
                        Log.Debug(TAG, "Eliminado: " + item.Id);
                        conn.Execute("delete from DeviceLocation where Id='" + item.Id +"'");
                    }

                }
            }
            

        }

        private void getLocationClick(object sender, EventArgs e)
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            TrackingService trackingLocation = new TrackingService(_locationManager, notifyLocationChanged);
        }

        public static void notifyLocationChanged(Location location)
        {
            var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            path = Path.Combine(path, "baseSindar.db3");
            var conn = new SQLiteConnection(path);
            conn.CreateTable<DeviceLocation>();
            var Insert = new DeviceLocation();
            Insert.Longitude = location.Longitude;
            Insert.Latitude = location.Latitude;
            Insert.Accuracy = location.Accuracy;
            Insert.SavedDate = DateTime.Now;
            Insert.Saved = false;
            Insert.Id = Guid.NewGuid();
            conn.Insert(Insert);
            Log.Debug("X:MainActivity","New Location: " + string.Format("{0:f6},{1:f6}", location.Latitude, location.Longitude));
        }
    }
}

