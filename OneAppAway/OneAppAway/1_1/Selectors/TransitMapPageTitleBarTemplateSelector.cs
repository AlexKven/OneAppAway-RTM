using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using static System.Math;

namespace OneAppAway._1_1.Selectors
{
    public class TransitMapPageTitleBarTemplateSelector : TitleBarTemplateSelectorBase
    {
        public TransitMapPageTitleBarTemplateSelector()
        {
            TitleTemplate = App.Current.Resources["SimpleTitleTemplate"] as DataTemplate;
            ControlsTemplate = CreateTitleTemplate(oldLarge);
            OverflowControlsTemplate = CreateOverflowTemplate(oldLarge);
        }
        private bool oldLarge = false;

        public override void ReceiveAvailableSize(double size, bool onMobile)
        {
            bool large = onMobile ? size >= 400 : size >= 650;
            ControlsWidth = onMobile ? size : Min(large ? 550 : 300, size);
            TitleWidth = size - ControlsWidth;
            if (large != oldLarge)
            {
                ControlsTemplate = CreateTitleTemplate(large);
                OverflowControlsTemplate = CreateOverflowTemplate(large);
            }
            oldLarge = large;
        }

        private DataTemplate CreateTitleTemplate(bool large)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" > ");
            builder.AppendLine("<Grid>");
            builder.AppendLine("<Grid.ColumnDefinitions>");
            builder.AppendLine("<ColumnDefinition Width=\"*\"/>");
            if (large)
                builder.AppendLine("<ColumnDefinition Width=\"Auto\"/>");
            builder.AppendLine("</Grid.ColumnDefinitions>");
            builder.AppendLine("<TextBox PlaceholderText=\"Search Stops\" VerticalAlignment=\"Center\" Margin=\"0,0,5,0\"/>");
            if (large)
            {
                builder.AppendLine("<CommandBar RequestedTheme=\"Dark\" Background=\"#303030\" Grid.Column=\"1\" BorderBrush=\"#303030\">");
                builder.AppendLine("<AppBarButton Icon=\"MapPin\" Label=\"Current Location\" Click=\"CurrentLocationButton_Click\"/>");
                builder.AppendLine("<AppBarButton Icon=\"ZoomIn\" Label=\"Zoom In\" Click=\"ZoomInButton_Click\" ClickMode=\"Press\"/>");
                builder.AppendLine("<AppBarButton Icon=\"ZoomOut\" Label=\"Zoom Out\" Click=\"ZoomOutButton_Click\" ClickMode=\"Press\"/>");
                builder.AppendLine("<AppBarButton Label=\"Find Stops\" Click=\"RefreshButton_Click\">");
                builder.AppendLine("<AppBarButton.Icon>");
                builder.AppendLine("<BitmapIcon UriSource=\"/Assets/Icons/RefreshStopsIcon.png\"/>");
                builder.AppendLine("</AppBarButton.Icon>");
                builder.AppendLine("</AppBarButton>");
                builder.AppendLine("</CommandBar>");
            }
            builder.AppendLine("</Grid>");
            builder.AppendLine("</DataTemplate>");
            return XamlReader.Load(builder.ToString()) as DataTemplate;
        }

        private DataTemplate CreateOverflowTemplate(bool large)
        {
            if (large)
                return null;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" > ");
            builder.AppendLine("<Grid>");
            builder.AppendLine("<CommandBar RequestedTheme=\"Dark\" Background=\"#303030\" Grid.Column=\"1\" BorderBrush=\"#303030\">");
            builder.AppendLine("<AppBarButton Icon=\"MapPin\" Label=\"Current Location\" Click=\"CurrentLocationButton_Click\"/>");
            builder.AppendLine("<AppBarButton Icon=\"ZoomIn\" Label=\"Zoom In\" Click=\"ZoomInButton_Click\" ClickMode=\"Press\"/>");
            builder.AppendLine("<AppBarButton Icon=\"ZoomOut\" Label=\"Zoom Out\" Click=\"ZoomOutButton_Click\" ClickMode=\"Press\"/>");
            builder.AppendLine("<AppBarButton Label=\"Find Stops\" Click=\"RefreshButton_Click\">");
            builder.AppendLine("<AppBarButton.Icon>");
            builder.AppendLine("<BitmapIcon UriSource=\"/Assets/Icons/RefreshStopsIcon.png\"/>");
            builder.AppendLine("</AppBarButton.Icon>");
            builder.AppendLine("</AppBarButton>");
            builder.AppendLine("</CommandBar>");
            builder.AppendLine("</Grid>");
            builder.AppendLine("</DataTemplate>");
            return XamlReader.Load(builder.ToString()) as DataTemplate;
        }
    }
}
