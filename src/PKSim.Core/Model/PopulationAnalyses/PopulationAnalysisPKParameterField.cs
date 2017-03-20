using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public class PopulationAnalysisPKParameterField : PopulationAnalysisNumericField, IQuantityField
   {
      public virtual string QuantityPath { get; set; }
      public virtual QuantityType QuantityType { get; set; }
      public virtual string PKParameter { get; set; }

      public override IReadOnlyList<double> GetValues(IPopulationDataCollector populationDataCollector)
      {
         return populationDataCollector.AllPKParameterValuesFor(QuantityPath, PKParameter);
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var pkParameterField = source as PopulationAnalysisPKParameterField;
         if (pkParameterField == null) return;
         QuantityPath = pkParameterField.QuantityPath;
         QuantityType = pkParameterField.QuantityType;
         PKParameter = pkParameterField.PKParameter;
      }

      public override string Id => new[] {QuantityPath, PKParameter}.ToPathString();
   }
}