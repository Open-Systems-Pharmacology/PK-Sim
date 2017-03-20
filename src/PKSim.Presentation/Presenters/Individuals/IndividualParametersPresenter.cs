using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualParametersPresenter : IIndividualItemPresenter
   {
   }

   public class IndividualParametersPresenter : AbstractSubPresenter<IIndividualParametersView, IIndividualParametersPresenter>, IIndividualParametersPresenter
   {
      private readonly IParameterGroupsPresenter _parameterGroupsPresenter;

      public IndividualParametersPresenter(IIndividualParametersView view, IParameterGroupsPresenter parameterGroupsPresenter) : base(view)
      {
         _parameterGroupsPresenter = parameterGroupsPresenter;
         _parameterGroupsPresenter.NoSelectionCaption = PKSimConstants.Information.NoParametersInIndividualSelection;
         _view.AddParametersView(_parameterGroupsPresenter.View);
      }

      public virtual void EditIndividual(Individual individual)
      {
         _parameterGroupsPresenter.InitializeWith(individual, x => x.GroupName != CoreConstants.Groups.RELATIVE_EXPRESSION);
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _parameterGroupsPresenter.InitializeWith(commandCollector);
         _parameterGroupsPresenter.StatusChanged += OnStatusChanged;
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _parameterGroupsPresenter.ReleaseFrom(eventPublisher);
         _parameterGroupsPresenter.StatusChanged -= OnStatusChanged;
      }

      public override bool CanClose
      {
         get { return base.CanClose && _parameterGroupsPresenter.CanClose; }
      }
   }
}