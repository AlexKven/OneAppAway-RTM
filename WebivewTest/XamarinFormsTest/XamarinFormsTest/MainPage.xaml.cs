using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormsTest
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
            MainWebView.Source = "Web/TestPage.html";
            MainWebView.EvaluateJavascript("t");
            MainWebView.LoadResource("file:///Assets/Web/TestPage.html");
            //""
		}
    }
}
