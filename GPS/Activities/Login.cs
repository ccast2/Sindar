using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using GPS.Models;
using Android.Content;
using Newtonsoft.Json;
using GPS.Services;

namespace GPS.Activities
{
    [Activity(Label = "Login")]
    public class Login : AppCompatActivity
    {
        private Button send;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);

            send = FindViewById<Button>(Resource.Id.LoginButton);
            send.Click += ValidateCredentials;
        }

        private async void ValidateCredentials(object sender, EventArgs e)
        {
            var progressDialog = ProgressDialog.Show(this, "Espere por favor...", "Validando...", true);
            var userName = FindViewById<EditText>(Resource.Id.UserName);
            var password = FindViewById<EditText>(Resource.Id.Password);

            if (String.IsNullOrEmpty(userName.Text.ToString()) || String.IsNullOrEmpty(password.Text.ToString()))
            {
                Toast.MakeText(this, "La placa y contraseña son obligatorias", ToastLength.Short).Show();
            }
            else
            {
                User user = new User();
                Context mContext = Android.App.Application.Context;
                user.uContext = mContext;
                var response = await user.login(userName.Text.ToString(), password.Text.ToString());
                if (user.Id > 0)
                {
                    PreferencesService ap = new PreferencesService(mContext);
                    ap.saveValue(user.auth);
                    var activity2 = new Intent(this, typeof(MainActivity));
                    user.uContext = null;
                    activity2.PutExtra("User", JsonConvert.SerializeObject(user));
                    StartActivity(activity2);
                }
            }
            progressDialog.Hide();
        }
    }
}