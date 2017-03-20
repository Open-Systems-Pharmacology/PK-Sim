using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_1;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;


namespace PKSim.ProjectConverter.v5_1
{
   public abstract class concern_for_Test4Convert_504 : ContextWithLoadedProject<Converter50To513>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Test4Convert_504");
      }
   }

   public class When_converting_the_project_Test4Convert : concern_for_Test4Convert_504
   {
      private Compound _compound;
      private Simulation _simulation;

      protected override void Context()
      {
         _compound = First<Compound>();
         _simulation = First<Simulation>();
      }

      [Observation]
      public void should_have_set_the_name_of_the_data_source_in_all_systemic_processes()
      {
         var allSystemicProcesses = _compound.AllProcesses<SystemicProcess>().ToList();
         allSystemicProcesses.Count.ShouldBeGreaterThan(0);

         foreach (var process in allSystemicProcesses)
         {
            process.DataSource.ShouldNotBeNull();
         }
      }

      [Observation]
      public void should_have_set_the_name_of_the_data_source_in_all_systemic_processes_in_the_simulation()
      {
         var allSystemicProcesses = _simulation.BuildingBlock<Compound>().AllProcesses<SystemicProcess>().ToList();
         allSystemicProcesses.Count.ShouldBeGreaterThan(0);

         foreach (var process in allSystemicProcesses)
         {
            process.DataSource.ShouldNotBeNull();
         }
      }

      [Observation]
      public void should_have_added_the_dynamic_formula_calculation_method_in_the_simulation()
      {
         _simulation.ModelProperties.CalculationMethodFor(CoreConstants.Category.DynamicFormulas).ShouldNotBeNull();
      }

      [Observation]
      public void should_have_the_value_of_sink_condition_to_the_expected_values()
      {
         var lumen = _simulation.Model.Root.Container(Constants.ORGANISM)
            .Container(CoreConstants.Organ.Lumen);

         lumen.Parameter(CoreConstants.Parameter.PARA_ABSORBTION_SINK).Value.ShouldBeEqualTo(CoreConstants.Parameter.SINK_CONDITION);
         lumen.Parameter(CoreConstants.Parameter.TRANS_ABSORBTION_SINK).Value.ShouldBeEqualTo(CoreConstants.Parameter.NO_SINK_CONDITION);
      }

      [Observation]
      public void should_have_converted_the_transporter_type_for_the_transporter_and_set_it_to_the_type_the_most_often_used()
      {
         var individual = First<Individual>();
         var transporter = individual.MoleculeByName<IndividualTransporter>("T1");
         transporter.TransportType.ShouldBeEqualTo(TransportType.Efflux);
      }

      [Observation]
      public void should_have_converted_the_transporter_type_for_the_transporter_in_the_simulation_as_well()
      {
         var individual = _simulation.BuildingBlock<Individual>();
         var transporter = individual.MoleculeByName<IndividualTransporter>("T1");
         transporter.TransportType.ShouldBeEqualTo(TransportType.Efflux);
      }

      [Observation]
      public void should_have_added_the_NOT_modifier_in_the_drug_absorption_rate_formulas()
      {
         var drugAbsorptionParams =
            _simulation.Model.Root.GetAllChildren<IParameter>(
               p => p.Name.Equals(ConverterConstants.Parameter.DrugAbsorptionLumenToMucosaRate)).ToList();

         drugAbsorptionParams.Count.ShouldBeGreaterThan(1);

         foreach (var drugAbsorptionParam in drugAbsorptionParams)
         {
            var paramFormula = drugAbsorptionParam.Formula.DowncastTo<FormulaWithFormulaString>();

            paramFormula.FormulaString.Contains("NOT k_Liquid").ShouldBeTrue();
         }
      }
   }
}