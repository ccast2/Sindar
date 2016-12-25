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
using System.Threading;
using Android.Util;

namespace GPS.Services
{
    [Service(Name = "com.xamarin.example.LocationService")]
    class LocationService : Service, ILocationListener
    {
        public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };
        public event EventHandler<ProviderDisabledEventArgs> ProviderDisabled = delegate { };
        public event EventHandler<ProviderEnabledEventArgs> ProviderEnabled = delegate { };
        public event EventHandler<StatusChangedEventArgs> StatusChanged = delegate { };

        protected LocationManager LocMgr = Android.App.Application.Context.GetSystemService("location") as LocationManager;


        readonly string TAG = "X:" + typeof(LocationService).Name;
        IBinder binder;
        static readonly int TimerWait = 4000;
        Timer timer;
        DateTime startTime;
        bool isStarted = false;
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug(TAG, "OnCreate called in the Location Service");
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            Log.Debug(TAG, "Service has been terminated");

            // Stop getting updates from the location manager:
            LocMgr.RemoveUpdates(this);
        }
        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(TAG, "Client now bound to service");
            binder = new LocationServiceBinder(this);
            return binder;
        }
        public void StartLocationUpdates()
        {
            //we can set different location criteria based on requirements for our app -
            //for example, we might want to preserve power, or get extreme accuracy
            var locationCriteria = new Criteria();

            locationCriteria.Accuracy = Accuracy.NoRequirement;
            locationCriteria.PowerRequirement = Power.NoRequirement;

            // get provider: GPS, Network, etc.
            var locationProvider = LocMgr.GetBestProvider(locationCriteria, true);
            Log.Debug(TAG, string.Format("You are about to get location updates via {0}", locationProvider));

            // Get an initial fix on location
            LocMgr.RequestLocationUpdates(locationProvider, 3000, 10, this);

            Log.Debug(TAG, "Now sending location updates");
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug(TAG, $"OnStartCommand called at {startTime}, flags={flags}, startid={startId}");
            if (isStarted)
            {
                TimeSpan runtime = DateTime.UtcNow.Subtract(startTime);
                Log.Debug(TAG, $"This service was already started, it's been running for {runtime:c}.");
            }
            else
            {
                startTime = DateTime.UtcNow;
                Log.Debug(TAG, $"Starting the service, at {startTime}.");
                timer = new Timer(HandleTimerCallback, startTime, 0, TimerWait);
                isStarted = true;
            }

            var notification = new Notification.Builder(this)
                .SetContentTitle("GPS")
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
            Log.Debug(TAG, $"This service has been running for {runTime:c} (since ${state}).");
        }
        public void OnLocationChanged(Location location)
        {
            Log.Debug(TAG, "Cambio ubicacion");
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