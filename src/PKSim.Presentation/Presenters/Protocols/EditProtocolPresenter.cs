using System;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.Protocols;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Events;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.Presenters.Protocols
{
   public interface IEditProtocolPresenter : IEditBuildingBockPresenter<Protocol>, IProtocolPresenter,
      IListener<ProtocolModeChangedEvent>,
      IListener<RollBackFinishedEvent>
   {
   }

   public class EditProtocolPresenter : SingleStartContainerPresenter<IEditProtocolView, IEditProtocolPresenter, Protocol, IProtocolItemPresenter>, IEditProtocolPresenter
   {
      private readonly IProtocolChartPresenter _protocolChartPresenter;
      private readonly ISchemaTask _schemaTask;
      private readonly IProtocolUpdater _protocolUpdater;
      private readonly IDialogCreator _dialogCreator;
      private Protocol _protocol;

      private ProtocolMode _protocolMode;

      public EditProtocolPresenter(IEditProtocolView view, ISubPresenterItemManager<IProtocolItemPresenter> subPresenterItemManager,
         IProtocolChartPresenter protocolChartPresenter, ISchemaTask schemaTask,
         IProtocolUpdater protocolUpdater, IDialogCreator dialogCreator)
         : base(view, subPresenterItemManager, ProtocolItems.All)
      {
         _protocolChartPresenter = protocolChartPresenter;
         _schemaTask = schemaTask;
         _protocolUpdater = protocolUpdater;
         _dialogCreator = dialogCreator;
         _view.UpdateChartControl(_protocolChartPresenter.View);
      }

      private void subPresenterChanged(object sender, EventArgs eventArgs)
      {
         plotProtocol();
      }

      private void plotProtocol()
      {
         _protocolChartPresenter.PlotProtocol(_protocol);
      }

      public override void InitializeWith(ICommandCollector commandRegister)
      {
         base.InitializeWith(commandRegister);
         _subPresenterItemManager.AllSubPresenters.Each(presenter => presenter.StatusChanged += subPresenterChanged);
      }

      public override object Subject
      {
         get { return _protocol; }
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         _subPresenterItemManager.AllSubPresenters.Each(presenter => presenter.StatusChanged -= subPresenterChanged);
         base.ReleaseFrom(eventPublisher);
      }

      public override void Edit(Protocol protocol)
      {
         if (!Equals(protocol, _protocol))
         {
            setProtocol(protocol);
            UpdateCaption();
         }
         _view.Display();
      }

      private void setProtocol(Protocol protocol)
      {
         _protocol = protocol;
         _protocolMode = modeFrom(protocol);
         _view.SetProtocolMode(_protocolMode);
         activeProtocolPresenter.EditProtocol(_protocol);
         _view.UpdateEditControl(activeProtocolView);
         plotProtocol();
      }

      public ProtocolMode ProtocolMode
      {
         get { return _protocolMode; }
         set
         {
            if (_protocolMode == value) return;
            _protocolMode = value;
            AddCommand(_schemaTask.SetProtocolMode(_protocol, _protocolMode));
         }
      }

      public bool SwitchModeConfirm(ProtocolMode protocolMode)
      {
         if (_protocolMode == protocolMode)
            return true;

         if (!_protocolUpdater.ValidateSwitchFrom(_protocol))
            return false;

         if (protocolMode == ProtocolMode.Advanced)
            return true;

         var result = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallySwitchProtocolMode);
         return result == ViewResult.Yes;
      }

      protected override void UpdateCaption()
      {
         _view.Caption = PKSimConstants.UI.EditProtocol(_protocol.Name);
      }

      private IView activeProtocolView
      {
         get { return activeProtocolPresenter.BaseView; }
      }

      private IProtocolItemPresenter activeProtocolPresenter
      {
         get
         {
            switch (ProtocolMode)
            {
               case ProtocolMode.Simple:
                  return PresenterAt(ProtocolItems.Simple);
               case ProtocolMode.Advanced:
                  return PresenterAt(ProtocolItems.Advanced);
               default:
                  throw new ArgumentOutOfRangeException();
            }
         }
      }

      private ProtocolMode modeFrom(Protocol protocol)
      {
         if (protocol.IsAnImplementationOf<SimpleProtocol>())
            return ProtocolMode.Simple;
         return ProtocolMode.Advanced;
      }

      public void Handle(ProtocolModeChangedEvent eventToHandle)
      {
         if (!Equals(eventToHandle.Protocol, _protocol)) return;
         setProtocol(eventToHandle.Protocol);
      }

      public void Handle(RollBackFinishedEvent eventToHandle)
      {
         //always update the plot after a roll back to ensure plot is updated
         plotProtocol();
      }
   }
}