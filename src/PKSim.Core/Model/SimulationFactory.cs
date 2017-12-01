using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Services;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Exceptions;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Creates a simulation based on properties given as parameter
   /// </summary>
   public interface ISimulationFactory : ICoreSimulationFactory
   {
      /// <summary>
      ///    Creates a new simulation using the given <paramref name="simulationSubject" /> and <paramref name="compounds" /> as
      ///    building block as well as the <paramref name="modelProperties" />.
      ///    The created simulation does not have a model yet. Only basic building blocks are set.
      ///    If the <paramref name="originalSimulation" /> is not null, all other building blocks such as
      ///    <see cref="Formulation" /> ,  <see cref="Protocol" /> or <see cref="Event" /> will be used as well as their existing
      ///    configuration in the simulation. 
      /// </summary>
      /// <remarks>References defined in <paramref name="originalSimulation"/> will be used in the newly created simulation. A clone of any existing simulation should be used</remarks>
      Simulation CreateFrom(ISimulationSubject simulationSubject, IReadOnlyList<Compound> compounds, ModelProperties modelProperties, Simulation originalSimulation = null);

      /// <summary>
      ///    Creates a simulation using the given <paramref name="modelCoreSimulation" /> as model.
      /// </summary>
      /// <typeparam name="TSimulation">Type of simulation to be created</typeparam>
      /// <param name="modelCoreSimulation">Model to use</param>
      TSimulation CreateBasedOn<TSimulation>(IModelCoreSimulation modelCoreSimulation) where TSimulation : Simulation;

      /// <summary>
      ///    Creates a full simulation (including model) where the existing protocol for the <paramref name="compound" /> is
      ///    replaced with the given <paramref name="ivProtocol" />
      /// </summary>
      /// <param name="ivProtocol">IV Protocol for the <paramref name="compound" /></param>
      /// <param name="compound">Compound for which bioavailability will be determined</param>
      /// <param name="originalSimulation">Original simulation that will be used to create the new simulation</param>
      IndividualSimulation CreateForBioAvailability(Protocol ivProtocol, Compound compound, IndividualSimulation originalSimulation);

      /// <summary>
      ///    Creates a full simulation (including model) where the all inhibition processes are turned off
      /// </summary>
      /// <param name="originalSimulation">Original simulation that will be used to create the new simulation</param>
      IndividualSimulation CreateForDDIRatio(IndividualSimulation originalSimulation);

      /// <summary>
      ///    Creates a full simulation (including model) where the protocal for the <paramref name="compound" /> is set to
      ///    <paramref name="protocol" />
      /// </summary>
      Simulation CreateForVSS(Protocol protocol, Individual individual, Compound compound);
   }

   public class SimulationFactory : ISimulationFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      private readonly ISimulationModelCreator _simulationModelCreator;
      private readonly IObjectIdResetter _objectIdResetter;
      private readonly ICompoundPropertiesUpdater _compoundPropertiesUpdater;
      private readonly ISimulationParametersUpdater _simulationParametersUpdater;
      private readonly IModelPropertiesTask _modelPropertiesTask;
      private readonly ICloner _cloner;
      private readonly IDiagramModelFactory _diagramModelFactory;
      private readonly IInteractionTask _interactionTask;

      public SimulationFactory(IObjectBaseFactory objectBaseFactory, 
         ISimulationBuildingBlockUpdater simulationBuildingBlockUpdater, ISimulationModelCreator simulationModelCreator,
         IObjectIdResetter objectIdResetter, ICompoundPropertiesUpdater compoundPropertiesUpdater,
         ISimulationParametersUpdater simulationParametersUpdater, IModelPropertiesTask modelPropertiesTask,
         ICloner cloner, IDiagramModelFactory diagramModelFactory, IInteractionTask interactionTask)
      {
         _objectBaseFactory = objectBaseFactory;
         _simulationBuildingBlockUpdater = simulationBuildingBlockUpdater;
         _simulationModelCreator = simulationModelCreator;
         _objectIdResetter = objectIdResetter;
         _compoundPropertiesUpdater = compoundPropertiesUpdater;
         _simulationParametersUpdater = simulationParametersUpdater;
         _modelPropertiesTask = modelPropertiesTask;
         _cloner = cloner;
         _diagramModelFactory = diagramModelFactory;
         _interactionTask = interactionTask;
      }

      private TSimulation create<TSimulation>() where TSimulation : Simulation
      {
         var simulation = _objectBaseFactory.Create<TSimulation>();
         simulation.Properties = new SimulationProperties();
         simulation.ReactionDiagramModel = _diagramModelFactory.Create();
         simulation.IsLoaded = true;
         return simulation;
      }

      public Simulation CreateFrom(ISimulationSubject simulationSubject, IReadOnlyList<Compound> compounds, ModelProperties modelProperties, Simulation originalSimulation = null)
      {
         var simulation = createSimulation(simulationSubject.GetType());

         //update the used building block in the simulation 
         originalSimulation?.UsedBuildingBlocks.Each(simulation.AddUsedBuildingBlock);

         _simulationBuildingBlockUpdater.UpdateUsedBuildingBlockInSimulationFromTemplate(simulation, simulationSubject, PKSimBuildingBlockType.SimulationSubject);
         _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(simulation, compounds, PKSimBuildingBlockType.Compound);

         //set basic properties
         if (originalSimulation != null)
         {
            simulation.UpdateFromOriginalSimulation(originalSimulation);
            simulation.Properties = originalSimulation.Properties;
         }

         //last but not least, update model properties to match new definition (this should be done last as UpdateFromOriginalSimulation resets the model properties)
         simulation.ModelProperties = modelProperties;
         updateCompoundProperties(simulation);

         return simulation;
      }

      public ISimulation CreateWithCalculationMethodsFrom(ISimulation templateSimulation, IEnumerable<CalculationMethodWithCompoundName> combination)
      {
         var simulation = templateSimulation.DowncastTo<IndividualSimulation>();
         var newSimulation = createModelLessSimulationBasedOn(simulation);

         combination.GroupBy(x => x.CompoundName).Each(grouping =>
         {
            replaceCalculationMethodsWithNewCalcualtionMethods(grouping, newSimulation.CompoundPropertiesFor(grouping.Key));
         });

         _simulationModelCreator.CreateModelFor(newSimulation);

         return newSimulation;
      }

      private void replaceCalculationMethodsWithNewCalcualtionMethods(IEnumerable<CalculationMethodWithCompoundName> combination, IWithCalculationMethods compoundProperties)
      {
         if (compoundProperties == null)
            return;

         combination.Each(method =>
         {
            compoundProperties.RemoveCalculationMethodFor(method.CalculationMethod.Category);
            compoundProperties.AddCalculationMethod(method.CalculationMethod);
         });
      }

      private void updateCompoundProperties(Simulation simulation) => _compoundPropertiesUpdater.UpdateCompoundPropertiesIn(simulation);

      public TSimulation CreateBasedOn<TSimulation>(IModelCoreSimulation modelCoreSimulation) where TSimulation : Simulation
      {
         var simulation = createSimulation<TSimulation>();
         simulation.Name = modelCoreSimulation.Name;
         simulation.Model = modelCoreSimulation.Model;
         simulation.Reactions = modelCoreSimulation.BuildConfiguration.Reactions;
         simulation.SimulationSettings= modelCoreSimulation.BuildConfiguration.SimulationSettings;
         simulation.Origin = assessOriginFrom(simulation.Model);
         _objectIdResetter.ResetIdFor(simulation);
         return simulation;
      }

      private Origin assessOriginFrom(IModel model)
      {
         var organism = model.Root.Container(Constants.ORGANISM);
         if (organism == null)
            return Origins.MoBi;

         var allChildrenNames = organism.AllChildrenNames();
         return CoreConstants.Organ.StandardOrgans.All(allChildrenNames.Contains) ? Origins.PKSim : Origins.MoBi;
      }

      private TSimulation createSimulation<TSimulation>() where TSimulation : Simulation
      {
         return createSimulation(typeof (TSimulation)).DowncastTo<TSimulation>();
      }

      private Simulation createSimulation(Type type)
      {
         if (type.IsAnImplementationOf<Individual>() || type.IsAnImplementationOf<IndividualSimulation>())
            return create<IndividualSimulation>();

         if (type.IsAnImplementationOf<Population>() || type.IsAnImplementationOf<PopulationSimulation>())
            return createPopulationSimulation();

         throw new ArgumentOutOfRangeException();
      }

      private PopulationSimulation createPopulationSimulation()
      {
         var simulation = create<PopulationSimulation>();
         simulation.SetAdvancedParameters(_objectBaseFactory.Create<AdvancedParameterCollection>());
         return simulation;
      }

      public Simulation CreateForVSS(Protocol protocol, Individual individual, Compound compound)
      {
         //we create a clone here to ensure that a name is set in the compound
         var vssCompound = _cloner.Clone(compound).WithName("VSS COMPOUND");
         var simulation = CreateFrom(individual, new[] {vssCompound}, _modelPropertiesTask.DefaultFor(individual.OriginData));
         _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(simulation, new[] {protocol}, PKSimBuildingBlockType.Protocol);
         _simulationModelCreator.CreateModelFor(simulation);
         return simulation;
      }

      public IndividualSimulation CreateForDDIRatio(IndividualSimulation originalSimulation)
      {
         //DDI Ratio=> manipulate inhibition parameters so that all DDI effects are effectively deactivated 
         var ddiRatioSimulation = createModelLessSimulationBasedOn(originalSimulation);

         _simulationModelCreator.CreateModelFor(ddiRatioSimulation);

         //now update all parameters from the orginal simulation
         _simulationParametersUpdater.ReconciliateSimulationParametersBetween(originalSimulation, ddiRatioSimulation);

         _interactionTask.AllInteractionContainers(ddiRatioSimulation)
            .SelectMany(c => c.AllParameters())
            .Each(p => p.Value = disabledDDIValueFor(p.Name));

         return ddiRatioSimulation;
      }

      private double disabledDDIValueFor(string parameterName)
      {
         if (parameterName.IsOneOf(CoreConstants.Parameter.EC50, CoreConstants.Parameter.K_KINACT_HALF))
            return 1;

         if (parameterName.IsOneOf(CoreConstants.Parameter.KINACT, CoreConstants.Parameter.EMAX))
            return 0;

         if (parameterName.IsOneOf(CoreConstants.Parameter.KI, CoreConstants.Parameter.KI_U,
                                   CoreConstants.Parameter.KI_C))
            return double.PositiveInfinity;

         //if we add any new parameters, the exception will be thrown per default, until we explicitely define
         //how to handle this parameter for "disabled DDI"-simulation
         throw new OSPSuiteException(PKSimConstants.Error.CannotCalculateDDIRatioFor(parameterName));
      }

      public IndividualSimulation CreateForBioAvailability(Protocol ivProtocol, Compound compound, IndividualSimulation originalSimulation)
      {
         return createForPKCalculation(originalSimulation, compound, (pkSimulation, pkCompound, allProtocols) =>
         {
            var protocolProperties = pkSimulation.CompoundPropertiesFor(pkCompound).ProtocolProperties;
            //remove old protocol used by compound and replace with new one
            allProtocols.Remove(protocolProperties.Protocol);
            allProtocols.Add(ivProtocol);
            protocolProperties.Protocol = ivProtocol;
         });
      }

      /// <summary>
      ///    Creates a new simulation using a clone of all buidling blocks defined in the given
      ///    <paramref name="originalSimulation" />.
      ///    The resulting simulation should only be used for addhoc calculations and be discared after use
      /// </summary>
      private IndividualSimulation createModelLessSimulationBasedOn(IndividualSimulation originalSimulation)
      {
         var clonedSimulation = _cloner.Clone(originalSimulation);

         return CreateFrom(clonedSimulation.Individual, clonedSimulation.Compounds, clonedSimulation.ModelProperties, clonedSimulation)
            .DowncastTo<IndividualSimulation>();
      }

      private IndividualSimulation createForPKCalculation(IndividualSimulation originalSimulation, Compound compound, Action<Simulation, Compound, List<Protocol>> updateUsedProtocolsForSimulation)
      {
         var pkSimulation = createModelLessSimulationBasedOn(originalSimulation);

         //a new template was created. We need to retrieve by name
         var pkCompound = pkSimulation.Compounds.FindByName(compound.Name);

         //cache all protocols currently used by simulation
         var allProtocols = pkSimulation.AllBuildingBlocks<Protocol>().ToList();

         //Remove them from the simulations
         pkSimulation.RemoveAllBuildingBlockOfType(PKSimBuildingBlockType.Protocol);

         //updates the list according to the action given
         updateUsedProtocolsForSimulation(pkSimulation, pkCompound, allProtocols);

         _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(pkSimulation, allProtocols, PKSimBuildingBlockType.Protocol);
         updateCompoundProperties(pkSimulation);

         _simulationModelCreator.CreateModelFor(pkSimulation);

         //now update all parameters from the orginal simulation
         _simulationParametersUpdater.ReconciliateSimulationParametersBetween(originalSimulation, pkSimulation);
         return pkSimulation;
      }
   }
}