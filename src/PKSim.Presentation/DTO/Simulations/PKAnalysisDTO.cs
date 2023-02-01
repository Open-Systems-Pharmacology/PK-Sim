using System.Data;
using OSPSuite.Utility.Reflection;

namespace PKSim.Presentation.DTO.Simulations
{
   public class PKAnalysisDTO : Notifier
   {
      private DataTable _dataTable;

      public PKAnalysisDTO(DataTable dataTable)
      {
         DataTable = dataTable;
      }

      public DataTable DataTable
      {
         get => _dataTable;
         private set
         {
            _dataTable = value;
            OnPropertyChanged(() => HasRows);
         }
      }

      public bool HasRows => DataTable.Rows.Count > 0;
   }

   public class IntegratedPKAnalysisDTO
   {
      public PKAnalysisDTO IndividualPKValues { get; set; }
      public PKAnalysisDTO AggregatedPKValues { get; set; }
   }
}
