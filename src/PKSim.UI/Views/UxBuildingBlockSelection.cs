using System;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views
{
   public partial class UxBuildingBlockSelection : BaseUserControl
   {
      private IBuildingBlockSelectionPresenter _presenter;
      private bool _displayNotification = true;
      private bool _allowEmptySelection = false;
      public event Action BuildingBlockSelectionChanged = delegate { };

      public UxBuildingBlockSelection()
      {
         InitializeComponent();
      }

      public void InitializeFor<TBuildingBlock>() where TBuildingBlock : IPKSimBuildingBlock
      {
         _presenter = IoC.Resolve<IBuildingBlockSelectionPresenter<TBuildingBlock>>();
         _presenter.StatusChanged += raiseBuildingBlockSelectionChanged;
         _presenter.DisplayNotification = _displayNotification;
         _presenter.AllowEmptySelection = _allowEmptySelection;
         lblDesigner.Visible = false;
         this.FillWith(_presenter.View);
      }

      private void raiseBuildingBlockSelectionChanged(object sender, EventArgs e)
      {
         OnEvent(BuildingBlockSelectionChanged);
      }

      private void cleanup()
      {
         if (_presenter == null)
            return;

         _presenter.StatusChanged -= raiseBuildingBlockSelectionChanged;
         _presenter = null;
      }

      public bool DisplayNotification
      {
         set
         {
            _displayNotification = value;
            _presenter.DisplayNotification = _displayNotification;
         }
      }

      /// <summary>
      ///    Can the user let a selection empty?
      /// </summary>
      public bool AllowEmptySelection
      {
         set
         {
            _allowEmptySelection = value;
            if (_presenter != null)
               _presenter.AllowEmptySelection = _allowEmptySelection;
         }
         get => _allowEmptySelection;
      }

      public void BindTo<TBuildingBlock>(TBuildingBlock buildingBlock) where TBuildingBlock : IPKSimBuildingBlock
      {
         _presenter.Edit(buildingBlock);
      }

      public IPKSimBuildingBlock BuildingBlock => _presenter.BuildingBlock;

      public string BuildingBlockType => _presenter.BuildingBlockType;

      public override bool HasError => !_presenter.CanClose;

      public Func<IPKSimBuildingBlock, bool> ExtraFilter
      {
         set
         {
            if(_presenter!=null)
               _presenter.ExtraFilter = value;

         }
      }
   }
}