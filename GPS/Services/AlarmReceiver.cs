using Android.Content;
using Android.Util;

namespace GPS.Services
{
    [BroadcastReceiver(Name = "com.xamarin.example.AlarmService")]
    class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Debug("Alarm", "Every 3 seconds");
        }
    }
}