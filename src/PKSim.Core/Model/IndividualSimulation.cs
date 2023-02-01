using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Model.Extensions;

namespace PKSim.Core.Model
{
   public class IndividualSimulation : Simulation
   {
      private DataRepository _dataRepository;
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

      public virtual IEnumerable<AnalysisChart> ChartAnalyses => Analyses.OfType<AnalysisChart>();

      public int IndividualId => Results.AllIndividualIds().FirstOrDefault();

      public override TBuildingBlock BuildingBlock<TBuildingBlock>() => AllBuildingBlocks<TBuildingBlock>().SingleOrDefault();

      public override void AcceptVisitor(IVisitor visitor)
      {
         base.AcceptVisitor(visitor);
         if (HasResults)
            DataRepository.AcceptVisitor(visitor);
      }

      public DataColumn PeripheralVenousBloodColumn(string compoundName) => DataRepository.PeripheralVenousBloodColumn(compoundName);

      /// <summary>
      ///    tries to find venous blood plasma if defined in the repository. returns null otherwise
      /// </summary>
      public DataColumn VenousBloodColumn(string compoundName) => DataRepository.VenousBloodColumn(compoundName);

      public DataColumn FabsOral(string compoundName) => DataRepository.FabsOral(compoundName);

      public void ClearPKCache()
      {
         AucDDI.Clear();
         CMaxDDI.Clear();
         AucIV.Clear();
      }
   }
}