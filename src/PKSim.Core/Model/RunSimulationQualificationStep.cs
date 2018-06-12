using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class RunSimulationQualificationStep : IQualificationStep
   {
      public Simulation Simulation { get; set; }

      public string Display => PKSimConstants.QualificationSteps.RunSimulation(Simulation.Name);

   }
}