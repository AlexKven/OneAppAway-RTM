using Android.App;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using System.IO;

namespace AndroidTest
{
    [Activity(Label = "AndroidTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            

            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);

            WebView webview = new WebView(this);
            SetContentView(webview);
            var wcc = new WebChromeClient();
            webview.SetWebChromeClient(new WebChromeClient());
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean)
            {
                var s = webview.Settings;
                s.AllowUniversalAccessFromFileURLs = true;
                s.AllowFileAccessFromFileURLs = true;
            }
            using (var htmlStream = Android.App.Application.Context.Assets.Open("Web/Jquery/index.html"))
            {
                StreamReader sr = new StreamReader(htmlStream);
                webview.LoadDataWithBaseURL("file:///android_asset/Web/Jquery/", sr.ReadToEnd(), "text/html", "utf-8", null);
                
            }
        }
    }
}

