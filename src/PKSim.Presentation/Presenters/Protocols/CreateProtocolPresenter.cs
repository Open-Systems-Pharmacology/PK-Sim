using System;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Protocols;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Protocols
{
   public interface ICreateProtocolPresenter : IProtocolPresenter, ICreateBuildingBlockPresenter<Protocol>
   {
   }

   public class CreateProtocolPresenter : AbstractSubPresenterContainerPresenter<ICreateProtocolView, ICreateProtocolPresenter, IProtocolItemPresenter>, ICreateProtocolPresenter
   {
      private readonly IBuildingBlockPropertiesMapper _propertiesMapper;
      private readonly IProtocolChartPresenter _protocolChartPresenter;
      private readonly IProtocolFactory _protocolFactory;
      private readonly IProtocolUpdater _protocolUpdater;
      private readonly IProtocolToProtocolPropertiesDTOMapper _protocolPropertiesDTOMapper;
      private Protocol _protocol;
      private ProtocolMode _protocolMode;
      private ObjectBaseDTO _protocolPropertiesDTO;

      public CreateProtocolPresenter(ICreateProtocolView view,
         ISubPresenterItemManager<IProtocolItemPresenter> subPresenterItemManager,
         IBuildingBlockPropertiesMapper propertiesMapper,
         IProtocolChartPresenter protocolChartPresenter,
         IProtocolFactory protocolFactory,
         IProtocolUpdater protocolUpdater,
         IProtocolToProtocolPropertiesDTOMapper protocolPropertiesDTOMapper, IDialogCreator dialogCreator)
         : base(view, subPresenterItemManager, ProtocolItems.All, dialogCreator)
      {
         _propertiesMapper = propertiesMapper;
         _protocolChartPresenter = protocolChartPresenter;
         _protocolFactory = protocolFactory;
         _protocolUpdater = protocolUpdater;
         _protocolPropertiesDTOMapper = protocolPropertiesDTOMapper;
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
        _subPresenterItemManager.AllSubPresenters.Each(pres =>
         {
            pres.StatusChanged += subPresenterChanged;
            pres.StatusChanging += subPresenterChanging;
         });

         ProtocolMode = ProtocolMode.Simple;
         _protocolPropertiesDTO = _protocolPropertiesDTOMapper.MapFrom(_protocol);
         _view.UpdateChartControl(_protocolChartPresenter.View);
         _view.BindToProperties(_protocolPropertiesDTO);
         refreshPlot();
      }

      public IPKSimCommand Create()
      {
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         _propertiesMapper.MapProperties(_protocolPropertiesDTO, _protocol);
         return _macroCommand;
      }

      public ProtocolMode ProtocolMode
      {
         get => _protocolMode;
         set
         {
            _protocolMode = value;
            var oldProtocol = _protocol;
            _protocol = _protocolFactory.Create(_protocolMode);
            _macroCommand.Clear();
            _protocolUpdater.UpdateProtocol(oldProtocol, _protocol);
            updateView();
         }
      }

      private void refreshPlot()
      {
         _protocolChartPresenter.PlotProtocol(_protocol);
      }

      private void subPresenterChanged(object sender, EventArgs eventArgs)
      {
         subPresenterChanging();
         refreshPlot();
      }

      private void subPresenterChanging()
      {
         updateControls();
      }

      public Protocol BuildingBlock => _protocol;

      public bool SwitchModeConfirm(ProtocolMode protocolMode)
      {
         if (!_protocolUpdater.ValidateSwitchFrom(_protocol))
            return false;

         if (protocolMode == ProtocolMode.Advanced)
            return true;

         var result = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallySwitchProtocolMode);
         return result == ViewResult.Yes;
      }

      private void updateView()
      {
         activeProtocolPresenter.EditProtocol(_protocol);
         _view.UpdateEditControl(activeProtocolView);
         updateControls();
      }

      public override void ViewChanged()
      {
         updateControls();
      }

      private void updateControls()
      {
         _view.OkEnabled = CanClose && activeProtocolPresenter.CanClose;
         _view.SimpleProtocolEnabled = true;
      }

      private IView activeProtocolView => activeProtocolPresenter.BaseView;

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
   }
}