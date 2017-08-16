namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatSpecies : FlatObject
   {
      public bool IsUserDefined { get; set; }
      public int Sequence { get; set; }
      public string IconName { get; set; }
      public bool IsHuman { get; set; }
   }
}