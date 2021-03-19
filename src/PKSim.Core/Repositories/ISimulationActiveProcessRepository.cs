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
      ///    Returns the transport transport process for the given <paramref name="individualProcessName"/> and <paramref name="compoundProcessName"/>
      /// </summary>
      PKSimTransport TransportFor(string individualProcessName, string compoundProcessName);
   }
}