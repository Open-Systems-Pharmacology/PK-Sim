using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Repositories
{
   public interface ICompoundParameterRateRepository : IStartableRepository<ParameterRateMetaData>
   {
      /// <summary>
      ///    Compound parameter groups with at least one editable parameter
      ///    Editable = Visible and Not Locked
      /// </summary>
      IEnumerable<IGroup> EditableGroups { get; }

      IEnumerable<ParameterRateMetaData> ParameterRatesFor(IGroup group);
   }
}