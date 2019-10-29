using System;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public class When_serializing_some_pkanalyses_ : ContextForSerialization<PopulationSimulationPKAnalyses>
   {
      private PopulationSimulationPKAnalyses _pkAnalyses;
      private PopulationSimulationPKAnalyses _deserializedPKAnalyses;
      private Random _random;
      private int _numberOfIndividuals;

      protected override void Context()
      {
         base.Context();
         _random = new Random();
         _pkAnalyses = new PopulationSimulationPKAnalyses();
         _pkAnalyses.AddPKAnalysis(createPKAnalyses("Path1"));
         _pkAnalyses.AddPKAnalysis(createPKAnalyses("Path2"));
         _pkAnalyses.AddPKAnalysis(createPKAnalyses("Path3"));
         _pkAnalyses.AddPKAnalysis(createPKAnalyses("Path4"));
         _pkAnalyses.AddPKAnalysis(createPKAnalyses("Path5"));
      }

      private QuantityPKParameter createPKAnalyses(string path)
      {
         var pk = new QuantityPKParameter {QuantityPath = path};
         pk.Name = "AUC";
         _numberOfIndividuals = 100000;
         pk.SetNumberOfIndividuals(_numberOfIndividuals);
         for (int i = 0; i < _numberOfIndividuals; i++)
         {
            pk.SetValue(i, (float) _random.NextDouble() * 100);
         }
         return pk;
      }

      protected override void Because()
      {
         _deserializedPKAnalyses = SerializeAndDeserialize(_pkAnalyses);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_pkanalyses()
      {
         _deserializedPKAnalyses.ShouldNotBeNull();
         _deserializedPKAnalyses.All().Count().ShouldBeEqualTo(5);
         _deserializedPKAnalyses.PKParameterFor("Path1", "AUC").Values.Length.ShouldBeEqualTo(_numberOfIndividuals);
      }
   }
}