using Android.App;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Sindar.Services;
using System;
using Android.Util;
using Sindar.Models;
using Sindar.Adapters;
using System.Collections.Generic;
using System.Linq;
using Android.Views;

using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Java.IO;
using Android.Content;
using Android.Provider;
using Android.Content.PM;
using Uri = Android.Net.Uri;
using Environment = Android.OS.Environment;
using Android.Graphics;

namespace Sindar
{
    [Activity(Label = "Sindar", Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        private string TAG = "X:" + typeof(MainActivity).Name;
        readonly string logTag = "MainActivity";
        private IEnumerable<DeviceLocation> savedLocations;
        private static Location currentLocation;
        public SyncService syncService = new SyncService();
        public ArrayAdapter<string> ListAdapter { get; private set; }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var editToolbar = FindViewById<Toolbar>(Resource.Id.topToolbar);
            editToolbar.Title = "Editing";
            editToolbar.InflateMenu(Resource.Menu.first_menu);
            editToolbar.MenuItemClick += (sender, e) =>
            {
                
                switch (e.Item.TitleFormatted.ToString())
                {
                    case "Nuevo":
                        break;
                    case "Configuración":
                        break;
                    default:
                        break;
                }

            };

            var editToolbar2 = FindViewById<Toolbar>(Resource.Id.middleToolbar);
            editToolbar2.Title = "Editing";
            editToolbar2.InflateMenu(Resource.Menu.edit_menus);
            editToolbar2.MenuItemClick += (sender, e) =>
            {
                Toast.MakeText(this, "Bottom toolbar tapped: " + e.Item.TitleFormatted, ToastLength.Short).Show();
            };




            App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
            {
                Log.Debug(logTag, "ServiceConnected Event Raised");
                App.Current.LocationService.LocationChanged += HandleLocationChanged;
                App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
                App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
                App.Current.LocationService.StatusChanged += HandleStatusChanged;
            };
            App.StartLocationService();
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                //Button button = FindViewById<Button>(Resource.Id.newPicture);
                //_imageView = FindViewById<ImageView>(Resource.Id.imageView1);
                //button.Click += TakeAPicture;
            }
            ListView listView = FindViewById<ListView>(Resource.Id.myListView);
            listView.ItemClick += OnListItemClick;
            List<Post> listData = new List<Post>();
            Post post1 = new Post();
            post1.title = "1";
            post1.description = "2";
            post1.url = "3";
            listData.Add(post1);
            listView.Adapter = new CustomListAdapter(this, listData);
            ListView listView2 = FindViewById<ListView>(Resource.Id.myListView2);
            listView2.ItemClick += OnListItemClick;
            List<Post> listData2 = new List<Post>();
            Post post2 = new Post();
            post2.title = "4";
            post2.description = "5";
            post2.url = "6";
            listData2.Add(post2);
            listView2.Adapter = new CustomListAdapter(this, listData2);
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //Post item = result.posts.ElementAt(e.Position);
            // Do whatever you like here
        }

        private void TakeAPicture(object sender, EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            AppCamera._file = new File(AppCamera._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(AppCamera._file));
            StartActivityForResult(intent, 0);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }

        private void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Log.Debug(logTag, "Location status changed, event raised");
        }

        private void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {
            Log.Debug(logTag, "Location provider enabled event raised");
        }

        private void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {
            Log.Debug(logTag, "Location provider disabled event raised");
        }

        private void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            currentLocation = e.Location;
            Log.Debug(logTag, "Foreground updating");
        }

        private async void syncAllLocations(object sender, EventArgs e)
        {
           
            var items = syncService.getLocations();
            RestClient Rest = new RestClient();
            savedLocations = await Rest.UpdateDeviceLocations(items);
            syncService.deleteLocations(savedLocations.ToList());           

        }

        private void CreateDirectoryForPictures ()
        {
            AppCamera._dir = new File (
                Environment.GetExternalStoragePublicDirectory (
                    Environment.DirectoryPictures), "CameraAppDemo");
            if (!AppCamera._dir.Exists ())
            {
                AppCamera._dir.Mkdirs( );
            }
        }

        private bool IsThereAnAppToTakePictures ()
        {
            Intent intent = new Intent (MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities (intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);


            Log.Debug(logTag, "entro");

            //// make it available in the gallery

            //intent mediascanintent = new intent(intent.actionmediascannerscanfile);
            //uri contenturi = uri.fromfile(appcamera._file);
            //mediascanintent.setdata(contenturi);
            //sendbroadcast(mediascanintent);

            //// display in imageview. we will resize the bitmap to fit the display.
            //// loading the full sized image will consume to much memory
            //// and cause the application to crash.

            //int height = resources.displaymetrics.heightpixels;
            //int width = _imageview.height;
            //appcamera.bitmap = appcamera._file.path.loadandresizebitmap(width, height);
            //if (appcamera.bitmap != null)
            //{
            //    _imageview.setimagebitmap(appcamera.bitmap);
            //    appcamera.bitmap = null;
            //}

            //// dispose of the java side bitmap.
            //gc.collect();
        }

    }

    internal class AppCamera
    {
        internal static File _dir;
        internal static File _file;

        public static Bitmap bitmap { get; internal set; }
    }
}

