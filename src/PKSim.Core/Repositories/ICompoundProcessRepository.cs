using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface ICompoundProcessRepository : IStartableRepository<CompoundProcess>
   {
      IEnumerable<TProcessType> All<TProcessType>() where TProcessType : CompoundProcess;

      /// <summary>
      ///    Returns compound compoundProcess template by name
      /// </summary>
      CompoundProcess ProcessByName(string processTemplateName);

      /// <summary>
      ///    Returns compound compoundProcess template by name
      /// </summary>
      TProcessType ProcessByName<TProcessType>(string processTemplateName) where TProcessType : CompoundProcess;
   }
}