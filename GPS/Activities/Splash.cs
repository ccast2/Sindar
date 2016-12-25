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
using Android.Support.V7.App;
using Android.Util;
using System.Threading.Tasks;

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
            //StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
        protected override void OnResume()
        {
            base.OnResume();

            Task startupWork = new Task(async () =>
            {
                Log.Debug(TAG, "Performing some startup work that takes a bit of time.");

                //Context mContext = Android.App.Application.Context;
                //AppPreferences ap = new AppPreferences(mContext);
                //string key = ap.getAccessKey();
                //session = new Session(key);
                //user = await session.ValidateKey();
                //if (user.Id > 0)
                //{
                //    StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                //}
                //else
                //{
                //    StartActivity(new Intent(Application.Context, typeof(Login)));
                //}

                Log.Debug(TAG, "Working in the background - important stuff.");
            });

            startupWork.ContinueWith(t => {
                Log.Debug(TAG, "Work is finished - start Activity1.");
                StartActivity(new Intent(Application.Context, typeof(Login)));

            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }
    }
}