using System;
using System.Xml.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Serialization.Xml.Extensions;
using OSPSuite.Presentation.Serialization.Extensions;
using OSPSuite.Serializer;
using OSPSuite.Serializer.Attributes;
using OSPSuite.Serializer.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public interface IPKSimXmlSerializerRepository : IOSPSuiteXmlSerializerRepository
   {
      void SerializeFormulaCache(XElement element, SerializationContext serializationContext, Type typeToSerialize);
      void DeserializeFormulaCache(XElement element, SerializationContext serializationContext, Type typeToDeserialize);
   }

   public class PKSimXmlSerializerRepository : OSPSuiteXmlSerializerRepository, IPKSimXmlSerializerRepository
   {
      protected override void AddInitialSerializer()
      {
         base.AddInitialSerializer();
         AttributeMapperRepository.AddAttributeMapper(new SpeciesXmlAttributeMapper());
         AttributeMapperRepository.AddAttributeMapper(new GenderXmlAttributeMapper());
         AttributeMapperRepository.AddAttributeMapper(new PopulationXmlAttributeMapper());
         AttributeMapperRepository.AddAttributeMapper(new ModelConfigurationXmlAttributeMapper());
         AttributeMapperRepository.AddAttributeMapper(new ObjectPathXmlAttributeMapper());
         AttributeMapperRepository.AddAttributeMapper(new SystemicProcessTypeXmlAttributeMapper());
         AttributeMapperRepository.AddAttributeMapper(new ApplicationTypeXmlAttributeMapper());
         AttributeMapperRepository.AddAttributeMapper(new ViewLayoutXmlAttributeMapper());
         AttributeMapperRepository.AddAttributeMapper(new DistributionTypeXmlAttributeMapper());
         AttributeMapperRepository.AddAttributeMapper(new IconSizeAttributeMapper());
         AttributeMapperRepository.AddAttributeMapper(new DosingIntervalXmlAttributeMapper());
         AttributeMapperRepository.AddAttributeMapper(new LabelGenerationStrategyXmlAttributeMapper());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<Localization, SerializationContext>());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<OrganType, SerializationContext>());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<BuildingBlockStatus, SerializationContext>());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<ProtocolMode, SerializationContext>());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<MembraneLocation, SerializationContext>());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<TissueLocation, SerializationContext>());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<IntracellularVascularEndoLocation, SerializationContext>());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<PivotArea, SerializationContext>());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<StatisticalAggregationType, SerializationContext>());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<NotificationType, SerializationContext>());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<BarType, SerializationContext>());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<AxisCountMode, SerializationContext>());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<PopulationAnalysisType, SerializationContext>());
         AttributeMapperRepository.AddAttributeMapper(new EnumAttributeMapper<InteractionType, SerializationContext>());

         //PKSim Serializers
         this.AddSerializers(x =>
         {
            x.Implementing<IPKSimXmlSerializer>();
            x.InAssemblyContainingType<PKSimXmlSerializerRepository>();
            x.UsingAttributeRepository(AttributeMapperRepository);
         });

         //SBSuite.Presentation serializer
         this.AddPresentationSerializers();
      }

      public void SerializeFormulaCache(XElement element, SerializationContext serializationContext, Type typeToSerialize)
      {
         if (formulaCacheManagementAlreadyTakenCareOf(typeToSerialize))
            return;

         this.AddFormulaCacheElement(element, serializationContext);
      }

      private bool formulaCacheManagementAlreadyTakenCareOf(Type type)
      {
         return
            type.IsAnImplementationOf<IPKSimBuildingBlock>() ||
            type.IsAnImplementationOf<IBuildingBlock>() ||
            type.IsAnImplementationOf<Model>();
      }

      public void DeserializeFormulaCache(XElement element, SerializationContext serializationContext, Type typeToDeserialize)
      {
         if (formulaCacheManagementAlreadyTakenCareOf(typeToDeserialize))
            return;

         this.DeserializeFormulaCacheIn(element, serializationContext);
      }
   }

   public interface IPKSimXmlSerializer : IXmlSerializer
   {
   }
}