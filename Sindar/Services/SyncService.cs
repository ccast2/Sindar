using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using SQLite;
using Sindar.Models;
using Android.Util;

namespace Sindar.Services
{
    public class SyncService
    {
        private SQLiteConnection conn;
        String path = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
            "baseSindar.db3");
        private readonly string TAG = "X:" + typeof(MainActivity).Name;

        public SyncService()
        {
            conn = new SQLiteConnection(path);
            conn.CreateTable<DeviceLocation>();
        }
        internal void SaveLocation(Location location)
        {
            var Insert = new DeviceLocation();
            Insert.Longitude = location.Longitude;
            Insert.Latitude = location.Latitude;
            Insert.Accuracy = location.Accuracy;
            Insert.SavedDate = DateTime.Now;
            Insert.Saved = false;
            Insert.Id = Guid.NewGuid();
            conn.Insert(Insert);
        }
        public IEnumerable<DeviceLocation> getLocations() {
            IEnumerable<DeviceLocation> locations;
            locations = new DeviceLocation[] { };
            var localLocations = from s in conn.Table<DeviceLocation>()
                                 select s;
            List<DeviceLocation> list = locations.ToList();
            foreach (var fila in localLocations)
            {
                DeviceLocation dev = new DeviceLocation();
                dev.Latitude = fila.Latitude;
                dev.Longitude = fila.Longitude;
                dev.Accuracy = fila.Accuracy;
                dev.SavedDate = fila.SavedDate;
                dev.Id = fila.Id;

                list.Add(dev);
            }
            var items = (IEnumerable<DeviceLocation>)list;
            return items;
        }

        public void deleteLocations(List<DeviceLocation> listSaved) {
            if (listSaved.Count > 0)
            {
                foreach (var item in listSaved)
                {
                    if (item.Saved)
                    {
                        Log.Debug(TAG, "Eliminado: " + item.Id);
                        conn.Execute("delete from DeviceLocation where Id='" + item.Id + "'");
                    }

                }
            }
        }
    }
}