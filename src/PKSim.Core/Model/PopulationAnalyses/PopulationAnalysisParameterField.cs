using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public class PopulationAnalysisParameterField : PopulationAnalysisNumericField
   {
      public string ParameterPath { get; set; }

      public override IReadOnlyList<double> GetValues(IPopulationDataCollector populationDataCollector)
      {
         return populationDataCollector.AllValuesFor(ParameterPath);
      }

      public override string Id => ParameterPath;

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var parameterField = source as PopulationAnalysisParameterField;
         if (parameterField == null) return;
         ParameterPath = parameterField.ParameterPath;
      }
   }
}