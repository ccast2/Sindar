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
using Android.Locations;
using Android.Util;

namespace Sindar.Services
{
    public class TrackingService : Java.Lang.Object,ILocationListener
    {
        private string TAG = "X:" + typeof(MainActivity).Name;
        private LocationManager _locationManager;
        private Location _currentLocation;
        private string _locationProvider;

        private int LOCATION_INTERVAL = 1000;
        private int LOCATION_DISTANCE = 10;
        private Action<Location> notifyLocationChanged;

        public TrackingService(LocationManager _locManager)
        {

        }


        public TrackingService(LocationManager _locManager, Action<Location> notifyLocationChanged) : this(_locManager)
        {
            this.notifyLocationChanged = notifyLocationChanged;
            this._locationManager = _locManager;
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
            }
            Log.Debug(TAG, "Using " + _locationProvider + ".");
            _locationManager.RequestLocationUpdates(_locationProvider, LOCATION_INTERVAL, LOCATION_DISTANCE, this);
        }

        public async void OnLocationChanged(Location location)
        {
            notifyLocationChanged(location);
            //(Log.Debug(TAG, "New Location");
            _currentLocation = location;
            if (_currentLocation == null)
            {
                Log.Debug(TAG, "Unable to determine your location. Try again in a short while.");
            }
            else
            {
                //Log.Debug(TAG, string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude));
            }
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Log.Debug(TAG, "No implementado");
        }
    }
}