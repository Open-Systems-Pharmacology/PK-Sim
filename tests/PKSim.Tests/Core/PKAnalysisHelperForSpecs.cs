using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public static class PKAnalysisHelperForSpecs
   {
      public static PKAnalysis GeneratePopulationAnalysis()
      {
         var analysis = new PKAnalysis();
         analysis.AddChildren(
            new PKSimParameter {Name = Constants.PKParameters.AUC, Value = 0.0},
            new PKSimParameter {Name = Constants.PKParameters.AUC_inf, Value = 1.0},
            new PKSimParameter {Name = Constants.PKParameters.C_max, Value = 2.0},
            new PKSimParameter {Name = Constants.PKParameters.Tmax, Value = 3.0},
            new PKSimParameter {Name = Constants.PKParameters.Thalf, Value = 4.0},
            new PKSimParameter {Name = Constants.PKParameters.MRT, Value = 5.0},
            new PKSimParameter {Name = Constants.PKParameters.FractionAucEndToInf, Value = 6.0}
         );
         return analysis;
      }

      public static PKAnalysis GenerateIndividualPKAnalysis()
      {
         var newPKAnalysis = new PKAnalysis();
         newPKAnalysis.AddChildren(
            new PKSimParameter {Name = Constants.PKParameters.AUC, Value = 0.0},
            new PKSimParameter {Name = Constants.PKParameters.AUC_norm, Value = 1.0},
            new PKSimParameter {Name = Constants.PKParameters.AUC_inf, Value = 2.0},
            new PKSimParameter {Name = Constants.PKParameters.AUC_inf_norm, Value = 3.0},
            new PKSimParameter {Name = Constants.PKParameters.C_max, Value = 4.0},
            new PKSimParameter {Name = Constants.PKParameters.C_max_norm, Value = 5.0},
            new PKSimParameter {Name = Constants.PKParameters.FractionAucEndToInf, Value = 6.0},
            new PKSimParameter {Name = Constants.PKParameters.Tmax, Value = 7.0},
            new PKSimParameter {Name = Constants.PKParameters.Thalf, Value = 8.0},
            new PKSimParameter {Name = Constants.PKParameters.MRT, Value = 9.0}
         );
         return newPKAnalysis;
      }

      public static GlobalPKAnalysis GenerateGlobalPKAnalysis(string drugName)
      {
         var globalPKAnalysis = new GlobalPKAnalysis();
         var container = new Container().WithName(drugName);
         globalPKAnalysis.Add(container);
         container.AddChildren(
            new PKSimParameter {Name = Constants.PKParameters.Vd, Value = 0.0},
            new PKSimParameter {Name = Constants.PKParameters.Vss, Value = 0.0});

         return globalPKAnalysis;
      }
   }
}