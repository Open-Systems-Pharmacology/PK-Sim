using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using IContainer = OSPSuite.Core.Domain.IContainer;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_converted_simulation_event_structure : ContextWithLoadedProject<Simulation>
   {
      protected IReadOnlyList<IContainer> AllApplicationsIn(Simulation simulation) =>
         simulation.Model.Root.GetAllChildren<IContainer>(x => x.ContainerType == ContainerType.Application).ToList();

      protected IReadOnlyList<IContainer> AllApplicationParentsIn(string simulationName) =>
         AllApplicationsIn(LoadSimulation(simulationName)).Select(x => x.ParentContainer).ToList();

      protected Simulation LoadSimulation(string simulationName) => FindByName<Simulation>(simulationName);

      //every application must be nested under a formulation container, whether or not the administration requires a formulation
      protected void ShouldAllBeFormulationContainers(IEnumerable<IContainer> applicationParents) =>
         applicationParents.Each(x => x.ContainerType.ShouldBeEqualTo(ContainerType.Formulation));
   }

   public class When_loading_a_project_saved_before_applications_were_nested_under_a_formulation_container : concern_for_converted_simulation_event_structure
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         //a 7.1 project with an IV protocol, saved when applications were added directly under the protocol container
         LoadProject("SimpleSimulationIV_710");
      }

      [Observation]
      public void the_application_of_a_protocol_without_formulation_should_be_nested_under_a_no_formulation_container()
      {
         var applicationParents = AllApplicationParentsIn("Human");

         applicationParents.Count.ShouldBeGreaterThan(0);
         ShouldAllBeFormulationContainers(applicationParents);
         applicationParents.AllNames().Distinct().ShouldOnlyContainInOrder(CoreConstants.ContainerName.NoFormulation);
      }

      [Observation]
      public void the_no_formulation_container_should_be_nested_under_the_protocol_container()
      {
         AllApplicationParentsIn("Human").Each(x => x.ParentContainer.Name.ShouldBeEqualTo("IV"));
      }

      [Observation]
      public void the_conversion_should_also_apply_to_the_other_simulations_of_the_project()
      {
         ShouldAllBeFormulationContainers(AllApplicationParentsIn("Dog"));
      }

      [Observation]
      public async Task the_converted_simulation_should_still_run()
      {
         //the applications are one level deeper after the conversion: this only works if all paths referencing them were updated
         var simulation = LoadSimulation("Human").DowncastTo<IndividualSimulation>();
         await IoC.Resolve<IIndividualSimulationEngine>().RunAsync(simulation, new SimulationRunOptions());
         simulation.HasResults.ShouldBeTrue();
      }
   }

   public class When_loading_a_v11_project_with_an_iv_protocol : concern_for_converted_simulation_event_structure
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("v11_expression_profile");
      }

      [Observation]
      public void the_application_should_be_nested_under_a_no_formulation_container()
      {
         var applicationParents = AllApplicationParentsIn("S");

         applicationParents.Count.ShouldBeGreaterThan(0);
         applicationParents.AllNames().Distinct().ShouldOnlyContainInOrder(CoreConstants.ContainerName.NoFormulation);
      }
   }

   public class When_loading_a_population_project_with_an_iv_protocol : concern_for_converted_simulation_event_structure
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimplePop_73");
      }

      [Observation]
      public void the_application_of_the_population_simulation_should_be_nested_under_a_no_formulation_container()
      {
         var applicationParents = AllApplicationParentsIn("POP");

         applicationParents.Count.ShouldBeGreaterThan(0);
         applicationParents.AllNames().Distinct().ShouldOnlyContainInOrder(CoreConstants.ContainerName.NoFormulation);
      }
   }

   public class When_loading_a_project_using_both_an_oral_and_an_iv_protocol : concern_for_converted_simulation_event_structure
   {
      private readonly List<IContainer> _allApplicationParents = new List<IContainer>();

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimpleProject_730");
      }

      protected override void Context()
      {
         base.Context();
         new[] { "S1", "S2", "S3" }.Each(x => _allApplicationParents.AddRange(AllApplicationParentsIn(x)));
      }

      [Observation]
      public void every_application_should_be_nested_under_a_formulation_container()
      {
         _allApplicationParents.Count.ShouldBeGreaterThan(0);
         ShouldAllBeFormulationContainers(_allApplicationParents);
      }

      [Observation]
      public void the_formulation_of_an_administration_requiring_one_should_not_be_replaced_by_a_no_formulation_container()
      {
         _allApplicationParents.Select(x => x.Name).Distinct().Count().ShouldBeGreaterThan(1);
      }
   }
}
