using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICompoundAdvancedParameterGroupPresenter : ISubPresenter

   {
      /// <summary>
      ///    Edit the alternative defined in the compound to edit for the group
      /// </summary>
      void EditCompound(PKSim.Core.Model.Compound compound);
   }

   public abstract class CompoundAdvancedParameterGroupPresenter<TView> : AbstractSubPresenter<TView, ICompoundAdvancedParameterGroupPresenter>, ICompoundAdvancedParameterGroupPresenter where TView : ICompoundAdvancedParameterGroupView
   {
      private readonly IMultiParameterEditPresenter _parameterEditPresenter;
      private readonly IParameterGroupTask _parameterGroupTask;
      private readonly string _groupName;

      protected CompoundAdvancedParameterGroupPresenter(TView view, IRepresentationInfoRepository representationInfoRepository,
                                                        IMultiParameterEditPresenter parameterEditPresenter, IParameterGroupTask parameterGroupTask,
                                                        string groupName)
         : base(view)
      {
         _parameterEditPresenter = parameterEditPresenter;
         _parameterGroupTask = parameterGroupTask;
         _parameterEditPresenter.IsSimpleEditor = true;
         _groupName = groupName;
         View.Caption = representationInfoRepository.DisplayNameFor(RepresentationObjectType.GROUP, _groupName);
         View.SetParameterView(_parameterEditPresenter.View);
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _parameterEditPresenter.InitializeWith(commandCollector);
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _parameterEditPresenter.ReleaseFrom(eventPublisher);
      }

      public virtual void EditCompound(Compound compound)
      {
         _parameterEditPresenter.Edit(_parameterGroupTask.ParametersIn(_groupName, compound.AllParameters()));
         _view.AdjustHeight();
      }
   }
}