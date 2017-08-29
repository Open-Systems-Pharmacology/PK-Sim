using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_UsedMoleculeRepository : ContextSpecification<IUsedMoleculeRepository>
   {
      private IPKSimProjectRetriever _projectRetriever;
      private PKSimProject _project;
      private Compound _compound1;
      private Compound _compound2;
      private Individual _individual1;
      private Individual _individual2;

      protected override void Context()
      {
         _project = new PKSimProject();
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();
         A.CallTo(() => _projectRetriever.Current).Returns(_project);

         _compound1 = A.Fake<Compound>();
         _compound1.IsLoaded = false;
         _compound2 = new Compound {IsLoaded = true};
         _compound2.AddProcess(new EnzymaticProcess {MoleculeName = "A", Name = "P1"});
         _compound2.AddProcess(new EnzymaticProcess {MoleculeName = "C", Name = "P2"});
         _compound2.AddProcess(new EnzymaticProcess {MoleculeName = "B", Name = "P3"});

         _individual1 = A.Fake<Individual>();
         _individual1.IsLoaded = false;
         _individual2 = new Individual {IsLoaded = true};
         _individual2.AddMolecule(new IndividualEnzyme().WithName("B"));
         _individual2.AddMolecule(new IndividualEnzyme().WithName("D"));

         _project.AddBuildingBlock(_compound1);
         _project.AddBuildingBlock(_compound2);
         _project.AddBuildingBlock(_individual1);
         _project.AddBuildingBlock(_individual2);
         sut = new UsedMoleculeRepository(_projectRetriever);
      }
   }

   public class When_retrieving_the_molecules_available_in_projects : concern_for_UsedMoleculeRepository
   {
      private IEnumerable<string> _moleculeNames;

      protected override void Because()
      {
         _moleculeNames = sut.All();
      }

      [Observation]
      public void should_return_the_distinct_molecule_names_defined_in_loaded_compounds_and_individuals()
      {
         _moleculeNames.ShouldOnlyContainInOrder("A", "B", "C", "D");
      }
   }
}