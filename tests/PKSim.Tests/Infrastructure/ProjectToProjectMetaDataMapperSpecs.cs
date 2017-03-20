using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Mappers;
using FakeItEasy;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ProjectToProjectMetaDataMapper : ContextSpecification<IProjectToProjectMetaDataMapper>
   {
      private ICompressedSerializationManager _compressedSerializationManager;
      private ISimulationToSimulationMetaDataMapper _simulationToSimulationMetaDataMapper;

      protected override void Context()
      {
         _compressedSerializationManager = A.Fake<ICompressedSerializationManager>();
         _simulationToSimulationMetaDataMapper = A.Fake<ISimulationToSimulationMetaDataMapper>();
         sut = new ProjectToProjectMetaDataMapper(_compressedSerializationManager, _simulationToSimulationMetaDataMapper);
      }
   }

   
   public class When_mapping_a_meta_data_object : concern_for_ProjectToProjectMetaDataMapper
   {
      [Observation]
      public void should_return_a_valid_project_meta_data()
      {
         sut.MapFrom(new PKSimProject()).ShouldNotBeNull();
      }
   }
}