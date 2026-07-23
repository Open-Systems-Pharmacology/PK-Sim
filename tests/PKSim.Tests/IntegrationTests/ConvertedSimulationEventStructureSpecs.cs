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
         LoadProject("Mucosa_MultipleTransportDirections");
      }

      protected override void Context()
      {
         base.Context();
         _simulation = FindByName<Simulation>("S1");
      }

      protected IContainer ApplicationContainerIn(IContainer protocolEventGroup) =>
         protocolEventGroup.GetAllChildren<IContainer>(x => x.ContainerType == ContainerType.Application).Single();
   }

   public class When_loading_a_project_saved_before_applications_were_nested_under_a_formulation_container : concern_for_converted_simulation_event_structure
   {
      [Observation]
      public void the_application_of_a_protocol_without_formulation_should_be_nested_under_a_formulation_container()
      {
         var protocolEventGroup = _simulation.Model.Root.EntityAt<IContainer>(Constants.EVENTS, "IV");
         var application = ApplicationContainerIn(protocolEventGroup);

         application.ParentContainer.Name.ShouldBeEqualTo(CoreConstants.ContainerName.NoFormulation);
         application.ParentContainer.ContainerType.ShouldBeEqualTo(ContainerType.Formulation);
         application.ParentContainer.ParentContainer.ShouldBeEqualTo(protocolEventGroup);
      }
   }
}
