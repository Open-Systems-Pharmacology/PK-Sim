using System.Text.RegularExpressions;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.UI.Services;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.ProteinExpression;
using PKSim.UI.Views.ProteinExpression;

namespace PKSim.UI
{
   public abstract class concern_for_ExpressionDataView : ContextSpecification<IExpressionDataView>
   {
      protected ExpressionDataFilter _genderFilter;
      protected string _discardedFilter;

      protected override void Context()
      {
         sut = CreateView();
         _genderFilter = new ExpressionDataFilter(new[] {new ExpressionDataFieldFilter(ExpressionDataFields.GENDER, ExpressionDataFilterType.Included, new object[] {"MALE"}, showBlanks: false)});
      }

      protected static IExpressionDataView CreateView()
      {
         var view = new ExpressionDataView(A.Fake<IImageListRetriever>(), A.Fake<IProteinExpressionToolTipCreator>());
         var query = new ExpressionQuery("CYP3A4", new[] {record("MALE", 1), record("FEMALE", 3), record(null, 20)}, new[] {new TissueContainerMapping("liver", "Liver")});
         var containerRecords = new ExpressionQueryCalculator().ContainerRecordsFor(query, new[] {new ExpressionContainerInfo("Liver", "Liver", 1)});
         view.SetData(query.ProteinName, new ExpressionDataTableMapper().DataTableFrom(containerRecords));
         return view;
      }

      private static ExpressionDataRecord record(string gender, double normValue) =>
         new ExpressionDataRecord {Database = "DB", Gender = gender, Tissue = "liver", NormValue = normValue, Unit = "EST"};

      protected static string WithLayoutFilterCriteria(string layoutSettings, string criteria)
      {
         var layoutWithoutCriteria = Regex.Replace(layoutSettings, @"\s*<property name=""ActiveFilter"".*?</property>\s*</property>", string.Empty, RegexOptions.Singleline);
         return layoutWithoutCriteria.Replace("<property name=\"Prefilter\"",
            $"<property name=\"ActiveFilter\" isnull=\"true\" iskey=\"true\"><property name=\"CriteriaString\">{criteria}</property></property><property name=\"Prefilter\"");
      }
   }

   public class When_setting_the_filter_of_the_expression_data_view : concern_for_ExpressionDataView
   {
      protected override void Because()
      {
         sut.SetFilter(_genderFilter);
      }

      [Observation]
      public void should_return_that_filter()
      {
         sut.GetFilter().ToString().ShouldBeEqualTo(_genderFilter.ToString());
      }
   }

   public class When_restoring_a_layout_saved_with_a_filter : concern_for_ExpressionDataView
   {
      private IExpressionDataView _restoredView;

      protected override void Context()
      {
         base.Context();
         sut.SetFilter(_genderFilter);
         _restoredView = CreateView();
      }

      protected override void Because()
      {
         _discardedFilter = _restoredView.SetLayoutSettings(sut.GetLayoutSettings());
      }

      [Observation]
      public void should_restore_the_filter_without_discarding_anything()
      {
         _discardedFilter.ShouldBeEmpty();
         _restoredView.GetFilter().ToString().ShouldBeEqualTo(_genderFilter.ToString());
      }
   }

   public class When_restoring_a_layout_whose_filter_criteria_repeat_its_field_filters : concern_for_ExpressionDataView
   {
      private IExpressionDataView _restoredView;
      private string _layout;

      protected override void Context()
      {
         base.Context();
         sut.SetFilter(_genderFilter);
         _layout = WithLayoutFilterCriteria(sut.GetLayoutSettings(), "[GENDER] = 'MALE' And [GENDER] In ('MALE')");
         _restoredView = CreateView();
      }

      protected override void Because()
      {
         _discardedFilter = _restoredView.SetLayoutSettings(_layout);
      }

      [Observation]
      public void should_keep_the_field_filters_and_discard_nothing()
      {
         _discardedFilter.ShouldBeEmpty();
         _restoredView.GetFilter().ToString().ShouldBeEqualTo(_genderFilter.ToString());
      }
   }

   public class When_restoring_a_layout_whose_filter_criteria_cannot_be_expressed_by_field_filters : concern_for_ExpressionDataView
   {
      private IExpressionDataView _restoredView;
      private string _layout;

      protected override void Context()
      {
         base.Context();
         _layout = WithLayoutFilterCriteria(sut.GetLayoutSettings(), "[NORM_VALUE] &gt; 5");
         _restoredView = CreateView();
      }

      protected override void Because()
      {
         _discardedFilter = _restoredView.SetLayoutSettings(_layout);
      }

      [Observation]
      public void should_return_the_discarded_criteria_and_remove_them()
      {
         _discardedFilter.ShouldBeEqualTo("[NORM_VALUE] > 5");
         _restoredView.GetFilter().FieldFilters.ShouldBeEmpty();
      }
   }
}
