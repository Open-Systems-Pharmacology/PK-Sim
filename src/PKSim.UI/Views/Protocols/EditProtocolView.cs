using System;
using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Views.Protocols;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Protocols
{
   public partial class EditProtocolView : BaseMdiChildView, IEditProtocolView
   {
      private RadioGroupItem _radioButtonSimple;

      public EditProtocolView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      public override ApplicationIcon ApplicationIcon
      {
         get { return ApplicationIcons.Protocol; }
      }

      public void AttachPresenter(IEditProtocolPresenter presenter)
      {
         _presenter = presenter;
      }

      public void UpdateChartControl(IView view)
      {
         splitter.Panel2.FillWith(view);
      }

      public void UpdateEditControl(IView view)
      {
         splitter.Panel1.FillWith(view);
      }

      public void SetProtocolMode(ProtocolMode protocolMode)
      {
         radioGroupProtocolMode.EditValue = protocolMode;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         radioGroupProtocolMode.SelectedIndexChanged += protocolModeChanged;
         radioGroupProtocolMode.EditValueChanging += protocolModeChanging;
      }

      private void protocolModeChanging(object sender, ChangingEventArgs e)
      {
         if (editProtocolPresenter.SwitchModeConfirm(e.NewValue.DowncastTo<ProtocolMode>()))
            return;

         e.Cancel = true;
      }

      private void protocolModeChanged(object sender, EventArgs e)
      {
         var radioGroup = sender as RadioGroup;
         if (radioGroup == null) return;
         editProtocolPresenter.ProtocolMode = (ProtocolMode) radioGroup.SelectedIndex;
      }

      public override void AddSubItemView(ISubPresenterItem subPresenterItem, IView viewToAdd)
      {
         UpdateEditControl(viewToAdd);
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
      }

      private IEditProtocolPresenter editProtocolPresenter => _presenter.DowncastTo<IEditProtocolPresenter>();
   }
}