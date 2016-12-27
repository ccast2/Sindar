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
using System.Threading.Tasks;

namespace GPS.Models
{
    class User
    {
        public string auth;
        public string plate;
        public int Id;
        public Context uContext;
        protected string baseUrl = "/Users";

        internal async Task<bool> getUser(string key)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                var data = new[]{
                    new KeyValuePair<string, string>("default","1")
                 };
                var url = "/";
                RestService rest = new RestService();
                string result = await rest.SendAsJson(baseUrl + url, data);
                User tmpUser = new User();

                if (result != "")
                {
                    tmpUser = JsonConvert.DeserializeObject<User>(result);
                    this.Id = tmpUser.Id;
                    this.plate = tmpUser.plate;
                    this.auth = tmpUser.auth;
                    return true;
                }
                return false;

            }
            else {
                Toast.MakeText(uContext, "Por favor revise su conexion a internet", ToastLength.Short).Show();
                return false;
            };

        }

        internal async Task<bool> login(string userName, string password)
        {

            if (CrossConnectivity.Current.IsConnected)
            {
                var data = new[]{
                    new KeyValuePair<string, string>("Plate",userName),
                    new KeyValuePair<string, string>("Password",password)
                 };
                var url = "/login";
                RestService rest = new RestService();
                string result = await rest.SendAsJson(baseUrl + url, data);
                User tmpUser = new User();

                if (result != "")
                {
                    tmpUser = JsonConvert.DeserializeObject<User>(result);
                    this.Id = tmpUser.Id;
                    this.plate = tmpUser.plate;
                    this.auth = tmpUser.auth;
                    return true;
                }
                return false;

            }
            else
            {
                Toast.MakeText(uContext, "Por favor revise su conexion a internet", ToastLength.Short).Show();
                return false;
            };
        }
    }
}