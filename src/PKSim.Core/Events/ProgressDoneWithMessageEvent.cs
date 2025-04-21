namespace PKSim.Core.Events
{
   public class ProgressDoneWithMessageEvent
   {
      public string Message { get; }

      public ProgressDoneWithMessageEvent(string message)
      {
         Message = message;
      }
   }
}