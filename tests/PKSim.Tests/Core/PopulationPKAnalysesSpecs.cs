using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationPKAnalyses : ContextSpecification<PopulationSimulationPKAnalyses>
   {
      protected override void Context()
      {
         sut = new PopulationSimulationPKAnalyses();
      }
   }

   public class When_retrieving_the_pk_parameters_calculated_for_a_given_quantity_path : concern_for_PopulationPKAnalyses
   {
      private QuantityPKParameter _pkParameter1;
      private QuantityPKParameter _pkParameter2;
      private QuantityPKParameter _pkParameter3;
      private QuantityPKParameter _pkParameter4;

      protected override void Context()
      {
         base.Context();
         _pkParameter1 = new QuantityPKParameter {QuantityPath = "Path1", Name = "AUC"};
         _pkParameter2 = new QuantityPKParameter {QuantityPath = "Path2", Name = "AUC"};
         _pkParameter3 = new QuantityPKParameter {QuantityPath = "Path1", Name = "CMax"};
         _pkParameter4 = new QuantityPKParameter {QuantityPath = "Path1", Name = "TMax"};
         sut.AddPKAnalysis(_pkParameter1);
         sut.AddPKAnalysis(_pkParameter2);
         sut.AddPKAnalysis(_pkParameter3);
         sut.AddPKAnalysis(_pkParameter4);
      }

      [Observation]
      public void should_return_only_the_pk_parameters_for_this_specific_quantity()
      {
         sut.AllPKParametersFor("Path1").ShouldOnlyContain(_pkParameter1,_pkParameter3,_pkParameter4);
      }
   }

   public class When_retrieving_the_list_of_all_pkparameters_defined_for_a_given_quantity_path : concern_for_PopulationPKAnalyses
   {
      protected override void Context()
      {
         base.Context();
         sut.AddPKAnalysis(new QuantityPKParameter { QuantityPath = "Path1", Name = "AUC" });
         sut.AddPKAnalysis(new QuantityPKParameter { QuantityPath = "Path2", Name = "AUC" });
         sut.AddPKAnalysis(new QuantityPKParameter { QuantityPath = "Path1", Name = "Cmax" });
         sut.AddPKAnalysis(new QuantityPKParameter { QuantityPath = "Path2", Name = "Tmax" });
      }

      [Observation]
      public void should_return_the_list_of_expected_pk_parameters()
      {
         sut.AllPKParameterNamesFor("Path1").ShouldOnlyContain("AUC","Cmax");
      }
   }
}	