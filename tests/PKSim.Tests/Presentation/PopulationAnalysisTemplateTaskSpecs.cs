using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Services;

using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using ISimulationAnalysisCreator = PKSim.Core.Services.ISimulationAnalysisCreator;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAnalysisTemplateTask : ContextSpecification<IPopulationAnalysisTemplateTask>
   {
      protected IDialogCreator _dialogCreator;
      protected ITemplateTask _templateTask;
      protected IKeyPathMapper _keyPathMapper;
      protected IEntitiesInContainerRetriever _entitiesInContainerRetriever;
      protected IEntityTask _entityTask;
      protected ICloner _cloner;
      protected IAnalysableToSimulationAnalysisWorkflowMapper _simulationAnalysisWorkflowMapper;
      protected ISimulationAnalysisCreator _simulationAnalysisCreator;
      protected ILazyLoadTask _lazyLoadTask;

      protected override void Context()
      {
         _dialogCreator = A.Fake<IDialogCreator>();
         _templateTask = A.Fake<ITemplateTask>();
         _keyPathMapper = A.Fake<IKeyPathMapper>();
         _entitiesInContainerRetriever = A.Fake<IEntitiesInContainerRetriever>();
         _entityTask = A.Fake<IEntityTask>();
         _cloner = A.Fake<ICloner>();
         _simulationAnalysisWorkflowMapper = A.Fake<IAnalysableToSimulationAnalysisWorkflowMapper>();
         _simulationAnalysisCreator = A.Fake<ISimulationAnalysisCreator>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         sut = new PopulationAnalysisTemplateTask(_templateTask, _dialogCreator, _entitiesInContainerRetriever,
            _keyPathMapper, _entityTask, _cloner, _simulationAnalysisWorkflowMapper, _simulationAnalysisCreator, _lazyLoadTask);
      }
   }

   public class When_saving_a_population_analysis_to_the_database : concern_for_PopulationAnalysisTemplateTask
   {
      private PopulationAnalysis _populationAnalysis;

      protected override void Context()
      {
         base.Context();
         _populationAnalysis = A.Fake<PopulationAnalysis>();
      }

      protected override void Because()
      {
         sut.SavePopulationAnalysis(_populationAnalysis);
      }

      [Observation]
      public void should_use_the_templating_service_to_save_the_population_analyse()
      {
         _templateTask.SaveToTemplate(_populationAnalysis, TemplateType.PopulationAnalysis, string.Empty);
      }
   }

   public class When_loading_a_population_analyses_that_matches_the_given_population_simulation : concern_for_PopulationAnalysisTemplateTask
   {
      private IPopulationDataCollector _populationDataCollector;
      private PopulationAnalysis _templatePopulationAnalysis;
      private PopulationPivotAnalysis _result;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _templatePopulationAnalysis = new PopulationPivotAnalysis();
         A.CallTo(_templateTask).WithReturnType<IReadOnlyList<PopulationAnalysis>>().Returns(new[] { _templatePopulationAnalysis });
      }

      protected override void Because()
      {
         _result = sut.LoadPopulationAnalysisFor<PopulationPivotAnalysis>(_populationDataCollector);
      }

      [Observation]
      public void should_return_the_population_from_template()
      {
         _result.ShouldBeEqualTo(_templatePopulationAnalysis);
      }
   }

   public class When_loading_a_population_analyses_that_fits_the_given_population_simulation_but_does_not_have_the_required_type : concern_for_PopulationAnalysisTemplateTask
   {
      private IPopulationDataCollector _populationDataCollector;
      private PopulationAnalysis _templatePopulationAnalysis;
      private PopulationPivotAnalysis _result;
      private IPopulationAnalysisField _field1;
      private IPopulationAnalysisField _field2;

      protected override void Context()
      {
         base.Context();
         _field1 = A.Fake<IPopulationAnalysisField>();
         _field2 = A.Fake<IPopulationAnalysisField>();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _templatePopulationAnalysis = A.Fake<PopulationAnalysis>();
         A.CallTo(() => _templatePopulationAnalysis.AllFields).Returns(new[] {_field1, _field2});
         A.CallTo(_templateTask).WithReturnType<IReadOnlyList<PopulationAnalysis>>().Returns(new[] { _templatePopulationAnalysis });
      }

      protected override void Because()
      {
         _result = sut.LoadPopulationAnalysisFor<PopulationPivotAnalysis>(_populationDataCollector);
      }

      [Observation]
      public void should_return_a_new_analyses_containing_the_field_defined_in_the_templates()
      {
         _result.ShouldBeAnInstanceOf<PopulationPivotAnalysis>();
      }
   }

   public class When_loading_a_population_analyses_for_a_population_simulation_with_multiple_output_resolving_to_the_same_key: concern_for_PopulationAnalysisTemplateTask
   {
      private IPopulationDataCollector _populationDataCollector;
      private PopulationAnalysis _templatePopulationAnalysis;
      private PopulationPivotAnalysis _result;
      private PathCache<IQuantity> _pathCacheQuantity;
      private IQuantity _quantity1;
      private IQuantity _quantity2;

      protected override void Context()
      {
         base.Context();
         _quantity1 = A.Fake<IQuantity>();
         _quantity2 = A.Fake<IQuantity>();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _templatePopulationAnalysis = A.Fake<PopulationAnalysis>();
         A.CallTo(_templateTask).WithReturnType<IReadOnlyList<PopulationAnalysis>>().Returns(new[] { _templatePopulationAnalysis });
         _pathCacheQuantity = new PathCacheForSpecs<IQuantity> { { "Path1", _quantity1 }, { "Path2", _quantity2 } };

         A.CallTo(() => _entitiesInContainerRetriever.OutputsFrom(_populationDataCollector)).Returns(_pathCacheQuantity);
         A.CallTo(() => _keyPathMapper.MapFrom("Path1", _quantity1.QuantityType, true)).Returns(new KeyPathMap(path: "Path"));
         A.CallTo(() => _keyPathMapper.MapFrom("Path2", _quantity2.QuantityType, true)).Returns(new KeyPathMap(path: "Path"));
      }

      protected override void Because()
      {
         _result = sut.LoadPopulationAnalysisFor<PopulationPivotAnalysis>(_populationDataCollector);
      }

      [Observation]
      public void should_return_a_new_analyses()
      {
         _result.ShouldBeAnInstanceOf<PopulationPivotAnalysis>();
      }
   }

   public class When_loading_a_population_analysis_with_parameter_not_available_in_the_population : concern_for_PopulationAnalysisTemplateTask
   {
      private IPopulationDataCollector _populationDataCollector;
      private PopulationPivotAnalysis _result;
      private PopulationPivotAnalysis _templatePopulationAnalysis;
      private PopulationAnalysisParameterField _parameterFound;
      private PopulationAnalysisParameterField _parameterNotFound;
      private PathCache<IParameter> _cache;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _templatePopulationAnalysis = new PopulationPivotAnalysis();

         _parameterFound = new PopulationAnalysisParameterField {ParameterPath = "P1"};
         _parameterNotFound = new PopulationAnalysisParameterField {ParameterPath = "P2"};

         _templatePopulationAnalysis.Add(_parameterFound);
         _templatePopulationAnalysis.Add(_parameterNotFound);

         _cache = new PathCacheForSpecs<IParameter> {{"P1", new PKSimParameter()}};

         A.CallTo(_templateTask).WithReturnType<IReadOnlyList<PopulationAnalysis>>().Returns(new[] { _templatePopulationAnalysis });

         A.CallTo(() => _entitiesInContainerRetriever.ParametersFrom(_populationDataCollector)).Returns(_cache);
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Yes);
      }

      protected override void Because()
      {
         _result = sut.LoadPopulationAnalysisFor<PopulationPivotAnalysis>(_populationDataCollector);
      }

      [Observation]
      public void should_remove_the_unknown_parameter()
      {
         _result.AllFields.ShouldOnlyContain(_parameterFound);
      }

      [Observation]
      public void should_notify_the_user_that_some_errors_occured_during_the_import()
      {
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().MustHaveHappened();
      }
   }

   public class When_loading_a_population_analysis_with_covariates_not_available_in_the_population : concern_for_PopulationAnalysisTemplateTask
   {
      private IPopulationDataCollector _populationDataCollector;
      private PopulationPivotAnalysis _result;
      private PopulationPivotAnalysis _templatePopulationAnalysis;
      private PopulationAnalysisCovariateField _covariateFound;
      private PopulationAnalysisCovariateField _covariateNotFound;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _templatePopulationAnalysis = new PopulationPivotAnalysis();

         _covariateFound = new PopulationAnalysisCovariateField {Covariate = "Covariate1"};
         _covariateNotFound = new PopulationAnalysisCovariateField {Covariate = "Covariate2"};

         _templatePopulationAnalysis.Add(_covariateFound);
         _templatePopulationAnalysis.Add(_covariateNotFound);

         A.CallTo(_templateTask).WithReturnType<IReadOnlyList<PopulationAnalysis>>().Returns(new[] { _templatePopulationAnalysis });

         A.CallTo(() => _populationDataCollector.AllCovariateNames).Returns(new List<string> {_covariateFound.Covariate});
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Yes);
      }

      protected override void Because()
      {
         _result = sut.LoadPopulationAnalysisFor<PopulationPivotAnalysis>(_populationDataCollector);
      }

      [Observation]
      public void should_remove_the_unknown_covariate()
      {
         _result.AllFields.ShouldOnlyContain(_covariateFound);
      }

      [Observation]
      public void should_notify_the_user_that_some_errors_occured_during_the_import()
      {
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().MustHaveHappened();
      }
   }

   public class When_loading_a_population_analysis_with_pk_parameter_not_available_in_the_population : concern_for_PopulationAnalysisTemplateTask
   {
      private IPopulationDataCollector _populationDataCollector;
      private PopulationPivotAnalysis _result;
      private PopulationPivotAnalysis _templatePopulationAnalysis;
      private PopulationAnalysisPKParameterField _pkParameterFound;
      private PopulationAnalysisPKParameterField _pkParameterNotFound;
      private PathCache<IQuantity> _cache;
      private PopulationAnalysisPKParameterField _quantityNotFound;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _templatePopulationAnalysis = new PopulationPivotAnalysis();

         _pkParameterFound = new PopulationAnalysisPKParameterField {QuantityType = QuantityType.Drug, PKParameter = "AUC", QuantityPath = "P1"};
         _quantityNotFound = new PopulationAnalysisPKParameterField {QuantityType = QuantityType.Drug, PKParameter = "AUC", QuantityPath = "P2"};
         _pkParameterNotFound = new PopulationAnalysisPKParameterField {QuantityType = QuantityType.Metabolite, PKParameter = "AUC", QuantityPath = "P3"};

         _templatePopulationAnalysis.Add(_pkParameterFound);
         _templatePopulationAnalysis.Add(_quantityNotFound);
         _templatePopulationAnalysis.Add(_pkParameterNotFound);

         _cache = new PathCacheForSpecs<IQuantity>
         {
            {"P1", new MoleculeAmount {QuantityType = QuantityType.Drug}},
            {"Q2", new MoleculeAmount {QuantityType = QuantityType.Drug}},
            {"P3", new MoleculeAmount {QuantityType = QuantityType.Metabolite}}
         };

         A.CallTo(() => _populationDataCollector.HasPKParameterFor("P1", "AUC")).Returns(true);
         A.CallTo(() => _populationDataCollector.HasPKParameterFor("P2", "AUC")).Returns(false);
         A.CallTo(() => _populationDataCollector.HasPKParameterFor("P3", "AUC")).Returns(false);

         A.CallTo(_templateTask).WithReturnType<IReadOnlyList<PopulationAnalysis>>().Returns(new []{_templatePopulationAnalysis });

         A.CallTo(() => _entitiesInContainerRetriever.OutputsFrom(_populationDataCollector)).Returns(_cache);
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Yes);

         A.CallTo(() => _keyPathMapper.MapFrom(_pkParameterFound.QuantityPath, QuantityType.Drug, true)).Returns(new KeyPathMap("P1"));
         A.CallTo(() => _keyPathMapper.MapFrom(_quantityNotFound.QuantityPath, QuantityType.Drug, true)).Returns(new KeyPathMap("QUANTITY NOT FOUND"));
         A.CallTo(() => _keyPathMapper.MapFrom(_pkParameterNotFound.QuantityPath, QuantityType.Metabolite, true)).Returns(new KeyPathMap("P3"));
      }

      protected override void Because()
      {
         _result = sut.LoadPopulationAnalysisFor<PopulationPivotAnalysis>(_populationDataCollector);
      }

      [Observation]
      public void should_remove_the_unknown_parameter()
      {
         _result.AllFields.ShouldOnlyContain(_pkParameterFound);
      }

      [Observation]
      public void should_notify_the_user_that_some_errors_occured_during_the_import()
      {
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().MustHaveHappened();
      }
   }

   public abstract class concern_for_LoadingDerivedFieldFromTemplate : concern_for_PopulationAnalysisTemplateTask
   {
      protected PopulationAnalysis _populationAnalysis;
      protected PopulationAnalysisDataField _dataField;
      protected PopulationAnalysisDerivedField _derivedField;

      protected override void Context()
      {
         base.Context();
         _populationAnalysis = new PopulationPivotAnalysis();
         _dataField = A.Fake<PopulationAnalysisDataField>().WithName("DATA");
         _derivedField = A.Fake<PopulationAnalysisDerivedField>().WithName("DERIVED");

         A.CallTo(_templateTask).WithReturnType<IReadOnlyList<PopulationAnalysisDerivedField>>().Returns(new []{_derivedField });
         _populationAnalysis.Add(_dataField);
      }
   }

   public class When_loading_a_dervied_field_with_a_name_that_already_exist : concern_for_LoadingDerivedFieldFromTemplate
   {
      protected override void Context()
      {
         base.Context();
         _derivedField.Name = _dataField.Name;
         A.CallTo(() => _derivedField.CanBeUsedFor(_dataField.DataType)).Returns(true);
         A.CallTo(_entityTask).WithReturnType<string>().Returns("NEW NAME");
      }

      protected override void Because()
      {
         _derivedField = sut.LoadDerivedFieldFor(_populationAnalysis, _dataField);
      }

      [Observation]
      public void should_have_ask_the_user_to_rename_the_field()
      {
         _derivedField.Name.ShouldBeEqualTo("NEW NAME");
      }

      [Observation]
      public void should_have_not_added_the_derived_field_to_the_analyses()
      {
         _populationAnalysis.Has(_derivedField).ShouldBeFalse();
      }
   }

   public class When_loading_a_derived_field_for_a_field_that_does_not_have_the_appropriate_type : concern_for_LoadingDerivedFieldFromTemplate
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _derivedField.CanBeUsedFor(_dataField.DataType)).Returns(false);
         A.CallTo(_entityTask).WithReturnType<string>().Returns("NEW NAME");
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.LoadDerivedFieldFor(_populationAnalysis, _dataField)).ShouldThrowAn<PKSimException>();
      }
   }

   public class When_loading_a_derived_field_for_a_field_that_has_a_different_name : concern_for_LoadingDerivedFieldFromTemplate
   {
      protected override void Context()
      {
         base.Context();
         var groupingField = A.Fake<PopulationAnalysisGroupingField>().WithName("DERIVED");
         _derivedField = groupingField;
         A.CallTo(() => _derivedField.CanBeUsedFor(_dataField.DataType)).Returns(true);
         A.CallTo(() => groupingField.ReferencedFieldName).Returns("ANOTHER NAME");
         A.CallTo(_templateTask).WithReturnType<IReadOnlyList<PopulationAnalysisDerivedField>>().Returns(new[]{ _derivedField });
      }

      [Observation]
      public void should_warn_the_user_that_the_field_might_not_be_the_one_he_intended_to_load()
      {
         _derivedField = sut.LoadDerivedFieldFor(_populationAnalysis, _dataField);
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.Warning.DerivedFieldWasSavedForAnotherField("ANOTHER NAME", _dataField.Name))).MustHaveHappened();
      }

      [Observation]
      public void should_return_null_if_the_user_decides_to_cancel_the_import()
      {
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.No);
         _derivedField = sut.LoadDerivedFieldFor(_populationAnalysis, _dataField);
         _derivedField.ShouldBeNull();
      }

      [Observation]
      public void should_return_the_derived_field_if_the_user_accepts_the_changes()
      {
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Yes);
         _derivedField = sut.LoadDerivedFieldFor(_populationAnalysis, _dataField);
         _derivedField.ShouldNotBeNull();
      }
   }

   public class When_loading_some_simulation_workflow_into_an_population_simulation_and_the_user_cancels : concern_for_PopulationAnalysisTemplateTask
   {
      private PopulationSimulation _populationSimulation;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = new PopulationSimulation();

         A.CallTo(() => _templateTask.LoadFromTemplate<SimulationAnalysisWorkflow>(TemplateType.PopulationSimulationAnalysisWorkflow)).Returns(new List<SimulationAnalysisWorkflow>());
      }

      protected override void Because()
      {
         sut.LoadPopulationAnalysisWorkflowInto(_populationSimulation);
      }

      [Observation]
      public void should_not_load_the_simulation()
      {
         A.CallTo(() => _lazyLoadTask.Load(_populationSimulation)).MustNotHaveHappened();
      }
   }

   public class When_loading_some_simulation_workflow_into_an_population_simulation : concern_for_PopulationAnalysisTemplateTask
   {
      private PopulationSimulation _populationSimulation;
      private SimulationAnalysisWorkflow _workflow;
      private ISimulationAnalysis _analysis1;
      private ISimulationAnalysis _analysis2;

      protected override void Context()
      {
         base.Context();
         _workflow = new SimulationAnalysisWorkflow {OutputSelections = new OutputSelections()};
         _analysis1 = A.Fake<ISimulationAnalysis>();
         _analysis2 = A.Fake<ISimulationAnalysis>();
         _workflow.Add(_analysis1);
         _workflow.Add(_analysis2);
         _populationSimulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => _templateTask.LoadFromTemplate<SimulationAnalysisWorkflow>(TemplateType.PopulationSimulationAnalysisWorkflow)).Returns(new[]{ _workflow });
      }

      protected override void Because()
      {
         sut.LoadPopulationAnalysisWorkflowInto(_populationSimulation);
      }

      [Observation]
      public void should_load_the_simulation()
      {
         A.CallTo(() => _lazyLoadTask.Load((ILazyLoadable) _populationSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_add_all_templates_chart_analysis_to_the_simulation()
      {
         A.CallTo(() => _simulationAnalysisCreator.AddSimulationAnalysisTo(_populationSimulation, _analysis1)).MustHaveHappened();
         A.CallTo(() => _simulationAnalysisCreator.AddSimulationAnalysisTo(_populationSimulation, _analysis2)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_output_settings_of_the_simulation()
      {
         A.CallTo(() => _populationSimulation.OutputSelections.UpdatePropertiesFrom(_workflow.OutputSelections, _cloner)).MustHaveHappened();
      }
   }

   public class When_saving_the_populatin_analysis_workflow_to_the_template_database : concern_for_PopulationAnalysisTemplateTask
   {
      private PopulationSimulation _populationSimulation;
      private SimulationAnalysisWorkflow _workflow;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>();
         _workflow = new SimulationAnalysisWorkflow();
         A.CallTo(() => _simulationAnalysisWorkflowMapper.MapFrom(_populationSimulation)).Returns(_workflow);
      }

      protected override void Because()
      {
         sut.SavePopulationAnalysisWorkflowFrom(_populationSimulation);
      }

      [Observation]
      public void should_load_the_simulation()
      {
         A.CallTo(() => _lazyLoadTask.Load((ILazyLoadable) _populationSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_createa_new_new_simulation_analysis_workflow_and_save_it_to_the_template_database()
      {
         A.CallTo(() => _templateTask.SaveToTemplate(_workflow, TemplateType.PopulationSimulationAnalysisWorkflow)).MustHaveHappened();
      }
   }
}