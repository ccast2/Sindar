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
using Java.Util.Concurrent;
using Java.Lang;
using System.Threading.Tasks;
using GPS.Models;

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
        }
        public override void OnDestroy()
        {
            base.OnDestroy();

            // Stop getting updates from the location manager:
            LocMgr.RemoveUpdates(this);
        }
        public override IBinder OnBind(Intent intent)
        {
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

            // Get an initial fix on location
            LocMgr.RequestLocationUpdates(locationProvider, 3000, 10, this);

        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (isStarted)
            {
                TimeSpan runtime = DateTime.UtcNow.Subtract(startTime);
            }
            else
            {
                startTime = DateTime.UtcNow;
                timer = new Timer(HandleTimerCallback, startTime, 0, TimerWait);
                isStarted = true;
            }

            var notification = new Notification.Builder(this)
                .SetContentTitle("GPS")
                .SetContentText("Ubicacíon")
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetOngoing(true)
                .Build();

            Task startupWork = new Task(() =>
            {
                TimerExampleState s = new TimerExampleState();

                TimerCallback timerDelegate = new TimerCallback(syncData);

                Timer timer = new Timer(timerDelegate, s, 1000, 3000);

                s.tmr = timer;
                

                while (s.tmr != null)
                    System.Threading.Thread.Sleep(0);
                
            });
            startupWork.Start();
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
            return StartCommandResult.NotSticky;
        }

        private void syncData(object state)
        {
            var sync = new SyncService();
            sync.pushLocations();
        }

        void HandleTimerCallback(object state)
        {
            TimeSpan runTime = DateTime.UtcNow.Subtract(startTime);
        }
        public void OnLocationChanged(Location location)
        {
            DatabaseService db = new DatabaseService();
            var tmp = new DeviceLocation();
            tmp.Latitude = location.Latitude;
            tmp.Longitude = location.Longitude;
            tmp.Accuracy = location.Accuracy;
            db.saveLocation(tmp);
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
class TimerExampleState
{
    public int counter = 0;
    public Timer tmr;
}

}