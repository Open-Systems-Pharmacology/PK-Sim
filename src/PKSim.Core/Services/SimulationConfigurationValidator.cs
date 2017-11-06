using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationConfigurationValidator
   {
      /// <summary>
      ///    Validate the configuration defined in the simulation.
      /// </summary>
      /// <param name="simulation">Simulation to validate</param>
      /// <exception cref="InvalidSimulationConfigurationException">is thrown if the configuration is not valid</exception>
      void ValidateConfigurationFor(Simulation simulation);
   }

   public class SimulationConfigurationValidator : ISimulationConfigurationValidator
   {
      private readonly IProtocolToSchemaItemsMapper _schemaItemsMapper;

      public SimulationConfigurationValidator(IProtocolToSchemaItemsMapper schemaItemsMapper)
      {
         _schemaItemsMapper = schemaItemsMapper;
      }

      public void ValidateConfigurationFor(Simulation simulation)
      {
         var compounds = simulation.Compounds;
         var simulationSubject = simulation.BuildingBlock<ISimulationSubject>();
         var protocols = simulation.Protocols;
         var modelConfiguration = simulation.ModelConfiguration;

         //ForComp model can only be used with small molecules
         if (string.Equals(modelConfiguration.ModelName, CoreConstants.Model.FourComp) && compounds.Any(x => !x.IsSmallMolecule))
            throw new InvalidSimulationConfigurationException(PKSimConstants.Error.FourCompModelCannotBeUsedWithLargeMolecule);

         if (simulation.NameIsOneOf(compounds.AllNames()))
            throw new InvalidSimulationConfigurationException(PKSimConstants.Error.CompoundAndSimulationCannotShareTheSameName);

         if (simulation.NameIsOneOf(simulationSubject.AllMolecules().AllNames()))
            throw new InvalidSimulationConfigurationException(PKSimConstants.Error.IndividualMoleculesAnSimulationCannotShareTheSameName);

         var schemaItems = protocols.SelectMany(x => _schemaItemsMapper.MapFrom(x)).ToList();
         var speciesPopulation = simulationSubject.OriginData.SpeciesPopulation;
         if (!speciesPopulation.IsBodySurfaceAreaDependent && schemaItems.Any(x => x.DoseIsPerBodySurfaceArea()))
            throw new InvalidSimulationConfigurationException(PKSimConstants.Error.DosePerBodySurfaceAreaProtocolCannotBeUsedWithSpeciesPopulation(speciesPopulation.DisplayName));

         if(speciesPopulation.IsNamed(CoreConstants.Population.Pregnant))
            throw new InvalidSimulationConfigurationException(PKSimConstants.Error.PregnantPopulationCanOnlyBeUsedWithMoBiModel(speciesPopulation.DisplayName));

      }
   }
}