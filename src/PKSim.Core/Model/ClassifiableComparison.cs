using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class ClassifiableComparison : Classifiable<ISimulationComparison>
   {
      public ClassifiableComparison() : base(ClassificationType.Comparison)
      {
      }

      public ISimulationComparison Comparison => Subject;
   }
}