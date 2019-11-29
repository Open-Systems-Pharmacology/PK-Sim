using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface ICompoundPropertiesUpdater
   {
      /// <summary>
      /// Ensure that the compound properties list defined in the <paramref name="simulation"/> has up to date references to the actual <see cref="Compound"/> 
      /// and <see cref="Protocol"/> building blocks defined in the simulation.
      /// </summary>
      void UpdateCompoundPropertiesIn(Simulation simulation);
   }

   public class CompoundPropertiesUpdater : ICompoundPropertiesUpdater
   {
      private readonly ICompoundToCompoundPropertiesMapper _compoundPropertiesMapper;

      public CompoundPropertiesUpdater(ICompoundToCompoundPropertiesMapper compoundPropertiesMapper)
      {
         _compoundPropertiesMapper = compoundPropertiesMapper;
      }

      public void UpdateCompoundPropertiesIn(Simulation simulation)
      {
         var allExistingCompoundProperties = simulation.CompoundPropertiesList.ToList();
         var allExistingProtocols = simulation.AllBuildingBlocks<Protocol>().ToList();
         simulation.Properties.ClearCompoundPropertiesList();

         //add existing ones back
         foreach (var compound in simulation.Compounds)
         {
            //using name comparison here and not references because references might not be uptodate. 
            var compoundProperties = allExistingCompoundProperties.Find(x => string.Equals(x.Compound.Name, compound.Name));
            if (compoundProperties == null) continue;

            compoundProperties.Compound = compound;
            updateUsedProtocol(compoundProperties, allExistingProtocols);
            simulation.Properties.AddCompoundProperties(compoundProperties);
         }

         //create new ones
         simulation.AllBuildingBlocks<Compound>().Where(c => simulation.CompoundPropertiesFor(c) == null)
            .Each(c => simulation.Properties.AddCompoundProperties(_compoundPropertiesMapper.MapFrom(c)));

         //update protocol to previous selected one in case of a one to one mapping (e.g. switching from compound A to compound B)
         if (!isUpdatingOneCompoundWithAnotherOne(simulation, allExistingCompoundProperties))
            return;

         var compoundPropertiesForSingleCompound = simulation.CompoundPropertiesFor(simulation.Compounds[0]);
         if (compoundPropertiesForSingleCompound.IsAdministered)
            return;

         compoundPropertiesForSingleCompound.ProtocolProperties = allExistingCompoundProperties[0].ProtocolProperties;
         updateUsedProtocol(compoundPropertiesForSingleCompound, allExistingProtocols);
      }

      private static bool isUpdatingOneCompoundWithAnotherOne(Simulation simulation, IReadOnlyList<CompoundProperties> allExistingCompoundProperties)
      {
         return simulation.Compounds.Count == 1 && allExistingCompoundProperties.Count == 1;
      }

      private void updateUsedProtocol(CompoundProperties compoundProperties, IEnumerable<Protocol> allExistingProtocols)
      {
         if (!compoundProperties.IsAdministered)
            return;

         var protocolProperties = compoundProperties.ProtocolProperties;
         protocolProperties.Protocol = allExistingProtocols.FindByName(protocolProperties.Protocol.Name);
      }
   }
}