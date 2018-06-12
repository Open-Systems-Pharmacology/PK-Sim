using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Mappers;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ProjectMetaDataToProjectMapper : ContextSpecification<IProjectMetaDataToProjectMapper>
   {
      private ISimulationMetaDataToSimulationMapper _simulationMapper;
      private ICompressedSerializationManager _entitySerializer;
      private ISerializationContextFactory _serializationContextFactory;
      protected PKSimProject _project;
      protected ProjectMetaData _projectMetaData;
      protected IndividualSimulationComparisonMetaData _simulationComparisonMetaData;

      protected override void Context()
      {
         _simulationMapper = A.Fake<ISimulationMetaDataToSimulationMapper>();
         _entitySerializer = A.Fake<ICompressedSerializationManager>();
         _serializationContextFactory = A.Fake<ISerializationContextFactory>();
         sut = new ProjectMetaDataToProjectMapper(_simulationMapper, _entitySerializer, _serializationContextFactory);

         _projectMetaData = new ProjectMetaData();
         _simulationComparisonMetaData = new IndividualSimulationComparisonMetaData {Id = "ComparisonData"};
      }
   }

   public class When_mapping_a_project_meta_data_to_project_with_version_current : concern_for_ProjectMetaDataToProjectMapper
   {
      protected override void Context()
      {
         base.Context();
         //this will trigger an update of the hasChanged flag to true
         _projectMetaData.AddSimulationComparison(_simulationComparisonMetaData);
         _projectMetaData.Version = ProjectVersions.Current;
      }

      protected override void Because()
      {
         _project = sut.MapFrom(_projectMetaData);
      }

      [Observation]
      public void should_have_set_the_has_changed_flag_to_false()
      {
         _project.HasChanged.ShouldBeFalse();
      }
   }

   public class When_mapping_a_project_meta_data_to_project_with_an_older_version : concern_for_ProjectMetaDataToProjectMapper
   {
      protected override void Context()
      {
         base.Context();
         _projectMetaData.AddSimulationComparison(_simulationComparisonMetaData);
         _projectMetaData.Version = ProjectVersions.V5_2_1;
      }

      protected override void Because()
      {
         _project = sut.MapFrom(_projectMetaData);
      }

      [Observation]
      public void should_not_have_reset_the_changed_flag()
      {
         _project.HasChanged.ShouldBeTrue();
      }
   }
}