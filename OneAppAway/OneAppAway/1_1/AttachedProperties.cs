using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Imaging;

namespace OneAppAway._1_1
{
    public static class AttachedProperties
    {
        //public static DependencyProperty ImageStreamSourceProperty = DependencyProperty.RegisterAttached("ImageStreamSource", typeof(object), typeof(MapIcon), new PropertyMetadata(null, OnImageStreamSourceChanged));

        //public static void OnImageStreamSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var icon = sender as MapIcon;
        //    if (icon == null)
        //        return;
        //    if (e.NewValue == null)
        //        icon.Image = null;
        //    else if (e.NewValue is Uri)
        //        icon.Image = RandomAccessStreamReference.CreateFromUri((Uri)e.NewValue);
        //    else if (e.NewValue is IStorageFile)
        //        icon.Image = RandomAccessStreamReference.CreateFromFile((IStorageFile)e.NewValue);
        //    else if (e.NewValue is IRandomAccessStream)
        //        icon.Image = RandomAccessStreamReference.CreateFromStream((IRandomAccessStream)e.NewValue);
        //    else
        //        icon.Image = null;
        //}

        //public static void SetImageStreamSource(MapIcon element, object value)
        //{
        //    element.SetValue(ImageStreamSourceProperty, value);
        //}

        //public static object GetImageStreamSource(MapIcon element)
        //{
        //    return element.GetValue(ImageStreamSourceProperty);
        //}

        public static readonly DependencyProperty ElementIDProperty = DependencyProperty.RegisterAttached("ElementID", typeof(string), typeof(MapElement), new PropertyMetadata(null));
        public static string GetElementID(MapElement element)
        {
            return (string)element.GetValue(ElementIDProperty);
        }
        public static void SetElementID(MapElement element, string value)
        {
            element.SetValue(ElementIDProperty, value);
        }

        public static readonly DependencyProperty ElementTypeProperty = DependencyProperty.RegisterAttached("ElementType", typeof(string), typeof(MapElement), new PropertyMetadata(null));
        public static string GetElementType(MapElement element)
        {
            return (string)element.GetValue(ElementTypeProperty);
        }
        public static void SetElementType(MapElement element, string value)
        {
            element.SetValue(ElementTypeProperty, value);
        }
    }
}
