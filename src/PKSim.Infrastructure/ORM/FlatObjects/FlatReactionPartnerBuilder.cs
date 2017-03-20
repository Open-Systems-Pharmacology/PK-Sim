namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatReactionPartner
   {
      public string Reaction { get; set; }
      public string Molecule { get; set; }
      public string Direction { get; set; }
      public double StoichCoeff { get; set; }
   }
}
