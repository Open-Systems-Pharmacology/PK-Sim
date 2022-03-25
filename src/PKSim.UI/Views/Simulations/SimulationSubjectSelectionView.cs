using System;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Assets;
using OSPSuite.Presentation;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationSubjectSelectionView : BaseModalView, ISimulationSubjectSelectionView
   {
      private ISimulationSubjectSelectionPresenter _presenter;

      public SimulationSubjectSelectionView(Shell shell): base(shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(ISimulationSubjectSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.UI.SimulationSubjectSelection;
         ApplicationIcon = ApplicationIcons.Simulation;
         radioGroupSimulationSubject.Properties.AllowMouseWheel = false;
         radioGroupSimulationSubject.Properties.Items.AddRange(new[]
                                                                  {
                                                                     new RadioGroupItem(typeof (PKSim.Core.Model.Individual), PKSimConstants.ObjectTypes.Individual),
                                                                     new RadioGroupItem(typeof ( PKSim.Core.Model.Population), PKSimConstants.ObjectTypes.Population)
                                                                  });
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         radioGroupSimulationSubject.SelectedIndexChanged += simulationSubjectChanged;
      }

      private void simulationSubjectChanged(object sender, EventArgs e)
      {
         var radioGroup = sender as RadioGroup;
         if (radioGroup == null) return;
         _presenter.SimulationSubjetType = (Type) radioGroup.Properties.Items[radioGroup.SelectedIndex].Value;
      }

      public Type SimulationSubjectType
      {
         set => radioGroupSimulationSubject.EditValue = value;
      }
   }
}