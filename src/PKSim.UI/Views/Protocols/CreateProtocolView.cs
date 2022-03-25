using System;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Views.Protocols;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Protocols
{
   public partial class CreateProtocolView : BaseModalContainerView, ICreateProtocolView
   {
      private ICreateProtocolPresenter _presenter;
      private ScreenBinder<ObjectBaseDTO> _propertiesBinder;
      private RadioGroupItem _radioButtonSimple;

      public CreateProtocolView(Shell shell) : base(shell)
      {
         InitializeComponent();
         ClientSize = new Size(UIConstants.Size.PROTOCOL_VIEW_WIDTH, UIConstants.Size.PROTOCOL_VIEW_HEIGHT);
      }

      public void AttachPresenter(ICreateProtocolPresenter presenter)
      {
         _presenter = presenter;
         radioGroupProtocolMode.EditValue = _presenter.ProtocolMode;
      }

      protected override void SetActiveControl()
      {
         ActiveControl = tbName;
      }

      public override void InitializeBinding()
      {
         radioGroupProtocolMode.EditValueChanging += protocolModeChanging;
         radioGroupProtocolMode.SelectedIndexChanged += protocolModeChanged;
         _propertiesBinder = new ScreenBinder<ObjectBaseDTO>();
         _propertiesBinder.Bind(x => x.Name).To(tbName);
         RegisterValidationFor(_propertiesBinder, NotifyViewChanged);
      }

      private void protocolModeChanging(object sender, ChangingEventArgs e)
      {
         if (_presenter.SwitchModeConfirm(e.NewValue.DowncastTo<ProtocolMode>()))
            return;

         e.Cancel = true;
      }

      private void protocolModeChanged(object sender, EventArgs e)
      {
         var radioGroup = sender as RadioGroup;
         if (radioGroup == null) return;
         _presenter.ProtocolMode = (ProtocolMode) radioGroup.Properties.Items[radioGroup.SelectedIndex].Value;
      }

      public void UpdateEditControl(IView view)
      {
         splitContainer.Panel1.FillWith(view);
      }

      public void UpdateChartControl(IView view)
      {
         splitContainer.Panel2.FillWith(view);
      }

      public void BindToProperties(ObjectBaseDTO protocolPropertiesDTO)
      {
         _propertiesBinder.BindToSource(protocolPropertiesDTO);
      }

      protected override bool ShouldClose => _presenter.ShouldClose;

      public override bool HasError => _propertiesBinder.HasError;

      public bool SimpleProtocolEnabled
      {
         set => _radioButtonSimple.Enabled = value;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         _radioButtonSimple = new RadioGroupItem(ProtocolMode.Simple, PKSimConstants.UI.SimpleProtocolMode);
         radioGroupProtocolMode.Properties.AllowMouseWheel = false;
         radioGroupProtocolMode.Properties.Items.AddRange(new[]
         {
            _radioButtonSimple,
            new RadioGroupItem(ProtocolMode.Advanced, PKSimConstants.UI.AdvancedProtocolMode)
         });

         layoutItemName.Text = PKSimConstants.UI.Name.FormatForLabel();
         Caption = PKSimConstants.UI.CreateAdministrationProtocol;
         ApplicationIcon = ApplicationIcons.Protocol;
         this.ReziseForCurrentScreen(fractionHeight: SCREEN_RESIZE_FRACTION, fractionWidth: SCREEN_RESIZE_FRACTION);
      }

      public override void AddSubItemView(ISubPresenterItem subPresenterItem, IView viewToAdd)
      {
         UpdateEditControl(viewToAdd);
      }
   }
}