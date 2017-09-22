namespace PKSim.Core.Snapshots
{
   public abstract class ClassificationSnapshotBase<TClassification, TClassifiable> : SnapshotBase
   {
      public string ClassificationType { get; set; }

      public TClassification[] Classifications { set; get; }
      public TClassifiable[] Classifiables { get; set; }
   }
}