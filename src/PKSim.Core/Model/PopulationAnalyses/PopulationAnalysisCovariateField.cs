using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public class PopulationAnalysisCovariateField : PopulationAnalysisDataRefField<string>, IStringValueField
   {
      private readonly List<GroupingItem> _groupingItems = new List<GroupingItem>();
      public virtual GroupingItem ReferenceGroupingItem { get; set; }

      public string Covariate { get; set; }

      public override string Id => Covariate;

      public override IReadOnlyList<string> GetValues(IPopulationDataCollector populationDataCollector)
      {
         return populationDataCollector.AllCovariateValuesFor(Covariate);
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var covariateField = source as PopulationAnalysisCovariateField;
         if (covariateField == null) return;
         Covariate = covariateField.Covariate;
         covariateField.GroupingItems.Each(x => AddGroupingItem(cloneManager.Clone(x)));
      }

      public virtual IReadOnlyList<GroupingItem> GroupingItems => this.GroupingItemsWithReference(_groupingItems);

      public virtual void AddGroupingItem(GroupingItem groupingItem)
      {
         _groupingItems.Add(groupingItem);
      }

      public override int Compare(object value1, object value2)
      {
         int compare;
         if(this.CouldCompareValuesToReference(value1, value2, out compare))
            return compare;

         return base.Compare(value1, value2);
      }
   }
}