using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.ProjectConverter.v5_2
{
   public class When_converting_the_SimpleProject_521_project : ContextWithLoadedProject<Converter521To522>
   {
      private Simulation _simDog;
      private Simulation _simHuman;
      private Individual _individualDog;
      private Individual _individualHuman;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimpleProject_521");
         _simDog = FindByName<Simulation>("Dog");
         _simHuman = FindByName<Simulation>("Human");
         _individualDog = FindByName<Individual>("Dog");
         _individualHuman = FindByName<Individual>("Human");
      }

      [Observation]
      public void should_have_added_the_GET_variability_factors_in_stomach_in_the_human_individual_using_a_formula()
      {
         checkGETStomachHuman(_simHuman.BuildingBlock<Individual>());
         checkGETStomachHuman(_individualHuman);
      }

      [Observation]
      public void should_have_added_the_GET_variability_factors_in_stomach_in_the_dog_individual_using_a_constant()
      {
         checkGETStomachDog(_simDog.BuildingBlock<Individual>());
         checkGETStomachDog(_individualDog);
      }

      private void checkGETStomachHuman(Individual individual)
      {
         string[] parameters = {ConverterConstants.Parameter.GET_Alpha_variability_factor, ConverterConstants.Parameter.GET_Beta_variability_factor};
         foreach (var parameter in parameters)
         {
            var formula = stomachFormula<DistributionFormula>(individual,parameter);
            formula.ShouldNotBeNull();
         }
      }

      private void checkGETStomachDog(Individual individual)
      {
         string[] parameters = { ConverterConstants.Parameter.GET_Alpha_variability_factor, ConverterConstants.Parameter.GET_Beta_variability_factor };
         foreach (var parameter in parameters)
         {
            var formula = stomachFormula<ConstantFormula>(individual, parameter);
            formula.Value.ShouldBeEqualTo(1);
         }
      }
      
   
      private static TFormula stomachFormula<TFormula>(Individual individual, string parameterName)
      {
         return formulaIn<TFormula>(individual, parameterName, CoreConstants.Organ.Lumen,CoreConstants.Compartment.Stomach);
      }

      private static TFormula formulaIn<TFormula>(Individual individual, string parameterName, string organName,string compartmentName=null)
      {
         var container= individual.Organism.Container(organName);
         if (compartmentName != null)
            container = container.Container(compartmentName);

         var parameter = container.Parameter(parameterName);
         parameter.ShouldNotBeNull();
         return parameter.Formula.DowncastTo<TFormula>();
      }
   }
}