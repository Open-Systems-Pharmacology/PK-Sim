using System.Xml.Linq;
using OSPSuite.Serializer.Xml.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class ParameterRangeXmlSerializerBase<TParameterRange> : BaseXmlSerializer<TParameterRange> where TParameterRange : ParameterRange
   {
      public override void PerformMapping()
      {
         Map(x => x.ParameterName);
         Map(x => x.ParameterDisplayName);
         Map(x => x.MinValue);
         Map(x => x.MaxValue);
         Map(x => x.Dimension);

         //do not map unit as it need to be deserialize after dimension was set
      }

      protected override void TypedDeserialize(TParameterRange parameterRange, XElement parameterRangeElement, SerializationContext serializationContext)
      {
         base.TypedDeserialize(parameterRange, parameterRangeElement, serializationContext);
         var unit = parameterRangeElement.GetAttribute(Constants.Serialization.Attribute.DISPLAY_UNIT);
         parameterRange.Unit = parameterRange.Dimension.UnitOrDefault(unit);
      }

      protected override XElement TypedSerialize(TParameterRange parameterRange, SerializationContext serializationContext)
      {
         var parameterRangeElement = base.TypedSerialize(parameterRange, serializationContext);
         parameterRangeElement.AddAttribute(Constants.Serialization.Attribute.DISPLAY_UNIT, parameterRange.Unit.Name);
         return parameterRangeElement;
      }
   }

   public class ParameterRangeXmlSerializer : ParameterRangeXmlSerializerBase<ParameterRange>
   {
   }

   public class ConstrainedParameterRangeXmlSerializer : ParameterRangeXmlSerializerBase<ConstrainedParameterRange>
   {
   }

   public class DiscreteParameterRangeXmlSerializer : ParameterRangeXmlSerializerBase<DiscreteParameterRange>
   {
   }
}