using System.Linq;
using PKSim.Core;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v5_4;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Assets;

namespace PKSim.ProjectConverter.v5_4
{
   public class When_converting_the_SimulationAndObservedData_531_project : ContextWithLoadedProject<Converter532To541>
   {
      private DataRepository _observedData;
      private Simulation _simulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimulationAndObservedData_531");
         _observedData = FirstObservedData();
         _simulation = First<Simulation>();
      }

      [Observation]
      public void should_have_added_a_classifiable_for_the_observed_data_defined_in_the_project()
      {
         var classifiableObservedData = _project.AllClassifiablesByType<ClassifiableObservedData>();
         classifiableObservedData.Count.ShouldBeEqualTo(1);
         classifiableObservedData.First().Subject.ShouldBeEqualTo(_observedData);
      }

      [Observation]
      public void should_have_converted_the_observed_data()
      {
         _observedData.ExtendedPropertyValueFor(ObservedData.MOLECULE).ShouldNotBeNull();
         _observedData.ExtendedPropertyValueFor(ObservedData.ORGAN).ShouldNotBeNull();
      }

      [Observation]
      public void should_have_added_a_classifiable_for_the_simulations_defined_in_the_project()
      {
         var classifiableSimulations = _project.AllClassifiablesByType<ClassifiableSimulation>();
         classifiableSimulations.Count.ShouldBeEqualTo(1);
         classifiableSimulations.First().Subject.ShouldBeEqualTo(_simulation);
      }

      [Observation]
      public void should_have_added_a_classifiable_for_the_comparison_defined_in_the_project()
      {
         var classifiableComparisons = _project.AllClassifiablesByType<ClassifiableComparison>();
         classifiableComparisons.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_added_the_event_group_type_to_the_application_root_folder_in_the_simulation()
      {
         _simulation.Model.Root.GetChildren<EventGroup>().Each(eg => eg.ContainerType.ShouldBeEqualTo(ContainerType.EventGroup));
      }

      [Observation]
      public void should_have_added_the_application_type_to_the_applications_root_folder_in_the_simulation()
      {
         _simulation.ApplicationsContainer.GetAllChildren<EventGroup>(x => x.Name.StartsWith("Application")).Each(
            app => app.ContainerType.ShouldBeEqualTo(ContainerType.Application));
      }
   }
}