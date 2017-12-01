using System.Collections.Generic;
using System.Threading;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Snapshots.Services;
using Compound = PKSim.Core.Model.Compound;
using Individual = PKSim.Core.Model.Individual;
using Simulation = PKSim.Core.Model.Simulation;

namespace PKSim.Infrastructure
{
   public static class DomainFactoryForSpecs
   {
      public static Individual CreateStandardIndividual(string population = CoreConstants.Population.ICRP)
      {
         var defaultIndividualRetriever = IoC.Resolve<IDefaultIndividualRetriever>();
         var populationRepository = IoC.Resolve<IPopulationRepository>();
         var cloneManager = IoC.Resolve<ICloneManagerForBuildingBlock>();
         return cloneManager.Clone(defaultIndividualRetriever.DefaultIndividualFor(populationRepository.FindByName(population)), new FormulaCache());
      }

      public static Compound CreateStandardCompound()
      {
         var objectCreator = IoC.Resolve<ISnapshotObjectCreator>();
         var compoundMapper = IoC.Resolve<PKSim.Core.Snapshots.Mappers.CompoundMapper>();

         var compoundSnapshot = objectCreator.StandardCompound(lipophilicity: -2, fractionUnbound: 0.8, molWeight: 400, solubilityAtRefPh: 1e-7).Result;
         return compoundMapper.MapToModel(compoundSnapshot).Result;
      }

      public static Protocol CreateStandardIVBolusProtocol()
      {
         var protocolFactory = IoC.Resolve<IProtocolFactory>();
         return protocolFactory.Create(ProtocolMode.Simple).WithName("Protocol");
      }

      public static Protocol CreateStandardIVProtocol()
      {
         var protocolFactory = IoC.Resolve<IProtocolFactory>();
         return protocolFactory.Create(ProtocolMode.Simple, ApplicationTypes.Intravenous).WithName("Protocol");
      }

      public static IndividualSimulation CreateDefaultSimulation()
      {
         var individual = CreateStandardIndividual();
         var compound = CreateStandardCompound();
         var protocol = CreateStandardIVBolusProtocol();
         return CreateSimulationWith(individual, compound, protocol) as IndividualSimulation;
      }

      public static IndividualSimulation CreateDefaultSimulationForModel(string modelName)
      {
         var individual = CreateStandardIndividual();
         var compound = CreateStandardCompound();
         var protocol = CreateStandardIVBolusProtocol();
         return CreateSimulationWith(individual, compound, protocol, modelName) as IndividualSimulation;
      }

      public static Simulation CreateSimulationWith(ISimulationSubject simulationSubject, Compound compound, Protocol protocol, string modelName)
      {
         var simulation = createModelLessSimulationWith(simulationSubject, compound, protocol, modelName);
         AddModelToSimulation(simulation);
         return simulation;
      }

      private static Simulation createModelLessSimulationWith(ISimulationSubject simulationSubject, Compound compound, Protocol protocol, string modelName)
      {
         return CreateModelLessSimulationWith(simulationSubject, compound, protocol, CreateModelPropertiesFor(simulationSubject, modelName));
      }

      public static ModelProperties CreateModelPropertiesFor(ISimulationSubject simulationSubject, string modelName)
      {
         var modelPropertiesTask = IoC.Resolve<IModelPropertiesTask>();
         return modelPropertiesTask.DefaultFor(simulationSubject.OriginData, modelName);
      }

      public static Simulation CreateSimulationWith(ISimulationSubject simulationSubject, Compound compound, Protocol protocol, bool allowAging = false)
      {
         var simulation = CreateModelLessSimulationWith(simulationSubject, compound, protocol, allowAging);
         AddModelToSimulation(simulation);
         return simulation;
      }

      public static void AddModelToSimulation(Simulation simulation)
      {
         var simModelConstructor = IoC.Resolve<ISimulationConstructor>();
         simModelConstructor.AddModelToSimulation(simulation);
      }

      public static Simulation CreateModelLessSimulationWith(ISimulationSubject simulationSubject, Compound compound, Protocol protocol, bool allowAging = false)
      {
         return CreateModelLessSimulationWith(simulationSubject, compound, protocol, CreateDefaultModelPropertiesFor(simulationSubject), allowAging);
      }

      public static Simulation CreateModelLessSimulationWith(ISimulationSubject simulationSubject, IReadOnlyList<Compound> compounds, IReadOnlyList<Protocol> protocols, bool allowAging = false)
      {
         return CreateModelLessSimulationWith(simulationSubject, compounds, protocols, CreateDefaultModelPropertiesFor(simulationSubject), allowAging);
      }

      public static ModelProperties CreateDefaultModelPropertiesFor(ISimulationSubject simulationSubject)
      {
         var modelPropertiesTask = IoC.Resolve<IModelPropertiesTask>();
         return modelPropertiesTask.DefaultFor(simulationSubject.OriginData);
      }

      public static Simulation CreateModelLessSimulationWith(ISimulationSubject simulationSubject, Compound compound, Protocol protocol, ModelProperties modelProperties, bool allowAging = false)
      {
         return CreateModelLessSimulationWith(simulationSubject, new[] {compound}, new[] {protocol}, modelProperties, allowAging);
      }

      public static Simulation CreateModelLessSimulationWith(
         ISimulationSubject simulationSubject, 
         IReadOnlyList<Compound> compounds, 
         IReadOnlyList<Protocol> protocols, 
         ModelProperties modelProperties, 
         bool allowAging = false)
      {
         var simModelConstructor = IoC.Resolve<ISimulationConstructor>();
         var simulationConstruction = new SimulationConstruction
         {
            SimulationSubject = simulationSubject,
            TemplateCompounds = compounds,
            TemplateProtocols = protocols,
            ModelProperties = modelProperties,
            AllowAging = allowAging,
            SimulationName = "simulation",
         };
         return simModelConstructor.CreateModelLessSimulationWith(simulationConstruction);
      }

      public static Population CreateDefaultPopulation(Individual individual)
      {
         var populationFactory = IoC.Resolve<IRandomPopulationFactory>();
         var populationSettings = IoC.Resolve<IIndividualToPopulationSettingsMapper>().MapFrom(individual);
         populationSettings.NumberOfIndividuals = 3;
         var population = populationFactory.CreateFor(populationSettings, new CancellationToken()).Result;
         return population.WithName("POP");
      }
   }
}