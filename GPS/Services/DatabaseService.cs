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
using SQLite;
using System.IO;
using GPS.Models;
using Android.Util;

namespace GPS.Services
{
    class DatabaseService
    {
        private SQLiteConnection conn;
        String path = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
            "mision1a.db3");
        private readonly string TAG = "X:" + typeof(DatabaseService).Name;


        public DatabaseService()
        {
            conn = new SQLiteConnection(path);
            conn.CreateTable<DeviceLocation>();
        }

        public void saveLocation(DeviceLocation location)
        {
            var Insert = new DeviceLocation();
            Insert.Longitude = location.Longitude;
            Insert.Latitude = location.Latitude;
            Insert.Accuracy = location.Accuracy;
            Insert.SavedDate = DateTime.Now;
            Insert.Processed = false;
            Insert.Id = Guid.NewGuid();
            conn.Insert(Insert);

            Log.Debug(TAG, "Ubicación guardada: " + Insert.Id);
        }

        public IEnumerable<DeviceLocation> getLocations()
        {
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

        internal void deleteLocations(List<DeviceLocation> list)
        {
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.Processed)
                    {
                        Log.Debug(TAG, "Eliminado: " + item.Id);
                        conn.Execute("delete from DeviceLocation where Id='" + item.Id + "'");
                    }

                }
            }
        }
    }
}