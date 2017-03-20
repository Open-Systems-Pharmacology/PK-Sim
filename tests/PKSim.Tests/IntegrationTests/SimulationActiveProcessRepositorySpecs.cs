using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using NUnit.Framework;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SimulationActiveProcessRepository : ContextForIntegration<ISimulationActiveProcessRepository>
   {
   }

   public class when_retrieving_all_simulation_active_processes_from_the_repository : concern_for_SimulationActiveProcessRepository
   {
      private IEnumerable<IPKSimProcess> _result;

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_element()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }
   }

   public class When_resolving_the_transport_process_for_a_given_individual_process_and_compound_process : concern_for_SimulationActiveProcessRepository
   {
      [Observation]
      public void should_return_the_expected_simulation_process()
      {
         sut.TransportFor("ActiveEffluxSpecific", "ActiveTransportSpecific_MM").Name.ShouldBeEqualTo("ActiveEffluxSpecific_MM");
      }
   }

   public class When_retrieving_all_process_defined_in_the_simulation_active_process_repository : concern_for_SimulationActiveProcessRepository
   {
      [Observation]
      public void the_kinetic_should_not_be_a_constant_rate()
      {
         sut.ProcessFor(CoreConstantsForSpecs.Process.LIVER_CLEARANCE).Formula.IsConstant().ShouldBeFalse();
      }

      [Observation]
      public void no_kinetic_should_be_constant_except_for_biliary_clearance()
      {
         var errorList = new List<string>();
         foreach (var process in sut.All())
         {
            //This processes are compound processes only and will be overriden with simulation processes
            if (process.NameIsOneOf(CoreConstantsForSpecs.Process.BILIARY_CLEARANCE, CoreConstantsForSpecs.Process.INDUCTION, CoreConstantsForSpecs.Process.IRREVERSIBLE_INHIBITION))
               continue;

            if (process.Formula.IsConstant())
               errorList.Add("Constant kinetic found for '{0}'".FormatWith(process.Name));
         }
         Assert.IsTrue(errorList.Count == 0, errorList.ToString("\n"));
      }
   }
}