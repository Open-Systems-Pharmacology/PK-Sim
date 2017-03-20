using System.Collections.Generic;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Extensions;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using PKSim.UI.Binders;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class PopulationAnalysisFieldListView : BaseUserControl, IPopulationAnalysisFieldListView
   {
      private IPopulationAnalysisFieldListPresenter _presenter;
      private readonly GridViewBinder<PopulationAnalysisFieldDTO> _gridViewBinder;
      public IPopulationAnalysisFieldsDragDropBinder DragDropBinder { get; private set; }

      public PopulationAnalysisFieldListView()
      {
         InitializeComponent();
         _gridViewBinder = new GridViewBinder<PopulationAnalysisFieldDTO>(gridView);
         DragDropBinder = new PopulationAnalysisFieldsDragDropBinder(_gridViewBinder);
         gridView.CustomDrawEmptyForeground += (o, e) => OnEvent(addMessageInEmptyArea, e);
      }

      public void AttachPresenter(IPopulationAnalysisFieldListPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.Bind(x => x.Name).AsReadOnly();
      }

      public void BindTo(IEnumerable<PopulationAnalysisFieldDTO> populationAnalysisFields)
      {
         _gridViewBinder.BindToSource(populationAnalysisFields);
      }

      public string Description
      {
         get { return lblDescription.Text; }
         set { lblDescription.Text = value; }
      }

      private void addMessageInEmptyArea(CustomDrawEventArgs e)
      {
         gridView.AddMessageInEmptyArea(e, _presenter.DragAndDropMessage);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         lblDescription.AsDescription();
         gridView.AllowsFiltering = false;
         gridView.ShowColumnHeaders = false;
         gridView.ShowRowIndicator = false;
         gridView.ShouldUseColorForDisabledCell = false;
         gridView.MultiSelect = true;
      }
   }
}