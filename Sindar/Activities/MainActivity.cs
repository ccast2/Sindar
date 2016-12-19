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
        readonly string logTag = "MainActivity";
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

                App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
                    Log.Debug(logTag, "ServiceConnected Event Raised");
                    // notifies us of location changes from the system
                    App.Current.LocationService.LocationChanged += HandleLocationChanged;
                    //notifies us of user changes to the location provider (ie the user disables or enables GPS)
                    App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
                    App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
                    // notifies us of the changing status of a provider (ie GPS no longer available)
                    App.Current.LocationService.StatusChanged += HandleStatusChanged;
                };
                App.StartLocationService();
            }
            else
            {
                StartActivity(typeof(Login));
            }
        }

        private void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Log.Debug(logTag, "Location status changed, event raised");
        }

        private void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {
            Log.Debug(logTag, "Location provider enabled event raised");
        }

        private void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {
            Log.Debug(logTag, "Location provider disabled event raised");
        }

        private void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            Android.Locations.Location location = e.Location;
            Log.Debug(logTag, "Foreground updating");
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

