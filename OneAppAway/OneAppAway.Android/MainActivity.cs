using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content.PM;

namespace OneAppAway.Android
{
    [Activity(Label = "OneAppAway.Android", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Holo.Light.NoActionBar")]
    public class MainActivity : Activity
    {
        private Button button1;

        protected override void OnCreate(Bundle bundle)
        {
            //// Remove title bar
            //RequestWindowFeature(WindowFeatures.NoTitle);

            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            button1 = (Button)this.FindViewById(Resource.Id.button1);
        }

        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater inflator = MenuInflater;
        //    inflator.Inflate(Resource.Menu.MainMenu, menu);
        //    return false;
        //}
    }
}

