using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using IdentificationParameter = OSPSuite.Core.Domain.ParameterIdentifications.IdentificationParameter;
using ModelParameterIdentification = OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentification;
using OutputMapping = OSPSuite.Core.Domain.ParameterIdentifications.OutputMapping;
using Simulation = PKSim.Core.Model.Simulation;
using SnapshotParameterIdentification = PKSim.Core.Snapshots.ParameterIdentification;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterIdentificationMapper : ContextSpecificationAsync<ParameterIdentificationMapper>
   {
      protected SnapshotParameterIdentification _snapshot;
      protected ModelParameterIdentification _parameterIdentification;
      protected PKSimProject _project;
      protected Simulation _simulation;
      private ParameterIdentificationConfigurationMapper _parameterIdentificationConfigurationMapper;
      protected ParameterIdentificationConfiguration _snapshotParameterIndentificationConfiguration;
      protected OutputMappingMapper _outputMappingMapper;
      protected OutputMapping _outputMapping;
      protected Snapshots.OutputMapping _snapshotOutputMapping;
      private IdentificationParameterMapper _identificationParameterMapper;
      private IdentificationParameter _identificationParameter;
      protected Snapshots.IdentificationParameter _snapshotIdentificationParameter;
      private ParameterIdentificationAnalysisMapper _parameterIdentificationAnalysisMapper;
      private ISimulationAnalysis _parameterIdentificationAnalysis;
      protected ParameterIdentificationAnalysis _snapshotParameterIdentificationAnalysis;

      protected override Task Context()
      {
         _parameterIdentificationConfigurationMapper = A.Fake<ParameterIdentificationConfigurationMapper>();
         _outputMappingMapper = A.Fake<OutputMappingMapper>();
         _identificationParameterMapper = A.Fake<IdentificationParameterMapper>();
         _parameterIdentificationAnalysisMapper= A.Fake<ParameterIdentificationAnalysisMapper>();

         _project = new PKSimProject();
         _simulation = new IndividualSimulation().WithName("S1");
         _project.AddBuildingBlock(_simulation);

         _parameterIdentification = new ModelParameterIdentification();
         _snapshotParameterIndentificationConfiguration = new ParameterIdentificationConfiguration();
         _snapshotOutputMapping = new Snapshots.OutputMapping();
         _outputMapping = new OutputMapping();
         _parameterIdentification.AddSimulation(_simulation);
         _parameterIdentification.AddOutputMapping(_outputMapping);

         _identificationParameter = new IdentificationParameter {Name = "IP"};
         _parameterIdentification.AddIdentificationParameter(_identificationParameter);

         _snapshotIdentificationParameter = new Snapshots.IdentificationParameter();
         _snapshotParameterIdentificationAnalysis = new ParameterIdentificationAnalysis();
         _parameterIdentificationAnalysis = A.Fake<ISimulationAnalysis>();
         _parameterIdentification.AddAnalysis(_parameterIdentificationAnalysis);


         sut = new ParameterIdentificationMapper(_parameterIdentificationConfigurationMapper, _outputMappingMapper, _identificationParameterMapper,_parameterIdentificationAnalysisMapper);


         A.CallTo(() => _parameterIdentificationConfigurationMapper.MapToSnapshot(_parameterIdentification.Configuration)).Returns(_snapshotParameterIndentificationConfiguration);
         A.CallTo(() => _outputMappingMapper.MapToSnapshot(_outputMapping)).Returns(_snapshotOutputMapping);
         A.CallTo(() => _identificationParameterMapper.MapToSnapshot(_identificationParameter)).Returns(_snapshotIdentificationParameter);
         A.CallTo(() => _parameterIdentificationAnalysisMapper.MapToSnapshot(_parameterIdentificationAnalysis)).Returns(_snapshotParameterIdentificationAnalysis);

         return _completed;
      }
   }

   public class When_mapping_a_parameter_identification_to_snapshot : concern_for_ParameterIdentificationMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_parameterIdentification, _project);
      }

      [Observation]
      public void should_return_a_snapshot_referencing_the_optimized_simulations_from_project()
      {
         _snapshot.Simulations.ShouldContain(_simulation.Name);
      }

      [Observation]
      public void should_have_mapped_the_parameter_identification_configuration()
      {
         _snapshot.Configuration.ShouldBeEqualTo(_snapshotParameterIndentificationConfiguration);
      }

      [Observation]
      public void should_have_mapped_the_output_mappings()
      {
         _snapshot.OutputMappings.ShouldContain(_snapshotOutputMapping);
      }

      [Observation]
      public void should_have_mapped_the_identification_parameters()
      {
         _snapshot.IdentificationParameters.ShouldContain(_snapshotIdentificationParameter);
      }

      [Observation]
      public void should_have_mapped_the_parameter_identificaiton_analysis()
      {
         _snapshot.Analyses.ShouldContain(_snapshotParameterIdentificationAnalysis);
      }
   }
}