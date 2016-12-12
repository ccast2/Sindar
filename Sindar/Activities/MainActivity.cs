using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Sindar.Services;
using System;
using Android.Util;
using System.IO;
using SQLite;

namespace Sindar
{
    [Activity(Label = "Sindar", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private LocationManager _locationManager;
        private string TAG = "X:" + typeof(MainActivity).Name;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            FindViewById<TextView>(Resource.Id.getLocation).Click += getLocationClick;
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
            Log.Debug("Main", "From main.");
            Log.Debug("Main", string.Format("{0:f6},{1:f6}", location.Latitude, location.Longitude));
        }
    }
}

