using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class ClassifiableSimulation : Classifiable<Simulation>
   {
      public ClassifiableSimulation() : base(ClassificationType.Simulation)
      {
      }

      public Simulation Simulation
      {
         get { return Subject; }
      }
   }
}