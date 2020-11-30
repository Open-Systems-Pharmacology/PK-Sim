using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   /// <summary>
   ///    Repository for active processes in the simulation
   /// </summary>
   public interface ISimulationActiveProcessRepository : IStartableRepository<IPKSimProcess>
   {
      /// <summary>
      ///    Returns the simulation compoundProcess template which corresponds to the input compound compoundProcess
      /// </summary>
      IPKSimProcess ProcessFor(string processName);

      /// <summary>
      ///    Returns the simulation compoundProcess template of type
      ///    <typeparam name="TActiveProcess">TActiveProcess</typeparam>
      ///    which corresponds to the input compound compoundProcess
      /// </summary>
      TActiveProcess ProcessFor<TActiveProcess>(string processName) where TActiveProcess : IPKSimProcess;

      /// <summary>
      ///    Returns the transport compoundProcess for the given compound compoundProcess
      /// </summary>
      PKSimTransport TransportFor(string individualProcessName, string compoundProcessName);
   }
}