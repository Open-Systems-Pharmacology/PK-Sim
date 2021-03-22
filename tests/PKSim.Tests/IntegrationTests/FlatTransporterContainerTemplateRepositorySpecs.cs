﻿using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_FlatTransporterContainerTemplateRepository : ContextForIntegration<ITransportTemplateRepository>
   {
   }

   public class When_retrieving_the_defined_active_transports_from_the_database : concern_for_FlatTransporterContainerTemplateRepository
   {
      private IFlatProcessRepository _flatProcessRepository;
      private ISimulationActiveProcessRepository _simulationProcessRepository;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _flatProcessRepository = IoC.Resolve<IFlatProcessRepository>();
         _simulationProcessRepository = IoC.Resolve<ISimulationActiveProcessRepository>();
      }

      [Observation]
      public void should_be_able_to_find_a_matching_kinetic_in_the_simulation_processes_with_the_accurate_kinetic()
      {
         var error = new List<string>();
         foreach (var transportProcess in sut.All().Select(x => x.ProcessName).Distinct())
         {
            foreach (var flatTransportProcess in _flatProcessRepository.All().Where(p => p.IsActiveTransport()))
            {
               var simProcess = _simulationProcessRepository.TransportFor(transportProcess, flatTransportProcess.Name);
             
               //This combination is not supported yet and is explicitly excluded
               if (transportProcess.Contains("BiDirectional") && flatTransportProcess.Name.Contains("_Hill"))
                  continue;

               if (simProcess == null)
                  error.Add($"Could not find sim process for individual process '{transportProcess}' and compound process '{flatTransportProcess.Name}'");
            }
         }

         error.Count().ShouldBeEqualTo(0, error.ToString("\n"));
      }
   }
}