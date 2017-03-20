using System;
using System.Collections.Generic;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public class FieldsMovedEventArgs : EventArgs
   {
      public IReadOnlyList<IPopulationAnalysisField> Fields { get; private set; }

      /// <summary>
      /// Felds on which the fields where moved (e.g dropped). The value is <c>null</c> if the fields where dropped on an empty space
      /// </summary>
      public IPopulationAnalysisField Target { get; private set; }

      /// <summary>
      /// Area on which the fields were moved
      /// </summary>
      public PivotArea Area { get; private set; }

      public FieldsMovedEventArgs(IReadOnlyList<IPopulationAnalysisField> fields, IPopulationAnalysisField target, PivotArea area)
      {
         Target = target;
         Fields = fields;
         Area = area;
      }
   }

   public interface IPopulationAnalysisFieldsDragDropBinder
   {
      /// <summary>
      /// Event is thrown whenevern fields are being dropped (reordered or moved)
      /// </summary>
      event EventHandler<FieldsMovedEventArgs> FieldsDropped;

      PivotArea Area { get; set; }
      int? MaximNumberOfAllowedFields { get; set; }
   }
}