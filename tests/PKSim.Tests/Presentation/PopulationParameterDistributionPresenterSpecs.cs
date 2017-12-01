using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.Populations;
using DistributionSettings = PKSim.Core.Chart.DistributionSettings;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationParameterDistributionPresenter : ContextSpecification<IPopulationDistributionPresenter>
   {
      protected IPopulationParameterDistributionView _view;
      protected IDistributionDataCreator _distributionDataCreator;
      private IRepresentationInfoRepository _representationInfoRepository;
      protected IDisplayUnitRetriever _displayUnitRetriever;
      protected IPKParameterRepository _pkParameterRepository;
      protected IApplicationSettings _applicationSettings;

      protected override void Context()
      {
         _view = A.Fake<IPopulationParameterDistributionView>();
         _distributionDataCreator = A.Fake<IDistributionDataCreator>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _displayUnitRetriever = A.Fake<IDisplayUnitRetriever>();
         _pkParameterRepository = A.Fake<IPKParameterRepository>();
         _applicationSettings = A.Fake<IApplicationSettings>();
         sut = new PopulationDistributionPresenter(_view, _distributionDataCreator, _representationInfoRepository, _displayUnitRetriever, _pkParameterRepository, _applicationSettings);
      }
   }

   public class When_ploting_the_distribution_for_a_pk_parameter : concern_for_PopulationParameterDistributionPresenter
   {
      private IPopulationDataCollector _population;
      private QuantityPKParameter _pkParameter;
      private DistributionSettings _settings;
      private Unit _displayUnit;
      private ContinuousDistributionData _data;

      protected override void Context()
      {
         base.Context();
         _population = A.Fake<IPopulationDataCollector>();
         _pkParameter = new QuantityPKParameter {Dimension = A.Fake<IDimension>(), Name = "toto", QuantityPath = "A|B|C"};
         A.CallTo(() => _pkParameterRepository.DisplayNameFor(_pkParameter.Name)).Returns("totoDisplay");
         _settings = new DistributionSettings();
         _displayUnit = A.Fake<Unit>();
         _data = new ContinuousDistributionData(AxisCountMode.Count, 10);
         A.CallTo(() => _displayUnitRetriever.PreferredUnitFor(_pkParameter.Dimension)).Returns(_displayUnit);
         A.CallTo(() => _distributionDataCreator.CreateFor(_population, _settings, _pkParameter, _pkParameter.Dimension, _displayUnit)).Returns(_data);
      }

      protected override void Because()
      {
         sut.Plot(_population, _pkParameter, _settings);
      }

      [Observation]
      public void should_retrieve_the_preferred_display_unit_of_the_parameter_and_use_it()
      {
         A.CallTo(() => _view.Plot(_data, _settings)).MustHaveHappened();
      }

      [Observation]
      public void should_use_the_parameter_display_name_as_x_axis_title()
      {
         _settings.XAxisTitle.Contains("totoDisplay").ShouldBeTrue();
      }

      [Observation]
      public void should_use_the_quantity_path_as_plot_title()
      {
         _settings.PlotCaption.ShouldBeEqualTo(_pkParameter.QuantityPath);
      }
   }

   public class When_copying_the_population_distribution_to_clipboard : concern_for_PopulationParameterDistributionPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _applicationSettings.WatermarkTextToUse).Returns("HELLO");
      }

      protected override void Because()
      {
         sut.CopyToClipboard();
      }

      [Observation]
      public void should_use_the_watermak_defined_in_the_application_settings()
      {
         A.CallTo(() => _view.CopyToClipboard(_applicationSettings.WatermarkTextToUse)).MustHaveHappened();
      }
   }
}