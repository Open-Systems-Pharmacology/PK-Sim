using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Assets;

namespace PKSim.Core.Services
{
   public interface IParameterChangeUpdater
   {
      /// <summary>
      /// Update all objects depending on the <paramref name="parameter"/> given as parameter. 
      /// For example, if the MolWeight of a compound building block is changed, all observed data defined for that compound will
      /// have the value of internal molweight value updated
      /// </summary>
      void UpdateObjectsDependingOn(IParameter parameter);

      /// <summary>
      /// Updates the internal molweight value stored in <paramref name="observedData"/> to match the molweight value of the compound buildingblock
      /// for which this <paramref name="observedData"/> was imported.
      /// </summary>
      void UpdateMolWeightIn(DataRepository observedData);
   }

   public class ParameterChangeUpdater : IParameterChangeUpdater
   {
      private readonly IBuildingBlockRetriever _buildingBlockRetriever;
      private readonly IObservedDataRepository _observedDataRepository;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public ParameterChangeUpdater(IBuildingBlockRetriever buildingBlockRetriever, IObservedDataRepository observedDataRepository, IBuildingBlockRepository buildingBlockRepository)
      {
         _buildingBlockRetriever = buildingBlockRetriever;
         _observedDataRepository = observedDataRepository;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public void UpdateObjectsDependingOn(IParameter parameter)
      {
         updateMolWeightDependencies(parameter);
      }

      private void updateMolWeightDependencies(IParameter parameter)
      {
         if (!parameter.IsNamed(Constants.Parameters.MOL_WEIGHT))
            return;

         var compound = _buildingBlockRetriever.BuildingBlockContaining(parameter) as Compound;
         if (compound == null)
            return;

         allObservedDataFor(compound).Each(x => updateMolWeight(x, compound));
      }

      private IEnumerable<DataRepository> allObservedDataFor(Compound compound)
      {
         return _observedDataRepository.All()
            .Where(x => string.Equals(moleculeNameFrom(x), compound.Name));
      }

      private string moleculeNameFrom(DataRepository observedData)
      {
         return observedData.ExtendedPropertyValueFor(ObservedData.Molecule);
      }

      public void UpdateMolWeightIn(DataRepository observedData)
      {
         var moleculeName = moleculeNameFrom(observedData);
         var compound = _buildingBlockRepository.All<Compound>().FindByName(moleculeName);
         updateMolWeight(observedData, compound);
      }

      private void updateMolWeight(DataRepository observedData, Compound compound)
      {
         //this can be the case when importing fraction
         if (compound == null) return;
         observedData.AllButBaseGrid().Each(x => x.DataInfo.MolWeight = compound.MolWeight);
      }
   }
}