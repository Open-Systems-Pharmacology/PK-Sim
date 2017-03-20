using System.Collections.Generic;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Events
{
   public abstract class PopulationAnalysisEvent
   {
      public PopulationAnalysis PopulationAnalysis { get; private set; }

      protected PopulationAnalysisEvent(PopulationAnalysis populationAnalysis)
      {
         PopulationAnalysis = populationAnalysis;
      }
   }

   public abstract class PopulationAnalysisFieldEvent : PopulationAnalysisEvent
   {
      public IPopulationAnalysisField PopulationAnalysisField { get; private set; }

      protected PopulationAnalysisFieldEvent(PopulationAnalysis populationAnalysis, IPopulationAnalysisField populationAnalysisField)
         : base(populationAnalysis)
      {
         PopulationAnalysisField = populationAnalysisField;
      }
   }

   public class FieldAddedToPopulationAnalysisEvent : PopulationAnalysisFieldEvent
   {
      public FieldAddedToPopulationAnalysisEvent(PopulationAnalysis populationAnalysis, IPopulationAnalysisField populationAnalysisField) :
         base(populationAnalysis, populationAnalysisField)
      {
      }
   }

   public class FieldUnitChangedInPopulationAnalysisEvent : PopulationAnalysisFieldEvent
   {
      public FieldUnitChangedInPopulationAnalysisEvent(PopulationAnalysis populationAnalysis, IPopulationAnalysisField populationAnalysisField)
         : base(populationAnalysis, populationAnalysisField)
      {
      }
   }

   public class FieldRemovedFromPopulationAnalysisEvent : PopulationAnalysisFieldEvent
   {
      public FieldRemovedFromPopulationAnalysisEvent(PopulationAnalysis populationAnalysis, IPopulationAnalysisField populationAnalysisField)
         : base(populationAnalysis, populationAnalysisField)
      {
      }
   }

   public class FieldRenamedInPopulationAnalysisEvent : PopulationAnalysisFieldEvent
   {
      public FieldRenamedInPopulationAnalysisEvent(PopulationAnalysis populationAnalysis, IPopulationAnalysisField populationAnalysisField)
         : base(populationAnalysis, populationAnalysisField)
      {
      }
   }

   public class FieldsMovedInPopulationAnalysisEvent : PopulationAnalysisEvent
   {
      public IReadOnlyList<IPopulationAnalysisField> PopulationAnalysisFields { get; private set; }

      public FieldsMovedInPopulationAnalysisEvent(PopulationAnalysis populationAnalysis, IReadOnlyList<IPopulationAnalysisField> populationAnalysisFields)
         : base(populationAnalysis)
      {
         PopulationAnalysisFields = populationAnalysisFields;
      }
   }

   public class PopulationAnalysisChartSettingsChangedEvent : PopulationAnalysisEvent
   {
      public PopulationAnalysisChartSettingsChangedEvent(PopulationAnalysis populationAnalysis):base(populationAnalysis)
      {
      }
   }

   public class PopulationAnalysisDataSelectionChangedEvent : PopulationAnalysisEvent
   {
      public PopulationAnalysisDataSelectionChangedEvent(PopulationAnalysis populationAnalysis)
         : base(populationAnalysis)
      {
      }
   }
}