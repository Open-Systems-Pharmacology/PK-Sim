using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.DTO.Protocols;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_SchemaItemToSchemaItemDTOMapper : ContextSpecification<ISchemaItemToSchemaItemDTOMapper>
   {
      protected IParameterToParameterDTOInContainerMapper<SchemaItemDTO> _parameterToParameterDTOMapper;
      protected PKSim.Presentation.DTO.Mappers.IParameterToParameterDTOMapper _parameterMapper;

      protected override void Context()
      {
         _parameterToParameterDTOMapper = A.Fake<IParameterToParameterDTOInContainerMapper<SchemaItemDTO>>();
         _parameterMapper = A.Fake<PKSim.Presentation.DTO.Mappers.IParameterToParameterDTOMapper>();
         sut = new SchemaItemToSchemaItemDTOMapper(_parameterToParameterDTOMapper, _parameterMapper);
      }
   }

   public class When_mapping_a_schema_item_to_a_schema_item_dto : concern_for_SchemaItemToSchemaItemDTOMapper
   {
      private SchemaItem _schemaItem;
      private SchemaItemDTO _schemaItemDTO;
      private ParameterDTO _doseParameterDTO;
      private ParameterDTO _startTimeParameterDTO;

      protected override void Context()
      {
         base.Context();
         _schemaItem = A.Fake<SchemaItem>();
         _doseParameterDTO = A.Fake<ParameterDTO>();
         _startTimeParameterDTO = A.Fake<ParameterDTO>();

         A.CallTo(_parameterToParameterDTOMapper).WithReturnType<IParameterDTO>()
            .ReturnsLazily(x =>
            {
               var parameter = x.Arguments[0].DowncastTo<IParameter>();
               return parameter == _schemaItem.Dose ? _doseParameterDTO : _startTimeParameterDTO;
            });
      }

      protected override void Because()
      {
         _schemaItemDTO = sut.MapFrom(_schemaItem);
      }

      [Observation]
      public void should_return_a_DTO_referencing_the_given_schema_item()
      {
         _schemaItemDTO.SchemaItem.ShouldBeEqualTo(_schemaItem);
      }

      [Observation]
      public void should_have_mapped_the_dose_parameter()
      {
         _schemaItemDTO.DoseParameter.ShouldBeEqualTo(_doseParameterDTO);
      }

      [Observation]
      public void should_have_mapped_the_start_time_parameter()
      {
         _schemaItemDTO.StartTimeParameter.ShouldBeEqualTo(_startTimeParameterDTO);
      }
   }

   public class When_mapping_an_infusion_schema_item_to_a_schema_item_dto : concern_for_SchemaItemToSchemaItemDTOMapper
   {
      private SchemaItem _schemaItem;
      private SchemaItemDTO _schemaItemDTO;

      protected override void Context()
      {
         base.Context();
         _schemaItem = new SchemaItem {ApplicationType = ApplicationTypes.Intravenous};
         _schemaItem.Add(DomainHelperForSpecs.ConstantParameterWithValue(15).WithName(Constants.Parameters.INFUSION_TIME));
         A.CallTo(() => _parameterMapper.MapFrom(A<IParameter>._)).Returns(A.Fake<IParameterDTO>());
      }

      protected override void Because()
      {
         _schemaItemDTO = sut.MapFrom(_schemaItem);
      }

      [Observation]
      public void should_have_mapped_the_infusion_time_parameter()
      {
         _schemaItemDTO.InfusionTimeParameter.ShouldNotBeNull();
      }
   }

   public class When_mapping_a_non_infusion_schema_item_to_a_schema_item_dto : concern_for_SchemaItemToSchemaItemDTOMapper
   {
      private SchemaItem _schemaItem;
      private SchemaItemDTO _schemaItemDTO;

      protected override void Context()
      {
         base.Context();
         _schemaItem = new SchemaItem {ApplicationType = ApplicationTypes.IntravenousBolus};
         A.CallTo(_parameterToParameterDTOMapper).WithReturnType<IParameterDTO>().Returns(A.Fake<IParameterDTO>());
      }

      protected override void Because()
      {
         _schemaItemDTO = sut.MapFrom(_schemaItem);
      }

      [Observation]
      public void should_not_have_mapped_an_infusion_time_parameter()
      {
         _schemaItemDTO.InfusionTimeParameter.ShouldBeNull();
      }
   }
}