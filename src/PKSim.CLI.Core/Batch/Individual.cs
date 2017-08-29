using System.Collections.Generic;

namespace PKSim.Core.Batch
{
   internal class Individual
   {
      public string Species { get; set; }
      public string Population { get; set; }
      public string Gender { get; set; }
      public double? Age { get; set; }
      public double? GestationalAge { get; set; }
      public double? Weight { get; set; }
      public double? Height { get; set; }
      public int Seed { get; set; }

      public List<Enzyme> Enzymes { get; set; }
      public List<OtherProtein> OtherProteins { get; set; }
      public List<Transporter> Transporters { get; set; }

      public Individual()
      {
         Enzymes = new List<Enzyme>();
         OtherProteins = new List<OtherProtein>();
         Transporters = new List<Transporter>();
         Seed = 123456;
      }
   }
}