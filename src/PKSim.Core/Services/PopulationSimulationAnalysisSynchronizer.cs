using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Services
{
   public interface IPopulationSimulationAnalysisSynchronizer
   {
      /// <summary>
      ///    Ensures that fields used in all population analyses of <paramref name="populationSimulation" /> are referencing
      ///    existing outputs.
      /// </summary>
      void UpdateAnalysesDefinedIn(PopulationSimulation populationSimulation);
   }

   public class PopulationSimulationAnalysisSynchronizer : IPopulationSimulationAnalysisSynchronizer
   {
      public void UpdateAnalysesDefinedIn(PopulationSimulation populationSimulation)
      {
         foreach (var analysis in populationSimulation.Analyses.OfType<PopulationAnalysisChart>().Select(x => x.BasePopulationAnalysis))
         {
            updateUsedField(analysis, populationSimulation);
         }
      }

      private void updateUsedField(PopulationAnalysis populationAnalysis, PopulationSimulation populationSimulation)
      {
         if (populationAnalysis == null)
            return;

         foreach (var quantityField in populationAnalysis.All<IQuantityField>().Where(field => !populationHasOutputFor(populationSimulation, field.QuantityPath)).ToList())
         {
            removeFieldsFromAnalysis(quantityField, populationAnalysis);
         }
      }

      private static bool populationHasOutputFor(PopulationSimulation populationSimulation, string quantityPath)
      {
         return populationSimulation.OutputSelections.AllOutputs.Any(x => string.Equals(x.Path, quantityPath));
      }

      private void removeFieldsFromAnalysis(IPopulationAnalysisField field, PopulationAnalysis populationAnalysis)
      {
         var allDerivedFields = populationAnalysis.AllFieldsReferencing(field);
         allDerivedFields.Each(populationAnalysis.Remove);
         populationAnalysis.Remove(field);
      }
   }
}