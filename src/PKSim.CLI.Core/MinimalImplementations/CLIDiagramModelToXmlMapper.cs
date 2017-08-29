using System.Xml;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Serialization.Diagram;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIDiagramModelToXmlMapper : IDiagramModelToXmlMapper
   {
      public IDiagramModel XmlDocumentToDiagramModel(XmlDocument xmlDoc)
      {
         return new CLIDiagramModel();
      }

      public XmlDocument DiagramModelToXmlDocument(IDiagramModel diagramModel)
      {
         var doc = new XmlDocument();
         doc.AppendChild(doc.CreateElement(ElementName));
         return doc;
      }

      public void Deserialize(IDiagramModel model, XmlDocument xmlDoc)
      {
      }

      public void AddElementBaseNodeBindingFor<T>(T node)
      {
      }

      public string ElementName => "DiagramModel";
   }
}