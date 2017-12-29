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

namespace XamarinFormsTest
{
	public partial class MainPage : ContentPage
	{
        private Android.Webkit.WebView MainWebView;

#if __ANDROID__
        class AssetWebViewClient : WebViewClient
        {
            public override WebResourceResponse ShouldInterceptRequest(Android.Webkit.WebView view, IWebResourceRequest request)
            {
                bool isStringAtPoint(string mainString, int index, string search)
                {
                    for (int k = 0; k < search.Length; k++)
                    {
                        if (index + k >= mainString.Length)
                            return false;
                        if (mainString[index + k] != search[k])
                            return false;
                    }
                    return true;
                }

                WebResourceResponse result;
                var path = request.Url.ToString();
                string mime;
                if (path.EndsWith(".css"))
                    mime = "text/css";
                else
                    mime = "application/x-javascript";
                path = path.Replace("file:///android_asset/Web", "Web/Jquery");
                string fileContents;
                using (var stream = Android.App.Application.Context.Assets.Open(path))
                {
                    StreamReader sr = new StreamReader(stream);
                    fileContents = sr.ReadToEnd();
                }

                StringBuilder newFileContents = new StringBuilder();
                
                int i = 0;
                while (i < fileContents.Length)
                {
                    if (isStringAtPoint(fileContents, i, "url(\""))
                    {
                        StringBuilder urlPath = new StringBuilder();
                        while (!isStringAtPoint(fileContents, i, "\")") && i < fileContents.Length)
                        {

                        }
                    }
                    newFileContents.Append(fileContents[i]);
                    i++;
                }

                //fileContents.Replace("images/ui-icons", "Web/Jquery/images/ui-icons");
                MemoryStream newStream = new MemoryStream();
                StreamWriter writer = new StreamWriter(newStream);
                writer.Write(fileContents);
                writer.Flush();
                newStream.Position = 0;

                result = new WebResourceResponse(mime, "UTF-8", newStream);
                

                return result;
            }
        }
#endif

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

            var about = assets.Open("AboutAssets.txt");

            using (var htmlStream = assets.Open("Web/Jquery/index.html"))
            {
                StreamReader sr = new StreamReader(htmlStream);
                MainWebView.LoadDataWithBaseURL("file:///android_asset/Web/Jquery", sr.ReadToEnd(), "text/html", "utf-8", null);
            }
#endif
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            RealTimeArrival rta = new RealTimeArrival();
            rta.Route = "12345";
            rta.PrevRoute = "12344";
            rta.RouteName = "123";
            rta.PrevRouteName = "122";
            rta.Trip = "987654321";
            rta.Stop = "12345";
            rta.ScheduledArrivalTime = DateTime.Now.AddMinutes(5);
            rta.PredictedArrivalTime = DateTime.Now.AddMinutes(7);
            rta.Vehicle = "12321";
            rta.Destination = "Prosperity";
            rta.DegreeOfConfidence = 0.7;
            rta.IsDropOffOnly = false;
            MainWebView.LoadUrl($"javascript:{BuildJSFunctionCall("addArrival", GetParamsFromRTA(rta))}");
            //MainWebView.EvaluateJavascript(BuildJSFunctionCall("addArrival", GetParamsFromRTA(rta)), null);
            
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
