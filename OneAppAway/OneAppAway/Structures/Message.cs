using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace OneAppAway
{
    public class Message
    {
        public int Id { get; set; }
        public string ShortSummary { get; set; }
        public string Caption { get; set; }
        public string FullText { get; set; }

        public static void ShowMessage(Message message)
        {
            if (SettingsManager.GetSetting("SuppressMessage" + message.Id, true, false))
                return;

            string title = message.Caption;
            string content = message.ShortSummary + " Tap for more...";
            string toastVisual = $@"<visual>
                          <binding template='ToastGeneric'>
                            <text>{title}</text>
                            <text>{content}</text>
                          </binding>
                        </visual>";
            string toastActions = $@"<actions>
                           <action activationType = 'foreground' arguments = 'dismiss' content = 'Ok' />
                           <action activationType = 'foreground' arguments = 'suppressMessage{message.Id.ToString()}' content = 'Stop Showing This' />
                         </actions>";
            string toastXmlString = $@"<toast activationType='foreground' launch='messageTapped{message.Id.ToString()}'>
                             {toastVisual}
                             {toastActions}
                           </toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(toastXmlString);
            var toast = new ToastNotification(toastXml);
            toast.Tag = "message" + message.Id.ToString();
            toast.Group = "messages";
            SettingsManager.SetSetting("Message" + message.Id, false, message.FullText);
            List<int> messages = SettingsManager.GetSetting("Message", false, new int[0]).ToList();
            if (!messages.Contains(message.Id))
            {
                messages.Add(message.Id);
                SettingsManager.SetSetting("Messages", false, messages.ToArray());
            }
                    
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
