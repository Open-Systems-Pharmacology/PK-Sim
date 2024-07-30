using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Views.Parameters;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditTableSolubilityParameterPresenter : ContextSpecification<IEditTableSolubilityParameterPresenter>
   {
      protected IEditTableParameterView _view;
      protected ITableSolubilityParameterPresenter _tableParameterPresenter;
      protected IFullPathDisplayResolver _fullPathDisplayResolver;
      protected ISimpleChartPresenter _chartPresenter;

      protected override void Context()
      {
         _view = A.Fake<IEditTableParameterView>();
         _tableParameterPresenter = A.Fake<ITableSolubilityParameterPresenter>();
         _fullPathDisplayResolver = A.Fake<IFullPathDisplayResolver>();
         _chartPresenter = A.Fake<ISimpleChartPresenter>();

         sut = new EditTableSolubilityParameterPresenter(_view, _tableParameterPresenter, _fullPathDisplayResolver, _chartPresenter);
      }
   }

   public class When_creating_an_edit_table_solubility_parameter_presenter : concern_for_EditTableSolubilityParameterPresenter
   {
      [Observation]
      public void should_update_the_import_tooltip_in_the_table_parameter_presenter()
      {
         _tableParameterPresenter.ImportToolTip.ShouldBeEqualTo(PKSimConstants.UI.ImportSolubilityTable);
      }

      [Observation]
      public void should_update_the_import_description_in_the_table_parameter_presenter()
      {
         _tableParameterPresenter.Description.ShouldBeEqualTo(PKSimConstants.UI.ImportSolubilityTableDescription);
      }
   }
}