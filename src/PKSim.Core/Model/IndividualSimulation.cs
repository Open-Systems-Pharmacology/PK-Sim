using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Chart;
using PKSim.Core.Model.Extensions;

namespace PKSim.Core.Model
{
   public class IndividualSimulation : Simulation
   {
      private DataRepository _dataRepository;
      public virtual IEnumerable<SimulationTimeProfileChart> TimeProfileAnalyses => Analyses.OfType<SimulationTimeProfileChart>();
      public Cache<string, double?> AucDDI { get; } = new Cache<string, double?>(onMissingKey: x => null);
      public Cache<string, double?> AucIV { get; } = new Cache<string, double?>(onMissingKey: x => null);
      public Cache<string, double?> CMaxDDI { get; } = new Cache<string, double?>(onMissingKey: x => null);

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

      public override DataRepository ResultsDataRepository => _dataRepository; 
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
      }

      public override void UpdateFromOriginalSimulation(Simulation originalSimulation)
      {
         base.UpdateFromOriginalSimulation(originalSimulation);
         var sourceIndividualSimulation = originalSimulation as IndividualSimulation;
         if (sourceIndividualSimulation == null) return;
      }


      public override void AcceptVisitor(IVisitor visitor)
      {
         base.AcceptVisitor(visitor);
         if (HasResults)
            DataRepository.AcceptVisitor(visitor);
      }

      public override DataColumn PeripheralVenousBloodColumn(string compoundName)
      {
         return DataRepository.PeripheralVenousBloodColumn(compoundName);
      }

      /// <summary>
      ///    tries to find venous blood plasma if defined in the repository. returns null otherwise
      /// </summary>
      public override DataColumn VenousBloodColumn(string compoundName) => DataRepository.VenousBloodColumn(compoundName);

      public override DataColumn FabsOral(string compoundName) => DataRepository.FabsOral(compoundName);

      public void ClearRatioPKCache()
      {
         AucDDI.Clear();
         CMaxDDI.Clear();
         AucIV.Clear();
      }
   }
}