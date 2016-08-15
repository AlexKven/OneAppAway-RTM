using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneAppAway._1_1.Data;
using Windows.Devices.Geolocation;
using OneAppAway._1_1.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;

namespace OneAppAway._1_1
{
    public static class Helpers
    {
        public static BasicGeoposition ToBasicGeoposition(this LatLon value)
        {
            return new BasicGeoposition() { Latitude = value.Latitude, Longitude = value.Longitude };
        }

        public static Geopoint ToGeopoint(this LatLon value)
        {
            return new Geopoint(value.ToBasicGeoposition());
        }

        public static LatLon ToLatLon(this BasicGeoposition value)
        {
            return new LatLon(value.Latitude, value.Longitude);
        }

        public static LatLon ToLatLon(this Geopoint value)
        {
            return value.Position.ToLatLon();
        }

        public static void BindToControl(this LightweightViewModelBase vm, FrameworkElement control, DependencyProperty propertyTo, string propertyFrom, bool twoWay = false, IValueConverter converter = null, object converterParam = null)
        {
            control.SetBinding(propertyTo, new Binding() { Source = vm, Path = new PropertyPath(propertyFrom), Converter = converter, ConverterParameter = converterParam, Mode = twoWay ? BindingMode.TwoWay : BindingMode.OneWay });
        }

        public static Task BeginAsync(this Storyboard storyboard)
        {
            System.Threading.Tasks.TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            if (storyboard == null)
                tcs.SetException(new ArgumentNullException());
            else
            {
                EventHandler<object> onComplete = null;
                onComplete = (s, e) => {
                    storyboard.Completed -= onComplete;
                    tcs.SetResult(true);
                };
                storyboard.Completed += onComplete;
                storyboard.Begin();
            }
            return tcs.Task;
        }
    }
}
