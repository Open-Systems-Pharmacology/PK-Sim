using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core.Snapshots.Services
{
   public class SnapshotConfiguration
   {
      public Individual Individual { get; set; }
      public Compound Compound { get; set; }
      public Protocol Protocol { get; set; }
      public string ModelName { get; set; } = CoreConstants.Model.FourComp;
      public string SimulationName { get; set; } = "S";
   }

   public interface ISnapshotObjectCreator
   {
      Task<Individual> DefaultIndividual(string name = "Ind");
      Task<Compound> StandardCompound(double lipophilicity, double fu, double molWeight, double solubilityAtRefPh = 9999, double refPh = 7, string name = "Drug");
      Task<Protocol> SimpleProtocol(double dose, string doseUnit, ApplicationType applicationType, string name = "Protocol");
      Task<Simulation> SnapshotSimulationFor(SnapshotConfiguration snapshotConfiguration);
      Task<Model.Simulation> SimulationFor(SnapshotConfiguration snapshotConfiguration);
   }

   public class SnapshotObjectCreator : ISnapshotObjectCreator
   {
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly IndividualMapper _individualMapper;
      private readonly ProjectMapper _projectMapper;

      public SnapshotObjectCreator(
         IDefaultIndividualRetriever defaultIndividualRetriever,
         IndividualMapper individualMapper,
         ProjectMapper projectMapper)
      {
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _individualMapper = individualMapper;
         _projectMapper = projectMapper;
      }

      public Task<Individual> DefaultIndividual(string name = "Ind")
      {
         var individual = _defaultIndividualRetriever.DefaultIndividual().WithName("Ind");
         return _individualMapper.MapToSnapshot(individual);
      }

      public Task<Compound> StandardCompound(double lipophilicity, double fu, double molWeight, double solubilityAtRefPh = 9999, double refPh = 7, string name = "Drug")
      {
         var compound = new Compound
         {
            Name = name,
            Lipophilicity = new[] {createAlternative(CoreConstants.Groups.COMPOUND_LIPOPHILICITY, CoreConstants.Species.Human, createParameter(CoreConstants.Parameter.LIPOPHILICITY, lipophilicity))},
            FractionUnbound = new[] {createAlternative(CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND, CoreConstants.Species.Human, createParameter(CoreConstants.Parameter.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE, fu))},
            Solubility = new[] {createAlternative(CoreConstants.Groups.COMPOUND_SOLUBILITY, null, 
               createParameter(CoreConstants.Parameter.SOLUBILITY_AT_REFERENCE_PH, solubilityAtRefPh),
               createParameter(CoreConstants.Parameter.REFERENCE_PH, refPh)
               )},
            Parameters = new[]
            {
               createParameter(CoreConstants.Parameter.MOLECULAR_WEIGHT, molWeight),
            }
         };

         return Task.FromResult(compound);
      }

      private Alternative createAlternative(string groupName, string species, params Parameter[] parameters) => new Alternative
      {
         Name = groupName,
         Species = species,
         Parameters = parameters,
      };

      private static Parameter createParameter(string parameterName, double parameterValue, string unit = null) => new Parameter
      {
         Name = parameterName,
         Value = parameterValue,
         Unit = unit
      };

      public Task<Protocol> SimpleProtocol(double dose, string doseUnit, ApplicationType applicationType, string name = "Protocol")
      {
         var protocol = new Protocol
         {
            Name = name,
            ApplicationType = applicationType.Name,
            Parameters = new[]
            {
               createParameter(CoreConstants.Parameter.INPUT_DOSE, dose, doseUnit)
            }
         };

         return Task.FromResult(protocol);
      }

      public Task<Simulation> SnapshotSimulationFor(SnapshotConfiguration snapshotConfiguration)
      {
         var simulation = new Simulation
         {
            Name = snapshotConfiguration.SimulationName,
            Individual = snapshotConfiguration.Individual.Name,
            Model = snapshotConfiguration.ModelName,

            Compounds = new[]
            {
               new CompoundProperties
               {
                  Name = snapshotConfiguration.Compound.Name,
                  Protocol = new ProtocolSelection
                  {
                     Name = snapshotConfiguration.Protocol.Name
                  }
               }
            }
         };

         return Task.FromResult(simulation);
      }

      public async Task<Model.Simulation> SimulationFor(SnapshotConfiguration snapshotConfiguration)
      {
         var project = new Project
         {
            Individuals = new[] {snapshotConfiguration.Individual},
            Compounds = new[] {snapshotConfiguration.Compound},
            Protocols = new[] {snapshotConfiguration.Protocol},
         };

         var snapshotsimulation = await SnapshotSimulationFor(snapshotConfiguration);
         project.Simulations = new[] {snapshotsimulation};

         var pksimProject = await _projectMapper.MapToModel(project);
         return pksimProject.BuildingBlockByName<Model.Simulation>(snapshotConfiguration.SimulationName);
      }
   }
}