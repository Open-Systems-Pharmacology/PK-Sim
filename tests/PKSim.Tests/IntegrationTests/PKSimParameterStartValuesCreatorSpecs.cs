using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;

namespace PKSim.IntegrationTests
{
   public class When_creating_a_parameter_start_value_building_block_for_a_simulation_whose_parameter_where_changed_from_default : ContextForSimulationIntegration<IPKSimParameterStartValuesCreator>
   {
      private IBuildConfigurationTask _buildConfigurationTask;
      private IEntityPathResolver _entityPathResolver;
      private IObjectPath _parameterPath;
      private IParameterStartValuesBuildingBlock _psv;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _buildConfigurationTask = IoC.Resolve<IBuildConfigurationTask>();
         _entityPathResolver = IoC.Resolve<IEntityPathResolver>();
         _simulation = DomainFactoryForSpecs.CreateDefaultSimulation();
      }

      protected override void Because()
      {
         var compoundName = _simulation.CompoundNames.First();
         var parameter = _simulation.Model.Root.EntityAt<IParameter>(compoundName, CoreConstantsForSpecs.Parameter.BLOOD_PLASMA_CONCENTRATION_RATIO);
         parameter.Value = 10;
         _parameterPath = _entityPathResolver.ObjectPathFor(parameter);
         _psv = _buildConfigurationTask.CreateFor(_simulation, shouldValidate: true, createAgingDataInSimulation: false).ParameterStartValues;
      }

      [Observation]
      public void should_have_created_one_entry_for_the_changed_parameter_in_the_parmaeter_start_value_building_block()
      {
         _psv[_parameterPath].ShouldNotBeNull();
         _psv[_parameterPath].StartValue.ShouldBeEqualTo(10);
      }
   }
}