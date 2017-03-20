using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IVSSCalculator
   {
      /// <summary>
      ///    Returns the VSS Phys-Chem for the compound named <paramref name="compoundName" /> in the
      ///    <paramref name="simulation" />
      /// </summary>
      IParameter VSSPhysChemFor(Simulation simulation, string compoundName);

      /// <summary>
      ///    Returns a cache [CompoundName, VSS Phys-Chem] with all VSS Phys-Chem values for all compound defined in the
      ///    <paramref name="simulation" />
      /// </summary>
      ICache<string, IParameter> VSSPhysChemFor(Simulation simulation);

      /// <summary>
      ///    Returns a cache [species, VSS Phys-Chem] with all possible vss values using the default individual for each species
      /// </summary>
      ICache<Species, IParameter> VSSPhysChemFor(Compound compound);

      /// <summary>
      ///    Returns a vss parameter with the given <see cref="value" />
      /// </summary>
      IParameter VSSParameterWithValue(double value);
   }

   public class VSSCalculator : IVSSCalculator
   {
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly ISimulationFactory _simulationFactory;
      private readonly IParameterFactory _parameterFactory;
      private readonly IProtocolFactory _protocolFactory;

      public VSSCalculator(IDefaultIndividualRetriever defaultIndividualRetriever, ISpeciesRepository speciesRepository,
         ISimulationFactory simulationFactory, IParameterFactory parameterFactory, IProtocolFactory protocolFactory)
      {
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _speciesRepository = speciesRepository;
         _simulationFactory = simulationFactory;
         _parameterFactory = parameterFactory;
         _protocolFactory = protocolFactory;
      }

      public ICache<string, IParameter> VSSPhysChemFor(Simulation simulation)
      {
         var cache = new Cache<string, IParameter>();
         simulation.CompoundNames.Each(name => cache[name] = VSSPhysChemFor(simulation, name));
         return cache;
      }

      public ICache<Species, IParameter> VSSPhysChemFor(Compound compound)
      {
         var cache = new Cache<Species, IParameter>();
         var protocol = _protocolFactory.Create(ProtocolMode.Simple);
         _speciesRepository.All().Each(species =>
         {
            var defaultIndividual = _defaultIndividualRetriever.DefaultIndividualFor(species);
            var simulation = _simulationFactory.CreateForVSS(protocol, defaultIndividual, compound);
            cache[species] = VSSPhysChemFor(simulation, compound.Name);
         });
         return cache;
      }

      public IParameter VSSParameterWithValue(double value)
      {
         return _parameterFactory.CreateFor(CoreConstants.PKAnalysis.VssPhysChem, value, CoreConstants.Dimension.VolumePerBodyWeight, PKSimBuildingBlockType.Simulation);
      }

      public IParameter VSSPhysChemFor(Simulation simulation, string compoundName)
      {
         var individual = simulation.Individual;
         var organism = individual.Organism;
         var bodyweight = individual.WeightParameter.Value;
         var hct = organism.Parameter(CoreConstants.Parameter.HCT).Value;

         var allPartitionCoefficients = simulation.Model.Neighborhoods
            .GetAllChildren<IParameter>(p => p.IsNamed(CoreConstants.Parameter.K_CELL_PLS))
            .Where(p => p.ParentContainer.IsNamed(compoundName)).ToList();


         //If bodyWeight = 0 Then Return
         double vss = 0;
         foreach (var organ in organism.OrgansByType(OrganType.Tissue))
         {
            var f_vas = organ.Parameter(CoreConstants.Parameter.FractionVascular).Value;
            var volume = organ.Parameter(CoreConstants.Parameter.VOLUME).Value;
            var k_cell_pls = getParameterFromNeighborhoodFor(organ, allPartitionCoefficients);
            vss += (k_cell_pls + f_vas * (1 - hct)) * volume;
         }


         foreach (var organ in organism.OrgansByType(OrganType.VascularSystem))
         {
            var volume = organ.Parameter(CoreConstants.Parameter.VOLUME).Value;
            vss += volume * (1 - hct);
         }

         vss /= bodyweight;
         return VSSParameterWithValue(vss);
      }

      private double getParameterFromNeighborhoodFor(Organ organ, IEnumerable<IParameter> allPartitionCoefficients)
      {
         foreach (var partitionCoeff in allPartitionCoefficients)
         {
            var neighborhood = partitionCoeff.NeighborhoodAncestor();
            //neighborhood are relationship between compartments=>we need to retrieve the organ hence parent container
            if (neighborhood.FirstNeighbor.ParentContainer.IsNamed(organ.Name))
               return partitionCoeff.Value;
         }
         return 0;
      }
   }
}