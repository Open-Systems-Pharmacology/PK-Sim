namespace PKSim.Presentation.Events
{
   public class ShowNotificationEvent
   {
      public string Url { get; }
      public string Caption { get; }
      public string Notification { get; }

      public ShowNotificationEvent(string caption, string notification, string url = null)
      {
         Url = url;
         Caption = caption;
         Notification = notification;
      }
   }
}