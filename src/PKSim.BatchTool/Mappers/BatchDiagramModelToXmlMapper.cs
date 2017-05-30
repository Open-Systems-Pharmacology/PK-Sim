using System.Xml;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Serialization.Diagram;
using PKSim.BatchTool.Services;

namespace PKSim.BatchTool.Mappers
{
   public class BatchDiagramModelToXmlMapper : IDiagramModelToXmlMapper
   {
      public IDiagramModel XmlDocumentToDiagramModel(XmlDocument xmlDoc)
      {
         return new BatchDiagramModel();
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

      public string ElementName
      {
         get { return "DiagramModel"; }
      }
   }
}