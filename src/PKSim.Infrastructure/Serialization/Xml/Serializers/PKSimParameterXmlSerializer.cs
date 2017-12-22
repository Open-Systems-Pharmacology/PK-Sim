using System.Globalization;
using System.Xml.Linq;
using OSPSuite.Serializer;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Serialization.Xml.Extensions;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class PKSimParameterBaseXmlSerializer<T> : EntityXmlSerializer<T>, IPKSimXmlSerializer where T : IParameter
   {
      protected PKSimParameterBaseXmlSerializer(string name) : base(name)
      {
      }

      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.Info);
         Map(x => x.Origin).WithMappingName(CoreConstants.Serialization.Origin);
         Map(x => x.DefaultValue).WithMappingName(CoreConstants.Serialization.Attribute.DefaultValue);
         Map(x => x.Dimension).WithMappingName(Constants.Serialization.Attribute.Dimension);
         Map(x => x.ValueOrigin);
         //fixed value will be saved in deserialized

         Map(x => x.BuildMode).WithMappingName(CoreConstants.Serialization.Attribute.Mode);
         MapReference(x => x.Formula).WithMappingName(Constants.Serialization.Attribute.FORMULA);
         MapReference(x => x.RHSFormula).WithMappingName(CoreConstants.Serialization.Attribute.RHSFormula);
      }

      protected override void TypedDeserialize(T parameter, XElement parameterElement, SerializationContext serializationContext)
      {
         base.TypedDeserialize(parameter, parameterElement, serializationContext);
         parameterElement.UpdateDisplayUnit(parameter);
         parameter.IsFixedValue = isFixedAttribute(parameterElement) == "1";

         if (parameter.IsFixedValue)
         {
            parameter.Value = parameterElement.GetAttribute(Constants.Serialization.Attribute.VALUE).ConvertedTo<double>();
         }
      }

      private static string isFixedAttribute(XElement quantityElement)
      {
         //Is Fixed was renamed from fix to isFixedValue. We check first for new version and retrieve the old one if required
         var attribute = quantityElement.Attribute(Constants.Serialization.Attribute.IS_FIXED_VALUE) ?? quantityElement.Attribute("fix");
         return attribute == null ? "0" : "1";
      }

      protected override XElement TypedSerialize(T parameter, SerializationContext serializationContext)
      {
         var parameterElement = base.TypedSerialize(parameter, serializationContext);

         var originNode = parameterElement.Element(CoreConstants.Serialization.Origin);
         if (originNode != null && !originNode.HasAttributes)
            originNode.Remove();

         parameterElement.AddDisplayUnitFor(parameter);

         if (parameter.Formula.IsConstant())
         {
            //constant formula=>we need to save the value and remove the formula attribute that won't be saved
            var attribute = parameterElement.Attribute(Constants.Serialization.Attribute.FORMULA);
            if (attribute != null)
               attribute.Remove();

            //remove unused formula from cache that was saved automatically in the reference mapping
            serializationContext.RemoveFormulaFromCache(parameter.Formula);
         }

         if (parameter.IsFixedValue)
         {
            addValueAttribute(parameter, parameterElement);
            addIsFixedValueAttribute(parameterElement);
         }
         else if (parameter.Formula.IsConstant())
            addValueAttribute(parameter, parameterElement);

         return parameterElement;
      }

      private void addValueAttribute(T parameter, XElement parameterElement)
      {
         parameterElement.AddAttribute(Constants.Serialization.Attribute.VALUE, parameter.Value.ToString(NumberFormatInfo.InvariantInfo));
      }

      private void addIsFixedValueAttribute(XElement parameterElement)
      {
         parameterElement.AddAttribute(Constants.Serialization.Attribute.IS_FIXED_VALUE, "1");
      }
   }

   public class PKSimParameterXmlSerializer : PKSimParameterBaseXmlSerializer<PKSimParameter>
   {
      public PKSimParameterXmlSerializer() : base(CoreConstants.Serialization.Parameter)
      {
      }

      protected override void TypedDeserialize(PKSimParameter parameter, XElement parameterElement, SerializationContext serializationContext)
      {
         base.TypedDeserialize(parameter, parameterElement, serializationContext);

         var formulaId = parameterElement.GetAttribute(Constants.Serialization.Attribute.FORMULA);

         //parameter is a formula=>nothing to do 
         if (!string.IsNullOrEmpty(formulaId))
            return;

         var value = parameterElement.GetAttribute(Constants.Serialization.Attribute.VALUE).ConvertedTo<double>();
         var formula = serializationContext.ObjectFactory.Create<ConstantFormula>().WithValue(value).WithDimension(parameter.Dimension);
         parameter.Formula = formula;
      }
   }

   public class PKSimDistributedParameterXmlSerializer : PKSimParameterBaseXmlSerializer<PKSimDistributedParameter>
   {
      public PKSimDistributedParameterXmlSerializer() : base(CoreConstants.Serialization.DistributedParameter)
      {
      }

      public override void PerformMapping()
      {
         //Children need to be read first since they might be used in deserialization
         MapEnumerable(x => x.Children, x => x.Add);
         base.PerformMapping();
      }
   }
}