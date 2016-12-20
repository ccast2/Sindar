using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sindar.Models;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

namespace Sindar.Activities
{
    [Activity(Label = "Login", Theme = "@android:style/Theme.Material.Light")]
    public class Login : AppCompatActivity
    {
        Session session;
        Context context = Android.App.Application.Context;
        private EditText userName;
        private Switch rememberMe;
        private EditText password;
        private Button send;
        private bool remember = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);
            userName = FindViewById<EditText>(Resource.Id.UserName);
            password = FindViewById<EditText>(Resource.Id.Password);
            rememberMe = FindViewById<Switch>(Resource.Id.RememberMe);
            send = FindViewById<Button>(Resource.Id.LoginButton);
            send.Click += ValidateCredentials;
        }

        private async void ValidateCredentials(object sender, EventArgs e)
        {
            var progressDialog = ProgressDialog.Show(this, "Espere por favor...", "Validando...", true);

            if (String.IsNullOrEmpty(userName.Text.ToString()) || String.IsNullOrEmpty(password.Text.ToString()))
            {
                Toast.MakeText(this, "La placa y contraseña son obligatorias", ToastLength.Short).Show();
            }
            else
            {
                session = new Session("");
                User user = new Models.User();


                user = await session.Login(userName.Text.ToString(), password.Text.ToString());

                if (user.Id > 0)
                {
                    Context mContext = Android.App.Application.Context;
                    AppPreferences ap = new AppPreferences(mContext);
                    ap.saveValue(user.Auth);
                    ap.saveValue(remember);
                    StartActivity(typeof(MainActivity));

                }
                else
                {
                    Toast.MakeText(this, "Placa o contraseña incorrecta", ToastLength.Short).Show();
                }
            }
            progressDialog.Hide();
        }
    }
}