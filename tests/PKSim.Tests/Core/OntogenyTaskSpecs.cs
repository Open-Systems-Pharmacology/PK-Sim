using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.Core;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_OntogenyTask : ContextSpecification<IOntogenyTask>
   {
      private IExecutionContext _context;
      protected IApplicationController _applicationController;
      protected IDataImporter _dataImporter;
      private IDimensionRepository _dimensionRepository;
      private IOntogenyRepository _ontogenyRepository;
      private IEntityTask _entityTask;
      private IFormulaFactory _formulaFactory;
      protected IDialogCreator _dialogCreator;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _applicationController = A.Fake<IApplicationController>();
         _dataImporter = A.Fake<IDataImporter>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _ontogenyRepository = A.Fake<IOntogenyRepository>();
         _entityTask = A.Fake<IEntityTask>();
         _formulaFactory = A.Fake<IFormulaFactory>();
         _dialogCreator = A.Fake<IDialogCreator>();
         sut = new OntogenyTask(_context, _applicationController, _dataImporter, _dimensionRepository, _ontogenyRepository, _entityTask, _formulaFactory, _dialogCreator);
      }
   }

   public class When_the_ontogeny_task_is_asked_to_show_the_data_for_an_undefined_ontogeny : concern_for_OntogenyTask
   {
      protected override void Because()
      {
         sut.ShowOntogenyData(new NullOntogeny());
      }

      [Observation]
      public void should_not_show_the_view_containing_the_data()
      {
         A.CallTo(() => _applicationController.Start<IShowOntogenyDataPresenter>()).MustNotHaveHappened();
      }
   }

   public class When_the_ontogeny_task_is_asked_to_show_the_data_for_an_defined_ontogeny : concern_for_OntogenyTask
   {
      private IShowOntogenyDataPresenter _presenter;
      private Ontogeny _ontogeny;

      protected override void Context()
      {
         base.Context();
         _presenter = A.Fake<IShowOntogenyDataPresenter>();
         A.CallTo(() => _applicationController.Start<IShowOntogenyDataPresenter>()).Returns(_presenter);
      }

      protected override void Because()
      {
         _ontogeny = new DatabaseOntogeny();
         sut.ShowOntogenyData(_ontogeny);
      }

      [Observation]
      public void should_show_the_view_containing_the_data()
      {
         A.CallTo(() => _applicationController.Start<IShowOntogenyDataPresenter>()).MustHaveHappened();
      }

      [Observation]
      public void should_display_the_ontogeny_in_the_view()
      {
         _presenter.Show(_ontogeny);
      }
   }

   public class When_loading_some_ontogeny_from_file_and_the_user_cancels_the_loading_action : concern_for_OntogenyTask
   {
      private IndividualMolecule _molecule;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _molecule = new IndividualEnzyme();
         _individual= A.Fake<Individual>();
         A.CallTo(_dataImporter).WithReturnType<DataRepository>().Returns(null);
      }

      [Observation]
      public void should_return_an_empty_command()
      {
         sut.LoadOntogenyForMolecule(_molecule, _individual).IsEmpty().ShouldBeTrue();
      }
   }
}