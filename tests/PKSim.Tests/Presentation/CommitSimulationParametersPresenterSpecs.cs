using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_CommitSimulationParametersPresenter : ContextSpecification<CommitSimulationParametersPresenter>
   {
      protected ICommitSimulationParametersView _view;
      protected IContainerTask _containerTask;
      protected IPKSimProjectRetriever _projectRetriever;
      protected IndividualSimulation _simulation;
      protected Compound _templateCompound;
      protected Compound _simulationCompound;
      protected PKSimProject _project;
      protected PathCache<IParameter> _parameterCache;

      protected override void Context()
      {
         _view = A.Fake<ICommitSimulationParametersView>();
         _containerTask = A.Fake<IContainerTask>();
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();

         _templateCompound = new Compound { Name = "Aspirin", Id = "TemplateId" };
         _simulationCompound = new Compound { Name = "Aspirin", Id = "SimCompId" };

         _project = new PKSimProject();
         _project.AddBuildingBlock(_templateCompound);
         A.CallTo(() => _projectRetriever.Current).Returns(_project);

         var root = new Container { Name = "Sim" };
         var lipophilicity = DomainHelperForSpecs.ConstantParameterWithValue(3.5).WithName("Lipophilicity");
         root.Add(lipophilicity);

         _simulation = new IndividualSimulation
         {
            Id = "SimId",
            Model = new OSPSuite.Core.Domain.Model { Root = root }
         };

         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("TemplateId", PKSimBuildingBlockType.Compound)
         {
            BuildingBlock = _simulationCompound
         });

         _parameterCache = new PathCacheForSpecs<IParameter>();
         _parameterCache.Add("Organism|Aspirin|Lipophilicity", lipophilicity);
         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(root)).Returns(_parameterCache);

         sut = new CommitSimulationParametersPresenter(_view, _containerTask, _projectRetriever);
      }
   }

   public class When_showing_commit_dialog_and_user_confirms : concern_for_CommitSimulationParametersPresenter
   {
      private IReadOnlyList<CompoundCommitInfo> _result;

      protected override void Context()
      {
         base.Context();
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         _result = sut.ShowCommitDialog(_simulation);
      }

      [Observation]
      public void should_return_commit_infos()
      {
         _result.ShouldNotBeNull();
         _result.Count.ShouldBeEqualTo(1);
         _result[0].Compound.ShouldBeEqualTo(_templateCompound);
      }

      [Observation]
      public void should_have_displayed_the_view()
      {
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }
   }

   public class When_showing_commit_dialog_and_user_cancels : concern_for_CommitSimulationParametersPresenter
   {
      private IReadOnlyList<CompoundCommitInfo> _result;

      protected override void Context()
      {
         base.Context();
         _simulation.ParameterChangeTracker.Track("Organism|Aspirin|Lipophilicity");
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override void Because()
      {
         _result = sut.ShowCommitDialog(_simulation);
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }
   }

   public class When_showing_commit_dialog_with_no_tracked_changes : concern_for_CommitSimulationParametersPresenter
   {
      private IReadOnlyList<CompoundCommitInfo> _result;

      protected override void Because()
      {
         _result = sut.ShowCommitDialog(_simulation);
      }

      [Observation]
      public void should_return_null_without_showing_dialog()
      {
         _result.ShouldBeNull();
         A.CallTo(() => _view.Display()).MustNotHaveHappened();
      }
   }
}
