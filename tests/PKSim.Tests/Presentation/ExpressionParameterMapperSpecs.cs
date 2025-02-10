using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Mappers;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using static PKSim.Core.CoreConstants;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExpressionParameterMapper : ContextSpecification<IExpressionParameterMapper<ExpressionParameterDTO>>
   {
      protected IParameterToParameterDTOInContainerMapper<ExpressionParameterDTO> _parameterMapper;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected IGroupRepository _groupRepository;
      protected IExpressionParameterTask _expressionParameterTask;
      protected Container _organ;
      protected Container _compartment;
      protected IParameter _parameter;
      protected ExpressionParameterDTO _expressionParameterDTO;
      protected Container _molecule;
      protected IGroup _group;

      protected override void Context()
      {
         _parameterMapper = A.Fake<IParameterToParameterDTOInContainerMapper<ExpressionParameterDTO>>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _groupRepository = A.Fake<IGroupRepository>();
         _expressionParameterTask = A.Fake<IExpressionParameterTask>();
         sut = new ExpressionParameterMapper<ExpressionParameterDTO>(_parameterMapper, _representationInfoRepository, _groupRepository,
            _expressionParameterTask);


         _organ = new Container().WithName("Organ");
         _compartment = new Container().WithName("Compartment").WithParentContainer(_organ);
         _molecule = new Container().WithName("CYP3A44");
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(5).WithParentContainer(_molecule);
         _group =new Group(); 
         A.CallTo(_groupRepository).WithReturnType<IGroup>().Returns(_group);
      }

      protected override void Because()
      {
         _expressionParameterDTO = sut.MapFrom(_parameter);
      }
   }

   public class When_mapping_an_global_blood_cells_parameter_expression_parameter_to_expression_parameter_dto : concern_for_ExpressionParameterMapper
   {
      protected override void Context()
      {
         base.Context();
         _parameter.Name = Parameters.FRACTION_EXPRESSED_BLOOD_CELLS;
         A.CallTo(() => _expressionParameterTask.ExpressionGroupFor(_parameter)).Returns(Groups.VASCULAR_SYSTEM);
      }

      [Observation]
      public void should_return_the_expected_properties()
      {
         _expressionParameterDTO.ContainerName.ShouldBeNullOrEmpty();
         _expressionParameterDTO.CompartmentName.ShouldBeEqualTo(Compartment.BLOOD_CELLS);
         _expressionParameterDTO.MoleculeName.ShouldBeEqualTo(_molecule.Name);
         _expressionParameterDTO.GroupName.ShouldBeEqualTo(Groups.VASCULAR_SYSTEM);
      }
   }

   public class When_mapping_an_global_vascular_parameter_expression_parameter_to_expression_parameter_dto : concern_for_ExpressionParameterMapper
   {
      protected override void Context()
      {
         base.Context();
         _parameter.Name = Parameters.FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME;
         A.CallTo(() => _expressionParameterTask.ExpressionGroupFor(_parameter)).Returns(Groups.VASCULAR_SYSTEM);
      }

      [Observation]
      public void should_return_the_expected_properties()
      {
         _expressionParameterDTO.ContainerName.ShouldBeNullOrEmpty();
         _expressionParameterDTO.CompartmentName.ShouldBeEqualTo(Compartment.VASCULAR_ENDOTHELIUM);
         _expressionParameterDTO.MoleculeName.ShouldBeEqualTo(_molecule.Name);
         _expressionParameterDTO.GroupName.ShouldBeEqualTo(Groups.VASCULAR_SYSTEM);
      }
   }


   public class When_mapping_a_local_parameter_expression_parameter_to_expression_parameter_dto : concern_for_ExpressionParameterMapper
   {
      protected override void Context()
      {
         base.Context();
         _parameter.Name = Constants.Parameters.REL_EXP;
         _molecule.WithParentContainer(_compartment);
         A.CallTo(() => _expressionParameterTask.ExpressionGroupFor(_parameter)).Returns(Groups.ORGANS_AND_TISSUES);
      }

      [Observation]
      public void should_return_the_expected_properties()
      {
         _expressionParameterDTO.ContainerName.ShouldBeEqualTo(_organ.Name);
         //parameter not in lumen directly under organism
         _expressionParameterDTO.CompartmentName.ShouldBeNullOrEmpty();
         _expressionParameterDTO.MoleculeName.ShouldBeEqualTo(_molecule.Name);
         _expressionParameterDTO.GroupName.ShouldBeEqualTo(Groups.ORGANS_AND_TISSUES);
      }
   }

   public class When_mapping_a_local_parameter_expression_parameter_in_lumen_to_expression_parameter_dto : concern_for_ExpressionParameterMapper
   {
      protected override void Context()
      {
         base.Context();
         //Ensure that the organ is in lumen
         _organ.Name = Organ.LUMEN;
         _parameter.Name = Constants.Parameters.REL_EXP;
         _molecule.WithParentContainer(_compartment);
         A.CallTo(() => _expressionParameterTask.ExpressionGroupFor(_parameter)).Returns(Groups.ORGANS_AND_TISSUES);
      }

      [Observation]
      public void should_return_the_expected_properties()
      {
         _expressionParameterDTO.ContainerName.ShouldBeEqualTo(_organ.Name);
         _expressionParameterDTO.CompartmentName.ShouldBeEqualTo(_compartment.Name);
         _expressionParameterDTO.MoleculeName.ShouldBeEqualTo(_molecule.Name);
         _expressionParameterDTO.GroupName.ShouldBeEqualTo(Groups.ORGANS_AND_TISSUES);
      }
   }
}