using System.Linq;
using System.Xml;
using System.Xml.Linq;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Serialization.Diagram;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class SimulationXmlSerializer<TSimulation> : BuildingBlockXmlSerializer<TSimulation> where TSimulation : Simulation
   {
      private const string REACTION_DIAGRAM_MODEL = "ReactionDiagramModel";

      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.ResultsVersion);
         Map(x => x.Properties);
         Map(x => x.Model);
         Map(x => x.Settings);
         Map(x => x.OutputMappings);
         MapEnumerable(x => x.Reactions, x => x.AddReactions);
         MapEnumerable(x => x.UsedBuildingBlocks, x => x.AddUsedBuildingBlock);
         MapEnumerable(x => x.UsedObservedData, x => x.AddUsedObservedData);
         Map(x => x.OverwriteParameterSetSelections);
         Map(x => x.ParameterChangeTracker);

         //Do not save charts that will be saved separately
      }

      protected override XElement TypedSerialize(TSimulation simulation, SerializationContext context)
      {
         var simulationElement = base.TypedSerialize(simulation, context);
         var diagramElement = reactionDiagramModelElementFor(simulation, context);
         if (diagramElement != null)
            simulationElement.Add(diagramElement);

         return simulationElement;
      }

      private XElement reactionDiagramModelElementFor(TSimulation simulation, SerializationContext context)
      {
         if (simulation.ReactionDiagramModel == null)
            return null;

         var xmlDoc = context.Resolve<IDiagramModelToXmlMapper>().DiagramModelToXmlDocument(simulation.ReactionDiagramModel);
         if (xmlDoc.DocumentElement == null)
            return null;

         var diagramElement = XDocument.Load(new XmlNodeReader(xmlDoc)).Root;
         diagramElement.Name = REACTION_DIAGRAM_MODEL;
         return diagramElement;
      }

      protected override void TypedDeserialize(TSimulation simulation, XElement simulationElement, SerializationContext context)
      {
         //before deserializing, it is possible but unlikely that the name of the used building block has changed
         //it would be then overwritten when loading the simulation=>we save the name for the template building block
         var usedBbNames = simulation.UsedBuildingBlocks.Select(ubb => new { ubb.TemplateId, ubb.Name }).ToList();

         base.TypedDeserialize(simulation, simulationElement, context);
         deserializeReactionDiagramModel(simulation, simulationElement, context);

         //reset the names for the used building blocks
         usedBbNames.Each(ubb =>
         {
            var usedBuildingBlock = simulation.UsedBuildingBlockByTemplateId(ubb.TemplateId);
            if (usedBuildingBlock == null) return;
            usedBuildingBlock.Name = ubb.Name;
         });
      }

      private void deserializeReactionDiagramModel(TSimulation simulation, XElement simulationElement, SerializationContext context)
      {
         var diagramElement = simulationElement.Element(REACTION_DIAGRAM_MODEL);
         if (diagramElement == null)
            return;

         var xmlDoc = new XmlDocument();
         xmlDoc.Load(diagramElement.CreateReader());
         var diagramModel = context.Resolve<IDiagramModelFactory>().Create();
         context.Resolve<IDiagramModelToXmlMapper>().Deserialize(diagramModel, xmlDoc);
         simulation.ReactionDiagramModel = diagramModel;
      }
   }

   public class PopulationSimulationXmlSerializer : SimulationXmlSerializer<PopulationSimulation>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.AgingData);
         Map(x => x.ParameterValuesCache);
      }
   }

   public class IndividualSimulationXmlSerializer : SimulationXmlSerializer<IndividualSimulation>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.AucIV);
         Map(x => x.AucDDI);
         Map(x => x.CMaxDDI);
      }
   }
}