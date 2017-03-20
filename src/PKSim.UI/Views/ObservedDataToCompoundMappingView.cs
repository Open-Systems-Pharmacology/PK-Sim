using System.Collections.Generic;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Assets;
using PKSim.Presentation.DTO.ObservedData;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views
{
   public partial class ObservedDataToCompoundMappingView : BaseModalView, IObservedDataToCompoundMappingView
   {
      private IObservedDataToCompoundMappingPresenter _presenter;
      private readonly UxRepositoryItemComboBox _compoundRepository;

      private readonly GridViewBinder<ObservedDataToCompoundMappingDTO> _gridViewBinder;

      public ObservedDataToCompoundMappingView(Shell shell) : base(shell)
      {
         InitializeComponent();
         _compoundRepository = new UxRepositoryItemComboBox(gridView);
         _gridViewBinder = new GridViewBinder<ObservedDataToCompoundMappingDTO>(gridView);
         gridView.AllowsFiltering = false;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _gridViewBinder.Bind(x => x.ObservedData)
            .WithCaption(PKSimConstants.UI.ObservedData)
            .AsReadOnly();

         _gridViewBinder.Bind(x => x.Compound)
            .WithRepository(dto => configureCompoundRepository())
            .WithShowButton(ShowButtonModeEnum.ShowAlways);
      }

      private RepositoryItem configureCompoundRepository()
      {
         _compoundRepository.FillComboBoxRepositoryWith(_presenter.AllCompounds());
         return _compoundRepository;
      }

      public void AttachPresenter(IObservedDataToCompoundMappingPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         lblDescription.AsDescription();
         lblDescription.Text = PKSimConstants.UI.ObservedDataMappingDescription;
         Text = PKSimConstants.UI.ObservedDataToCompoundMapping;
      }

      public void BindTo(IEnumerable<ObservedDataToCompoundMappingDTO> observedDataToCompoundMappingDtos)
      {
         _gridViewBinder.BindToSource(observedDataToCompoundMappingDtos);
         gridView.BestFitColumns();
      }
   }
}