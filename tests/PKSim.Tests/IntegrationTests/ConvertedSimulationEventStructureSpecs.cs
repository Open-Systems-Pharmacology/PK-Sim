using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_converted_simulation_event_structure : ContextWithLoadedProject<Simulation>
   {
      protected Simulation _simulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         //a project saved before applications were nested under a formulation container, containing an IV protocol
         LoadProject("SimpleSimulationIV_710");
      }

      protected override void Context()
      {
         base.Context();
         _simulation = FindByName<Simulation>("Human");
      }

      protected IContainer ApplicationContainerIn(Simulation simulation) =>
         simulation.Model.Root.GetAllChildren<IContainer>(x => x.ContainerType == ContainerType.Application).Single();
   }

   public class When_loading_a_project_saved_before_applications_were_nested_under_a_formulation_container : concern_for_converted_simulation_event_structure
   {
      [Observation]
      public void the_application_of_a_protocol_without_formulation_should_be_nested_under_a_formulation_container()
      {
         var application = ApplicationContainerIn(_simulation);

         application.ParentContainer.Name.ShouldBeEqualTo(CoreConstants.ContainerName.NoFormulation);
         application.ParentContainer.ContainerType.ShouldBeEqualTo(ContainerType.Formulation);
      }

      [Observation]
      public void the_formulation_container_should_be_nested_under_the_protocol_container()
      {
         var formulationContainer = ApplicationContainerIn(_simulation).ParentContainer;

         formulationContainer.ParentContainer.Name.ShouldBeEqualTo("IV");
      }
   }
}
