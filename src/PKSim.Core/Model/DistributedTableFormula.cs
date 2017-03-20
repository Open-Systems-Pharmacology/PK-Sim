using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class DistributedTableFormula : TableFormula
   {
      private readonly IList<DistributionMetaData> _allDistributionMetaData = new List<DistributionMetaData>();

      public DistributedTableFormula()
      {
         //Distributed table formula are never used as rate hence false
         UseDerivedValues = false;
      }

      /// <summary>
      ///    Percentile with which the value in y were created
      /// </summary>
      public double Percentile { get; set; }

      /// <summary>
      ///    Add one distribtuion meta data. (should be one per point)
      /// </summary>
      /// <param name="distributionMeta"> </param>
      public void AddDistributionMetaData(DistributionMetaData distributionMeta)
      {
         _allDistributionMetaData.Add(distributionMeta);
      }

      public IEnumerable<DistributionMetaData> AllDistributionMetaData()
      {
         return _allDistributionMetaData;
      }

      public void RemoveDistributionMetaData(DistributionMetaData distributionMetaData)
      {
         _allDistributionMetaData.Remove(distributionMetaData);
      }

      public override int RemovePoint(double x, double y)
      {
         var index = base.RemovePoint(x, y);
         if (index >= 0)
            _allDistributionMetaData.RemoveAt(index);
         return index;
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var sourceTableFormula = source as DistributedTableFormula;
         if (sourceTableFormula == null) return;
         Percentile = sourceTableFormula.Percentile;
         _allDistributionMetaData.Clear();
         sourceTableFormula.AllDistributionMetaData().Each(AddDistributionMetaData);
      }

      public void AddPoint(double x, double y, DistributionMetaData distributionMetaData)
      {
         int index = AddPoint(x, y);
         if (index >= 0)
            _allDistributionMetaData[index] = distributionMetaData;
         else
            _allDistributionMetaData.Insert(~index, distributionMetaData);
      }
   }
}