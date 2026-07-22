using System.Collections.Generic;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using CoreAgingData = PKSim.Core.Model.AgingData;
using RAgingData = OSPSuite.R.Domain.AgingData;

namespace PKSim.R.Mappers
{
   public interface IAgingDataMapper : IMapper<CoreAgingData, RAgingData>
   {
   }

   public class AgingDataMapper : IAgingDataMapper
   {
      /// <summary>
      ///    Flattens PK-Sim's per-parameter aging data into the flat arrays of the R <see cref="RAgingData" />.
      ///    Reads <see cref="CoreAgingData.AllParameterData" /> directly rather than <c>ToDataTable()</c>, which quotes the
      ///    parameter path. Callers must guard against null or empty aging data - this mapper always returns a populated
      ///    (never null) object.
      /// </summary>
      public RAgingData MapFrom(CoreAgingData agingData)
      {
         var individualIds = new List<int>();
         var parameterPaths = new List<string>();
         var times = new List<double>();
         var values = new List<double>();

         agingData.AllParameterData.Each(parameterAgingData =>
         {
            for (var i = 0; i < parameterAgingData.IndividualIndexes.Count; i++)
            {
               individualIds.Add(parameterAgingData.IndividualIndexes[i]);
               parameterPaths.Add(parameterAgingData.ParameterPath);
               times.Add(parameterAgingData.Times[i]);
               values.Add(parameterAgingData.Values[i]);
            }
         });

         return new RAgingData
         {
            IndividualIds = individualIds.ToArray(),
            ParameterPaths = parameterPaths.ToArray(),
            Times = times.ToArray(),
            Values = values.ToArray(),
         };
      }
   }
}
