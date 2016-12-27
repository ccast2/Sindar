using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Android.Util;
using System.Threading.Tasks;
using GPS.Models;
using GPS.Services;
using Newtonsoft.Json;

namespace GPS.Activities
{
    [Activity(Label = "GPS", MainLauncher = true, Theme = "@style/MyTheme.Splash")]
    public class Splash : AppCompatActivity
    {
        private string TAG = "X:" + typeof(Splash).Name;
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            Log.Debug(TAG, "SplashActivity.OnCreate");
        }
        protected override async void OnResume()
        {
            base.OnResume();

            Log.Debug(TAG, "Performing some startup work that takes a bit of time.");

            Context mContext = Android.App.Application.Context;
            User currentUser = new User();
            currentUser.uContext = mContext;
            PreferencesService ap = new PreferencesService(mContext);
            string key = ap.getAccessKey();
            var response = await currentUser.getUser(key);

            if (currentUser.Id > 0)
            {
                var activity2 = new Intent(this, typeof(MainActivity));
                currentUser.uContext = null;
                activity2.PutExtra("User", JsonConvert.SerializeObject(currentUser));
                StartActivity(activity2);
            }
            else
            {
                StartActivity(new Intent(Application.Context, typeof(Login)));
            }
            Log.Debug(TAG, "Working in the background - important stuff.");
        }
    }
}