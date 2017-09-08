using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization.Exchange;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SimulationTransferLoader : ContextSpecification<ISimulationTransferLoader>
   {
      protected IDimensionFactory _dimensionFactory;
      protected IObjectBaseFactory _objectBaseFactory;
      protected ISimulationPersistor _simulationPersister;
      protected IProjectRetriever _projectRetriever;
      protected IDialogCreator _dialogCreator;
      private string _pkmlFile;
      protected SimulationTransfer _simulationTransfer;
      protected SimulationTransfer _result;
      protected PKSimProject _project;
      protected IJournalTask _journalTask;
      private ICloneManagerForModel _cloneManagerForModel;

      protected override void Context()
      {
         _pkmlFile = "file";
         _project = new PKSimProject();
         _dimensionFactory = A.Fake<IDimensionFactory>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _simulationPersister = A.Fake<ISimulationPersistor>();
         _projectRetriever = A.Fake<IProjectRetriever>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _journalTask = A.Fake<IJournalTask>();
         _cloneManagerForModel= A.Fake<ICloneManagerForModel>();

         sut = new SimulationTransferLoader(_dimensionFactory, _objectBaseFactory, _simulationPersister, _projectRetriever, _dialogCreator, _journalTask,_cloneManagerForModel);
         _simulationTransfer = new SimulationTransfer();
         A.CallTo(() => _projectRetriever.CurrentProject).Returns(_project);
         A.CallTo(() => _simulationPersister.Load(_pkmlFile, _dimensionFactory, _objectBaseFactory, A<IWithIdRepository>._,_cloneManagerForModel)).Returns(_simulationTransfer);
      }

      protected override void Because()
      {
         _result = sut.Load(_pkmlFile);
      }
   }

   public class When_loading_a_simulation : concern_for_SimulationTransferLoader
   {
      [Observation]
      public void should_simply_return_the_simulation()
      {
         _result.ShouldBeEqualTo(_simulationTransfer);
      }
   }

   public class When_loading_a_simulation_that_contains_information_on_the_used_journal_and_no_jounral_is_currently_loaded : concern_for_SimulationTransferLoader
   {
      protected override void Context()
      {
         base.Context();
         _simulationTransfer.JournalPath = "XX";
         _project.JournalPath = "";
      }

      [Observation]
      public void should_load_the_journal()
      {
         A.CallTo(() => _journalTask.LoadJournal(_simulationTransfer.JournalPath,false)).MustHaveHappened();
      }
   }
}