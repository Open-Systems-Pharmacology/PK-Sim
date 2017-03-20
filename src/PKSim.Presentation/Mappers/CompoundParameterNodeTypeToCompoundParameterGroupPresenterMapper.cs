using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Compounds;

namespace PKSim.Presentation.Mappers
{
   public interface ICompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper : IMapper<CompoundParameterNodeType, ICompoundParameterGroupPresenter>
   {
   }

   public class CompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper : ICompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper 
   {
      private readonly IContainer _container;

      public CompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper(IContainer container)
      {
         _container = container;
      }

      public ICompoundParameterGroupPresenter MapFrom(CompoundParameterNodeType compoundParameterNodeType)
      {
         return getPresenter(compoundParameterNodeType);
      }

      private ICompoundParameterGroupPresenter getPresenter(CompoundParameterNodeType compoundParameterNodeType)
      {
         if (compoundParameterNodeType == CompoundParameterNodeType.SpecificIntestinalPermeability)
         {
            return _container.Resolve<IIntestinalPermeabilityWithCalculationMethodPresenter>();
         }
         if (compoundParameterNodeType == CompoundParameterNodeType.DistributionCalculation)
         {
            return _container.Resolve<IDistributionWithCalculationMethodGroupPresenter>();
         }
         return null;
      }
   }
}