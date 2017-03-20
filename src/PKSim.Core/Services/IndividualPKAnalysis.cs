using PKSim.Core.Model;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core.Services
{
   public class IndividualPKAnalysis
   {
      public virtual DataColumn DataColumn { get; set; }
      public virtual PKAnalysis PKAnalysis { get; set; }

      public IndividualPKAnalysis(DataColumn dataColumn, PKAnalysis pkAnalysis)
      {
         DataColumn = dataColumn;
         PKAnalysis = pkAnalysis;
      }
   }

   public class NullIndividualPKAnalysis : IndividualPKAnalysis
   {
      public NullIndividualPKAnalysis() : base(null, new PKAnalysis())
      {
      }
   }
}