using System;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Retrieve the formulation used in a simulation for a given mapping
   /// </summary>
   public interface IFormulationFromMappingRetriever
   {
      /// <summary>
      ///    Returns the formulation building block used in the simulation for the given mapping.
      ///    This will not be the template building block but the used building block in general unless the simualtion as not
      ///    initialized
      ///    yet. In that case, the template with the corresponding id will be returned if available
      /// </summary>
      /// <param name="simulation">Simulation where formulation should be defined</param>
      /// <param name="formulationMapping">mapping for which a formulation should be retrieved</param>
      /// <returns>the formulation used for the mapping or null if the parameter is null or does not use any formulation</returns>
      /// <exception cref="ArgumentException">An exception is thrown is the formulation id used in the mapping is not found</exception>
      Formulation FormulationUsedBy(Simulation simulation, FormulationMapping formulationMapping);

      /// <summary>
      ///    Returns the template formulation building block used in the simulation for the given mapping.
      ///    If the status of the building block is changed, returns the used building block in the simulation, otherwise the
      ///    template building block
      /// </summary>
      /// <param name="simulation">Simulation where formulation should be defined</param>
      /// <param name="formulationMapping">mapping for which a formulation should be retrieved</param>
      /// <returns>the formulation used for the mapping or null if the parameter is null or does not use any formulation</returns>
      /// <exception cref="ArgumentException">An exception is thrown is the formulation id used in the mapping is not found</exception>
      Formulation TemplateFormulationUsedBy(Simulation simulation, FormulationMapping formulationMapping);
   }

   public class FormulationFromMappingRetriever : IFormulationFromMappingRetriever
   {
      private readonly IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public FormulationFromMappingRetriever(IBuildingBlockInSimulationManager buildingBlockInSimulationManager, IBuildingBlockRepository buildingBlockRepository)
      {
         _buildingBlockInSimulationManager = buildingBlockInSimulationManager;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public Formulation FormulationUsedBy(Simulation simulation, FormulationMapping formulationMapping)
      {
         return retrieveFormulation(simulation, formulationMapping, b => b.BuildingBlock as Formulation);
      }

      public Formulation TemplateFormulationUsedBy(Simulation simulation, FormulationMapping formulationMapping)
      {
         return retrieveFormulation(simulation, formulationMapping, _buildingBlockInSimulationManager.TemplateBuildingBlockUsedBy<Formulation>);
      }

      private Formulation retrieveFormulation(Simulation simulation, FormulationMapping formulationMapping, Func<UsedBuildingBlock, Formulation> formulationRetriever)
      {
         if (!isValid(formulationMapping))
            return null;

         var templateFormulation = _buildingBlockRepository.ById<Formulation>(formulationMapping.TemplateFormulationId);
         var usedBuildingBlock = simulation.UsedBuildingBlockByTemplateId(formulationMapping.TemplateFormulationId);

         //this forumation was not used in the simulation yet, return the template
         if (usedBuildingBlock == null)
            return templateFormulation;

         //formulation was used in the simulation. 
         var formulation = formulationRetriever(usedBuildingBlock);
         return formulation ?? templateFormulation;
      }

      private bool isValid(FormulationMapping formulationMapping)
      {
         return formulationMapping != null && !formulationMapping.TemplateFormulationId.IsNullOrEmpty();
      }
   }
}