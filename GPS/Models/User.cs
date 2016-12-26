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
using Plugin.Connectivity;
using GPS.Services;
using Newtonsoft.Json;

namespace GPS.Models
{
    class User
    {
        private string auth;
        public string plate;
        public int Id;
        public Context uContext;

        public void getUser()
        {
            if (CrossConnectivity.Current.IsConnected)
            {


            }
            else {
                Toast.MakeText(uContext, "Por favor revise su conexion a internet", ToastLength.Short).Show();
            };
        }

        internal async void login(string userName, string password)
        {

            if (CrossConnectivity.Current.IsConnected)
            {
                var data = new[]{
                new KeyValuePair<string, string>("Plate",userName),
                new KeyValuePair<string, string>("Password",password)
             };
                var url = "login";
                RestService rest = new RestService();
                string result = await rest.SendAsJson(url, data);
                User tmpUser = new User();

                if (result != "")
                {
                    tmpUser = JsonConvert.DeserializeObject<User>(result);
                    this.Id = tmpUser.Id;
                    this.plate = tmpUser.plate;
                    this.auth = tmpUser.auth;
                }

            }
            else
            {
                Toast.MakeText(uContext, "Por favor revise su conexion a internet", ToastLength.Short).Show();
            };
        }
    }
}