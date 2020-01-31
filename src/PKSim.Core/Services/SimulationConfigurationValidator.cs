using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Extensions;
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
         var formulations = simulation.AllBuildingBlocks<Formulation>();
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

         if (speciesPopulation.IsNamed(CoreConstants.Population.PREGNANT))
            throw new InvalidSimulationConfigurationException(PKSimConstants.Error.PregnantPopulationCanOnlyBeUsedWithMoBiModel(speciesPopulation.DisplayName));

         var administeredCompoundWithSuperSaturationEnabled = compounds.Where(x => x.SupersaturationEnabled).Select(x => new
         {
            Compound = x,
            simulation.CompoundPropertiesFor(x).ProtocolProperties
         }).Where(x => x.ProtocolProperties.IsAdministered).Select(x => new
         {
            x.Compound,
            x.ProtocolProperties.Protocol,
            x.ProtocolProperties.FormulationMappings
         });

         administeredCompoundWithSuperSaturationEnabled.Each(x => validateSupersaturationUsageFor(x.Compound, x.Protocol, x.FormulationMappings));

         formulations.Where(x=>x.FormulationType==CoreConstants.Formulation.TABLE).Each(validateTableFormulation);
      }

      private void validateTableFormulation(Formulation tableFormulation)
      {
         var formulaParameter = tableFormulation.Parameter(CoreConstants.Parameters.FRACTION_DOSE);
         var tableFormula = formulaParameter?.Formula as TableFormula;

         //at least one point in this formulation
         if(tableFormula!=null && tableFormula.AllPoints().Any()) 
            return;

         throw new InvalidSimulationConfigurationException(PKSimConstants.Error.TableFormulationRequiresAtLeastOnePoint(tableFormulation.Name));
      }

      private void validateSupersaturationUsageFor(Compound compound, Protocol protocol, IReadOnlyList<FormulationMapping> formulationMappings)
      {
         var protocolSchemaItems = _schemaItemsMapper.MapFrom(protocol);

         var hasOralApplicationsNotUsingParticles = protocolSchemaItems
            .Where(x => x.IsOral)
            .Select(x => formulationMappings.Find(f => string.Equals(f.FormulationKey, x.FormulationKey)))
            .Any(item => !string.Equals(item?.Formulation.FormulationType, CoreConstants.Formulation.PARTICLES));

         if (hasOralApplicationsNotUsingParticles)
            throw new InvalidSimulationConfigurationException(PKSimConstants.Error.SaturationEnabledCanOnlyBeUsedForOralApplicationUsingParticleDissolution(compound.Name));

         var hasUserDefinedApplicationInLumen = protocolSchemaItems
            .Where(x => x.IsUserDefined)
            .Any(x => string.Equals(x.TargetOrgan, CoreConstants.Organ.Lumen));

         if (hasUserDefinedApplicationInLumen)
            throw new InvalidSimulationConfigurationException(PKSimConstants.Error.SaturationEnabledCanOnlyBeUsedForOralApplicationUsingParticleDissolution(compound.Name));
      }
   }
}