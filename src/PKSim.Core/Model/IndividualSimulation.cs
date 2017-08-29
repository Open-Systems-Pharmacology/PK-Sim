using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Chart;

namespace PKSim.Core.Model
{
   public class IndividualSimulation : Simulation
   {
      private DataRepository _dataRepository;
      private readonly ICache<string, CompoundPK> _compoundPKCache;

      public IndividualSimulation()
      {
         _compoundPKCache = new Cache<string, CompoundPK>(x => x.CompoundName, x => new CompoundPK());
      }

      public virtual IEnumerable<SimulationTimeProfileChart> TimeProfileAnalyses => Analyses.OfType<SimulationTimeProfileChart>();

      public void ClearPKCache()
      {
         _compoundPKCache.Clear();
      }


      public CompoundPK CompoundPKFor(string compoundName)
      {
         if(!_compoundPKCache.Contains(compoundName))
            _compoundPKCache.Add(new CompoundPK{CompoundName = compoundName});

         return _compoundPKCache[compoundName];
      }

      public void AddCompoundPK(CompoundPK compoundPK)
      {
         _compoundPKCache.Add(compoundPK);
      }

      public double? AucIVFor(string compoundName)
      {
         return compoundPKValue(compoundName, x => x.AucIV);
      }

      public double? AucDDIFor(string compoundName)
      {
         return compoundPKValue(compoundName, x => x.AucDDI);
      }

      public double? CmaxDDIFor(string compoundName)
      {
         return compoundPKValue(compoundName, x => x.CmaxDDI);
      }

      private double? compoundPKValue(string compoundName, Func<CompoundPK, double?> pkValueFunc)
      {
         return pkValueFunc(_compoundPKCache[compoundName]);
      }

      public IEnumerable<CompoundPK> AllCompoundPK => _compoundPKCache;

      /// <summary>
      ///    Representation in memory of the actual simulation results
      /// </summary>
      public virtual DataRepository DataRepository
      {
         get => _dataRepository;
         set
         {
            _dataRepository = value;
            HasChanged = true;
            ResultsVersion = Version;
         }
      }

      public override bool HasResults => !DataRepository.IsNull() && DataRepository.Any();

      public override TBuildingBlock BuildingBlock<TBuildingBlock>()
      {
         return AllBuildingBlocks<TBuildingBlock>().SingleOrDefault();
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceIndividualSimulation = sourceObject as IndividualSimulation;
         if (sourceIndividualSimulation == null) return;
         updateAucIVCacheFrom(sourceIndividualSimulation);
      }

      public override void UpdateFromOriginalSimulation(Simulation originalSimulation)
      {
         base.UpdateFromOriginalSimulation(originalSimulation);
         var sourceIndividualSimulation = originalSimulation as IndividualSimulation;
         if (sourceIndividualSimulation == null) return;
         updateAucIVCacheFrom(sourceIndividualSimulation);
      }

      private void updateAucIVCacheFrom(IndividualSimulation sourceSimulation)
      {
         ClearPKCache();
         sourceSimulation.AllCompoundPK.Each(AddCompoundPK);
      }

      public override void AcceptVisitor(IVisitor visitor)
      {
         base.AcceptVisitor(visitor);
         if(HasResults)
            DataRepository.AcceptVisitor(visitor);
      }
   }
}