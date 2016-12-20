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
using System.Threading;
using System.Device.Location;

namespace Sindar.Services
{
    [Service(Name = "com.xamarin.example.LocationService")]
    public class LocationService : Service, ILocationListener
    {
        public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };
        public event EventHandler<ProviderDisabledEventArgs> ProviderDisabled = delegate { };
        public event EventHandler<ProviderEnabledEventArgs> ProviderEnabled = delegate { };
        public event EventHandler<StatusChangedEventArgs> StatusChanged = delegate { };

        // Set our location manager as the system location service
        protected LocationManager LocMgr = Android.App.Application.Context.GetSystemService("location") as LocationManager;

        readonly string logTag = "LocationService";
        IBinder binder;
        static readonly int TimerWait = 4000;
        Timer timer;
        DateTime startTime;
        bool isStarted = false;
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        Location currentLocation;
        readonly int minDist = 10;

        SyncService syncService = new SyncService();


        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug(logTag, "OnCreate called in the Location Service");
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug(logTag, $"OnStartCommand called at {startTime}, flags={flags}, startid={startId}");
            if (isStarted)
            {
                TimeSpan runtime = DateTime.UtcNow.Subtract(startTime);
                Log.Debug(logTag, $"This service was already started, it's been running for {runtime:c}.");
            }
            else
            {
                startTime = DateTime.UtcNow;
                Log.Debug(logTag, $"Starting the service, at {startTime}.");
                timer = new Timer(HandleTimerCallback, startTime, 0, TimerWait);
                isStarted = true;
            }

            var notification = new Notification.Builder(this)
                .SetContentTitle("Sindar")
                .SetContentText("Ubicacíon")
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetOngoing(true)
                .Build();

            // Enlist this instance of the service as a foreground service
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
            return StartCommandResult.NotSticky;
        }

        void HandleTimerCallback(object state)
        {
            TimeSpan runTime = DateTime.UtcNow.Subtract(startTime);
            Log.Debug(logTag, $"This service has been running for {runTime:c} (since ${state}).");
        }
        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(logTag, "Client now bound to service");
            binder = new LocationServiceBinder(this);
            return binder;
        }
        // Handle location updates from the location manager
        public void StartLocationUpdates()
        {
            //we can set different location criteria based on requirements for our app -
            //for example, we might want to preserve power, or get extreme accuracy
            var locationCriteria = new Criteria();

            locationCriteria.Accuracy = Accuracy.NoRequirement;
            locationCriteria.PowerRequirement = Power.NoRequirement;

            // get provider: GPS, Network, etc.
            var locationProvider = LocMgr.GetBestProvider(locationCriteria, true);
            Log.Debug(logTag, string.Format("You are about to get location updates via {0}", locationProvider));

            // Get an initial fix on location
            LocMgr.RequestLocationUpdates(locationProvider, 2000, 0, this);

            Log.Debug(logTag, "Now sending location updates");
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            Log.Debug(logTag, "Service has been terminated");

            // Stop getting updates from the location manager:
            LocMgr.RemoveUpdates(this);
        }

        public void OnLocationChanged(Location location)
        {
            

            if (currentLocation == null)
            {
                currentLocation = location;
            }
            var valid = validateLocation(currentLocation, location);

            if (valid)
            {
                this.LocationChanged(this, new LocationChangedEventArgs(location));
                currentLocation = location;
                // This should be updating every time we request new location updates
                // both when teh app is in the background, and in the foreground
                Log.Debug(logTag, String.Format("Latitude is {0}", location.Latitude));
                Log.Debug(logTag, String.Format("Longitude is {0}", location.Longitude));
                Log.Debug(logTag, String.Format("Altitude is {0}", location.Altitude));
                Log.Debug(logTag, String.Format("Speed is {0}", location.Speed));
                Log.Debug(logTag, String.Format("Accuracy is {0}", location.Accuracy));
                Log.Debug(logTag, String.Format("Bearing is {0}", location.Bearing));

                syncService.SaveLocation(location);
            }


        }

        private bool validateLocation(Location c, Location n)
        {
            
            var sCoord = new GeoCoordinate(c.Latitude, c.Longitude);
            var eCoord = new GeoCoordinate(n.Latitude, n.Longitude);

            var distance = sCoord.GetDistanceTo(eCoord);
            var valid = true;
            if (minDist < distance)
            {
                valid = true;
            } else {
                valid = false;
                Log.Debug(logTag, "Discard");
            }
            return valid;
                
        }

        public void OnProviderDisabled(string provider)
        {
            this.ProviderDisabled(this, new ProviderDisabledEventArgs(provider));
        }

        public void OnProviderEnabled(string provider)
        {
            this.ProviderEnabled(this, new ProviderEnabledEventArgs(provider));
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            this.StatusChanged(this, new StatusChangedEventArgs(provider, status, extras));
        }
    }
}