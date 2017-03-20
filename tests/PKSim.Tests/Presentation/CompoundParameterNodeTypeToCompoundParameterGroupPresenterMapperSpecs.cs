using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using FakeItEasy;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper : ContextSpecification<CompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper>
   {
      private IContainer _container;

      protected override void Context()
      {
         _container = A.Fake<IContainer>();
         sut = new CompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper(_container);
      }
   }

   public class when_node_type_is_distribution_calculation : concern_for_CompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper
   {
      private ICompoundParameterGroupPresenter _result;

      protected override void Because()
      {
         _result = sut.MapFrom(CompoundParameterNodeType.DistributionCalculation);
      }

      [Observation]
      public void presenter_should_be_of_type_intenstinal_permeability()
      {
         _result.ShouldBeAnInstanceOf<IDistributionWithCalculationMethodGroupPresenter>();
      }
   }

   public class when_node_type_is_intestinal_permeability : concern_for_CompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper
   {
      private ICompoundParameterGroupPresenter _result;

      protected override void Because()
      {
         _result = sut.MapFrom(CompoundParameterNodeType.SpecificIntestinalPermeability);
      }

      [Observation]
      public void presenter_should_be_of_type_intenstinal_permeability()
      {
         _result.ShouldBeAnInstanceOf<IIntestinalPermeabilityWithCalculationMethodPresenter>();
      }
   }

   public class when_node_type_is_not_recognized : concern_for_CompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper
   {
      private ICompoundParameterGroupPresenter _result;

      protected override void Because()
      {
         _result = sut.MapFrom(null);
      }

      [Observation]
      public void result_should_be_null()
      {
         _result.ShouldBeNull();
      }
   }
}
