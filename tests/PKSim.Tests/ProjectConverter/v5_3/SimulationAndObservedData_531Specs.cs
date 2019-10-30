using System.Linq;
using OSPSuite.Assets;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v5_3;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v5_3
{
   public class When_converting_the_SimulationAndObservedData_531_project : ContextWithLoadedProject<Converter531To532>
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
   }
}