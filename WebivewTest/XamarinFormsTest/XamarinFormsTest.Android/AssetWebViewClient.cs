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

            bool isSingleParamaterUrlAtPoint(string str, int index, out string url, out string fullString)
            {
                url = null;
                fullString = null;
                if (isStringAtPoint(str, index, "url(\""))
                {
                    StringBuilder urlBuilder = new StringBuilder();
                    int k = 5;
                    while (!isStringAtPoint(str, index + k, "\")") && index + k < str.Length)
                    {
                        if (str[index + k] == ',')
                            return false;
                        k++;
                    }
                    if (!isStringAtPoint(str, index + k, "\")"))
                        return false;
                    url = str.Substring(index + 5, k - 5);
                    fullString = str.Substring(index, k + 2);
                    return true;
                }
                return false;
            }

            string ConvertToBase64(Stream strm)
            {
                byte[] bytes;
                using (var memoryStream = new MemoryStream())
                {
                    strm.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }

                return Convert.ToBase64String(bytes);
            }

            WebResourceResponse result;
            var path = request.Url.ToString();
            string mime;
            if (path.EndsWith(".css"))
                mime = "text/css";
            else
                mime = "application/x-javascript";
            path = path.Replace("file:///android_asset/", "Web/");
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
                if (isSingleParamaterUrlAtPoint(fileContents, i, out string url, out string chunk) && url.ToLower().EndsWith(".png"))
                {
                    var imagePath = Path.Combine("Web/Jquery", url);
                    string base64;
                    using (var stream = Android.App.Application.Context.Assets.Open(imagePath))
                    {
                        base64 = ConvertToBase64(stream);
                    }

                    newFileContents.Append(@"url(""data:image/png;base64,");
                    newFileContents.Append(base64);
                    newFileContents.Append(@""")");
                    i += chunk.Length;
                    continue;
                }
                newFileContents.Append(fileContents[i]);
                i++;
            }

            //fileContents.Replace("images/ui-icons", "Web/Jquery/images/ui-icons");
            MemoryStream newStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(newStream);
            writer.Write(newFileContents);
            writer.Flush();
            newStream.Position = 0;

            result = new WebResourceResponse(mime, "UTF-8", newStream);


            return result;
        }
    }
}