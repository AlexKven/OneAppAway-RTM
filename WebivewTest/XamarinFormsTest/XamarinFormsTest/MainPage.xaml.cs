using Android.Content.Res;
using Android.Webkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebivewTest;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
#if __ANDROID__
using XamarinFormsTest.Droid;
#endif

namespace XamarinFormsTest
{
	public partial class MainPage : ContentPage
	{
        private Android.Webkit.WebView MainWebView;

        public MainPage()
		{
			InitializeComponent();
            //MainWebView.Source = "Web/TestPage.html";
            //MainWebView.EvaluateJavascript("t");
            //MainWebView.LoadResource("file:///android_asset/Web/TestPage.html");

#if __ANDROID__
            MainWebView = new Android.Webkit.WebView(Forms.Context);
            var formsView = MainWebView.ToView();
            Grid.SetRow(formsView, 1);
            MainGrid.Children.Add(formsView);
            MainWebView.SetWebChromeClient(new WebChromeClient());
            MainWebView.SetWebViewClient(new AssetWebViewClient());
            var s = MainWebView.Settings;
            s.AllowUniversalAccessFromFileURLs = true;
            s.AllowFileAccessFromFileURLs = true;
            s.DomStorageEnabled = true;
            s.JavaScriptEnabled = true;
            s.AllowFileAccess = true;
            s.JavaScriptCanOpenWindowsAutomatically = true;
            s.AllowContentAccess = true;
            

            AssetManager assets = Android.App.Application.Context.Assets;


            using (var htmlStream = assets.Open("DefaultArrivalsView.html"))
            {
                StreamReader sr = new StreamReader(htmlStream);
                //MainWebView.LoadData(sr.ReadToEnd(), "text/html", "utf-8");
                MainWebView.LoadDataWithBaseURL("file:///android_asset/", sr.ReadToEnd(), "text/html", "utf-8", null);
            }
#endif
        }

        int inc = 0;
        private void Button_Clicked(object sender, EventArgs e)
        {
            RealTimeArrival rta = new RealTimeArrival();
            rta.Route = "12345";
            rta.PrevRoute = "12344";
            rta.RouteName = "123";
            rta.PrevRouteName = "122";
            rta.Trip = (987654321 + inc++).ToString();
            rta.Stop = "12345";
            rta.ScheduledArrivalTime = DateTime.Now.AddMinutes(5);
            rta.PredictedArrivalTime = DateTime.Now.AddMinutes(7);
            rta.Vehicle = "12321";
            rta.Destination = "Prosperity";
            rta.DegreeOfConfidence = 0.7;
            rta.IsDropOffOnly = false;
            //MainWebView.LoadUrl($"javascript:{BuildJSFunctionCall("addArrival", GetParamsFromRTA(rta))}");
            MainWebView.EvaluateJavascript(BuildJSFunctionCall("addArrival", GetParamsFromRTA(rta)), null);
            
            //await MainWebView.InvokeScriptAsync("addArrival", GetParamsFromRTA(rta));
        }

        private string BuildJSFunctionCall(string name, params string[] parameters)
        {
            StringBuilder sb = new StringBuilder(name);
            sb.Append('(');
            for (int i = 0; i < parameters.Length; i++)
            {
                sb.Append(parameters[0]);
                if (i <= parameters.Length - 2)
                    sb.Append(',');
            }
            sb.Append(')');
            return sb.ToString();
        }

        private string[] GetParamsFromRTA(RealTimeArrival rta)
        {
            return new string[]
            {
                rta.Trip,
                rta.Stop,
                rta.Route,
                rta.PrevRoute,
                rta.RouteName,
                rta.PrevRouteName,
                rta.ScheduledArrivalTime?.ToString(),
                rta.PredictedArrivalTime?.ToString(),
                rta.Vehicle,
                rta.Destination,
                rta.FrequencyMinutes?.ToString(),
                rta.ScheduledVehicleLocation?.ToString(),
                rta.KnownVehicleLocation?.ToString(),
                rta.Orientation?.ToString(),
                rta.DegreeOfConfidence.ToString(),
                rta.IsDropOffOnly.ToString()
            }.Select(e => e ?? "").ToArray();
        }
    }
}
