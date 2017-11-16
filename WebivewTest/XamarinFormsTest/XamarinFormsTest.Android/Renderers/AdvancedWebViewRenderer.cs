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
using Xamarin.Forms.Platform.Android;
using XamarinFormsTest.Views;
using System.Threading;
using Xamarin.Forms;
using System.Threading.Tasks;
using Android.Webkit;
using XamarinFormsTest.Droid.Renderers;

[assembly: ExportRenderer(typeof(AdvancedWebViewRenderer), typeof(AdvancedWebView))]
namespace XamarinFormsTest.Droid.Renderers
{
    public class AdvancedWebViewRenderer : WebViewRenderer
    {
        

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);
            
            var webView = e.NewElement as AdvancedWebView;
            if (webView != null)
            {
                webView.EvaluateJavascript = async (js) =>
                {
                    var reset = new ManualResetEvent(false);
                    var response = string.Empty;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Control?.EvaluateJavascript(js, new JavascriptCallback((r) => { response = r; reset.Set(); }));
                    });
                    await Task.Run(() => { reset.WaitOne(); });
                    return response;

                };
                webView.LoadResource = (url) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Control?.LoadUrl(url);
                    });
                };
            }
        }
    }

    internal class JavascriptCallback : Java.Lang.Object, IValueCallback
    {
        public JavascriptCallback(Action<string> callback)
        {
            _callback = callback;
        }

        private Action<string> _callback;
        public void OnReceiveValue(Java.Lang.Object value)
        {
            _callback?.Invoke(Convert.ToString(value));
        }
    }

}