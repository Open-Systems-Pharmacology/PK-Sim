namespace PKSim.Core.Snapshots
{
   public class ObservedDataCollection
   {
      public string[] ObservedData { get; set; }
      public bool ApplyGrouping { get; set; }
      public ObservedDataCurveOptions[] CurveOptions { get; set; }
   }
}