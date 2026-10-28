using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using OSPSuite.Core.Serialization;
using PKSim.Core.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using static PKSim.Core.CoreConstants.Serialization;
using SerializationAttribute = PKSim.Core.CoreConstants.Serialization.Attribute;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Saves an expression query into the query configuration of a molecule and restores it
   /// </summary>
   public interface IExpressionQuerySerializer
   {
      string Serialize(ExpressionQuery query);
      ExpressionQuery Deserialize(string queryConfiguration);
   }

   public class ExpressionQuerySerializer : IExpressionQuerySerializer
   {
      private const string LAYOUT_PROPERTY = "property";
      private const string LAYOUT_PROPERTY_NAME = "name";
      private readonly IExpressionDataTableMapper _expressionDataTableMapper;

      public ExpressionQuerySerializer(IExpressionDataTableMapper expressionDataTableMapper)
      {
         _expressionDataTableMapper = expressionDataTableMapper;
      }

      public string Serialize(ExpressionQuery query)
      {
         var element = new XElement(QueryConfiguration);
         element.Add(new XAttribute(SerializationAttribute.ProteinName, query.ProteinName));
         element.Add(new XAttribute(SerializationAttribute.SelectedUnit, query.SelectedUnit ?? string.Empty));
         element.Add(new XElement(ExpressionDataSet, _expressionDataTableMapper.DataSetFrom(query).SaveToXmlString()));

         if (!string.IsNullOrEmpty(query.LayoutSettings))
            element.Add(new XElement(LayoutSettings, query.LayoutSettings));

         element.Add(filterElementFor(query.Filter));
         return element.ToString(SaveOptions.DisableFormatting);
      }

      public ExpressionQuery Deserialize(string queryConfiguration)
      {
         var rootElement = XElementSerializer.PermissiveLoad(new MemoryStream(Encoding.Default.GetBytes(queryConfiguration)));

         var expressionDataSetElement = rootElement.Element(ExpressionDataSet);
         if (expressionDataSetElement == null)
            throw new PKSimException("XML Element ExpressionDataSet missing!");

         var proteinNameAttribute = rootElement.Attribute(SerializationAttribute.ProteinName);
         if (proteinNameAttribute == null)
            throw new PKSimException("XML Element ProteinName missing!");

         var dataSet = new DataSet();
         dataSet.ReadFromXmlString(expressionDataSetElement.Value);
         var records = _expressionDataTableMapper.RecordsFrom(dataSet.Tables[ExpressionDataTableMapper.EXPRESSION_DATA]);
         var mapping = _expressionDataTableMapper.MappingFrom(dataSet.Tables[ExpressionDataTableMapper.MAPPING_DATA]);
         var layoutSettings = rootElement.Element(LayoutSettings)?.Value;
         var filterElement = rootElement.Element(Filter);

         return new ExpressionQuery(proteinNameAttribute.Value, records, mapping)
         {
            SelectedUnit = rootElement.Attribute(SerializationAttribute.SelectedUnit)?.Value ?? string.Empty,
            LayoutSettings = layoutSettings,
            Filter = filterElement != null ? filterFrom(filterElement) : filterFromLayoutWithoutFilter(layoutSettings)
         };
      }

      private static XElement filterElementFor(ExpressionDataFilter filter) =>
         new XElement(Filter, filter.FieldFilters.Select(x =>
            new XElement(FieldFilter,
               new XAttribute(SerializationAttribute.FieldName, x.FieldName),
               new XAttribute(SerializationAttribute.FilterType, x.FilterType),
               new XAttribute(SerializationAttribute.ShowBlanks, x.ShowBlanks),
               x.Values.Select(value => new XElement(Value, Convert.ToString(value, CultureInfo.InvariantCulture))))));

      private static ExpressionDataFilter filterFrom(XElement filterElement) =>
         new ExpressionDataFilter(filterElement.Elements(FieldFilter).Select(x =>
         {
            var fieldName = x.Attribute(SerializationAttribute.FieldName).Value;
            return new ExpressionDataFieldFilter(fieldName,
               Enum.Parse<ExpressionDataFilterType>(x.Attribute(SerializationAttribute.FilterType).Value),
               x.Elements(Value).Select(value => valueFrom(fieldName, value.Value)),
               bool.Parse(x.Attribute(SerializationAttribute.ShowBlanks).Value));
         }));

      private static object valueFrom(string fieldName, string value) =>
         ExpressionDataFields.IsNumeric(fieldName) ? double.Parse(value, CultureInfo.InvariantCulture) : value;

      /// <summary>
      ///    Queries saved before the filter was saved on its own kept it in the pivot grid layout, either as prefilter criteria
      ///    or as field filter values. Only the view can convert such a filter, so it is returned as null
      /// </summary>
      private static ExpressionDataFilter filterFromLayoutWithoutFilter(string layoutSettings) =>
         layoutHasFilter(layoutSettings) ? null : new ExpressionDataFilter();

      private static bool layoutHasFilter(string layoutSettings)
      {
         if (string.IsNullOrEmpty(layoutSettings))
            return false;

         var properties = XDocument.Parse(layoutSettings).Descendants(LAYOUT_PROPERTY).ToList();

         var filterHasCriteria = properties.Where(x => isNamed(x, "Prefilter") || isNamed(x, "ActiveFilter"))
            .SelectMany(x => x.Elements(LAYOUT_PROPERTY))
            .Any(x => (isNamed(x, "Criteria") || isNamed(x, "CriteriaString")) && x.Attribute("isnull") == null && !string.IsNullOrEmpty(x.Value));

         var fieldHasFilterValues = properties.Where(x => isNamed(x, "FilterValues"))
            .SelectMany(x => x.Elements(LAYOUT_PROPERTY))
            .Any(x => (isNamed(x, "ValuesCore") && x.Attribute("value")?.Value != "0") ||
                      (isNamed(x, "ShowBlanks") && string.Equals(x.Value, bool.FalseString, StringComparison.OrdinalIgnoreCase)));

         return filterHasCriteria || fieldHasFilterValues;
      }

      private static bool isNamed(XElement property, string name) => string.Equals(property.Attribute(LAYOUT_PROPERTY_NAME)?.Value, name);
   }
}
