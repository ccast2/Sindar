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
using Sindar.Activities;
using Android.Content;

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
        Session session;

        public MainActivity()
        {

        }

        protected override async void OnCreate(Bundle bundle)
        {
            var progressDialog = ProgressDialog.Show(this, "Espere por favor...", "Cargando...", true);
            base.OnCreate(bundle);
            Context mContext = Android.App.Application.Context;
            AppPreferences ap = new AppPreferences(mContext);
            string key = ap.getAccessKey();
            bool rememberMe = ap.getRemeberValue();
            session = new Session(key);
            User user;
            if (!rememberMe)
            {
                user = new User();
            }
            else {
                user = await session.ValidateKey();
            }

            progressDialog.Hide();

            if (user.Id > 0)
            {
                SetContentView(Resource.Layout.Main);
                FindViewById<TextView>(Resource.Id.getLocation).Click += getLocationClick;
                FindViewById<TextView>(Resource.Id.syncLocation).Click += syncAllLocations;
            }
            else
            {
                StartActivity(typeof(Login));
            }
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

