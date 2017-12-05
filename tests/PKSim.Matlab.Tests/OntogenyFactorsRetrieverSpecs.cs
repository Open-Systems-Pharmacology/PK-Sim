using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Matlab
{
   public abstract class concern_for_OntogenyFactorsRetriever : ContextSpecification<IOntogenyFactorsRetriever>
   {
      protected IOntogenyRepository _ontogenyRepository;
      protected List<MoleculeOntogeny> _moleculeNames;

      protected override void Context()
      {
         _ontogenyRepository = A.Fake<IOntogenyRepository>();
         sut = new OntogenyFactorsRetriever(_ontogenyRepository);
         _moleculeNames = new List<MoleculeOntogeny>();
      }
   }

   public class When_retrieving_the_ontogeny_factors_for_an_empty_list : concern_for_OntogenyFactorsRetriever
   {
      [Observation]
      public void should_return_that_no_factor_is_available()
      {
         sut.FactorsFor(new Core.Model.OriginData {Species = new Species {Name = "Dog"}}, _moleculeNames).ShouldBeEmpty();
      }
   }

   public class When_retrieving_the_ontogeny_for_a_species_for_which_no_ontogeny_were_defined : concern_for_OntogenyFactorsRetriever
   {
      private Core.Model.OriginData _originData;

      protected override void Context()
      {
         base.Context();
         _originData = new Core.Model.OriginData {Species = new Species {Name = "Dog"}};
         A.CallTo(() => _ontogenyRepository.AllFor(_originData.Species.Name)).Returns(new List<Ontogeny>());
         _moleculeNames.Add(new MoleculeOntogeny("CYP", "CYP3A4"));
      }

      [Observation]
      public void should_return_that_no_factor_is_available()
      {
         sut.FactorsFor(_originData, _moleculeNames).ShouldBeEmpty();
      }
   }

   public class When_retrieving_the_ontogeny_for_a_species_for_which_ontogeny_were_defined_but_not_for_the_given_molecule : concern_for_OntogenyFactorsRetriever
   {
      private Core.Model.OriginData _originData;
      private Ontogeny _ontogeny;

      protected override void Context()
      {
         base.Context();
         _ontogeny = new DatabaseOntogeny {SpeciesName = "Dog", Name = "UD6"};
         _originData = new Core.Model.OriginData {Species = new Species {Name = "Dog"}};
         A.CallTo(() => _ontogenyRepository.AllFor(_originData.Species.Name)).Returns(new[] {_ontogeny});
         _moleculeNames.Add(new MoleculeOntogeny("CYP", "CYP3A4"));
      }

      [Observation]
      public void should_return_that_no_factor_is_available()
      {
         sut.FactorsFor(_originData, _moleculeNames).ShouldBeEmpty();
      }
   }

   public class When_retrieving_the_ontogeny_for_a_species_and_molecule_for_which_ontogeny_were_defined : concern_for_OntogenyFactorsRetriever
   {
      private Core.Model.OriginData _originData;
      private Ontogeny _ontogeny;
      private IEnumerable<ParameterValue> _allFactors;

      protected override void Context()
      {
         base.Context();
         _ontogeny = new DatabaseOntogeny {SpeciesName = "Dog", Name = "CYP3A4"};
         _originData = new Core.Model.OriginData {Species = new Species {Name = "Dog"}};
         A.CallTo(() => _ontogenyRepository.AllFor(_originData.Species.Name)).Returns(new[] {_ontogeny});
         A.CallTo(() => _ontogenyRepository.OntogenyFactorFor(_ontogeny, CoreConstants.Groups.ONTOGENY_LIVER, _originData, null)).Returns(0.5);
         _moleculeNames.Add(new MoleculeOntogeny("CYP", "CYP3A4"));
      }

      protected override void Because()
      {
         _allFactors = sut.FactorsFor(_originData, _moleculeNames);
      }

      [Observation]
      public void should_return_the_defined_factor_for_the_individual()
      {
         _allFactors.Count().ShouldBeEqualTo(2);
         _allFactors.ElementAt(0).Value.ShouldBeEqualTo(0.5);
         _allFactors.ElementAt(0).ParameterPath.ShouldBeEqualTo("CYP|Ontogeny factor");
      }
   }

   public class When_retrieving_the_ontogenies_for_various_containers : concern_for_OntogenyFactorsRetriever
   {
      private Core.Model.OriginData _originData;
      private IEnumerable<ParameterValue> _allFactors;

      protected override void Context()
      {
         base.Context();
         var ontogeny1 = new DatabaseOntogeny {SpeciesName = "Dog", Name = "CYP3A1"};
         var ontogeny2 = new DatabaseOntogeny {SpeciesName = "Dog", Name = "CYP3A2"};
         var ontogeny3 = new DatabaseOntogeny {SpeciesName = "Dog", Name = "CYP3A3"};

         _originData = new Core.Model.OriginData {Species = new Species {Name = "Dog"}};
         A.CallTo(() => _ontogenyRepository.AllFor(_originData.Species.Name)).Returns(new[] {ontogeny1, ontogeny2, ontogeny3});
         A.CallTo(() => _ontogenyRepository.OntogenyFactorFor(ontogeny1, CoreConstants.Groups.ONTOGENY_LIVER, _originData, null)).Returns(0.1);
         A.CallTo(() => _ontogenyRepository.OntogenyFactorFor(ontogeny2, CoreConstants.Groups.ONTOGENY_DUODENUM, _originData, null)).Returns(0.22);
         A.CallTo(() => _ontogenyRepository.OntogenyFactorFor(ontogeny2, CoreConstants.Groups.ONTOGENY_LIVER, _originData, null)).Returns(0.21);
         A.CallTo(() => _ontogenyRepository.OntogenyFactorFor(ontogeny3, CoreConstants.Groups.ONTOGENY_LIVER, _originData, null)).Returns(0.31);

         _moleculeNames.Add(new MoleculeOntogeny("CYP1", "CYP3A1"));
         _moleculeNames.Add(new MoleculeOntogeny("CYP2", "CYP3A2"));
         _moleculeNames.Add(new MoleculeOntogeny("CYP3", "CYP3A3"));
         _moleculeNames.Add(new MoleculeOntogeny("CYP4", "ENZYME_NOT_FOUND"));
      }

      protected override void Because()
      {
         _allFactors = sut.FactorsFor(_originData, _moleculeNames);
      }

      [Observation]
      public void should_return_the_defined_factors_for_the_individual()
      {
         _allFactors.Count().ShouldBeEqualTo(3 * 2);
      }

      [Observation]
      public void should_have_created_a_valid_path_for_CYP3A1()
      {
         _allFactors.ElementAt(0).Value.ShouldBeEqualTo(0.1);
         _allFactors.ElementAt(0).ParameterPath.ShouldBeEqualTo("CYP1|Ontogeny factor");

         _allFactors.ElementAt(1).Value.ShouldBeEqualTo(0);
         _allFactors.ElementAt(1).ParameterPath.ShouldBeEqualTo("CYP1|Ontogeny factor GI");
      }

      [Observation]
      public void should_have_created_a_valid_path_for_CYP3A2()
      {
         _allFactors.ElementAt(2).Value.ShouldBeEqualTo(0.21);
         _allFactors.ElementAt(2).ParameterPath.ShouldBeEqualTo("CYP2|Ontogeny factor");

         _allFactors.ElementAt(3).Value.ShouldBeEqualTo(0.22);
         _allFactors.ElementAt(3).ParameterPath.ShouldBeEqualTo("CYP2|Ontogeny factor GI");
      }

      [Observation]
      public void should_have_created_a_valid_path_for_CYP3A3()
      {
         _allFactors.ElementAt(4).Value.ShouldBeEqualTo(0.31);
         _allFactors.ElementAt(4).ParameterPath.ShouldBeEqualTo("CYP3|Ontogeny factor");

         _allFactors.ElementAt(5).Value.ShouldBeEqualTo(0);
         _allFactors.ElementAt(5).ParameterPath.ShouldBeEqualTo("CYP3|Ontogeny factor GI");
      }
   }
}