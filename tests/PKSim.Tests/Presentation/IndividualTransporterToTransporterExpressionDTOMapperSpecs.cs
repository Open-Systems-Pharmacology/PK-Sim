using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualTransporterToTransporterExpressionDTOMapper : ContextSpecification<IIndividualTransporterToTransporterExpressionDTOMapper>
   {
      protected IExpressionParameterMapper<TransporterExpressionParameterDTO> _expressionParameterMapper;
      protected ITransportDirectionRepository _transporterDirectionRepository;
      protected ITransporterContainerTemplateRepository _transporterContainerTemplateRepository;

      protected override void Context()
      {
         _expressionParameterMapper = A.Fake<IExpressionParameterMapper<TransporterExpressionParameterDTO>>();
         _transporterContainerTemplateRepository = A.Fake<ITransporterContainerTemplateRepository>();
         _transporterDirectionRepository = A.Fake<ITransportDirectionRepository>();
         sut = new IndividualTransporterToTransporterExpressionDTOMapper(_expressionParameterMapper, _transporterDirectionRepository, _transporterContainerTemplateRepository);
      }
   }

   public class When_mapping_an_individual_transporter_to_individual_transporter_dto : concern_for_IndividualTransporterToTransporterExpressionDTOMapper
   {
      private IndividualTransporter _transporter;
      private ISimulationSubject _simulationSubject;
      private IndividualTransporterDTO _dto;
      private TransporterExpressionContainer _transporterExpressionContainer;
      private IParameter _expressionParameter;
      private TransporterExpressionParameterDTO _expressionParameterDTO;

      protected override void Context()
      {
         base.Context();
         _transporter = new IndividualTransporter().WithName("TRANS");
         _simulationSubject = A.Fake<ISimulationSubject>();
         _simulationSubject.Species.Name = "Human";
         var liver = new Container().WithName(CoreConstants.Organ.LIVER);
         var liverCell = new Container().WithName(CoreConstants.Compartment.INTRACELLULAR).WithParentContainer(liver);
         _transporterExpressionContainer = new TransporterExpressionContainer().WithParentContainer(liverCell);
         _expressionParameter = DomainHelperForSpecs.ConstantParameterWithValue(0.5).WithName(CoreConstants.Parameters.REL_EXP);
         _transporterExpressionContainer.Add(_expressionParameter);
         _expressionParameterDTO = new TransporterExpressionParameterDTO();
         A.CallTo(() => _expressionParameterMapper.MapFrom(_expressionParameter)).Returns(_expressionParameterDTO);
         A.CallTo(() => _transporterContainerTemplateRepository.HasTransporterTemplateFor(_simulationSubject.Species.Name, _transporter.Name)).Returns(true);

         A.CallTo(() => _simulationSubject.AllMoleculeContainersFor<TransporterExpressionContainer>(_transporter)).Returns(new[] {_transporterExpressionContainer});
      }

      protected override void Because()
      {
         _dto = sut.MapFrom(_transporter, _simulationSubject);
      }

      [Observation]
      public void should_return_a_dto_with_the_expected_loaded_from_database_flag()
      {
         _dto.DefaultAvailableInDatabase.ShouldBeTrue();
      }

      [Observation]
      public void should_have_added_one_expression_parameter_dto_for_each_expression_parameter()
      {
         _dto.AllExpressionParameters.ShouldContain(_expressionParameterDTO);
      }

      [Observation]
      public void should_have_set_the_expected_properties_in_the_expression_parameter_dto()
      {
         _expressionParameterDTO.TransporterExpressionContainer.ShouldBeEqualTo(_transporterExpressionContainer);
         _expressionParameterDTO.TransportDirection.Id.ShouldBeEqualTo(_transporterExpressionContainer.TransportDirection);
      }
   }
}