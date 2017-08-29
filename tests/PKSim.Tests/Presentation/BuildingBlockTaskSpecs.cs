using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Exceptions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Services;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Presentation
{
   public abstract class concern_for_BuildingBlockTask : ContextSpecification<IBuildingBlockTask>
   {
      protected IExecutionContext _executionContext;
      protected IApplicationController _applicationController;
      protected IPKSimBuildingBlock _buildingBlock;
      protected ICloneBuildingBlockPresenter _clonePresenter;
      protected IRenameObjectPresenter _renamePresenter;
      protected IDialogCreator _dialogCreator;
      protected PKSimProject _project;
      protected IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;
      protected IEntityTask _entityTask;
      protected ITemplateTaskQuery _templateTaskQuery;
      protected ISingleStartPresenterTask _singleStartPresenterTask;
      protected IBuildingBlockRepository _buildingBlockRepository;
      protected ILazyLoadTask _lazyLoadTask;
      protected ISimulationReferenceUpdater _simulationReferenceUpdater;
      protected IPresentationSettingsTask _presenterSettingsTask;
      protected ISnapshotTask _snapshotTask;

      protected override void Context()
      {
         _project = A.Fake<PKSimProject>();
         _entityTask = A.Fake<IEntityTask>();
         _templateTaskQuery = A.Fake<ITemplateTaskQuery>();
         _executionContext = A.Fake<IExecutionContext>();
         A.CallTo(() => _executionContext.CurrentProject).Returns(_project);
         _applicationController = A.Fake<IApplicationController>();
         _buildingBlockInSimulationManager = A.Fake<IBuildingBlockInSimulationManager>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _clonePresenter = A.Fake<ICloneBuildingBlockPresenter>();
         _renamePresenter = A.Fake<IRenameObjectPresenter>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _singleStartPresenterTask = A.Fake<ISingleStartPresenterTask>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _presenterSettingsTask = A.Fake<IPresentationSettingsTask>();
         _simulationReferenceUpdater = A.Fake<ISimulationReferenceUpdater>();
         _snapshotTask= A.Fake<ISnapshotTask>();

         sut = new BuildingBlockTask(
            _executionContext,
            _applicationController, 
            _dialogCreator,
            _buildingBlockInSimulationManager,
            _entityTask, 
            _templateTaskQuery, 
            _singleStartPresenterTask, 
            _buildingBlockRepository, 
            _lazyLoadTask, 
            _presenterSettingsTask,
            _simulationReferenceUpdater, 
            _snapshotTask);

         A.CallTo(() => _applicationController.Start<ICloneBuildingBlockPresenter>()).Returns(_clonePresenter);
         A.CallTo(() => _applicationController.Start<IRenameObjectPresenter>()).Returns(_renamePresenter);

         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
      }
   }

   public class When_performing_a_clone_operation_on_a_building_block : concern_for_BuildingBlockTask
   {
      protected override void Because()
      {
         sut.Clone(_buildingBlock);
      }

      [Observation]
      public void should_retrieve_the_clone_presenter_and_start_it()
      {
         A.CallTo(() => _applicationController.Start<ICloneBuildingBlockPresenter>()).MustHaveHappened();
         A.CallTo(() => _clonePresenter.CreateCloneFor(_buildingBlock)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_entity_if_necessary()
      {
         A.CallTo(() => _executionContext.Load(_buildingBlock)).MustHaveHappened();
      }
   }

   public class When_the_clone_operation_was_canceled_by_the_user : concern_for_BuildingBlockTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _clonePresenter.CreateCloneFor(_buildingBlock)).Returns(null);
      }

      protected override void Because()
      {
         sut.Clone(_buildingBlock);
      }

      [Observation]
      public void should_not_change_the_history()
      {
         A.CallTo(() => _executionContext.AddToHistory(A<IPKSimCommand>.Ignored)).MustNotHaveHappened();
      }
   }

   public class When_the_clone_operation_was_confirmed_by_the_user : concern_for_BuildingBlockTask
   {
      private AddBuildingBlockToProjectCommand _command;
      private IPKSimBuildingBlock _clonedBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _buildingBlock.Name = "AA";
         _clonedBuildingBlock = A.Fake<IPKSimBuildingBlock>();
         A.CallTo(() => _clonePresenter.CreateCloneFor(_buildingBlock)).Returns(_clonedBuildingBlock);
         A.CallTo(() => _executionContext.AddToHistory(A<IPKSimCommand>.Ignored)).Invokes(x => _command = x.GetArgument<AddBuildingBlockToProjectCommand>(0));
      }

      protected override void Because()
      {
         sut.Clone(_buildingBlock);
      }

      [Observation]
      public void should_add_a_command_to_the_history()
      {
         _command.ShouldNotBeNull();
      }

      [Observation]
      public void should_update_the_creation_mode_of_the_cloned_building_block_to_clone()
      {
         _clonedBuildingBlock.Creation.CreationMode.ShouldBeEqualTo(CreationMode.Clone);
         _clonedBuildingBlock.Creation.ClonedFrom.ShouldBeEqualTo(_buildingBlock.Name);
      }
   }

   public class When_asked_to_delete_a_building_block_that_is_not_used_in_any_simulation_and_the_user_cancels_the_action : concern_for_BuildingBlockTask
   {
      private string _buildingBlockType;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _buildingBlockInSimulationManager.SimulationsUsing(_buildingBlock)).Returns(new List<Simulation>());
         A.CallTo(() => _buildingBlock.Name).Returns("toto");
         _buildingBlockType = "Individual";
         A.CallTo(() => _entityTask.TypeFor(_buildingBlock)).Returns(_buildingBlockType);
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteObjectOfType(_buildingBlockType, _buildingBlock.Name))).Returns(ViewResult.No);
      }

      protected override void Because()
      {
         sut.Delete(_buildingBlock);
      }

      [Observation]
      public void should_not_delete_the_building_block()
      {
         A.CallTo(() => _project.RemoveBuildingBlock(_buildingBlock)).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_load_the_building_block()
      {
         A.CallTo(() => _executionContext.Load(_buildingBlock)).MustNotHaveHappened();
      }
   }

   public class When_asked_to_delete_a_building_block_that_is_not_used_in_any_simulation_and_the_user_confirms_the_deletion : concern_for_BuildingBlockTask
   {
      private string _buildingBlockType;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _buildingBlockInSimulationManager.SimulationsUsing(_buildingBlock)).Returns(new List<Simulation>());
         _buildingBlock.Id = "toto";
         _buildingBlockType = "Individual";
         A.CallTo(() => _entityTask.TypeFor(_buildingBlock)).Returns(_buildingBlockType);
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteObjectOfType(_buildingBlockType, _buildingBlock.Name))).Returns(ViewResult.Yes);
      }

      protected override void Because()
      {
         sut.Delete(_buildingBlock);
      }

      [Observation]
      public void should_load_the_building_block()
      {
         A.CallTo(() => _executionContext.Load(_buildingBlock)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_action_to_the_history()
      {
         A.CallTo(() => _executionContext.AddToHistory(A<IPKSimCommand>.Ignored)).MustHaveHappened();
      }

      [Observation]
      public void the_building_block_should_have_been_deleted()
      {
         A.CallTo(() => _project.RemoveBuildingBlock(_buildingBlock)).MustHaveHappened();
      }
   }

   public class When_asked_to_delete_a_building_block_that_is_not_used_in_any_simulation_and_the_user_confirms_the_deletion_and_the_loading_is_unsuccessful : concern_for_BuildingBlockTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _buildingBlockInSimulationManager.SimulationsUsing(_buildingBlock)).Returns(new List<Simulation>());
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Yes);
         A.CallTo(() => _executionContext.Load(_buildingBlock)).Throws(new OSPSuiteException());
      }

      protected override void Because()
      {
         sut.Delete(_buildingBlock);
      }

      [Observation]
      public void should_add_the_action_to_the_history()
      {
         A.CallTo(() => _executionContext.AddToHistory(A<IPKSimCommand>.Ignored)).MustHaveHappened();
      }

      [Observation]
      public void the_building_block_should_have_been_deleted()
      {
         A.CallTo(() => _project.RemoveBuildingBlock(_buildingBlock)).MustHaveHappened();
      }
   }

   public class when_deleting_a_simulation : concern_for_BuildingBlockTask
   {
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
      }

      protected override void Because()
      {
         sut.Delete(_simulation);
      }

      [Observation]
      public void the_parameter_identification_task_should_be_used_to_update_parameter_identifications()
      {
         A.CallTo(() => _simulationReferenceUpdater.RemoveSimulationFromParameterIdentificationsAndSensitivityAnalyses(_simulation)).MustHaveHappened();
      }

      [Observation]
      public void the_presenter_settings_task_must_be_used_to_remove_presenter_settigns_for_the_building_block()
      {
         A.CallTo(() => _presenterSettingsTask.RemovePresentationSettingsFor(_simulation)).MustHaveHappened();
      }
   }

   public class When_asked_to_delete_a_building_block_that_used_in_one_simulation : concern_for_BuildingBlockTask
   {
      private string _buildingBlockType;
      private Simulation _simulation1;

      protected override void Context()
      {
         base.Context();
         _simulation1 = A.Fake<Simulation>();
         _buildingBlock.Id = "toto";
         _buildingBlockType = "Individual";
         A.CallTo(() => _entityTask.TypeFor(_buildingBlock)).Returns(_buildingBlockType);
         A.CallTo(() => _buildingBlockInSimulationManager.SimulationsUsing(_buildingBlock)).Returns(new[] {_simulation1});
      }

      [Observation]
      public void should_not_allow_the_deletion()
      {
         The.Action(() => sut.Delete(_buildingBlock)).ShouldThrowAn<CannotDeleteBuildingBlockException>();
      }
   }

   public class When_the_building_block_task_is_renaming_a_building_block : concern_for_BuildingBlockTask
   {
      private IPKSimCommand _command;

      protected override void Because()
      {
         sut.Rename(_buildingBlock);
      }

      protected override void Context()
      {
         base.Context();
         _command = A.Fake<IPKSimCommand>();
         A.CallTo(() => _entityTask.Rename(_buildingBlock)).Returns(_command);
      }

      [Observation]
      public void should_rename_the_building_block_and_add_the_resulting_command_to_the_history()
      {
         A.CallTo(() => _executionContext.AddToHistory(_command)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_entity_if_necessary()
      {
         A.CallTo(() => _executionContext.Load(_buildingBlock)).MustHaveHappened();
      }
   }

   public class When_the_building_block_task_is_told_to_load_a_building_block : concern_for_BuildingBlockTask
   {
      private IPKSimBuildingBlock _buildingBlockToLoad;

      protected override void Context()
      {
         base.Context();
         _buildingBlockToLoad = A.Fake<IPKSimBuildingBlock>();
      }

      protected override void Because()
      {
         sut.Load(_buildingBlockToLoad);
      }

      [Observation]
      public void the_building_block_should_have_been_loaded()
      {
         A.CallTo(() => _executionContext.Load(_buildingBlockToLoad)).MustHaveHappened();
      }
   }

   public class When_the_building_block_task_is_retrieving_all_the_building_blocks_of_a_given_type : concern_for_BuildingBlockTask
   {
      private IEnumerable<Individual> _results;
      private Individual _ind1;
      private Individual _ind2;

      protected override void Context()
      {
         base.Context();
         _ind1 = new Individual();
         _ind2 = new Individual();
         A.CallTo(() => _buildingBlockRepository.All<Individual>()).Returns(new[] {_ind1, _ind2});
      }

      protected override void Because()
      {
         _results = sut.All<Individual>();
      }

      [Observation]
      public void should_retun_the_building_block_of_the_type_defined_in_the_repository()
      {
         _results.ShouldOnlyContain(_ind1, _ind2);
      }
   }

   public class When_loading_the_results_of_a_simulation : concern_for_BuildingBlockTask
   {
      private Simulation _simulationToLoad;

      protected override void Context()
      {
         base.Context();
         _simulationToLoad = A.Fake<Simulation>();
      }

      protected override void Because()
      {
         sut.LoadResults(_simulationToLoad);
      }

      [Observation]
      public void should_load_the_simulation_first()
      {
         A.CallTo(() => _executionContext.Load(_simulationToLoad)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_results()
      {
         A.CallTo(() => _lazyLoadTask.LoadResults(_simulationToLoad)).MustHaveHappened();
      }
   }

   public class When_loading_a_template_for_a_given_building_block_type : concern_for_BuildingBlockTask
   {
      private ITemplatePresenter _templatePresenter;
      private ISimulationSubject _templateIndividual;
      private ISimulationSubject _individual;
      private IPKSimCommand _command;

      protected override void Context()
      {
         base.Context();
         _templatePresenter = A.Fake<ITemplatePresenter>();
         _templateIndividual = new Individual();
         A.CallTo(() => _applicationController.Start<ITemplatePresenter>()).Returns(_templatePresenter);
         A.CallTo(_templatePresenter).WithReturnType<IReadOnlyList<ISimulationSubject>>().Returns(new[] {_templateIndividual});

         A.CallTo(() => _executionContext.AddToHistory((A<IPKSimCommand>._)))
            .Invokes(x => _command = x.GetArgument<IPKSimCommand>(0));
      }

      protected override void Because()
      {
         _individual = sut.LoadFromTemplate<ISimulationSubject>(PKSimBuildingBlockType.SimulationSubject).FirstOrDefault();
      }

      [Observation]
      public void should_retrieve_the_available_template_from_the_template_database()
      {
         _individual.ShouldBeAnInstanceOf<Individual>();
      }

      [Observation]
      public void should_add_the_building_block_the_project()
      {
         _command.ShouldBeAnInstanceOf<AddBuildingBlockToProjectCommand>();
      }
   }

   public class When_loading_a_template_for_a_given_building_block_type_whose_name_is_already_being_used_even_in_a_different_case : concern_for_BuildingBlockTask
   {
      private ITemplatePresenter _templatePresenter;
      private ISimulationSubject _templateIndividual;
      private ISimulationSubject _existingIndiviudal;
      private IPKSimCommand _command;

      protected override void Context()
      {
         base.Context();
         _templatePresenter = A.Fake<ITemplatePresenter>();
         _templateIndividual = new Individual().WithName("Existing");
         _existingIndiviudal = new Individual().WithName("ExiStIng");
         A.CallTo(() => _applicationController.Start<ITemplatePresenter>()).Returns(_templatePresenter);
         A.CallTo(_templatePresenter).WithReturnType<IReadOnlyList<ISimulationSubject>>().Returns(new[] {_templateIndividual});
         A.CallTo(() => _project.All(_templateIndividual.BuildingBlockType)).Returns(new[] {_existingIndiviudal});

         A.CallTo(() => _executionContext.AddToHistory((A<IPKSimCommand>._)))
            .Invokes(x => _command = x.GetArgument<IPKSimCommand>(0));
      }

      protected override void Because()
      {
         sut.LoadFromTemplate<ISimulationSubject>(PKSimBuildingBlockType.SimulationSubject);
      }

      [Observation]
      public void should_rename_the_loaded_building_block_if_the_name_was_already_used_in_the_project()
      {
         A.CallTo(() => _entityTask.Rename(_templateIndividual)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_building_block_the_project()
      {
         _command.ShouldBeAnInstanceOf<AddBuildingBlockToProjectCommand>();
      }
   }

   public class When_loading_a_template_for_a_given_building_block_type_whose_name_is_already_being_used_and_the_user_cancels_the_rename : concern_for_BuildingBlockTask
   {
      private ITemplatePresenter _templatePresenter;
      private ISimulationSubject _templateIndividual;
      private ISimulationSubject _existingIndiviudal;
      private ISimulationSubject _individual;

      protected override void Context()
      {
         base.Context();
         _templatePresenter = A.Fake<ITemplatePresenter>();
         _templateIndividual = new Individual().WithName("Existing");
         _existingIndiviudal = new Individual().WithName("ExiStIng");
         A.CallTo(() => _applicationController.Start<ITemplatePresenter>()).Returns(_templatePresenter);
         A.CallTo(_templatePresenter).WithReturnType<ISimulationSubject>().Returns(_templateIndividual);
         A.CallTo(() => _project.All(_templateIndividual.BuildingBlockType)).Returns(new[] {_existingIndiviudal});
         A.CallTo(() => _entityTask.Rename(_templateIndividual)).Returns(new PKSimEmptyCommand());
      }

      protected override void Because()
      {
         _individual = sut.LoadFromTemplate<ISimulationSubject>(PKSimBuildingBlockType.SimulationSubject).FirstOrDefault();
      }

      [Observation]
      public void should_not_add_the_building_block_to_the_project()
      {
         A.CallTo(() => _executionContext.AddToHistory(A<IPKSimCommand>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_return_null()
      {
         _individual.ShouldBeNull();
      }
   }

   public abstract class When_saving_building_blocks_to_the_user_template_database : concern_for_BuildingBlockTask
   {
      protected ICache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>> _cache;
      protected IPKSimBuildingBlock _compound;
      protected IPKSimBuildingBlock _metabolite;
      protected IReadOnlyList<Template> _allTemplateItems;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound().WithId("Comp");
         _metabolite = new Compound().WithId("Met");
         _cache = new Cache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>>
         {
            [_compound] = new List<IPKSimBuildingBlock> {_metabolite}
         };

         A.CallTo(() => _templateTaskQuery.SaveToTemplate(A<IReadOnlyList<Template>>._))
            .Invokes(x => _allTemplateItems = x.GetArgument<IReadOnlyList<Template>>(0));
      }

      protected override void Because()
      {
         sut.SaveAsTemplate(_cache, TemplateDatabaseType.User);
      }
   }

   public class When_saving_some_templates_with_references_to_the_user_template_database : When_saving_building_blocks_to_the_user_template_database
   {
      [Observation]
      public void should_load_all_templates()
      {
         A.CallTo(() => _executionContext.Load(_compound)).MustHaveHappened();
         A.CallTo(() => _executionContext.Load(_metabolite)).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_template_and_their_reference()
      {
         _allTemplateItems.Count.ShouldBeEqualTo(2);
         _allTemplateItems[0].Object.ShouldBeEqualTo(_compound);
         _allTemplateItems[0].References.ShouldContain(_allTemplateItems[1]);
         _allTemplateItems[1].Object.ShouldBeEqualTo(_metabolite);
         _allTemplateItems[1].References.ShouldBeEmpty();
      }
   }

   public class When_saving_some_templates_with_references_defined_twice_to_the_user_template_database : When_saving_building_blocks_to_the_user_template_database
   {
      protected override void Context()
      {
         base.Context();
         _cache[_metabolite] = new List<IPKSimBuildingBlock>();
      }

      [Observation]
      public void should_save_the_template_and_their_reference_only_once()
      {
         _allTemplateItems.Count.ShouldBeEqualTo(2);
         _allTemplateItems[0].Object.ShouldBeEqualTo(_compound);
         _allTemplateItems[0].References.ShouldContain(_allTemplateItems[1]);
         _allTemplateItems[1].Object.ShouldBeEqualTo(_metabolite);
         _allTemplateItems[1].References.ShouldBeEmpty();
      }
   }

   public class When_saving_some_templates_with_reference_to_the_user_template_database_and_one_reference_already_exists_by_name_and_the_user_cancels_the_action : When_saving_building_blocks_to_the_user_template_database
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _templateTaskQuery.Exists(TemplateDatabaseType.User, _metabolite.Name, TemplateType.Compound)).Returns(true);
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Cancel);
      }

      [Observation]
      public void should_not_save_the_template_or_any_of_the_references()
      {
         _allTemplateItems.ShouldBeNull();
      }
   }

   public class When_adding_a_building_block_to_the_project : concern_for_BuildingBlockTask
   {
      private IPKSimCommand _command;

      protected override void Because()
      {
         _command = sut.AddToProject(_buildingBlock);
      }

      [Observation]
      public void should_add_the_command_to_the_history()
      {
         A.CallTo(() => _executionContext.AddToHistory(_command)).MustHaveHappened();
      }
   }

   public class When_adding_a_building_block_to_the_project_adn_the_resulting_command_should_not_be_added_to_the_history : concern_for_BuildingBlockTask
   {
      private IPKSimCommand _command;

      protected override void Because()
      {
         _command = sut.AddToProject(_buildingBlock, addToHistory: false);
      }

      [Observation]
      public void should_add_the_command_to_the_history()
      {
         A.CallTo(() => _executionContext.AddToHistory(_command)).MustNotHaveHappened();
      }
   }
}