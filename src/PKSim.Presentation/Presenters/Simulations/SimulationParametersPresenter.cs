using PKSim.Assets;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationParametersPresenter : ISubPresenter
   {
   }

   public abstract class SimulationParametersPresenter : AbstractSubPresenter<ISimulationParametersView, ISimulationParametersPresenter>, ISimulationParametersPresenter
   {
      protected readonly IParameterGroupsPresenter _parameterGroupsPresenter;

      protected SimulationParametersPresenter(ISimulationParametersView view, IParameterGroupsPresenter parameterGroupsPresenter)
         : base(view)
      {
         _parameterGroupsPresenter = parameterGroupsPresenter;
         _parameterGroupsPresenter.NoSelectionCaption = PKSimConstants.Information.NoParametersInSimulationSelection;
         _view.AddParametersView(_parameterGroupsPresenter.View);
         AddSubPresenters(parameterGroupsPresenter);
      }

      public override bool CanClose => _parameterGroupsPresenter.CanClose;
   }
}