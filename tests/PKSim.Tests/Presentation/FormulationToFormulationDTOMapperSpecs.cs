using System.Collections.Generic;
using PKSim.Presentation.DTO.Formulations;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

using PKSim.Presentation.DTO.Mappers;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;

namespace PKSim.Presentation
{
   public abstract class concern_for_FormulationToFormulationDTOMapper : ContextSpecification<IFormulationToFormulationDTOMapper>
   {
      protected IRepresentationInfoRepository _representationInfoRepository;

      protected override void Context()
      {
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         sut = new FormulationToFormulationDTOMapper(_representationInfoRepository);
      }
   }

   
   public class When_mapping_a_schema_to_a_schema_dto : concern_for_FormulationToFormulationDTOMapper
   {
      private  PKSim.Core.Model.Formulation _formulation;
      private FormulationDTO _result;
      private RepresentationInfo _formulationInfo;
      private IParameter _para1;
      private IParameter _para2;

      protected override void Context()
      {
         base.Context();
         _formulation  =new Formulation();
         _formulation.FormulationType = "tralala";
         _formulationInfo =new RepresentationInfo();
         _para1 = A.Fake<IParameter>().WithName("Para1");
         _para2 = A.Fake<IParameter>().WithName("Para2");
         _formulation.Add(_para1);
         _formulation.Add(_para2);
         _formulationInfo.Description = " traaa";
         _formulationInfo.DisplayName = "tutu";
         A.CallTo(() => _representationInfoRepository.InfoFor(RepresentationObjectType.CONTAINER, _formulation.FormulationType)).Returns(_formulationInfo);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_formulation);
      }

      [Observation]
      public void should_return_a_formulation_dto_with_one_parameter_per_parameter_defined_in_the_formulation()
      {
         _result.Parameters.ShouldOnlyContain(_para1,_para2);
      }

      [Observation]
      public void should_return_a_formulation_dto_with_the_formulation_type_defined_with_the_formulation_name_and_display_name()
      {
         _result.Type.Id.ShouldBeEqualTo(_formulation.FormulationType);
         _result.Type.DisplayName.ShouldBeEqualTo(_formulationInfo.DisplayName);
         _result.Description.ShouldBeEqualTo(_formulationInfo.Description);
      }
   }
}	