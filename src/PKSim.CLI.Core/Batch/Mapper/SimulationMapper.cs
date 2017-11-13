using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Batch.Mapper
{
   internal interface ISimulationMapper : IMapper<Simulation, SimulationForBatch>
   {
   }

   internal class SimulationMapper : ISimulationMapper
   {
      private readonly ICompoundMapper _compoundMapper;
      private readonly IIndividualMapper _individualMapper;
      private readonly IApplicationProtocolMapper _protocolMapper;
      private readonly ISimulationConstructor _simulationConstructor;
      private readonly IModelPropertiesMapper _modelPropertiesMapper;
      private readonly IFormulationMapper _formulationMapper;
      private readonly ILogger _logger;

      public SimulationMapper(ICompoundMapper compoundMapper, IIndividualMapper individualMapper,
         IApplicationProtocolMapper protocolMapper, ISimulationConstructor simulationConstructor,
         IModelPropertiesMapper modelPropertiesMapper, IFormulationMapper formulationMapper, ILogger logger)
      {
         _compoundMapper = compoundMapper;
         _individualMapper = individualMapper;
         _protocolMapper = protocolMapper;
         _simulationConstructor = simulationConstructor;
         _modelPropertiesMapper = modelPropertiesMapper;
         _formulationMapper = formulationMapper;
         _logger = logger;
      }

      public SimulationForBatch MapFrom(Simulation batchSimulation)
      {
         var individual = _individualMapper.MapFrom(batchSimulation.Individual);
         var compounds = batchSimulation.Compounds.Select(_compoundMapper.MapFrom).ToList();
         var protocols = batchSimulation.ApplicationProtocols.Select(_protocolMapper.MapFrom).ToList();

         var protocolForCompound = new Cache<Model.Compound, Protocol>();
         foreach (var applicationProtocol in batchSimulation.ApplicationProtocols)
         {
            var protocol = _protocolMapper.MapFrom(applicationProtocol);
            var compound = compounds.FindByName(applicationProtocol.CompoundName);
            if (compound != null)
               protocolForCompound.Add(compound, protocol);
         }

         //if protocol for compound is not empty, that means that name were specified explictely in json file and we should use that
         var protocolToUse = protocolForCompound.Any() ? protocolForCompound.ToList() : protocols;

         //a requirement is that compounds and protocols have the same length. Fill missing entries with null
         while (protocolToUse.Count < compounds.Count)
         {
            protocolToUse.Add(null);
         }

         var modelProperties = _modelPropertiesMapper.MapFrom(batchSimulation.Configuration, individual);
         var formulation = _formulationMapper.MapFrom(batchSimulation.Formulation);

         var simulationConstruction = new SimulationConstruction
         {
            SimulationSubject = individual,
            TemplateCompounds = compounds,
            TemplateProtocols = protocols,
            TemplateFormulation = formulation,
            ModelProperties = modelProperties,
            SimulationName = Simulation.Name,
            AllowAging = batchSimulation.Configuration.AllowAging,
            Interactions = batchSimulation.Interactions
         };

         var simulation = _simulationConstructor
            .CreateSimulation(simulationConstruction)
            .DowncastTo<IndividualSimulation>();


         var config = batchSimulation.Configuration;
         var interval = simulation.OutputSchema.Intervals.First();

         //remove old ones
         foreach (var otherInterval in simulation.OutputSchema.Intervals.Skip(1).ToList())
         {
            simulation.OutputSchema.RemoveInterval(otherInterval);
         }

         interval.StartTime.Value = config.StartTime;
         interval.EndTime.Value = config.EndTime;
         interval.Resolution.Value = config.Resolution;
         simulation.Solver.AbsTol = config.AbsTol;
         simulation.Solver.RelTol = config.RelTol;

         _logger.AddDebug($"Start Time = {config.StartTime}");
         _logger.AddDebug($"End Time = {config.EndTime}");
         _logger.AddDebug($"Resolution = {config.Resolution}");
         _logger.AddDebug($"AbsTol = {config.AbsTol}");
         _logger.AddDebug($"RelTol = {config.RelTol}");
         _logger.AddDebug($"UseJacobian = {config.UseJacobian}");

         var simForBatch = new SimulationForBatch
         {
            Simulation = simulation,
            Configuration = config
         };
         simForBatch.ParameterVariationSets.AddRange(batchSimulation.ParameterVariationSets);

         return simForBatch;
      }
   }
}