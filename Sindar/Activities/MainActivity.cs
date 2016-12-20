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
using Android.Views;

using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Sindar
{
    [Activity(Label = "Sindar", Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        private LocationManager _locationManager;
        private string TAG = "X:" + typeof(MainActivity).Name;
        readonly string logTag = "MainActivity";
        private IEnumerable<DeviceLocation> savedLocations;
        private static Location currentLocation;
        public SyncService syncService = new SyncService();

        public MainActivity()
        {

        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);


            SetContentView(Resource.Layout.Main);
            //FindViewById<TextView>(Resource.Id.getLocation).Click += getLocationClick;
            //FindViewById<TextView>(Resource.Id.syncLocation).Click += syncAllLocations;
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "My AppCompat Toolbar";
            var editToolbar = FindViewById<Toolbar>(Resource.Id.edit_toolbar);
            editToolbar.Title = "Editing";
            editToolbar.InflateMenu(Resource.Menu.edit_menus);
            editToolbar.MenuItemClick += (sender, e) =>
            {
                Toast.MakeText(this, "Bottom toolbar tapped: " + e.Item.TitleFormatted, ToastLength.Short).Show();
            };


            App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
            {
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
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
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
            //notifyLocationChanged(location);
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
            }
         }
    }
}

