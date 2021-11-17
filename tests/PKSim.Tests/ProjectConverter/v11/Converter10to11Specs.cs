using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v10;
using PKSim.Infrastructure.ProjectConverter.v11;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v11
{
   public class When_converting_the_simple_project_730_project_to_11 : ContextWithLoadedProject<Converter10to11>
   {
      private List<PopulationSimulation> _allSimulations;
      private List<Population> _allPopulations;
      private List<Individual> _allIndividuals;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimplePop_73");
         _allSimulations = All<PopulationSimulation>().ToList();
         _allPopulations = All<Population>().ToList();
         _allIndividuals = All<Individual>().ToList();
         _allSimulations.Each(Load);
         _allPopulations.Each(Load);
         _allIndividuals.Each(Load);
      }

      [Observation]
      public void should_have_created_an_expression_profile_for_the_individual()
      {
         var ind = _allIndividuals.FindByName("Human");
         var cyp3A4 = ind.MoleculeByName<IndividualEnzyme>("CYP3A4");
         var expressionProfile = FindByName<ExpressionProfile>(CoreConstants.ContainerName.ExpressionProfileName(cyp3A4.Name, ind.Species, ind.Name));
         expressionProfile.ShouldNotBeNull();
      }
   }

}