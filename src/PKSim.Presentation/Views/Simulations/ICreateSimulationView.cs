using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ICreateSimulationView : ISimulationWizardView
   {
      void BindToProperties(ObjectBaseDTO simulationPropertiesDTO);
   }

   public interface ICloneSimulationView : ICreateSimulationView
   {
   }
}