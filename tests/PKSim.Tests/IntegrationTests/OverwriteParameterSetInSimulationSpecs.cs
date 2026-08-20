using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public class When_changing_a_simulation_parameter_overwritten_by_the_applied_overwrite_parameter_set : ContextForSimulationIntegration<IExecutionContext>
   {
      private Compound _compound;
      private string _parameterPath;
      private IParameter _overwrittenParameter;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = IoC.Resolve<IExecutionContext>();

         var templateIndividual = DomainFactoryForSpecs.CreateStandardIndividual();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(templateIndividual, _compound, protocol).DowncastTo<IndividualSimulation>();
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);

         var compoundParameter = firstValidCompoundParameter();
         _parameterPath = compoundParameter.Key;

         var overwriteParameterSet = new OverwriteParameterSet { Name = "TestSet" };
         overwriteParameterSet.Add(new ParameterValue { Path = _parameterPath.ToObjectPath(), Value = compoundParameter.Value.Value + 1.0 });
         _simulation.AddOverwriteParameterSetSelection(_compound.Name, overwriteParameterSet);

         //rebuild the model so the selected overwrite parameter set is applied during construction
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
         _overwrittenParameter = parameterCache()[_parameterPath];
      }

      private KeyValuePair<string, IParameter> firstValidCompoundParameter()
      {
         return parameterCache().KeyValues
            .First(kv => kv.Value.BuildingBlockType == PKSimBuildingBlockType.Simulation &&
                         _simulation.CompoundNameForParameterPath(kv.Key) == _compound.Name &&
                         !double.IsNaN(kv.Value.Value));
      }

      private PathCache<IParameter> parameterCache() => IoC.Resolve<IContainerTask>().CacheAllChildren<IParameter>(_simulation.Model.Root);

      protected override void Because()
      {
         new SetParameterValueCommand(_overwrittenParameter, _overwrittenParameter.Value + 1.0).Run(sut);
      }

      [Observation]
      public void should_track_the_change_as_an_uncommitted_change_of_the_compound()
      {
         _simulation.ParameterChangeTracker.IsTracked(_parameterPath).ShouldBeTrue();
      }

      [Observation]
      public void should_not_flag_the_compound_used_in_the_simulation_as_altered()
      {
         _simulation.UsedBuildingBlockByTemplateId(_compound.Id).Altered.ShouldBeFalse();
      }
   }
}
