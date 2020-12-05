using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class SimulationActiveProcessRepository : StartableRepository<IPKSimProcess>, ISimulationActiveProcessRepository
   {
      private readonly IFlatProcessRepository _flatProcessesRepository;
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IFlatProcessToActiveProcessMapper _activeProcessMapper;
      private readonly List<IPKSimProcess> _allSimulationActiveProcesses;

      public SimulationActiveProcessRepository(IFlatProcessRepository flatProcessesRepository,
         IParameterContainerTask parameterContainerTask,
         IFlatProcessToActiveProcessMapper activeProcessMapper)
      {
         _flatProcessesRepository = flatProcessesRepository;
         _parameterContainerTask = parameterContainerTask;
         _activeProcessMapper = activeProcessMapper;
         _allSimulationActiveProcesses = new List<IPKSimProcess>();
      }

      public override IEnumerable<IPKSimProcess> All()
      {
         Start();
         return _allSimulationActiveProcesses;
      }

      protected override void DoStart()
      {
         var query = _flatProcessesRepository.All().Where(processIsSimulationActiveProcess)
            .MapAllUsing(_activeProcessMapper)
            .Where(activeProcess => activeProcess != null);

         _allSimulationActiveProcesses.AddRange(query);

         _allSimulationActiveProcesses.Each(process => _parameterContainerTask.AddProcessBuilderParametersTo(process));
      }

      private bool processIsSimulationActiveProcess(FlatProcess process)
      {
         return process.IsTemplate && CoreConstants.Groups.AllSimulationActiveProcesses.Contains(process.GroupName);
      }

      public IPKSimProcess ProcessFor(string processName)
      {
         Start();
         var simulationProcessName = simulationProcessNameFrom(processName);
         return _allSimulationActiveProcesses.FindByName(simulationProcessName) ?? _allSimulationActiveProcesses.FindByName(processName);
      }

      public TActiveProcess ProcessFor<TActiveProcess>(string processName) where TActiveProcess : IPKSimProcess
      {
         return ProcessFor(processName).DowncastTo<TActiveProcess>();
      }

      public PKSimTransport TransportFor(string individualProcessName, string compoundProcessName)
      {
         //do we have a process with the individual name? yes, then use it
         var transport = ProcessFor<PKSimTransport>(individualProcessName);
         if (transport != null)
            return transport;

         return ProcessFor<PKSimTransport>(simulationProcessNameFrom(individualProcessName, compoundProcessName));
      }

   
      private string simulationProcessNameFrom(string compoundProcessName) => simulationProcessNameFrom(compoundProcessName, compoundProcessName);

      private string simulationProcessNameFrom(string simulationPrefix, string compoundProcessName)
      {
         var compoundProcess = _flatProcessesRepository.FindByName(compoundProcessName);
         if (compoundProcess==null)
            throw new ArgumentException($"Cannot find simulation process named '{compoundProcessName}'");

         //already a process in simulation. Nothing to do
         if (compoundProcess.GroupName == CoreConstants.Groups.SIMULATION_ACTIVE_PROCESS)
            return compoundProcessName;

         //this is a composed name
         return $"{simulationPrefix}_{compoundProcess.KineticType}";
      }
   }
}