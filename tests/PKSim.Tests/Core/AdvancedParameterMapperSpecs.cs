﻿using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using AdvancedParameter = PKSim.Core.Model.AdvancedParameter;
using ILogger = OSPSuite.Core.Services.ILogger;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_AdvancedParameterMapper : ContextSpecificationAsync<AdvancedParameterMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected AdvancedParameter _advancedParameter;
      protected IAdvancedParameterFactory _advancedParameterFactory;
      protected Parameter _meanSnapshot;
      protected Parameter _deviationSnapshot;
      protected IEntityPathResolver _entityPathResolver;
      protected ILogger _logger;
      protected int _originalSeed;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _advancedParameterFactory = A.Fake<IAdvancedParameterFactory>();
         _logger = A.Fake<ILogger>();

         _advancedParameter = new AdvancedParameter
         {
            DistributedParameter = DomainHelperForSpecs.NormalDistributedParameter(10, 5),
            ParameterPath = "ParameterPath",
            Name = "ParameterName"
         };

         _originalSeed = _advancedParameter.Seed;
         sut = new AdvancedParameterMapper(_parameterMapper, _advancedParameterFactory, _entityPathResolver, _logger);

         _meanSnapshot = new Parameter
         {
            Name = _advancedParameter.DistributedParameter.MeanParameter.Name,
         };

         _deviationSnapshot = new Parameter
         {
            Name = _advancedParameter.DistributedParameter.DeviationParameter.Name,
         };

         A.CallTo(() => _parameterMapper.MapToSnapshot(_advancedParameter.DistributedParameter.MeanParameter)).Returns(_meanSnapshot);
         A.CallTo(() => _parameterMapper.MapToSnapshot(_advancedParameter.DistributedParameter.DeviationParameter)).Returns(_deviationSnapshot);
         return Task.FromResult(true);
      }
   }

   public class When_mapping_an_advanced_parameter_to_snapshot : concern_for_AdvancedParameterMapper
   {
      private Snapshots.AdvancedParameter _snapshot;

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_advancedParameter);
      }

      [Observation]
      public void should_return_a_snapshot_containing_the_expected_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_advancedParameter.ParameterPath);
         _snapshot.DistributionType.ShouldBeEqualTo(_advancedParameter.DistributionType.Id);
         _snapshot.Seed.ShouldBeEqualTo(_originalSeed);
      }
   }

   public class When_mapping_an_advanced_parameter_snapshot_to_snapshot_for_an_unknown_parameter : concern_for_AdvancedParameterMapper
   {
      private Snapshots.AdvancedParameter _snapshot;
      private AdvancedParameter _result;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_advancedParameter);
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot, new PathCacheForSpecs<IParameter>());
      }

      [Observation]
      public void should_warn_the_user_that_a_snapshot_parameter_was_not_found()
      {
         A.CallTo(() => _logger.AddToLog(PKSimConstants.Error.SnapshotParameterNotFound(_snapshot.Name), LogLevel.Warning, A<string>._)).MustHaveHappened();
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }
   }

   public class When_mapping_an_advanced_parameter_snapshot_to_snapshot_usign_the_wrong_overload : concern_for_AdvancedParameterMapper
   {
      private Snapshots.AdvancedParameter _snapshot;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_advancedParameter);
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.MapToModel(_snapshot)).ShouldThrowAn<NotSupportedException>();
      }
   }

   public class When_mapping_an_advanced_parameter_snapshot_to_snapshot_for_a_well_defined_parameter : concern_for_AdvancedParameterMapper
   {
      private Snapshots.AdvancedParameter _snapshot;
      private AdvancedParameter _newAdvancedParameter;
      private PathCache<IParameter> _pathCache;
      private IParameter _parameter;
      private AdvancedParameter _mappedAdvancedParameter;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_advancedParameter);
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(5);
         _mappedAdvancedParameter = new AdvancedParameter {DistributedParameter = DomainHelperForSpecs.NormalDistributedParameter()};
         _pathCache = new PathCacheForSpecs<IParameter> {{_advancedParameter.ParameterPath, _parameter}};
         A.CallTo(() => _advancedParameterFactory.Create(_parameter, DistributionTypes.ById(_snapshot.DistributionType))).Returns(_mappedAdvancedParameter);
      }

      protected override async Task Because()
      {
         _newAdvancedParameter = await sut.MapToModel(_snapshot, _pathCache);
      }

      [Observation]
      public void should_return_the_expected_advanced_parameter_with_mapped_parameters()
      {
         _newAdvancedParameter.ShouldBeEqualTo(_mappedAdvancedParameter);
      }

      [Observation]
      public void should_have_updated_the_original_seed_defined_in_the_snapshot()
      {
         _newAdvancedParameter.Seed.ShouldBeEqualTo(_originalSeed);
      }

      [Observation]
      public void should_map_distribution_parameters_from_snapshot()
      {
         A.CallTo(() => _parameterMapper.MapParameters(_snapshot.Parameters, _newAdvancedParameter.DistributedParameter, _newAdvancedParameter.DistributedParameter.Name)).MustHaveHappened();
      }
   }

   public class When_mapping_all_advanced_parameters_snapshot_into_an_advanced_parameter_container : concern_for_AdvancedParameterMapper
   {
      private Snapshots.AdvancedParameter _snapshot;
      private IAdvancedParameterContainer _advancedParameterContainer;
      private IParameter _parameter;
      private PathCache<IParameter> _pathCache;
      private AdvancedParameter _mappedAdvancedParameter;

      protected override async Task Context()
      {
         await base.Context();
         _advancedParameterContainer = A.Fake<IAdvancedParameterContainer>();
         _snapshot = await sut.MapToSnapshot(_advancedParameter);
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(5);
         _pathCache = new PathCacheForSpecs<IParameter> {{_advancedParameter.ParameterPath, _parameter}};

         A.CallTo(() => _advancedParameterContainer.AllParameters(_entityPathResolver)).Returns(_pathCache);
         _mappedAdvancedParameter = new AdvancedParameter {DistributedParameter = DomainHelperForSpecs.NormalDistributedParameter()};
         A.CallTo(() => _advancedParameterFactory.Create(_parameter, DistributionTypes.ById(_snapshot.DistributionType))).Returns(_mappedAdvancedParameter);
      }

      protected override async Task Because()
      {
         await sut.MapToModel(new[] {_snapshot}, _advancedParameterContainer);
      }

      [Observation]
      public void should_remove_all_previously_defined_advanced_parameters()
      {
         A.CallTo(() => _advancedParameterContainer.RemoveAllAdvancedParameters()).MustHaveHappened();
      }

      [Observation]
      public void should_add_new_advanced_parameters()
      {
         A.CallTo(() => _advancedParameterContainer.AddAdvancedParameter(_mappedAdvancedParameter, true)).MustHaveHappened();
      }
   }
}