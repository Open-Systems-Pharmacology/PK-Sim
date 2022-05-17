using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using IContainer = OSPSuite.Core.Domain.IContainer;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ObjectBaseSerializationManager : ContextForIntegration<ISerializationManager>
   {
      protected IContainerTask _containerTask;

      protected override void Context()
      {
         sut = IoC.Resolve<ISerializationManager>();
         _containerTask = IoC.Resolve<IContainerTask>();
      }
   }

   public abstract class When_serializing_two_building_blocks<TBuildingBlock> : concern_for_ObjectBaseSerializationManager where TBuildingBlock : IContainer
   {
      protected PathCache<IParameter> _allSourceParameters;
      protected PathCache<IParameter> _allTargetParameters;

      protected TBuildingBlock _sourceBuildingBlock;
      protected TBuildingBlock _deserializedBuildingBlock;

      protected override void Because()
      {
         var stream = sut.Serialize(_sourceBuildingBlock);
         _deserializedBuildingBlock = sut.Deserialize<TBuildingBlock>(stream);
         _allSourceParameters = _containerTask.CacheAllChildren<IParameter>(_sourceBuildingBlock);
         _allTargetParameters = _containerTask.CacheAllChildren<IParameter>(_deserializedBuildingBlock);
      }

      protected void CheckParametersAreEqualsInBuildingBlocks()
      {
         var errorList = new List<string>();

         foreach (var sourceKeyValue in _allSourceParameters.KeyValues)
         {
            var sourceParameter = sourceKeyValue.Value;
            var targetParameter = _allTargetParameters[sourceKeyValue.Key];
            if (targetParameter == null)
            {
               errorList.Add($"Could not find parameter '{sourceKeyValue.Key}'");
               continue;
            }

            var sourceFormula = sourceParameter.Formula;
            var targetFormula = targetParameter.Formula;

            if (sourceFormula.IsExplicit() && sourceFormula.Id != targetFormula.Id)
            {
               errorList.Add($"Formula Ids for parameter '{sourceKeyValue.Key}' are not equal ({sourceFormula.Id} vs {targetFormula.Id})");
               continue;
            }

            if (!ValueComparer.AreValuesEqual(tryGetValue(sourceParameter), tryGetValue(targetParameter)))
            {
               errorList.Add($"For parameter '{sourceKeyValue.Key}'. source value ({sourceParameter.Value}) is not equal target value ({targetParameter.Value}),");
               continue;
            }
         }

         Assert.IsTrue(errorList.Count == 0, errorList.ToString("\n"));
      }

      private double tryGetValue(IParameter targetParameter)
      {
         try
         {
            return targetParameter.Value;
         }
         catch (OSPSuiteException)
         {
            return double.NaN;
         }
      }
   }

   public class When_deserializing_a_serialized_individual : When_serializing_two_building_blocks<Individual>
   {
      protected override void Context()
      {
         base.Context();
         _sourceBuildingBlock = DomainFactoryForSpecs.CreateStandardIndividual();
      }

      [Observation]
      public void the_parameters_of_the_resulting_individual_should_have_the_same_values_as_the_parameter_in_the_original_individual()
      {
         CheckParametersAreEqualsInBuildingBlocks();
      }
   }

   public class When_deserializing_a_serialized_compound : When_serializing_two_building_blocks<Compound>
   {
      protected override void Context()
      {
         base.Context();
         _sourceBuildingBlock = DomainFactoryForSpecs.CreateStandardCompound();
      }

      [Observation]
      public void the_parameters_of_the_resulting_compound_should_have_the_same_values_as_the_parameter_in_the_original_compound()
      {
         CheckParametersAreEqualsInBuildingBlocks();
      }
   }

   public class When_deserializing_a_serialized_protocol : When_serializing_two_building_blocks<Protocol>
   {
      protected override void Context()
      {
         base.Context();
         _sourceBuildingBlock = DomainFactoryForSpecs.CreateStandardIVProtocol();
      }

      [Observation]
      public void the_parameters_of_the_resulting_protocol_should_have_the_same_values_as_the_parameter_in_the_original_protocol()
      {
         CheckParametersAreEqualsInBuildingBlocks();
      }
   }

   public class When_deserializing_a_serialized_individual_with_a_disease_state : When_serializing_two_building_blocks<Individual>
   {
      private DiseaseState _diseaseState;

      protected override void Context()
      {
         base.Context();
         var diseaseStateRepository = IoC.Resolve<IDiseaseStateRepository>();
         _sourceBuildingBlock = DomainFactoryForSpecs.CreateStandardIndividual();
         _diseaseState = diseaseStateRepository.AllFor(_sourceBuildingBlock.OriginData.Population).FirstOrDefault();

         _sourceBuildingBlock.OriginData.DiseaseState = _diseaseState;
         _sourceBuildingBlock.OriginData.AddDiseaseStateParameter(new OriginDataParameter(40, "mg"){Name = "TEST"});
      }

      [Observation]
      public void the_parameters_of_the_resulting_individual_should_have_the_same_values_as_the_parameter_in_the_original_individual()
      {
         CheckParametersAreEqualsInBuildingBlocks();
         _deserializedBuildingBlock.OriginData.DiseaseState.ShouldBeEqualTo(_diseaseState);
         var diseaseStateParam = _deserializedBuildingBlock.OriginData.DiseaseStateParameters.FindByName("TEST");
         diseaseStateParam.ShouldNotBeNull();
         diseaseStateParam.Value.ShouldBeEqualTo(40);
         diseaseStateParam.Unit.ShouldBeEqualTo("mg");
      }
   }
}