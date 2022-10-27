using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class CompoundPKContext
   {
      private readonly ICache<string, CompoundPK> _compoundPKCache;

      public CompoundPKContext()
      {
         _compoundPKCache = new Cache<string, CompoundPK>(x => x.CompoundName, x => new CompoundPK());
      }

      public CompoundPK CompoundPKFor(string compoundName)
      {
         if (!_compoundPKCache.Contains(compoundName))
            _compoundPKCache.Add(new CompoundPK { CompoundName = compoundName });

         return _compoundPKCache[compoundName];
      }

      public void AddCompoundPK(CompoundPK compoundPK)
      {
         _compoundPKCache[compoundPK.CompoundName] = compoundPK;
      }

      /// <summary>
      /// Initializes the context compound PK ratio parameters from the calculated simulation PK Parameters
      /// Adds BioAvailability, AUCRatio, and CMaxRatio to the context
      /// </summary>
      /// <param name="populationSimulation">The simulation that contains the PK Parameters</param>
      public void InitializeQuantityPKParametersFrom(PopulationSimulation populationSimulation)
      {
         populationSimulation.CompoundNames.Each(compoundName =>
         {
            var bioAvailability = populationSimulation.PKAnalyses.PKParameterFor(compoundName, CoreConstants.PKAnalysis.Bioavailability);
            var aucRatio = populationSimulation.PKAnalyses.PKParameterFor(compoundName, CoreConstants.PKAnalysis.AUCRatio);
            var cMaxRatio = populationSimulation.PKAnalyses.PKParameterFor(compoundName, CoreConstants.PKAnalysis.C_maxRatio);
            var compoundPK = CompoundPKFor(compoundName);

            if (bioAvailability != null)
               compoundPK.AddQuantityPKParameter(bioAvailability);

            if (aucRatio != null)
               compoundPK.AddQuantityPKParameter(aucRatio);

            if (cMaxRatio != null)
               compoundPK.AddQuantityPKParameter(cMaxRatio);
         });

      }
   }
}