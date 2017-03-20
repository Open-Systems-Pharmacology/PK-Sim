using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IFormulationRepository : IStartableRepository<Formulation>
   {
      IEnumerable<Formulation> AllFor(string applicationRoute);
      Formulation DefaultFormulationFor(string applicationRoute);
      Formulation FormulationBy(string formulationType);
   }
}