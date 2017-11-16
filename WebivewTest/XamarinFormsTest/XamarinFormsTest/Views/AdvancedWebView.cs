using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormsTest.Views
{
    class AdvancedWebView : WebView
    {
        public static BindableProperty EvaluateJavascriptProperty =
    BindableProperty.Create(nameof(EvaluateJavascript), typeof(Func<string, Task<string>>), typeof(AdvancedWebView), null, BindingMode.OneWayToSource);

        public Func<string, Task<string>> EvaluateJavascript
        {
            get { return (Func<string, Task<string>>)GetValue(EvaluateJavascriptProperty); }
            set { SetValue(EvaluateJavascriptProperty, value); }
        }

        public static BindableProperty LoadResourceProperty =
    BindableProperty.Create(nameof(LoadResource), typeof(Action<string>), typeof(AdvancedWebView), null, BindingMode.OneWayToSource);

        public Action<string> LoadResource
        {
            get { return (Action<string>)GetValue(LoadResourceProperty); }
            set { SetValue(LoadResourceProperty, value); }
        }
    }
}
