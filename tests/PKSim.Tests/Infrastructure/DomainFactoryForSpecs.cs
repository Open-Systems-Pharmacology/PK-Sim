using System;
using System.Collections.Generic;
using System.Threading;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Infrastructure
{
   public static class DomainFactoryForSpecs
   {
      public static Individual CreateStandardIndividual(string population = CoreConstants.Population.ICRP)
      {
         var defaultIndividualRetriever = IoC.Resolve<IDefaultIndividualRetriever>();
         var populationRepository = IoC.Resolve<IPopulationRepository>();
         var cloneManager = IoC.Resolve<ICloneManagerForBuildingBlock>();
         return cloneManager.Clone(defaultIndividualRetriever.DefaultIndividualFor(populationRepository.FindByName(population)), new FormulaCache()).WithName("Individual");
      }

      public static Compound CreateStandardCompound()
      {
         var objectCreator = IoC.Resolve<ISnapshotObjectCreator>();
         var compoundMapper = IoC.Resolve<CompoundMapper>();

         var compoundSnapshot = objectCreator.StandardCompound(lipophilicity: -2, fractionUnbound: 0.8, molWeight: 400, solubilityAtRefPh: 1e-7).Result;
         return compoundMapper.MapToModel(compoundSnapshot, new SnapshotContext()).Result;
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

      public static Protocol CreateStandardOralProtocol()
      {
         var protocolFactory = IoC.Resolve<IProtocolFactory>();
         return protocolFactory.Create(ProtocolMode.Simple, ApplicationTypes.Oral).WithName("Protocol");
      }

      public static Formulation CreateParticlesFormulation(int numberOfBins)
      {
         var formulationRepository = IoC.Resolve<IFormulationRepository>();
         var formulation = formulationRepository.FormulationBy(CoreConstants.Formulation.PARTICLES);
         formulation.Parameter(Constants.Parameters.NUMBER_OF_BINS).Value = numberOfBins;

         //set mono/polydisperse property (0=mono, 1=poly).
         formulation.Parameter(Constants.Parameters.PARTICLE_DISPERSE_SYSTEM).Value = (numberOfBins > 1) ? CoreConstants.Parameters.POLYDISPERSE : CoreConstants.Parameters.MONODISPERSE;

         return formulation;
      }

      public static ExpressionProfile CreateExpressionProfileAndAddToIndividual<TMolecule>(Individual individual, string moleculeName = "CYP3A4", Action<ExpressionProfile> updateAction = null) where TMolecule : IndividualMolecule
      {
         var moleculeExpressionTask = IoC.Resolve<IMoleculeExpressionTask<Individual>>();
         var expressionProfile = CreateExpressionProfile<TMolecule>(moleculeName);
         if (updateAction != null)
            updateAction(expressionProfile);
         moleculeExpressionTask.AddExpressionProfile(individual, expressionProfile);
         return expressionProfile;
      }

      public static ExpressionProfile CreateExpressionProfile<TMolecule>(string moleculeName = "CYP3A4") where TMolecule : IndividualMolecule
      {
         var expressionProfileFactory = IoC.Resolve<IExpressionProfileFactory>();
         var expressionProfile = expressionProfileFactory.Create<TMolecule>(moleculeName);
         expressionProfile.Category = "Standard";
         return expressionProfile;
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

      public static Simulation CreateSimulationWith(ISimulationSubject simulationSubject, Compound compound, Protocol protocol, string modelName, Formulation formulation = null)
      {
         var simulation = createModelLessSimulationWith(simulationSubject, compound, protocol, modelName, formulation);
         AddModelToSimulation(simulation);
         return simulation;
      }

      private static Simulation createModelLessSimulationWith(ISimulationSubject simulationSubject, Compound compound,
         Protocol protocol, string modelName,
         Formulation formulation = null)
      {
         return CreateModelLessSimulationWith(simulationSubject, compound, protocol, CreateModelPropertiesFor(simulationSubject, modelName), false, formulation);
      }

      public static ModelProperties CreateModelPropertiesFor(ISimulationSubject simulationSubject, string modelName)
      {
         var modelPropertiesTask = IoC.Resolve<IModelPropertiesTask>();
         return modelPropertiesTask.DefaultFor(simulationSubject.OriginData, modelName);
      }

      public static Simulation CreateSimulationWith(ISimulationSubject simulationSubject, Compound compound, Protocol protocol, bool allowAging = false, Formulation formulation = null)
      {
         var simulation = CreateModelLessSimulationWith(simulationSubject, compound, protocol, allowAging, formulation);
         AddModelToSimulation(simulation);
         return simulation;
      }

      public static Simulation CreateSimulationWith(ISimulationSubject simulationSubject, Compound compound,
         Protocol protocol, Formulation formulation, bool allowAging = false)
      {
         var simulation = CreateModelLessSimulationWith(simulationSubject, compound, protocol, allowAging, formulation);
         AddModelToSimulation(simulation);
         return simulation;
      }

      public static void AddModelToSimulation(Simulation simulation)
      {
         var simModelConstructor = IoC.Resolve<ISimulationConstructor>();
         simModelConstructor.AddModelToSimulation(simulation);
      }

      public static Simulation CreateModelLessSimulationWith(ISimulationSubject simulationSubject,
         Compound compound, Protocol protocol,
         bool allowAging = false, Formulation formulation = null)
      {
         return CreateModelLessSimulationWith(simulationSubject, compound, protocol, CreateDefaultModelPropertiesFor(simulationSubject), allowAging, formulation);
      }

      public static Simulation CreateModelLessSimulationWith(ISimulationSubject simulationSubject,
         IReadOnlyList<Compound> compounds,
         IReadOnlyList<Protocol> protocols,
         bool allowAging = false, Formulation formulation = null)
      {
         return CreateModelLessSimulationWith(simulationSubject, compounds, protocols, CreateDefaultModelPropertiesFor(simulationSubject), allowAging, formulation);
      }

      public static ModelProperties CreateDefaultModelPropertiesFor(ISimulationSubject simulationSubject)
      {
         var modelPropertiesTask = IoC.Resolve<IModelPropertiesTask>();
         return modelPropertiesTask.DefaultFor(simulationSubject.OriginData);
      }

      public static Simulation CreateModelLessSimulationWith(ISimulationSubject simulationSubject, Compound compound,
         Protocol protocol, ModelProperties modelProperties,
         bool allowAging = false, Formulation formulation = null)
      {
         return CreateModelLessSimulationWith(simulationSubject, new[] {compound}, new[] {protocol}, modelProperties, allowAging, formulation);
      }

      public static Simulation CreateModelLessSimulationWith(ISimulationSubject simulationSubject,
         IReadOnlyList<Compound> compounds,
         IReadOnlyList<Protocol> protocols,
         ModelProperties modelProperties, bool allowAging = false,
         Formulation formulation = null)
      {
         var simModelConstructor = IoC.Resolve<ISimulationConstructor>();
         var simulationConstruction = new SimulationConstruction
         {
            SimulationSubject = simulationSubject,
            TemplateCompounds = compounds,
            TemplateProtocols = protocols,
            ModelProperties = modelProperties,
            SimulationName = "simulation",
            TemplateFormulation = formulation,
            AllowAging = allowAging,
         };
         return simModelConstructor.CreateModelLessSimulationWith(simulationConstruction);
      }

      public static Population CreateDefaultPopulation(Individual individual)
      {
         var populationFactory = IoC.Resolve<IRandomPopulationFactory>();
         var populationSettings = IoC.Resolve<IIndividualToPopulationSettingsMapper>().MapFrom(individual);
         //Non Age dependent species. We have to make sure that we set correct weight so that the algorithm can randomize target weight
         if (!individual.IsAgeDependent)
         {
            var weightValue = individual.MeanWeight;
            var weightRange = populationSettings.ParameterRange(CoreConstants.Parameters.MEAN_WEIGHT);
            weightRange.MinValue = weightValue / 10;
            weightRange.MaxValue = weightValue * 10;
         }

         populationSettings.NumberOfIndividuals = 3;
         var population = populationFactory.CreateFor(populationSettings, new CancellationToken()).Result;
         return population.WithName("POP");
      }
   }
}