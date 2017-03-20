using System.Linq;
using System.Xml.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.Infrastructure.Serialization.Xml;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.IntegrationTests
{
   public class When_converting_the_xml : ContextForIntegration<IParameterValuesCacheConverter>
   {
      private XElement _popElement;
      private SerializationContext _serializaionContext;

      protected override void Context()
      {
         base.Context();
         _serializaionContext = SerializationTransaction.Create();
         _popElement =
            new XElement("PopulationSimulation",
               new XElement("ParameterValuesCache",
                  new XElement("ParameterValues",
                     new XElement("DocumentElement",
                        new XElement("ParameterValues",
                           new XElement("Organism_x007C_BMI", 14),
                           new XElement("Organism_x007C_Weight", 22)),
                        new XElement("ParameterValues",
                           new XElement("Organism_x007C_BMI", 18),
                           new XElement("Organism_x007C_Weight", 28))),
                     new XElement("DataTableStructure",
                        new XElement("DataTableColumn", new XAttribute("type", "System.Double"), new XAttribute("name", "Organism|BMI")),
                        new XElement("DataTableColumn", new XAttribute("type", "System.Double"), new XAttribute("name", "Organism|Weight"))
                        ))));
      }

      protected override void Because()
      {
         sut.Convert(_popElement);
      }

      [Observation]
      public void should_have_remove_the_old_xml_and_created_a_new_one_that_will_be_readable()
      {
         _popElement.Descendants("DataTableStructure").ShouldBeEmpty();
      }

      [Observation]
      public void should_be_able_to_deserialize_the_new_node_and_retrieve_the_same_values()
      {
         var newParameterValueCacheElement = _popElement.Descendants("ParameterValuesCache").FirstOrDefault();
         var xmlReader = IoC.Resolve<IXmlReader<ParameterValuesCache>>();
         var parameterValueCache = xmlReader.ReadFrom(newParameterValueCacheElement,_serializaionContext);
         parameterValueCache.ValuesFor("Organism|BMI").ShouldContain(14, 18);
         parameterValueCache.ValuesFor("Organism|Weight").ShouldContain(22, 28);
      }
   }
}