using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_GlobalPKAnalysisPresenter : ContextSpecification<GlobalPKAnalysisPresenter>
   {
      protected IGlobalPKAnalysisView _view;
      protected IGlobalPKAnalysisTask _globalPKAnalysisTask;
      protected IPresentationSettingsTask _presenterSettingsTask;
      protected string _compoundName;
      protected IReadOnlyList<Simulation> _simulations;
      protected GlobalPKAnalysis _globalPKAnalysis;
      protected ISimulationRunner _simulationRunner;

      protected override void Context()
      {
         _view = A.Fake<IGlobalPKAnalysisView>();
         _globalPKAnalysisTask = A.Fake<IGlobalPKAnalysisTask>();
         var globalPKAnalysisDTOMapper = A.Fake<IGlobalPKAnalysisToGlobalPKAnalysisDTOMapper>();
         var heavyWorkManager = A.Fake<IHeavyWorkManager>();
         var representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _presenterSettingsTask = A.Fake<IPresentationSettingsTask>();
         _simulationRunner = A.Fake<ISimulationRunner>();
         sut = new GlobalPKAnalysisPresenter(_view, _globalPKAnalysisTask, globalPKAnalysisDTOMapper, heavyWorkManager, representationInfoRepository, _presenterSettingsTask, _simulationRunner);

         _simulations = new List<Simulation>();
         _compoundName = "DRUG";
         _globalPKAnalysis = PKAnalysisHelperForSpecs.GenerateGlobalPKAnalysis(_compoundName);
         A.CallTo(() => _globalPKAnalysisTask.CalculateGlobalPKAnalysisFor(_simulations)).Returns(_globalPKAnalysis);
      }
   }

   public class when_pk_analysis_has_no_parameter : concern_for_GlobalPKAnalysisPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _globalPKAnalysisTask.CalculateGlobalPKAnalysisFor(_simulations)).Returns(new GlobalPKAnalysis());
         sut.CalculatePKAnalysis(_simulations);
      }

      [Observation]
      public void has_parameters_should_return_false()
      {
         sut.HasParameters().ShouldBeFalse();
      }
   }

   public class when_calculating_pk_analysis : concern_for_GlobalPKAnalysisPresenter
   {
      private IParameter _parameter;
      private Unit _preferredDisplayUnit;
      private DefaultPresentationSettings _settings;

      protected override void Context()
      {
         base.Context();
         _settings = new DefaultPresentationSettings();
         A.CallTo(() => _presenterSettingsTask.PresentationSettingsFor<DefaultPresentationSettings>(A<IPresenterWithSettings>._, A<IWithId>._)).Returns(_settings);
         sut.LoadSettingsForSubject(A.Fake<IWithId>());
         _parameter = A.Fake<IParameter>();
         A.CallTo(() => _parameter.Dimension).Returns(new Dimension(new BaseDimensionRepresentation(), "dimensionfake", "baseUnitFake"));
         _parameter.Dimension.AddUnit("newname", 1000, 0);
         _globalPKAnalysis.Container(_compoundName).Add(_parameter);


         _preferredDisplayUnit = _parameter.Dimension.Units.Last();
         _settings.SetSetting(_parameter.Name, _preferredDisplayUnit);
      }

      protected override void Because()
      {
         sut.CalculatePKAnalysis(_simulations);
      }

      [Observation]
      public void has_parameters_should_return_true()
      {
         sut.HasParameters().ShouldBeTrue();
      }

      [Observation]
      public void adjustments_to_the_preferred_display_units_must_be_made_before_the_view_is_rendered()
      {
         A.CallTo(_parameter).Where(x => x.Method.Name.Equals("set_DisplayUnit")).WhenArgumentsMatch(x => x.Get<Unit>(0).Equals(_preferredDisplayUnit)).MustHaveHappened();
         A.CallTo(() => _view.BindTo(A<GlobalPKAnalysisDTO>._)).MustHaveHappened();
      }
   }

   public class When_asked_of_the_pk_analysis_calculation_is_available_for_a_pk_parameter_that_was_not_calculated_yet : concern_for_GlobalPKAnalysisPresenter
   {
      protected override void Context()
      {
         base.Context();
         _globalPKAnalysis.Container(_compoundName).Add(new NullParameter().WithName(CoreConstants.PKAnalysis.Bioavailability));
         sut.CalculatePKAnalysis(_simulations);
      }

      [Observation]
      public void should_return_true()
      {
         sut.ShouldCalculateBioAvailability(_compoundName, CoreConstants.PKAnalysis.Bioavailability).ShouldBeTrue();
      }
   }

   public class When_asked_of_the_pk_analysis_calculation_is_available_for_all_simple_pk_parameter_that_were_not_calculated_yet : concern_for_GlobalPKAnalysisPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _globalPKAnalysisTask.CalculateGlobalPKAnalysisFor(_simulations)).Returns(PKAnalysisHelperForSpecs.GenerateEmptyGlobalPKAnalysis(_compoundName));
         sut.CalculatePKAnalysis(_simulations);
      }

      [Observation]
      public void should_return_true()
      {
         sut.ShouldCalculateAll().ShouldBeTrue();
      }
   }

   public class When_asked_of_the_pk_analysis_calculation_is_available_for_a_pk_parameter_was_already_calculated : concern_for_GlobalPKAnalysisPresenter
   {
      protected override void Context()
      {
         base.Context();
         _globalPKAnalysis.Container(_compoundName).Add(new PKSimParameter().WithName(CoreConstants.PKAnalysis.Bioavailability));
         sut.CalculatePKAnalysis(_simulations);
      }

      [Observation]
      public void should_return_true()
      {
         sut.ShouldCalculateBioAvailability(_compoundName, CoreConstants.PKAnalysis.Bioavailability).ShouldBeFalse();
      }
   }
}