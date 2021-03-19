using System.Collections.Generic;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IPopulationTask : IBuildingBlockTask<Population>
   {
      void AddToProjectBasedOn(Individual individual);

      /// <summary>
      /// Starts the extraction of individuals workflow from the given <paramref name="population"/>. The user will be prompted to select the individuals to extract
      /// </summary>
      /// <param name="population">Population used for the extraction</param>
      /// <param name="individualIds">Optional individualIds to exctrat</param>
      void ExtractIndividuals(Population population, IEnumerable<int> individualIds=null);
   }
}