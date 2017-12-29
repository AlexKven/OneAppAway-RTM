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
using Android.Webkit;
using System.IO;

namespace XamarinFormsTest.Droid
{
    public class AssetWebViewClient : WebViewClient
    {
        public override WebResourceResponse ShouldInterceptRequest(Android.Webkit.WebView view, IWebResourceRequest request)
        {
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

            fileContents.Replace("images/ui-icons", "file:///android_assets/Web/Jquery/images/ui-icons/");
            MemoryStream newStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(newStream);
            writer.Write(fileContents);
            writer.Flush();
            newStream.Position = 0;

            result = new WebResourceResponse(mime, "UTF-8", newStream);


            return result;
        }
    }
}