using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_CompoundProcessRepository : ContextForIntegration<ICompoundProcessRepository>
   {
      protected ISimulationActiveProcessRepository _simulationActProcRepo;
      protected IFlatProcessRepository _flatProcessRepository;

      protected override void Context()
      {
         _simulationActProcRepo = IoC.Resolve<ISimulationActiveProcessRepository>();
         _flatProcessRepository = IoC.Resolve<IFlatProcessRepository>();
         sut = IoC.Resolve<ICompoundProcessRepository>();
      }
   }

   public class When_retrieving_all_compounds_active_processes_templates : concern_for_CompoundProcessRepository
   {
      private IEnumerable<CompoundProcess> _compoundActProcTemplates;

      protected override void Because()
      {
         _compoundActProcTemplates = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_process()
      {
         _compoundActProcTemplates.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void every_compound_process_template_that_is_not_a_transport_process_or_an_interaction_process_should_have_corresponding_simulation_process_template()
      {
         IList<string> error = new List<string>();
         foreach (var compoundActProcTemplate in _compoundActProcTemplates)
         {
            var flatProcess = _flatProcessRepository.FindByName(compoundActProcTemplate.InternalName);
            if (flatProcess.IsActiveTransport() || flatProcess.IsInteraction())
               continue;

            var simProcess = _simulationActProcRepo.ProcessFor(compoundActProcTemplate.Name);
            if (simProcess == null)
               error.Add(string.Format("Could not find sim process for '{0}'", compoundActProcTemplate.Name));
         }

         error.Count().ShouldBeEqualTo(0, error.ToString("\n"));
      }
   }

   public class When_retrieving_the_irreversible_inhibition_process : concern_for_CompoundProcessRepository
   {
      [Observation]
      public void should_have_created_a_process_with_5_parameters_and_the_default_value_for_Ki_should_be_K_kinact_half()
      {
         var irrerverisbleInhibition = sut.ProcessByName<InhibitionProcess>(CoreConstantsForSpecs.Process.IRREVERSIBLE_INHIBITION);
         irrerverisbleInhibition.AllParameters().Count().ShouldBeEqualTo(3);
      
         var K_kinact_half = irrerverisbleInhibition.Parameter(CoreConstants.Parameters.K_KINACT_HALF);
         K_kinact_half.Value = 5;

         var Ki = irrerverisbleInhibition.Parameter(CoreConstants.Parameters.KI);
         Ki.Value.ShouldBeEqualTo(5);
      }
   }
}