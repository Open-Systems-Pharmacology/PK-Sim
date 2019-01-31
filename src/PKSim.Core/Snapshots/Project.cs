using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Snapshots
{
   public class Project : IWithDescription, IWithName
   {
      [Required]
      public int Version { get; set; }

      public string Name { get; set; }
      public string Description { get; set; }
      public Individual[] Individuals { get; set; }
      public Population[] Populations { get; set; }
      public Compound[] Compounds { get; set; }
      public Formulation[] Formulations { get; set; }
      public Protocol[] Protocols { get; set; }
      public Event[] Events { get; set; }
      public Simulation[] Simulations { get; set; }
      public ParameterIdentification[] ParameterIdentifications { get; set; }
      public DataRepository[] ObservedData { get; set; }
      public SimulationComparison[] SimulationComparisons { get; set; }
      public QualificationPlan[] QualificationPlans { get; set; }
      public Classification[] ObservedDataClassifications { get; set; }
      public Classification[] SimulationComparisonClassifications { get; set; }
      public Classification[] SimulationClassifications { get; set; }
      public Classification[] ParameterIdentificationClassifications { get; set; }
      public Classification[] QualificationPlanClassifications { get; set; }

      public IReadOnlyList<IBuildingBlockSnapshot> BuildingBlocksByType(PKSimBuildingBlockType buildingBlockType)
      {
         switch (buildingBlockType)
         {
            case PKSimBuildingBlockType.Compound:
               return Compounds;
            case PKSimBuildingBlockType.Formulation:
               return Formulations;
            case PKSimBuildingBlockType.Protocol:
               return Protocols;
            case PKSimBuildingBlockType.Individual:
               return Individuals;
            case PKSimBuildingBlockType.Population:
               return Populations;
            case PKSimBuildingBlockType.Event:
               return Events;
            default:
               return null;
         }
      }

      public IBuildingBlockSnapshot BuildingBlockByTypeAndName(PKSimBuildingBlockType buildingBlockType, string name) =>
         BuildingBlocksByType(buildingBlockType)?.FindByName(name);

      public bool Swap(IBuildingBlockSnapshot newBuildingBlock)
      {
         if (newBuildingBlock == null)
            return false;

         var type = newBuildingBlock.BuildingBlockType;
         var name = newBuildingBlock.Name;
         var originalBuildingBlock = BuildingBlockByTypeAndName(type, name);

         if (originalBuildingBlock == null)
            throw new PKSimException($"Could not find {type} '{name}` in snapshot '${Name}'.");

         switch (type)
         {
            case PKSimBuildingBlockType.Compound:
               return swap(Compounds, originalBuildingBlock, newBuildingBlock);
            case PKSimBuildingBlockType.Formulation:
               return swap(Formulations, originalBuildingBlock, newBuildingBlock);
            case PKSimBuildingBlockType.Protocol:
               return swap(Protocols, originalBuildingBlock, newBuildingBlock);
            case PKSimBuildingBlockType.Individual:
               return swap(Individuals, originalBuildingBlock, newBuildingBlock);
            case PKSimBuildingBlockType.Population:
               return swap(Populations, originalBuildingBlock, newBuildingBlock);
            case PKSimBuildingBlockType.Event:
               return swap(Events, originalBuildingBlock, newBuildingBlock);
            default:
               return false;
         }
      }

      private bool swap<T>(T[] buildingBlocks, IBuildingBlockSnapshot originalBuildingBlock, IBuildingBlockSnapshot newBuildingBlock)
      {
         var index = Array.IndexOf(buildingBlocks, originalBuildingBlock);
         if (index < 0)
            return false;

         buildingBlocks[index] = newBuildingBlock.DowncastTo<T>();
         return true;
      }
   }
}