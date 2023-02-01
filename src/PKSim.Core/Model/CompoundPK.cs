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
      
      public string CompoundName { get; set; }

      private double? valueStoreFor(ICache<int, double?> allValues, int individualId)
      {
         if (!allValues.Contains(individualId))
            allValues[individualId] = null;

         return allValues[individualId];
      }

      public ICache<int, double?> AllBioAvailabilityAucInf = new Cache<int, double?>();
      public ICache<int, double?> AllDDIAucInf = new Cache<int, double?>();
      public ICache<int, double?> AllDDICMax = new Cache<int, double?>();

      public double? BioAvailabilityAucInfFor(int individualId) => valueStoreFor(AllBioAvailabilityAucInf, individualId);

      public double? DDIAucInfFor(int individualId) => valueStoreFor(AllDDIAucInf, individualId);

      public void AddDDICMax(int individualId, double? cMax)
      {
         AllDDICMax[individualId] = cMax;
      }

      public void AddDDIAucInf(int individualId, double? aucInf)
      {
         AllDDIAucInf[individualId] = aucInf;
      }
      
      public double? CMaxDDIFor(int individualId) => valueStoreFor(AllDDICMax, individualId);

      public void AddBioavailability(int individualId, double? aucInf)
      {
         AllBioAvailabilityAucInf[individualId] = aucInf;
      }

      public void AddQuantityPKParameter(QuantityPKParameter quantityPKParameter)
      {
         _quantityPKParameters.Add(quantityPKParameter);
      }

      public ICache<string, QuantityPKParameter> QuantityPKParameters() => _quantityPKParameters;

      public double? QuantityFor(int individualId, string parameterName)
      {
         if(_quantityPKParameters.Contains(parameterName))
            return _quantityPKParameters[parameterName].ValueFor(individualId);

         return null;
      }
   }
}