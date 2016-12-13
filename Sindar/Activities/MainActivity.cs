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
        private static Location currentLocation;
        public SyncService syncService = new SyncService();

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
           
            var items = syncService.getLocations();
            RestClient Rest = new RestClient();
            savedLocations = await Rest.UpdateDeviceLocations(items);
            syncService.deleteLocations(savedLocations.ToList());           

        }

        private void getLocationClick(object sender, EventArgs e)
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            TrackingService trackingLocation = new TrackingService(_locationManager, notifyLocationChanged);
        }

        public void notifyLocationChanged(Location location)
        {
            if (location != null)
            {
                currentLocation = location;
                syncService.SaveLocation(location);
            }
         }
    }
}

