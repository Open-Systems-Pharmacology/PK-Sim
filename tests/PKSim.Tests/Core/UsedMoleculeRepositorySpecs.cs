using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
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
      private ExpressionProfile _expressionProfile;
      private IOntogenyRepository _ontogenyRepository;
      private IMoleculeParameterRepository _moleculeParameterRepository;
      private ITransporterContainerTemplateRepository _transporterContainerTemplateRepository;

      protected override void Context()
      {
         _project = new PKSimProject();
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();
         _transporterContainerTemplateRepository= A.Fake<ITransporterContainerTemplateRepository>();
         A.CallTo(() => _projectRetriever.Current).Returns(_project);

         _compound1 = A.Fake<Compound>();
         _compound1.IsLoaded = false;
         _compound2 = new Compound {IsLoaded = true};
         _compound2.AddProcess(new EnzymaticProcess {MoleculeName = "ProjA", Name = "P1"});
         _compound2.AddProcess(new EnzymaticProcess {MoleculeName = "ProjC", Name = "P2"});
         _compound2.AddProcess(new EnzymaticProcess {MoleculeName = "ProjB", Name = "P3"});


         _ontogenyRepository = A.Fake<IOntogenyRepository>();
         _moleculeParameterRepository = A.Fake<IMoleculeParameterRepository>();

         _expressionProfile = A.Fake<ExpressionProfile>();
         _expressionProfile.IsLoaded = true;
         A.CallTo(() => _expressionProfile.MoleculeName).Returns("ProjE");

         _project.AddBuildingBlock(_compound1);
         _project.AddBuildingBlock(_compound2);
         _project.AddBuildingBlock(_expressionProfile);
         sut = new UsedMoleculeRepository(_projectRetriever, _ontogenyRepository, _moleculeParameterRepository, _transporterContainerTemplateRepository);

         var molParam1 = new MoleculeParameter {MoleculeName = "DbB"};
         var molParam2 = new MoleculeParameter {MoleculeName = "DbA"};
         A.CallTo(() => _moleculeParameterRepository.All()).Returns(new[] {molParam1, molParam2});
         A.CallTo(() => _transporterContainerTemplateRepository.AllTransporterNames).Returns(new[] {"ATRANS1", "TRANS2"});

         A.CallTo(() => _ontogenyRepository.AllFor(CoreConstants.Species.HUMAN)).Returns(new[] {new DatabaseOntogeny {Name = "OntoC"}});
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
      public void should_return_the_distinct_molecule_names_defined_in_loaded_compounds_and_expression_profile_first_followed_by_the_predefined_molecule_names()
      {
         _moleculeNames.ShouldOnlyContainInOrder("ProjA", "ProjB", "ProjC", "ProjE", "ATRANS1", "DbA", "DbB", "OntoC", "TRANS2");
      }
   }
}