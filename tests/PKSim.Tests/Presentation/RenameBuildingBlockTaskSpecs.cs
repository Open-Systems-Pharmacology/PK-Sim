using System;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using static OSPSuite.Core.Domain.Constants;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using SimulationResults = OSPSuite.Core.Domain.Data.SimulationResults;

namespace PKSim.Presentation
{
   public abstract class concern_for_RenameBuildingBlockTask : ContextSpecification<IRenameBuildingBlockTask>
   {
      protected IApplicationController _applicationController;
      protected IBuildingBlockTask _buildingBlockTask;
      private IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      private ILazyLoadTask _lazyloadTask;
      private IHeavyWorkManager _heavyWorkManager;
      protected ObjectPathFactory _objectPathFactory;
      protected IndividualSimulation _individualSimulation;
      protected Simulation _simulation;
      protected IContainer _root;
      private IRenameAbsolutePathVisitor _renameAbsolutePathVisitor;
      protected IContainerTask _containerTask;
      private IObjectReferencingRetriever _objectReferencingRetriever;
      protected IProjectRetriever _projectRetriever;
      protected IParameterIdentificationSimulationPathUpdater _parameterIdentificationSimulationPathUpdater;
      protected string _initialSimulationName;
      protected IDataRepositoryNamer _dataRepositoryNamer;
      protected ICurveNamer _curveNamer;
      protected IExpressionProfileUpdater _expressionProfileUpdater;

      protected override void Context()
      {
         _applicationController = A.Fake<IApplicationController>();
         _buildingBlockTask = A.Fake<IBuildingBlockTask>();
         _buildingBlockInProjectManager = A.Fake<IBuildingBlockInProjectManager>();
         _lazyloadTask = A.Fake<ILazyLoadTask>();
         _heavyWorkManager = A.Fake<IHeavyWorkManager>();
         _containerTask = A.Fake<IContainerTask>();
         _objectReferencingRetriever = A.Fake<IObjectReferencingRetriever>();
         _projectRetriever = A.Fake<IProjectRetriever>();
         _renameAbsolutePathVisitor = new RenameAbsolutePathVisitor();
         _objectPathFactory = new ObjectPathFactoryForSpecs();
         _parameterIdentificationSimulationPathUpdater = A.Fake<IParameterIdentificationSimulationPathUpdater>();
         _dataRepositoryNamer = A.Fake<IDataRepositoryNamer>();
         _curveNamer = A.Fake<ICurveNamer>();
         _expressionProfileUpdater = A.Fake<IExpressionProfileUpdater>();

         sut = new RenameBuildingBlockTask(
            _buildingBlockTask,
            _buildingBlockInProjectManager,
            _applicationController,
            _lazyloadTask,
            _containerTask,
            _heavyWorkManager,
            _renameAbsolutePathVisitor,
            _objectReferencingRetriever,
            _projectRetriever,
            _parameterIdentificationSimulationPathUpdater,
            _dataRepositoryNamer,
            _curveNamer,
            _expressionProfileUpdater);

         _initialSimulationName = "S";
         _individualSimulation = new IndividualSimulation().WithName(_initialSimulationName);
         _individualSimulation.Model = new Model().WithName(_initialSimulationName);
         _simulation = _individualSimulation;
         _root = new Container().WithName(_initialSimulationName);
         _individualSimulation.Model.Root = _root;
      }
   }

   public class When_renaming_a_simulation : concern_for_RenameBuildingBlockTask
   {
      private string _newName;
      private IFormula _f1;
      private IFormula _f2;
      private IFormula _f3;
      private TableFormulaWithXArgument _f4;
      private IndividualResults _individualResults;

