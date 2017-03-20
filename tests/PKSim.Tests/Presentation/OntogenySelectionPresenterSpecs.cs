using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation
{
   public abstract class concern_for_OntogenySelectionPresenter : ContextSpecification<IOntogenySelectionPresenter<Individual>>
   {
      protected IOntogenySelectionView _view;
      protected IOntogenyRepository _ontogenyRepository;
      protected IOntogenyTask<Individual> _ontogenyTask;
      protected List<Ontogeny> _allOntogenies;
      protected ICommandCollector _commandRegister;

      protected override void Context()
      {
         _view = A.Fake<IOntogenySelectionView>();
         _ontogenyRepository = A.Fake<IOntogenyRepository>();
         _ontogenyTask = A.Fake<IOntogenyTask<Individual>>();
         _commandRegister = A.Fake<ICommandCollector>();
         _allOntogenies = new List<Ontogeny>();
         A.CallTo(() => _ontogenyRepository.AllFor(CoreConstants.Species.Human)).Returns(_allOntogenies);
         sut = new OntogenySelectionPresenter<Individual>(_view, _ontogenyRepository, _ontogenyTask);
         sut.InitializeWith(_commandRegister);
      }
   }

   public class When_the_ontogeny_selection_presenter_is_asked_for_the_available_ontogenies : concern_for_OntogenySelectionPresenter
   {
      private Ontogeny _onto1;
      private Ontogeny _onto2;
      private List<Ontogeny> _allAvailableOntogenies;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         _onto1 = new DatabaseOntogeny();
         _onto2 = new DatabaseOntogeny();
         _allOntogenies.AddRange(new[] {_onto1, _onto2});
         sut.Edit(new IndividualEnzyme {Ontogeny = _onto1}, _individual);
      }

      protected override void Because()
      {
         _allAvailableOntogenies = sut.AllOntogenies().ToList();
      }

      [Observation]
      public void should_return_the_defined_ontogenies_and_the_null_ontogeny_as_default()
      {
         _allAvailableOntogenies.ShouldContain(_onto1, _onto2);
         _allAvailableOntogenies.ElementAt(0).ShouldBeAnInstanceOf<NullOntogeny>();
         _allAvailableOntogenies.Count.ShouldBeEqualTo(3);
      }
   }

   public class When_the_ontogeny_selection_presenter_is_editing_a_protein : concern_for_OntogenySelectionPresenter
   {
      private IndividualProtein _protein;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         _protein = A.Fake<IndividualProtein>();
      }

      protected override void Because()
      {
         sut.Edit(_protein, _individual);
      }

      [Observation]
      public void should_set_the_selected_ontogeny_for_given_protein_into_the_view()
      {
         A.CallTo(() => _view.BindTo(_protein)).MustHaveHappened();
      }
   }

   public class When_the_user_selects_a_new_ontogeny_for_the_edited_protein : concern_for_OntogenySelectionPresenter
   {
      private IndividualProtein _protein;
      private Ontogeny _selectedOntogeny;
      private IPKSimCommand _command;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _selectedOntogeny = new DatabaseOntogeny();
         _individual = A.Fake<Individual>();
         _protein = A.Fake<IndividualProtein>();
         _command = A.Fake<IPKSimCommand>();
         A.CallTo(() => _ontogenyTask.SetOntogenyForMolecule(_protein, _selectedOntogeny, _individual)).Returns(_command);
         sut.Edit(_protein, _individual);
      }

      protected override void Because()
      {
         sut.SelectedOntogenyIs(_selectedOntogeny);
      }

      [Observation]
      public void should_change_the_ontogeny_for_the_selected_protein()
      {
         A.CallTo(() => _ontogenyTask.SetOntogenyForMolecule(_protein, _selectedOntogeny, _individual)).MustHaveHappened();
      }

      [Observation]
      public void should_register_the_resulting_command_into_the_command_register()
      {
         A.CallTo(() => _commandRegister.AddCommand(_command)).MustHaveHappened();
      }
   }
}