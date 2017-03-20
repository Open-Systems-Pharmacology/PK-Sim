using System;
using System.Collections.Generic;
using System.Linq;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public abstract class PopulationAnalysisDataField : PopulationAnalysisFieldBase
   {
      protected PopulationAnalysisDataField(Type dataType) : base(dataType)
      {
      }

      public abstract IReadOnlyList<object> GetValuesAsObjects(IPopulationDataCollector populationDataCollector);

      /// <summary>
      ///    Specifies wether derived field can be defined for the current <see cref="PopulationAnalysisDataField" />. Default is
      ///    <c>true</c>
      /// </summary>
      public virtual bool DerivedFieldAllowed => true;
   }

   public abstract class PopulationAnalysisDataField<T> : PopulationAnalysisDataField
   {
      protected PopulationAnalysisDataField() : base(typeof (T))
      {
      }

      public abstract IReadOnlyList<T> GetValues(IPopulationDataCollector populationDataCollector);

      public override IReadOnlyList<object> GetValuesAsObjects(IPopulationDataCollector populationDataCollector)
      {
         return GetValues(populationDataCollector).Cast<object>().ToList();
      }
   }

   public abstract class PopulationAnalysisDataRefField<T> : PopulationAnalysisDataField<T> where T : class
   {
      public override IReadOnlyList<object> GetValuesAsObjects(IPopulationDataCollector populationDataCollector)
      {
         return GetValues(populationDataCollector);
      }
   }
}