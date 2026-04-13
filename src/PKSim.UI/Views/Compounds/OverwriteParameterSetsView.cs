using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Controls;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.UI.Views.Compounds;

public partial class OverwriteParameterSetsView : BaseUserControl, IOverwriteParameterSetsView
{
   private IOverwriteParameterSetsPresenter _presenter;
   private readonly GridViewBinder<OverwriteParameterSetDTO> _gridViewBinderSets;
   private readonly GridViewBinder<OverwriteParameterValueDTO> _gridViewBinderParameterValues;

   public OverwriteParameterSetsView()
   {
      InitializeComponent();

      _gridViewBinderSets = new GridViewBinder<OverwriteParameterSetDTO>(gridViewSets)
      {
         BindingMode = BindingMode.OneWay
      };

      _gridViewBinderParameterValues = new GridViewBinder<OverwriteParameterValueDTO>(gridViewParameterValues)
      {
         BindingMode = BindingMode.OneWay
      };

      gridViewSets.FocusedRowChanged += (o, e) => OnEvent(selectedSetChanged);
   }

   public void AttachPresenter(IOverwriteParameterSetsPresenter presenter)
   {
      _presenter = presenter;
   }

   public override void InitializeBinding()
   {
      _gridViewBinderSets.Bind(x => x.Name)
         .WithCaption(PKSimConstants.UI.Name);

      _gridViewBinderSets.Bind(x => x.IsDefault)
         .WithCaption(PKSimConstants.UI.IsDefault);

      _gridViewBinderSets.Bind(x => x.Species)
         .WithCaption(PKSimConstants.UI.Species);

      _gridViewBinderSets.Bind(x => x.DiseaseState)
         .WithCaption(PKSimConstants.UI.DiseaseState);

      _gridViewBinderParameterValues.Bind(x => x.Path)
         .WithCaption(Captions.Diff.ObjectPath);

      _gridViewBinderParameterValues.Bind(x => x.Value)
         .WithCaption(Captions.Value);

      _gridViewBinderParameterValues.Bind(x => x.Unit)
         .WithCaption(Captions.Unit);

      _gridViewBinderParameterValues.Bind(x => x.ValueOrigin)
         .WithCaption(Captions.ValueOrigin);
   }

   public override void InitializeResources()
   {
      base.InitializeResources();
      Caption = PKSimConstants.UI.OverwriteParameterSetsTabCaption;
      ApplicationIcon = ApplicationIcons.ParameterValues;
   }

   public void BindTo(IReadOnlyList<OverwriteParameterSetDTO> overwriteParameterSets) =>
      _gridViewBinderSets.BindToSource(overwriteParameterSets);

   private void selectedSetChanged() => bindToSelectedParameterValues();

   private void bindToSelectedParameterValues()
   {
      var selectedSet = _gridViewBinderSets.FocusedElement;
      if (selectedSet == null)
      {
         _gridViewBinderParameterValues.BindToSource([]);
         return;
      }

      _gridViewBinderParameterValues.BindToSource(selectedSet.ParameterValues);
   }
}