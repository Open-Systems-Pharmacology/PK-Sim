using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_InteractionProcessRetriever : ContextSpecification<IInteractionProcessRetriever>
   {
      private IPartialProcessRetriever _partialProcessRetriever;
      protected Individual _individual;
      protected Compound _compound;
      protected Simulation _simulation;
      protected InteractionProcess _notSelectedInteractionProcess;

      protected override void Context()
      {
         _partialProcessRetriever = new PartialProcessRetriever();
         _individual = new Individual();
         _simulation = A.Fake<Simulation>();
         _compound = new Compound().WithName("Comp");
         _notSelectedInteractionProcess = A.Fake<InteractionProcess>();
         A.CallTo(() => _simulation.Individual).Returns(_individual);
         A.CallTo(() => _simulation.Compounds).Returns(new[] {_compound});

         sut = new InteractionProcessRetriever(_partialProcessRetriever) {NotSelectedInteractionProcess = _notSelectedInteractionProcess};
      }
   }

   public class When_retreving_all_possible_processes_of_a_given_type_for_a_simulation_using_default_map : concern_for_InteractionProcessRetriever
   {
      private IReadOnlyList<SimulationPartialProcess> _results;
      private Compound _compound2;
      private IndividualMolecule _moleculeMappable;
      private InhibitionProcess _inhibitionProcess1;
      private InhibitionProcess _inhibitionProcess2;
      private IndividualEnzyme _moleculeNotMapped;
      private IndividualEnzyme _moleculeAlreadySelected;
      private InductionProcess _inductionProcess1;
      private InhibitionProcess _inhibitionProcess3;

      protected override void Context()
      {
         base.Context();
         _moleculeMappable = new IndividualEnzyme().WithName("MoleculeMapped");
         _moleculeAlreadySelected = new IndividualEnzyme().WithName("MoleculeAlreadySelected");
         _moleculeNotMapped = new IndividualEnzyme().WithName("MoleculeNotMapped");
         _inhibitionProcess1 = new InhibitionProcess().WithName("InhibitionProcess1");
         _inhibitionProcess2 = new InhibitionProcess().WithName("InhibitionProcess2");
         _inductionProcess1 = new InductionProcess().WithName("InductionProcess1");
         _inhibitionProcess3 = new InhibitionProcess().WithName("InhibitionProcess3");

         _individual.AddMolecule(_moleculeNotMapped);
         _individual.AddMolecule(_moleculeMappable);
         _individual.AddMolecule(_moleculeAlreadySelected);

         _compound.AddProcess(_inhibitionProcess1);

         _compound2 = new Compound().WithName("Comp2");
         _compound2.AddProcess(_inhibitionProcess2);
         _inhibitionProcess2.MoleculeName = _moleculeMappable.Name;

         _compound2.AddProcess(_inhibitionProcess3);
         _inhibitionProcess3.MoleculeName = _moleculeMappable.Name;

         _compound2.AddProcess(_inductionProcess1);
         _inductionProcess1.MoleculeName = _moleculeMappable.Name;

         var interactionProperties = new InteractionProperties();
         interactionProperties.AddInteraction(new InteractionSelection {MoleculeName = _moleculeAlreadySelected.Name, ProcessName = _inhibitionProcess1.Name, CompoundName = _compound.Name});

         A.CallTo(() => _simulation.InteractionProperties).Returns(interactionProperties);
         A.CallTo(() => _simulation.Compounds).Returns(new[] {_compound, _compound2});
      }

      protected override void Because()
      {
         _results = sut.AllFor(_simulation, _simulation.InteractionProperties.Interactions, addDefaultPartialProcess: true);
      }

      [Observation]
      public void should_only_return_well_mapped_processes()
      {
         _results.Count.ShouldBeEqualTo(3);
         _results.Any(x => x.CompoundProcess == _notSelectedInteractionProcess).ShouldBeFalse();
      }

      [Observation]
      public void should_return_the_well_defined_preselected_processes()
      {
         var processMapping = _results.First(x => x.MoleculeName == _moleculeAlreadySelected.Name);
         processMapping.ProcessName.ShouldBeEqualTo(_inhibitionProcess1.Name);
      }

      [Observation]
      public void should_return_the_default_preselected_process_that_could_be_mapped_automatically_for_all_different_types_of_interactions()
      {
         var processMappings = _results.Where(x => x.MoleculeName == _moleculeMappable.Name);
         processMappings.Select(x=>x.ProcessName).ShouldContain(_inhibitionProcess2.Name, _inductionProcess1.Name);
      }

      [Observation]
      public void should_not_mapped_map_more_than_one_intereaction_process_for_the_same_type()
      {
         var processMappings = _results.Where(x => x.MoleculeName == _moleculeMappable.Name);
         processMappings.Select(x => x.ProcessName).ShouldNotContain(_inhibitionProcess3.Name);
      }

      [Observation]
      public void should_not_contain_entry_for_molecules_that_could_not_be_mapped_automatically()
      {
         _results.Any(x => x.MoleculeName == _moleculeNotMapped.Name).ShouldBeFalse();
      }
   }
}