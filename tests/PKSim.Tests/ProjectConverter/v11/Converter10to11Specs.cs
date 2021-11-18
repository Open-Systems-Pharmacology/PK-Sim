using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
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

   public class When_converting_the_simple_project_expression_v9_project_to_11 : ContextWithLoadedProject<Converter10to11>
   {
      private List<PopulationSimulation> _allSimulations;
      private List<Population> _allPopulations;
      private List<Individual> _allIndividuals;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("expression_v9");
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
         var ind = _allIndividuals.FindByName("IND");
         var cyp3A4 = ind.MoleculeByName<IndividualEnzyme>("CYP3A4");
         var expressionProfile = FindByName<ExpressionProfile>(CoreConstants.ContainerName.ExpressionProfileName(cyp3A4.Name, ind.Species, ind.Name));
         expressionProfile.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_created_an_expression_profile_for_the_population()
      {
         var pop = _allPopulations.FindByName("POP");
         var cyp3A4 = pop.MoleculeByName<IndividualEnzyme>("CYP3A4");
         var expressionProfile = FindByName<ExpressionProfile>(CoreConstants.ContainerName.ExpressionProfileName(cyp3A4.Name, pop.Species, pop.Name));
         expressionProfile.ShouldNotBeNull();
      }
   }
}