      protected override void Context()
      {
         base.Context();
         _newName = "TOTO";
         _f1 = new ExplicitFormula("A+B");
         _f2 = new ExplicitFormula("A+B");
         _f3 = new ExplicitFormula("A+B");
         _f4 = new TableFormulaWithXArgument();
         _f1.AddObjectPath(new FormulaUsablePath(_initialSimulationName, "Liver", "Cell"));
         _f1.AddObjectPath(new FormulaUsablePath("Drug", "LogP"));
         _f2.AddObjectPath(new FormulaUsablePath("SU", "Liver", "Cell"));
         _f2.AddObjectPath(new FormulaUsablePath("Drug", "LogP"));
         _f3.AddObjectPath(new FormulaUsablePath(_initialSimulationName, "Liver", "Cell"));
         _f3.AddObjectPath(new FormulaUsablePath(_initialSimulationName, "LogP"));
         _f4.AddTableObjectPath(new FormulaUsablePath(_initialSimulationName, "SolubilityTable") {Alias = "Sol"});

         var p1 = new PKSimParameter().WithName("P1").WithFormula(_f1);
         var p2 = new PKSimParameter().WithName("P2").WithFormula(_f2);
         var p3 = new PKSimParameter().WithName("P3").WithFormula(_f3);
         var p4 = new PKSimParameter().WithName("P4").WithFormula(_f4);
         var c1 = new Container().WithName("C1");
         c1.Add(p3);
         c1.Add(p4);
         _root.Add(p1);
         _root.Add(p2);
         _root.Add(c1);

         var results = new SimulationResults {Time = new QuantityValues {ColumnId = "0", QuantityPath = "baseGrid"}};
         _individualResults = new IndividualResults {IndividualId = 1};
         results.Add(_individualResults);

         var quantityCache = new PathCacheForSpecs<IQuantity>
         {
            {"Liver|Cell|Drug", new MoleculeAmount {QuantityType = QuantityType.Drug}},
            {"Liver|Cell|Meta", new MoleculeAmount {QuantityType = QuantityType.Metabolite}},
         };

         _individualResults.Add(new QuantityValues {PathList = new[] {"Liver", "Cell", "Drug"}.ToList()});
         _individualResults.Add(new QuantityValues {PathList = new[] {"Liver", "Cell", "Meta"}.ToList()});

         _individualSimulation.Results = results;
         _individualSimulation.DataRepository = new DataRepository();
         _individualSimulation.AddReactions(new ReactionBuildingBlock());
         _individualSimulation.Settings = new SimulationSettings();
         A.CallTo(_containerTask).WithReturnType<PathCache<IQuantity>>().Returns(quantityCache);

         A.CallTo(() => _curveNamer.RenameCurvesWithOriginalNames(_individualSimulation, A<Action>._, true)).Invokes(x => x.Arguments[1].DowncastTo<Action>()());
      }

      protected override void Because()
      {
         sut.RenameSimulation(_individualSimulation, _newName);
      }

      [Observation]
      public void the_data_repository_should_also_be_renamed()
      {
         A.CallTo(() => _dataRepositoryNamer.Rename(_individualSimulation.DataRepository, _newName)).MustHaveHappened();
      }

      [Observation]
      public void the_curves_should_also_be_renamed()
      {
         A.CallTo(() => _curveNamer.RenameCurvesWithOriginalNames(_individualSimulation, A<Action>._, true)).MustHaveHappened();
      }

