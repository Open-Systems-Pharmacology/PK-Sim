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
   public abstract class concern_for_PartialProcessRetriever : ContextSpecification<IPartialProcessRetriever>
   {
      protected Simulation _simulation;
      protected Individual _individual;
      protected Compound _compound;
      protected EnzymaticProcess _notSelectedPartialProcess;

      protected override void Context()
      {
         _simulation = A.Fake<Simulation>();
         _individual = new Individual();
         _compound = new Compound().WithName("Comp");
         A.CallTo(() => _simulation.Individual).Returns(_individual);
         A.CallTo(() => _simulation.Compounds).Returns(new []{_compound});

         sut = new PartialProcessRetriever();
         _notSelectedPartialProcess = A.Fake<EnzymaticProcess>();
         sut.NotSelectedPartialProcess = _notSelectedPartialProcess;

      }
   }

   public class When_retrieving_the_partial_processes_from_a_given_type_that_could_be_created_in_a_simulation : concern_for_PartialProcessRetriever
   {
      private IEnumerable<SimulationPartialProcess> _results;
      private EnzymaticProcess _pp2;
      private EnzymaticProcess _pp1;
      private EnzymaticProcess _pp3;
      private IndividualEnzyme _enzyme;
      private List<ProcessSelection> _availableProcesses;
      private EnzymaticProcess _pp4;
      private IndividualEnzyme _anotherProtein;
      private string _moleculeName;
      private EnzymaticProcess _pp5;
      private string _unknowProteinName;
      private IndividualEnzyme _notMappedProtein;

      protected override void Context()
      {
         base.Context();
         _pp1 = A.Fake<EnzymaticProcess>().WithName("pp1");
         _pp1.MoleculeName = "enzyme1";

         _pp2 = A.Fake<EnzymaticProcess>().WithName("pp2");
         _pp2.MoleculeName = "enzyme1";

         _pp3 = A.Fake<EnzymaticProcess>().WithName("pp3");
         _pp3.MoleculeName = "enzyme3";

         _pp4 = A.Fake<EnzymaticProcess>().WithName("pp4");
         _pp4.MoleculeName = "enzyme4";

         _pp5 = A.Fake<EnzymaticProcess>().WithName("pp5");
         _pp5.MoleculeName = "enzyme5";

         _unknowProteinName = "unknow";
         _moleculeName = "TITI";
        
         _compound.AddProcess(_pp1);
         _compound.AddProcess(_pp2);
         _compound.AddProcess(_pp3);
         _compound.AddProcess(_pp4);
         _compound.AddProcess(_pp5);
         _enzyme = A.Fake<IndividualEnzyme>().WithName(_pp3.MoleculeName);
         _anotherProtein = A.Fake<IndividualEnzyme>().WithName(_moleculeName);
         _notMappedProtein = A.Fake<IndividualEnzyme>().WithName("not mapped");
         _individual.AddMolecule(_enzyme);
         _individual.AddMolecule(_anotherProtein);
         _individual.AddMolecule(_notMappedProtein);

         _availableProcesses = new List<ProcessSelection>
         {
            new ProcessSelection {ProcessName = _pp3.Name, MoleculeName = _notMappedProtein.Name,CompoundName = "DOES NOT EXIST"},
            new ProcessSelection {ProcessName = _pp4.Name, MoleculeName = _moleculeName,CompoundName = _compound.Name},
            new ProcessSelection {ProcessName = _pp5.Name, MoleculeName = _unknowProteinName,CompoundName = _compound.Name}
         };
      }

      protected override void Because()
      {
         _results = sut.AllFor<IndividualEnzyme, EnzymaticProcess>(_simulation, _compound, _availableProcesses, addDefaultPartialProcess: true);
      }

      [Observation]
      public void should_contain_one_partial_process_mapping_for_each_enzyme_defined_in_the_individual()
      {
         _results.Count().ShouldBeEqualTo(_individual.AllMolecules().Count());
      }

      [Observation]
      public void should_have_mapped_a_valid_expression_for_a_protein_available_in_the_compound()
      {
         var mappedEnzyme = _results.First(x => x.MoleculeName.Equals(_enzyme.Name));
         mappedEnzyme.IndividualMolecule.ShouldBeEqualTo(_enzyme);
         mappedEnzyme.CompoundProcess.ShouldBeEqualTo(_pp3);
      }

      [Observation]
      public void should_have_mapped_a_non_existing_predefined_mmapping_to_a_non_existant_mapping()
      {
         var mappedEnzyme = _results.First(x => x.MoleculeName.Equals(_notMappedProtein.Name));
         mappedEnzyme.CompoundProcess.ShouldBeEqualTo(_notSelectedPartialProcess);
      }

      [Observation]
      public void should_have_used_the_predefined_settings_if_available()
      {
         var mappedEnzyme = _results.First(x => x.MoleculeName.Equals(_anotherProtein.Name));
         mappedEnzyme.CompoundProcess.ShouldBeEqualTo(_pp4);
      }
   }

}