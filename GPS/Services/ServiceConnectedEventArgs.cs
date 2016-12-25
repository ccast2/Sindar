using System;
using Android.OS;


namespace GPS.Services
{
    class ServiceConnectedEventArgs : EventArgs
    {
        public IBinder Binder { get; set; }
    }
}