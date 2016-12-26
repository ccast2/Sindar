using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Android.Util;
using System.Threading.Tasks;
using GPS.Models;

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
        protected override void OnResume()
        {
            base.OnResume();

            Log.Debug(TAG, "Performing some startup work that takes a bit of time.");

            Context mContext = Android.App.Application.Context;
            User currentUser = new User();
            currentUser.uContext = mContext;
            currentUser.getUser();

            if (currentUser.Id > 0)
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }
            else
            {
                StartActivity(new Intent(Application.Context, typeof(Login)));
            }
            Log.Debug(TAG, "Working in the background - important stuff.");
        }
    }
}