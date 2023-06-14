using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Presenters.AdvancedParameters;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.AdvancedParameters;


namespace PKSim.Presentation
{
   public abstract class concern_for_AdvancedParameterPresenter : ContextSpecification<IAdvancedParameterPresenter>
   {
      private IEditDistributionParametersPresenter _distributionPresenter;
      private IAdvancedParameterView _view;
      private IAdvancedParameterToAdvancedParameterDTOMapper _mapper;

      protected override void Context()
      {
         _view= A.Fake<IAdvancedParameterView>();
         _mapper= A.Fake<IAdvancedParameterToAdvancedParameterDTOMapper>();
         _distributionPresenter= A.Fake<IEditDistributionParametersPresenter>(); 
         sut = new AdvancedParameterPresenter(_view,_mapper,_distributionPresenter);
      }
   }

   public class When_retrieving_the_available_distribution_type_for_an_advanced_parameter : concern_for_AdvancedParameterPresenter
   {
      private AdvancedParameterDTO _normalDTO;
      private AdvancedParameterDTO _unknownParameter;

      protected override void Context()
      {
         base.Context();
         _normalDTO = new AdvancedParameterDTO();
         _unknownParameter = new AdvancedParameterDTO {DistributionType = DistributionType.Unknown};
      }

      [Observation]
      public void should_return_all_distribution_but_the_unknown_type_for_a_standard_parameter()
      {
         sut.AllDistributions(_normalDTO).ShouldOnlyContain(DistributionType.Normal,DistributionType.LogNormal, DistributionType.Uniform,DistributionType.Discrete);
      }

      [Observation]
      public void should_return_all_distribution_with_the_unknown_type_for_a_parameter_already_using_the_unknown_distribution()
      {
         sut.AllDistributions(_unknownParameter).ShouldOnlyContain(DistributionType.Normal, DistributionType.LogNormal, DistributionType.Uniform, DistributionType.Discrete, DistributionType.Unknown);

      }
   }
}	