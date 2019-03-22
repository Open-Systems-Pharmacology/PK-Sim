using DevExpress.Utils;
using FakeItEasy;
using NUnit.Framework;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.UI.Binders;
using PKSim.UI.Views.PopulationAnalyses;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.UI.Services;
using PKSim.Core.Chart;

using PKSim.Presentation.Presenters.PopulationAnalyses;

namespace PKSim.UI.Tests
{
   public abstract class concern_for_ChartDataBinder : ContextSpecification<IChartsDataBinder<BoxWhiskerXValue, BoxWhiskerYValue>>
   {
      protected BoxWhiskerChartView _view;
      private IImageListRetriever _imageListRetriever;
      private IToolTipCreator _toolTipCreator;

      protected override void Context()
      {
         _imageListRetriever = A.Fake<IImageListRetriever>();
         _toolTipCreator = A.Fake<IToolTipCreator>();
         A.CallTo(() => _imageListRetriever.AllImages16x16).Returns(new ImageCollection());
         _view = new BoxWhiskerChartView(_imageListRetriever, _toolTipCreator);
         sut = _view.ChartsDataBinder;
      }
   }

   public class When_binding_to_an_undefined_chart : concern_for_ChartDataBinder
   {
      protected override void Because()
      {
         sut.Bind(null, new TimeProfileAnalysisChart());
      }

      [Test]
      public void should_clear_the_series()
      {
         _view.Chart.Series.Count.ShouldBeEqualTo(0);
      }
   }
}