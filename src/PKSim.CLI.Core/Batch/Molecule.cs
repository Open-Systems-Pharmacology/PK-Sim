using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Batch
{
   internal abstract class Molecule : IWithName
   {
      public string Name { get; set; }
      public double ReferenceConcentration { get; set; }
      public double HalfLifeLiver { get; set; }
      public double HalfLifeIntestine { get; set; }
      public Dictionary<string, double> Expressions { get; set; }

      protected Molecule()
      {
         ReferenceConcentration = CoreConstants.DEFAULT_REFERENCE_CONCENTRATION_VALUE;
         HalfLifeLiver = CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_LIVER_VALUE_IN_MIN;
         HalfLifeIntestine = CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_INTESTINE_VALUE_IN_MIN;
         Expressions = new Dictionary<string, double>();
      }
   }

   internal class Enzyme : Molecule
   {
   }

   internal class OtherProtein : Molecule
   {
   }

   internal class Transporter : Molecule
   {
      public string TransportType { get; set; }

      public Transporter()
      {
         TransportType = OSPSuite.Core.Domain.TransportType.Efflux.ToString();
      }
   }
}