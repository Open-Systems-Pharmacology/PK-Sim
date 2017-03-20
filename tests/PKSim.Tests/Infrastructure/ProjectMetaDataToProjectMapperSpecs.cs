using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Mappers;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using FakeItEasy;
using PKSim.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ProjectMetaDataToProjectMapper : ContextSpecification<IProjectMetaDataToProjectMapper>
   {
      private ISimulationMetaDataToSimulationMapper _simulationMapper;
      private ICompressedSerializationManager _entitySerializer;
      private ISerializationContextFactory _serializationContextFactory;

      protected override void Context()
      {
         _simulationMapper = A.Fake<ISimulationMetaDataToSimulationMapper>();
         _entitySerializer = A.Fake<ICompressedSerializationManager>();
         _serializationContextFactory= A.Fake<ISerializationContextFactory>();
         sut = new ProjectMetaDataToProjectMapper(_simulationMapper,_entitySerializer, _serializationContextFactory);
      }
   }

   
   public class when_mapping_a_project_object : concern_for_ProjectMetaDataToProjectMapper
   {
      [Observation]
      public void should_return_a_valid_meta_data_object_for_a_defined_entity_type()
      {
         sut.MapFrom(new ProjectMetaData()).ShouldBeAnInstanceOf<PKSimProject>();
      }

      [Observation]
      public void should_thrown_an_exception_for_an_unknown_type()
      {
         The.Action(() => sut.MapFrom(A<ProjectMetaData>.Ignored)).ShouldThrowAn<Exception>();
      }
   }
}