using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart.ParameterIdentifications;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using DataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using ParameterIdentification = OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentification;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterIdentificationAnalysisMapper<T> : ContextSpecificationAsync<ParameterIdentificationAnalysisMapper> where T : ISimulationAnalysis, new()
   {
      protected ParameterIdentificationAnalysisChartMapper _parameterIdentificationAnalysisChartMapper;
      protected DataRepositoryMapper _dataRepositoryMapper;
      protected T _parameterIdentificationAnalysis;
      protected ParameterIdentificationAnalysis _snapshot;
      protected CurveChart _chartSnapshot;
      protected DataRepository _localRepository;
      protected Snapshots.DataRepository _snapshotLocalRepository;
      private IIdGenerator _idGenerator;
      protected ParameterIdentificationContext _parameterIdentificationContext;
      private ParameterIdentification _parameterIdentification;
      private PKSimProject _project;
      private SnapshotContext _baseContext;

      protected override Task Context()
      {
         _parameterIdentificationAnalysisChartMapper = A.Fake<ParameterIdentificationAnalysisChartMapper>();
         _dataRepositoryMapper = A.Fake<DataRepositoryMapper>();
         _idGenerator = A.Fake<IIdGenerator>();
         sut = new ParameterIdentificationAnalysisMapper(_parameterIdentificationAnalysisChartMapper, _dataRepositoryMapper, _idGenerator);

         _parameterIdentificationAnalysis = new T().WithName("Chart");
         _chartSnapshot = new CurveChart();
         _localRepository = DomainHelperForSpecs.ObservedData();
         _snapshotLocalRepository = new Snapshots.DataRepository();
         A.CallTo(() => _dataRepositoryMapper.MapToSnapshot(_localRepository)).Returns(_snapshotLocalRepository);

         _parameterIdentification = new ParameterIdentification();
         _project = new PKSimProject();
         _baseContext = new SnapshotContext(_project, ProjectVersions.V10);
         _parameterIdentificationContext = new ParameterIdentificationContext(_parameterIdentification, _baseContext);
         return _completed;
      }
   }

   public class When_mapping_a_parameter_identification_residual_histogram_analysis_to_snapshot : concern_for_ParameterIdentificationAnalysisMapper<ParameterIdentificationResidualHistogram>
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_parameterIdentificationAnalysis);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_expected_standard_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_parameterIdentificationAnalysis.Name);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_analysis_type_as_property()
      {
         _snapshot.Type.ShouldBeEqualTo(_parameterIdentificationAnalysis.GetType().Name);
      }

      [Observation]
      public void should_set_the_other_properties_to_null()
      {
         _snapshot.Chart.ShouldBeNull();
         _snapshot.DataRepositories.ShouldBeNull();
      }
   }

   public class When_mapping_a_parameter_identification_residual_histogram_snapshot_tp_analysis : concern_for_ParameterIdentificationAnalysisMapper<ParameterIdentificationResidualHistogram>
   {
      private ISimulationAnalysis _newAnalysis;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_parameterIdentificationAnalysis);
      }

      protected override async Task Because()
      {
         _newAnalysis = await sut.MapToModel(_snapshot, _parameterIdentificationContext);
      }

      [Observation]
      public void should_return_the_expected_analysis()
      {
         _newAnalysis.ShouldBeAnInstanceOf<ParameterIdentificationResidualHistogram>();
      }
   }

   public class When_mapping_a_parameter_identification_time_profile_analysis_to_snapshot : concern_for_ParameterIdentificationAnalysisMapper<ParameterIdentificationTimeProfileChart>
   {
      protected override async Task Context()
      {
         await base.Context();
         A.CallTo(() => _parameterIdentificationAnalysisChartMapper.MapToSnapshot(_parameterIdentificationAnalysis)).Returns(_chartSnapshot);
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_parameterIdentificationAnalysis);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_expected_standard_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_parameterIdentificationAnalysis.Name);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_analysis_type_as_property()
      {
         _snapshot.Type.ShouldBeEqualTo(_parameterIdentificationAnalysis.GetType().Name);
      }

      [Observation]
      public void should_have_mapped_the_underlying_chart()
      {
         _snapshot.Chart.ShouldBeEqualTo(_chartSnapshot);
      }

      [Observation]
      public void should_set_the_other_properties_to_null()
      {
         _snapshot.DataRepositories.ShouldBeNull();
      }
   }

   public class When_mapping_a_parameter_identification_time_profile_analysis_snapshot_to_analysis : concern_for_ParameterIdentificationAnalysisMapper<ParameterIdentificationTimeProfileChart>
   {
      private ISimulationAnalysis _newAnalysis;

      protected override async Task Context()
      {
         await base.Context();
         A.CallTo(() => _parameterIdentificationAnalysisChartMapper.MapToSnapshot(_parameterIdentificationAnalysis)).Returns(_chartSnapshot);
         A.CallTo(() => _parameterIdentificationAnalysisChartMapper.MapToModel(_chartSnapshot, A<SimulationAnalysisContext>._)).Returns(new ParameterIdentificationTimeProfileChart());
         _snapshot = await sut.MapToSnapshot(_parameterIdentificationAnalysis);
      }

      protected override async Task Because()
      {
         _newAnalysis = await sut.MapToModel(_snapshot, _parameterIdentificationContext);
      }

      [Observation]
      public void should_return_the_expected_analysis()
      {
         _newAnalysis.ShouldBeAnInstanceOf<ParameterIdentificationTimeProfileChart>();
      }

      [Observation]
      public void should_ensure_that_the_factory_method_returns_the_expected_chart()
      {
         _parameterIdentificationAnalysisChartMapper.ChartFactoryFunc().ShouldBeAnInstanceOf<ParameterIdentificationTimeProfileChart>();
      }
   }

   public class When_mapping_a_parameter_identification_time_profile_confidence_interval_analysis_to_snapshot : concern_for_ParameterIdentificationAnalysisMapper<ParameterIdentificationTimeProfileConfidenceIntervalChart>
   {
      protected override async Task Context()
      {
         await base.Context();
         _parameterIdentificationAnalysis.AddRepository(_localRepository);
         A.CallTo(() => _parameterIdentificationAnalysisChartMapper.MapToSnapshot(_parameterIdentificationAnalysis)).Returns(_chartSnapshot);
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_parameterIdentificationAnalysis);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_expected_standard_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_parameterIdentificationAnalysis.Name);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_analysis_type_as_property()
      {
         _snapshot.Type.ShouldBeEqualTo(_parameterIdentificationAnalysis.GetType().Name);
      }

      [Observation]
      public void should_have_mapped_the_underlying_chart()
      {
         _snapshot.Chart.ShouldBeEqualTo(_chartSnapshot);
      }

      [Observation]
      public void should_have_mapped_the_local_data_repositories()
      {
         _snapshot.DataRepositories.ShouldContain(_snapshotLocalRepository);
      }
   }
}