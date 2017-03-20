using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization.Exchange;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_MoBiExportTask : ContextSpecification<IMoBiExportTask>
   {
      protected IBuildConfigurationTask _buildConfigurationTask;
      protected ISimulationToModelCoreSimulationMapper _simulationMapper;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected IPKSimConfiguration _configuration;
      protected ILazyLoadTask _lazyLoadTask;
      protected IDialogCreator _dialogCreator;
      protected ICoreCalculationMethodRepository _coreCalculationMethodRepository;
      protected ISimulationPersistor _simulationPersistor;
      protected IProjectRetriever _projectRetriever;
      protected IObjectIdResetter _objectIdResetter;
      private IJournalRetriever _journalRetriever;

      protected override void Context()
      {
         _buildConfigurationTask = A.Fake<IBuildConfigurationTask>();
         _simulationMapper = A.Fake<ISimulationToModelCoreSimulationMapper>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _configuration = A.Fake<IPKSimConfiguration>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _simulationPersistor = A.Fake<ISimulationPersistor>();
         _projectRetriever = A.Fake<IProjectRetriever>();
         _objectIdResetter= A.Fake<IObjectIdResetter>();
         _journalRetriever= A.Fake<IJournalRetriever>();

         sut = new MoBiExportTask(_buildConfigurationTask, _simulationMapper, _representationInfoRepository,
            _configuration, _lazyLoadTask, _dialogCreator, _simulationPersistor, _projectRetriever, _objectIdResetter,_journalRetriever);
      }
   }

   public class When_exporting_a_simulation_to_pkml_file_that_was_imported_from_pkml : concern_for_MoBiExportTask
   {
      private Simulation _sim;
      private string _fileName;

      protected override void Context()
      {
         base.Context();
         _sim = A.Fake<Simulation>();
         _fileName = "toto";
         A.CallTo(() => _sim.IsImported).Returns(true);
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.SaveSimulationToFile(_sim, _fileName)).ShouldThrowAn<PKSimException>();
      }
   }

   public class When_exporting_a_simulation_to_pkml_file : concern_for_MoBiExportTask
   {
      private Simulation _sim;
      private string _fileName;
      private SimulationTransfer _simulationTransfer;
      private DataRepository _observedData1;

      protected override void Context()
      {
         base.Context();
         _sim = A.Fake<Simulation>();
         _fileName = "toto";
         _observedData1=new DataRepository();
         A.CallTo(() => _sim.UsedObservedData).Returns(new[] {new UsedObservedData {Id = "OBS"}});
         A.CallTo(() => _sim.IsImported).Returns(false);
         A.CallTo(() => _simulationPersistor.Save(A<SimulationTransfer>._, _fileName))
            .Invokes(x => _simulationTransfer = x.GetArgument<SimulationTransfer>(0));

         A.CallTo(() => _projectRetriever.CurrentProject.Favorites).Returns(new Favorites {"FAV1"});
         A.CallTo(() => _projectRetriever.CurrentProject.ObservedDataBy("OBS")).Returns(_observedData1);
      }

      protected override void Because()
      {
         sut.SaveSimulationToFile(_sim, _fileName);
      }

      [Observation]
      public void should_export_the_observed_data_defined_in_the_current_project()
      {
         _simulationTransfer.AllObservedData.ShouldOnlyContain(_observedData1);
      }

      [Observation]
      public void should_export_the_favorites_data_defined_in_the_current_project()
      {
         _simulationTransfer.Favorites.ShouldOnlyContain("FAV1");
      }

      [Observation]
      public void should_have_resetted_the_id_of_the_simulation()
      {
         A.CallTo(() => _objectIdResetter.ResetIdFor(_simulationTransfer.Simulation)).MustHaveHappened();
      }
   }
}