      [Observation]
      public void should_update_paths_in_the_parameter_identification()
      {
         A.CallTo(() => _parameterIdentificationSimulationPathUpdater.UpdatePathsForRenamedSimulation(_individualSimulation, _initialSimulationName, _newName)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_simulation()
      {
         A.CallTo(() => _buildingBlockTask.Load(_simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_have_renamed_the_absolute_object_path_in_the_simulation()
      {
         _f1.ObjectPaths.ElementAt(0).ShouldOnlyContain(_objectPathFactory.CreateFormulaUsablePathFrom(_newName, "Liver", "Cell").ToArray());
         _f3.ObjectPaths.ElementAt(0).ShouldOnlyContain(_objectPathFactory.CreateFormulaUsablePathFrom(_newName, "Liver", "Cell").ToArray());
         _f3.ObjectPaths.ElementAt(1).ShouldOnlyContain(_objectPathFactory.CreateFormulaUsablePathFrom(_newName, "LogP").ToArray());
         _f4.ObjectPaths.ElementAt(0).ShouldOnlyContain(_objectPathFactory.CreateFormulaUsablePathFrom(_newName, "SolubilityTable").ToArray());
      }

      [Observation]
      public void should_not_have_change_the_relative_paths()
      {
         _f1.ObjectPaths.ElementAt(1).ShouldOnlyContain(_objectPathFactory.CreateFormulaUsablePathFrom("Drug", "LogP").ToArray());
         _f2.ObjectPaths.ElementAt(0).ShouldOnlyContain(_objectPathFactory.CreateFormulaUsablePathFrom("SU", "Liver", "Cell").ToArray());
         _f2.ObjectPaths.ElementAt(1).ShouldOnlyContain(_objectPathFactory.CreateFormulaUsablePathFrom("Drug", "LogP").ToArray());
      }

      [Observation]
      public void should_have_change_the_absolute_path_of_quantity_info_that_were_calculated_in_the_simulation()
      {
         _individualResults.HasValuesFor("Liver|Cell|Drug").ShouldBeTrue();
         _individualResults.HasValuesFor("Liver|Cell|Meta").ShouldBeTrue();
      }
   }

   public class When_renaming_a_simulation_that_was_currently_being_edited : concern_for_RenameBuildingBlockTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _applicationController.HasPresenterOpenedFor(_simulation)).Returns(true);
      }

      protected override void Because()
      {
         sut.RenameSimulation(_simulation, "newName");
      }

      [Observation]
      public void should_close_the_presenter_before_performing_the_renaming_operation()
      {
         A.CallTo(() => _applicationController.Close(_simulation)).MustHaveHappened();
      }

      [Observation]
      public void should_edit_the_simulation()
      {
         A.CallTo(() => _buildingBlockTask.Edit(_simulation)).MustHaveHappened();
      }
   }

   public class When_renaming_a_simulation_that_was_not_currently_being_edited : concern_for_RenameBuildingBlockTask
   {
      private ICommandCollector _commandCollector;

      protected override void Context()
      {
         base.Context();
         _commandCollector = A.Fake<ICommandCollector>();
         A.CallTo(() => _applicationController.HasPresenterOpenedFor(_individualSimulation)).Returns(false);
      }

      protected override void Because()
      {
         sut.RenameSimulation(_individualSimulation, "newName");
      }

      [Observation]
      public void should_not_open_the_presenter_for_the_simulation()
      {
         A.CallTo(() => _applicationController.Open(_individualSimulation, _commandCollector)).MustNotHaveHappened();
      }
   }

   public class When_renaming_a_compound : concern_for_RenameBuildingBlockTask
   {
      private string _oldName;
      private IPKSimBuildingBlock _compound;
      private IProject _project;
      private DataRepository _observedData1;
      private DataRepository _observedData2;

      protected override void Context()
      {
         base.Context();
         _project = A.Fake<IProject>();
         _oldName = "OLD";
         _compound = new Compound().WithName("NEW");
         A.CallTo(() => _projectRetriever.CurrentProject).Returns(_project);
         _observedData1 = new DataRepository("1");
         _observedData1.ExtendedProperties.Add(ObservedData.MOLECULE, new ExtendedProperty<string>());
         _observedData1.ExtendedProperties[ObservedData.MOLECULE].ValueAsObject = _oldName;

         _observedData2 = new DataRepository("2");
         _observedData2.ExtendedProperties.Add(ObservedData.MOLECULE, new ExtendedProperty<string>());
         _observedData2.ExtendedProperties[ObservedData.MOLECULE].ValueAsObject = "NOT USING";
         A.CallTo(() => _project.AllObservedData).Returns(new[] {_observedData1, _observedData2});
      }

      protected override void Because()
      {
         sut.RenameBuildingBlock(_compound, _oldName);
      }

      [Observation]
      public void should_also_rename_the_molecule_meta_data_of_observed_data_imported_for_this_compound()
      {
         _observedData1.ExtendedPropertyValueFor(ObservedData.MOLECULE).ShouldBeEqualTo(_compound.Name);
         _observedData2.ExtendedPropertyValueFor(ObservedData.MOLECULE).ShouldBeEqualTo("NOT USING");
      }
   }

   public class When_renaming_an_expression_profile_molecule : concern_for_RenameBuildingBlockTask
   {
      private ExpressionProfile _expressionProfile;
      private string _newName;
      private string _newMoleculeName;

      protected override void Context()
      {
         base.Context();
         _expressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _newMoleculeName = "CYP";
         _newName = ContainerName.ExpressionProfileName(_newMoleculeName, _expressionProfile.Species.DisplayName, "SICK");
      }

      protected override void Because()
      {
         sut.RenameBuildingBlock(_expressionProfile, _newName);
      }

      [Observation]
      public void should_have_renamed_the_expression_profile()
      {
         _expressionProfile.Name.ShouldBeEqualTo(_newName);
      }

      [Observation]
      public void should_also_rename_all_associated_simulation_subject()
      {
         A.CallTo(() => _expressionProfileUpdater.UpdateMoleculeName(_expressionProfile, _newMoleculeName)).MustHaveHappened();
      }
   }
}