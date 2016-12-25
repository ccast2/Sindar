using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace GPS.Activities
{
    [Activity(Label = "Login")]
    public class Login : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);

            // Create your application here
        }
    }
}