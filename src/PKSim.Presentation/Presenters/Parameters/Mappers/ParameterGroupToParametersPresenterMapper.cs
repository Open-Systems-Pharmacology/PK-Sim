using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Presentation.Presenters.Applications;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Presenters.Events;
using PKSim.Presentation.Presenters.Formulations;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Core.Domain;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation.Presenters.Parameters.Mappers
{
   public interface IParameterGroupToCustomizableParametersPresenter : IMapper<IGroup, ICustomParametersPresenter>
   {
   }

   public class ParameterGroupToCustomizableParametersPresenter : IParameterGroupToCustomizableParametersPresenter
   {
      private readonly IContainer _container;

      public ParameterGroupToCustomizableParametersPresenter(IContainer container)
      {
         _container = container;
      }

      public ICustomParametersPresenter MapFrom(IGroup group)
      {
         if (group.IsNamed(CoreConstants.Groups.SIMULATION_SETTINGS))
            return _container.Resolve<IEditOutputSchemaPresenter>();

         if (group.IsNamed(CoreConstants.Groups.SOLVER_SETTINGS))
            return _container.Resolve<IEditSolverSettingsPresenter>();

         if (group.IsNamed(CoreConstants.Groups.FAVORITES))
            return _container.Resolve<IFavoriteParametersPresenter>();

         if (group.IsNamed(CoreConstants.Groups.USER_DEFINED))
            return _container.Resolve<IUserDefinedParametersPresenter>();

         if (group.IsNamed(CoreConstants.Groups.BLOOD_FLOW_RATES))
            return _container.Resolve<IBloodFlowRatesParametersPresenter>();

         if (group.IsNamed(CoreConstants.Groups.FORMULATIONS))
            return _container.Resolve<IFormulationParametersPresenter>();

         if (group.IsNamed(CoreConstants.Groups.EVENTS))
            return _container.Resolve<IEventParametersPresenter>();

         if (group.IsNamed(CoreConstants.Groups.ALL))
            return _container.Resolve<IAllParametersPresenter>();

         if (group.NameIsOneOf(CoreConstants.Groups.AllSimulationCompoundGroups))
            return _container.Resolve<ISimulationCompoundParametersPresenter>();

         //dynamic parameters
         if (group.Name.StartsWith(CoreConstants.Groups.COMPOUND_PROCESS_ITEM))
         {
            var presenter = _container.Resolve<IParametersByGroupPresenter>();
            presenter.ShowFavorites = true;
            presenter.HeaderVisible = true;
            return presenter;
         }

         if (group.Name.StartsWith(CoreConstants.Groups.RELATIVE_EXPRESSION_ITEM))
            return _container.Resolve<ISimulationExpressionsPresenter>();

         if (group.Name.StartsWith(CoreConstants.Groups.COMPOUND_ITEM))
            return _container.Resolve<ICompoundInSimulationPresenter>();

         if (group.Name.StartsWith(CoreConstants.Groups.PROTOCOL_ITEM))
            return _container.Resolve<IApplicationParametersPresenter>();
         
         //no need to display parameters in relative expressions group. They will be displayed per molecule 
         if (group.IsNamed(CoreConstants.Groups.RELATIVE_EXPRESSION))
            return null;
      
         //no need to display parameters in Compound group. They will be displayed per molecule 
         if (group.IsNamed(CoreConstants.Groups.COMPOUND))
            return null;

         //no need to display parameters in Protocol group. They will be displayed per molecule 
         if (group.IsNamed(CoreConstants.Groups.PROTOCOL))
            return null;

         return _container.Resolve<IMultiParameterEditPresenter>();
      }
   }
}