﻿using System;
using Android.App;
using Android.OS;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using GPS.Services;
using Android.Util;
using Android.Locations;
using GPS.Models;
using Newtonsoft.Json;
using Android.Content;
using Java.Util.Concurrent;
using Java.Lang;
using GPS.Activities;

namespace GPS
{
    [Activity(Label = "GPS", Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        readonly string TAG = "MainActivity";
        User currentUser;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            string userString = Intent.GetStringExtra("User") ?? "Data not available";
            currentUser = JsonConvert.DeserializeObject<User>(userString);
            Context mContext = Android.App.Application.Context;
            currentUser.uContext = mContext;
            renderMainView();
            startGPS();
        }


        private void startGPS()
        {
            App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
            {
                Log.Debug(TAG, "ServiceConnected Event Raised");
                App.Current.LocationService.LocationChanged += HandleLocationChanged;
                App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
                App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
                App.Current.LocationService.StatusChanged += HandleStatusChanged;
            };
            App.StartLocationService();
        }

        private void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Log.Debug(TAG, "Location status changed, event raised");
        }

        private void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {
            Log.Debug(TAG, "Location provider enabled event raised");
        }

        private void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {
            Log.Debug(TAG, "Location provider disabled event raised");
        }

        private void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            Log.Debug(TAG, "Foreground updating");
        }

        private void renderMainView()
        {
            var eventsToolbar = FindViewById<Toolbar>(Resource.Id.topToolbar);
            eventsToolbar.Title = "Eventos";
            eventsToolbar.InflateMenu(Resource.Menu.first_menu);
            eventsToolbar.MenuItemClick += (sender, e) =>
            {
                switch (e.Item.TitleFormatted.ToString())
                {
                    case "Nuevo":
                        createNewEvent();
                        break;
                    case "Configuración":
                        break;
                    default:
                        break;
                }
            };
            var servicesToolbar = FindViewById<Toolbar>(Resource.Id.middleToolbar);
            servicesToolbar.Title = "Servicios";
            servicesToolbar.InflateMenu(Resource.Menu.services_menu);
            servicesToolbar.MenuItemClick += (sender, e) =>
            {
                switch (e.Item.TitleFormatted.ToString())
                {
                    case "Nuevo":
                        break;
                    default:
                        break;
                }
            };
        }

        private void createNewEvent()
        {
            StartActivity(new Intent(Application.Context, typeof(DeviceEventActivity)));
        }
    }
}

