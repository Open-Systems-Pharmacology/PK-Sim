using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
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

   public class When_retreving_the_availalbe_distribution_type_for_an_advanced_parameter : concern_for_AdvancedParameterPresenter
   {
      private AdvancedParameterDTO _normalDTO;
      private AdvancedParameterDTO _unknowParameter;

      protected override void Context()
      {
         base.Context();
         _normalDTO = new AdvancedParameterDTO();
         _unknowParameter = new AdvancedParameterDTO {DistributionType = DistributionTypes.Unknown};
      }

      [Observation]
      public void should_return_all_distributuon_but_the_unknown_type_for_a_standard_parameter()
      {
         sut.AllDistributions(_normalDTO).ShouldOnlyContain(DistributionTypes.Normal,DistributionTypes.LogNormal, DistributionTypes.Uniform,DistributionTypes.Discrete);
      }

      [Observation]
      public void should_return_all_distributuon_with_the_unknown_type_for_a_parameter_already_using_the_unkown_distribution()
      {
         sut.AllDistributions(_unknowParameter).ShouldOnlyContain(DistributionTypes.Normal, DistributionTypes.LogNormal, DistributionTypes.Uniform, DistributionTypes.Discrete, DistributionTypes.Unknown);

      }
   }
}	