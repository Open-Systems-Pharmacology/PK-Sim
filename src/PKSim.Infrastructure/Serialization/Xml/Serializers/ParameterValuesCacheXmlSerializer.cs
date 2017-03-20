using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ParameterValuesCacheXmlSerializer : BaseXmlSerializer<ParameterValuesCache>
   {
      public override void PerformMapping()
      {
         MapEnumerable(x => x.AllParameterValues, x => x.Add);
      }
   }

   public class ParameterValuesXmlSerializer : BaseXmlSerializer<ParameterValues>
   {
      public override void PerformMapping()
      {
         Map(x => x.ParameterPath);
         Map(x => x.Values);

         //Percentiles will be saved in deserialized 
      }

      protected override void TypedDeserialize(ParameterValues parameterValues, XElement element, SerializationContext serializationContext)
      {
         base.TypedDeserialize(parameterValues, element, serializationContext);
         var percentilesElements = element.Element(CoreConstants.Serialization.Percentiles);
         if (element.Element(CoreConstants.Serialization.Percentiles) != null)
         {
            var percentileSerializer = SerializerRepository.SerializerFor<List<double>>();
            parameterValues.Percentiles = percentileSerializer.Deserialize<List<double>>(percentilesElements, serializationContext);
         }
         else
         {
            parameterValues.Percentiles = new double[parameterValues.Count].InitializeWith(CoreConstants.DEFAULT_PERCENTILE).ToList();
         }
      }

      protected override XElement TypedSerialize(ParameterValues parameterValues, SerializationContext serializationContext)
      {
         var element = base.TypedSerialize(parameterValues, serializationContext);
         var allPercentiles = parameterValues.Percentiles.Distinct().ToList();
         if (allPercentiles.Count == 1 && allPercentiles[0] == CoreConstants.DEFAULT_PERCENTILE)
            return element;

         var doubleSerializer = SerializerRepository.SerializerFor(parameterValues.Percentiles);
         element.Add(doubleSerializer.Serialize(parameterValues.Percentiles, serializationContext));

         return element;
      }
   }
}