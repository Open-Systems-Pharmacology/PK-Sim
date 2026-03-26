using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundToMolWeightDTOMapper : ContextSpecification<CompoundToMolWeightDTOMapper>
   {
      protected IParameterToParameterDTOMapper _parameterDTOMapper;

      protected override void Context()
      {
         _parameterDTOMapper = A.Fake<IParameterToParameterDTOMapper>();

         sut = new CompoundToMolWeightDTOMapper(_parameterDTOMapper);
      }
   }

   public class When_mapping_compound_parameters_and_all_expected_parameters_are_present : concern_for_CompoundToMolWeightDTOMapper
   {
      private MolWeightDTO _result;
      private IParameter _effective;
      private IParameter _molWeight;
      private IParameter _hasHalogens;

      protected override void Context()
      {
         base.Context();

         _effective = DomainHelperForSpecs.ConstantParameterWithValue().WithName(CoreConstants.Parameters.EFFECTIVE_MOLECULAR_WEIGHT);
         _molWeight = DomainHelperForSpecs.ConstantParameterWithValue().WithName(Constants.Parameters.MOL_WEIGHT);
         _hasHalogens = DomainHelperForSpecs.ConstantParameterWithValue().WithName(Constants.Parameters.HAS_HALOGENS);

         var expectedMolWeightParamDto = new ParameterDTO(_effective);
         var expectedEffectiveDto = new EffectiveMolWeightParameterDTO(_effective);
         var expectedHasHalogensDto = new ParameterDTO(_hasHalogens);

         A.CallTo(() => _parameterDTOMapper.MapEffectiveMolWeightDTOFrom(_effective)).Returns(expectedEffectiveDto);
         A.CallTo(() => _parameterDTOMapper.MapFrom(_molWeight)).Returns(expectedMolWeightParamDto);
         A.CallTo(() => _parameterDTOMapper.MapFrom(_hasHalogens)).Returns(expectedHasHalogensDto);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(new List<IParameter> { _effective, _molWeight, _hasHalogens });
      }

      [Observation]
      public void should_return_dto_with_the_mapped_mol_weight_parameter()
      {
         _result.MolWeightEffParameter.ShouldBeAnInstanceOf<EffectiveMolWeightParameterDTO>();
      }

      [Observation]
      public void should_return_dto_with_the_mapped_effective_mol_weight_parameter()
      {
         _result.MolWeightParameter.ShouldBeAnInstanceOf<ParameterDTO>();
      }

      [Observation]
      public void should_return_dto_with_the_mapped_has_halogens_parameter()
      {
         _result.HasHalogensParameter.ShouldBeAnInstanceOf<ParameterDTO>();
      }

      [Observation]
      public void should_call_the_expected_mappers_with_the_correct_parameters()
      {
         A.CallTo(() => _parameterDTOMapper.MapEffectiveMolWeightDTOFrom(_effective)).MustHaveHappened();
         A.CallTo(() => _parameterDTOMapper.MapFrom(_molWeight)).MustHaveHappened();
         A.CallTo(() => _parameterDTOMapper.MapFrom(_hasHalogens)).MustHaveHappened();
      }
   }
}