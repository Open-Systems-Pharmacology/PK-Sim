using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Model
{
   /// <summary>
   /// Represents a cache containing specific PK-Values calculated for a given compound
   /// </summary>
   public class CompoundPK
   {
      private readonly Cache<string, QuantityPKParameter> _quantityPKParameters = new Cache<string, QuantityPKParameter>(getKey:x => x.Name);

      public CompoundPK()
      {
         bioAvailabilities = new Cache<int, double?>();
         ddiAucInf = new Cache<int, double?>();
         ddiCMax = new Cache<int, double?>();
      }
      public string CompoundName { get; set; }

      /// <summary>
      /// AUC_inf of plasma curve following a IV infusion of 15 min
      /// </summary>
      private Cache<int, double?> bioAvailabilities { get; set; }

      /// <summary>
      /// AUC_inf of plasma curve following an application where only <see cref="CompoundName"/> is applied
      /// </summary>
      private Cache<int, double?> ddiAucInf { get; set; }
      private Cache<int, double?> ddiCMax { get; set; }

      private double? valueStoreFor(ICache<int, double?> allValues, int individualId)
      {
         if (!allValues.Contains(individualId))
            allValues[individualId] = null;

         return allValues[individualId];
      }

      public ICache<int, double?> AllBioAvailabilities => bioAvailabilities;
      public ICache<int, double?> AllDDIAucInf => ddiAucInf;
      public ICache<int, double?> AllDDICMax => ddiCMax;

      public double? BioAvailabilityAucInfFor(int individualId)
      {
         return valueStoreFor(bioAvailabilities, individualId);
      }

      public double? DDIAucInfFor(int individualId)
      {
         return valueStoreFor(ddiAucInf, individualId);
      }

      public void AddDDICMax(int individualId, double? cMax)
      {
         ddiCMax[individualId] = cMax;
      }

      public void AddDDIAucInf(int individualId, double? aucInf)
      {
         ddiAucInf[individualId] = aucInf;
      }
      
      public double? CMaxDDIFor(int individualId)
      {
         return valueStoreFor(ddiCMax, individualId);
      }

      public void AddBioavailability(int individualId, double? aucInf)
      {
         bioAvailabilities[individualId] = aucInf;
      }

      public void AddQuantityPKParameter(QuantityPKParameter quantityPKParameter)
      {
         _quantityPKParameters.Add(quantityPKParameter);
      }

      public ICache<string, QuantityPKParameter> QuantityPKParameters() => _quantityPKParameters;

      public double? QuantityFor(int individualId, string parameterName)
      {
         var quantityParameter = _quantityPKParameters.FirstOrDefault(x => string.Equals(x.Name, parameterName));

         return quantityParameter?.ValueFor(individualId);
      }
   }
}