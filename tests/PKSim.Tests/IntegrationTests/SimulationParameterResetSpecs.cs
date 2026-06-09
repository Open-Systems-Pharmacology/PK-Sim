using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using static PKSim.Core.CoreConstants.Organ;
using static OSPSuite.Core.Domain.Constants.Parameters;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SimulationParameterReset : ContextForSimulationIntegration<IParameterTask>
   {
      protected Individual _individual;
      protected IDistributedParameter _modelVolume;
      protected double _originalValue;
      protected double _originalPercentile;
      protected double _newValue;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         var compound = DomainFactoryForSpecs.CreateStandardCompound();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();

         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, compound, protocol) as IndividualSimulation;

         var project = new PKSimProject();
         project.AddBuildingBlock(compound);
         project.AddBuildingBlock(protocol);
         project.AddBuildingBlock(_simulation);
         project.AddBuildingBlock(_individual);
         var workspace = IoC.Resolve<ICoreWorkspace>();
         workspace.Project = project;

         //The UI edits/resets the parameters in the simulation MODEL (Model.Root). The Simulation.Individual is only
         //a synchronization copy. The distributed parameters in the model are the ones affected by #3562.
         _modelVolume = _simulation.Model.Root.GetAllChildren<IDistributedParameter>()
            .First(x => x.IsNamed(VOLUME) && x.ParentContainer.IsNamed(VENOUS_BLOOD));

         _originalValue = _modelVolume.Value;
         _originalPercentile = _modelVolume.Percentile;
         _newValue = _originalValue * 3;

         //simulate user changing the value in the simulation (sut is not resolved yet in GlobalContext)
         IoC.Resolve<IParameterTask>().SetParameterValue(_modelVolume, _newValue);
      }
   }

   public class When_resetting_a_distributed_organ_volume_changed_in_a_simulation : concern_for_SimulationParameterReset
   {
      protected override void Because()
      {
         //simulate user clicking the reset button
         sut.ResetParameter(_modelVolume);
      }

      [Observation]
      public void should_restore_the_original_value()
      {
         _modelVolume.Value.ShouldBeEqualTo(_originalValue, 1e-7);
      }

      [Observation]
      public void should_restore_the_original_percentile()
      {
         _modelVolume.Percentile.ShouldBeEqualTo(_originalPercentile, 1e-7);
      }

      [Observation]
      public void should_no_longer_be_a_fixed_value()
      {
         _modelVolume.IsFixedValue.ShouldBeFalse();
      }

      [Observation]
      public void should_be_marked_as_default()
      {
         _modelVolume.IsDefault.ShouldBeTrue();
      }
   }
}
