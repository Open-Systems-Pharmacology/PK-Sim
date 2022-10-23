using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class SimulationXmlSerializer<TSimulation> : BuildingBlockXmlSerializer<TSimulation> where TSimulation : Simulation
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.ResultsVersion);
         Map(x => x.Properties);
         Map(x => x.Model);
         Map(x => x.ReactionDiagramModel);
         Map(x => x.Reactions);
         Map(x => x.SimulationSettings);
         Map(x => x.OutputMappings);
         MapEnumerable(x => x.UsedBuildingBlocks, x => x.AddUsedBuildingBlock);
         MapEnumerable(x => x.UsedObservedData, x => x.AddUsedObservedData);

         //Do not save charts that will be saved separately
      }

      protected override void TypedDeserialize(TSimulation simulation, XElement simulationElement, SerializationContext context)
      {
         //before deserializing, it is possible but unlikely that the name of the used building block has changed
         //it would be then overwritten when loading the simulation=>we save the name for the template building block
         var usedBbNames = simulation.UsedBuildingBlocks.Select(ubb => new { ubb.TemplateId, ubb.Name }).ToList();

         base.TypedDeserialize(simulation, simulationElement, context);

         //reset the names for the used building blocks
         usedBbNames.Each(ubb =>
         {
            var usedBuildingBlock = simulation.UsedBuildingBlockByTemplateId(ubb.TemplateId);
            if (usedBuildingBlock == null) return;
            usedBuildingBlock.Name = ubb.Name;
         });
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
         MapEnumerable(x => x.AllCompoundPK, x => x.AddCompoundPK);
      }
   }
}