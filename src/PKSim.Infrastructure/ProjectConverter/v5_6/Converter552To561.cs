using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.Xml.Serializers;

namespace PKSim.Infrastructure.ProjectConverter.v5_6
{
   public class Converter552To561 : IObjectConverter,
      IVisitor<Simulation>,
      IVisitor<Compound>,
      IVisitor<Individual>,
      IVisitor<Population>,
      IVisitor<IndividualSimulation>
   {
      private readonly ICompoundCalculationMethodCategoryRepository _compoundCalculationMethodCategoryRepository;
      private readonly IIndividualFactory _individualFactory;
      private readonly ICloner _cloner;
      private readonly INeighborhoodFinalizer _neighborhoodFinalizer;
      private readonly IContainerTask _containerTask;
      private readonly IPKSimXmlSerializerRepository _serializerRepository;
      private readonly IReactionBuildingBlockCreator _reactionBuildingBlockCreator;
      private readonly IDiagramModelFactory _diagramModelFactory;
      private readonly ICompoundProcessRepository _compoundProcessRepository;
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private const string DUMMY_COMPOUND_NAME = "DUMMY_COMPOUND_NAME";

      //do not use the LiverZones constant defined in CoreConstant because it could create issue for future conversion
      //if we decide to add another zone
      public static readonly IEnumerable<string> LIVER_ZONES = new List<string>
      {
         CoreConstants.Compartment.Pericentral,
         CoreConstants.Compartment.Periportal
      };

      private readonly IDialogCreator _dialogCreator;
      private readonly IIndividualCalculationMethodsUpdater _individualCalculationMethodsUpdater;
      private readonly IIndividualUpdater _individualUpdater;
      private bool _converted;

      public Converter552To561(ICompoundCalculationMethodCategoryRepository compoundCalculationMethodCategoryRepository, IIndividualFactory individualFactory,
         ICloner cloner, INeighborhoodFinalizer neighborhoodFinalizer, IContainerTask containerTask,
         IPKSimXmlSerializerRepository serializerRepository, IReactionBuildingBlockCreator reactionBuildingBlockCreator,
         IDiagramModelFactory diagramModelFactory, ICompoundProcessRepository compoundProcessRepository, IParameterSetUpdater parameterSetUpdater, IDialogCreator dialogCreator,
         IIndividualCalculationMethodsUpdater individualCalculationMethodsUpdater,
         IIndividualUpdater individualUpdater)
      {
         _compoundCalculationMethodCategoryRepository = compoundCalculationMethodCategoryRepository;
         _individualFactory = individualFactory;
         _cloner = cloner;
         _neighborhoodFinalizer = neighborhoodFinalizer;
         _containerTask = containerTask;
         _serializerRepository = serializerRepository;
         _reactionBuildingBlockCreator = reactionBuildingBlockCreator;
         _diagramModelFactory = diagramModelFactory;
         _compoundProcessRepository = compoundProcessRepository;
         _parameterSetUpdater = parameterSetUpdater;
         _dialogCreator = dialogCreator;
         _individualCalculationMethodsUpdater = individualCalculationMethodsUpdater;
         _individualUpdater = individualUpdater;
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V5_5_2;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V5_6_1, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         element.DescendantsAndSelfNamed(CoreConstants.Serialization.WorkspaceLayout).Each(removeEmptyPresenterSettings);
         element.DescendantsAndSelfNamed("OriginData").Each(convertOriginData);
         element.DescendantsAndSelfNamed("ModelProperties").Each(convertModelProperties);
         element.DescendantsAndSelfNamed("Properties", "SimulationProperties").Each(convertSimulationProperties);
         element.DescendantsAndSelfNamed("IndividualSimulation").Each(convertIndividualSimulation);
         element.DescendantsAndSelfNamed("SummaryChart").Each(convertSummaryChart);

         return (ProjectVersions.V5_6_1, true);
      }

      private void convertSummaryChart(XElement element)
      {
         element.Name = "IndividualSimulationComparison";
      }

      private void removeEmptyPresenterSettings(XElement element)
      {
         foreach (var emptyPresenterSettingsNode in element.Descendants("EmptyPresenterSettings").ToList())
         {
            emptyPresenterSettingsNode.Remove();
         }
      }

      private void convertIndividualSimulation(XElement individualSimulationElement)
      {
         //this value is not always available
         var aucIV = individualSimulationElement.GetAttribute("aucIV");
         if (string.IsNullOrEmpty(aucIV)) return;

         var allCompoundPKElement = new XElement("AllCompoundPK");
         individualSimulationElement.Add(allCompoundPKElement);
         var aucIVValue = aucIV.ConvertedTo<double>();

         //Add existing value to cache with a dummy compound name that will be replaced with the real compound name once the object 
         //are loaded
         var compoundPK = new CompoundPK {CompoundName = DUMMY_COMPOUND_NAME, AucIV = aucIVValue};
         var compoundPKSerializer = _serializerRepository.SerializerFor(compoundPK);
         var compoundPKElement = compoundPKSerializer.Serialize(compoundPK, SerializationTransaction.Create());
         allCompoundPKElement.Add(compoundPKElement);
      }

      private void convertSimulationProperties(XElement simulationPropertiesElement)
      {
         var compoundPropertiesElement = simulationPropertiesElement.Element("CompoundProperties");
         if (compoundPropertiesElement == null) return;

         var processesElement = new XElement("Processes");
         var compoundPropertiesListElement = new XElement("CompoundPropertiesList");

         //compound properties
         compoundPropertiesElement.Remove();
         compoundPropertiesListElement.Add(compoundPropertiesElement);
         simulationPropertiesElement.Add(compoundPropertiesListElement);

         //processes
         moveProcessSelectionElement(compoundPropertiesElement, processesElement, "MetabolizationSelection");
         moveProcessSelectionElement(compoundPropertiesElement, processesElement, "TransportAndExcretionSelection");
         moveProcessSelectionElement(compoundPropertiesElement, processesElement, "SpecificBindingSelection");
         compoundPropertiesElement.Add(processesElement);

         //protocol properties
         var protocolProperties = simulationPropertiesElement.Element("ProtocolProperties");
         if (protocolProperties == null) return;
         protocolProperties.Remove();
         compoundPropertiesElement.Add(protocolProperties);
      }

      private void moveProcessSelectionElement(XElement compoundPropertiesElement, XElement processesElement, string processSelectionName)
      {
         var processSelectionElement = compoundPropertiesElement.Element(processSelectionName);
         if (processSelectionElement == null) return;
         processSelectionElement.Remove();
         processesElement.Add(processSelectionElement);
      }

      private void convertModelProperties(XElement modelPropertiesElement)
      {
         convertCalculationMethodsIn(modelPropertiesElement, "AllCalculationMethods");
      }

      private void convertOriginData(XElement originDataElement)
      {
         convertCalculationMethodsIn(originDataElement, "CalculationMethods");
      }

      private static void convertCalculationMethodsIn(XElement originDataElement, string calculationMethodsName)
      {
         var calculationMethodsElement = originDataElement.Element(calculationMethodsName);
         if (calculationMethodsElement == null) return;

         var allCalculationMethodsCacheElement = new XElement("All");
         allCalculationMethodsCacheElement.Add(calculationMethodsElement.Descendants("CalculationMethod"));
         calculationMethodsElement.Remove();

         originDataElement.Add(new XElement("CalculationMethodCache", allCalculationMethodsCacheElement));
      }

      private void convertSimulation(Simulation simulation)
      {
         updateReferencesToBuildingBlocks(simulation);
         moveMoleculeCalculationMethodsFromModelConfigurationToCompoundProperties(simulation);
         removeOldStaticInhibitionReferencesFromProcessMapping(simulation, simulation.Compounds.FirstOrDefault());
         convertCompound(simulation.Compounds.FirstOrDefault());
         convertIndividual(simulation.Individual);
         updateFractionUnboundGroup(simulation);
         createReactionBuildingBlockAndDiagramFor(simulation);
      }

      private void removeOldStaticInhibitionReferencesFromProcessMapping(Simulation simulation, Compound compound)
      {
         if (compound == null) return;
         var compoundProperties = simulation.CompoundPropertiesList.FirstOrDefault();
         if (compoundProperties == null) return;

         var oldMappedInhibitionProcessList = new List<string>();
         compoundProperties.Processes.AllProcessSelectionGroups.Each(x => removeOldStaticInhibitionReferences(x, compound, oldMappedInhibitionProcessList));

         if (oldMappedInhibitionProcessList.Any())
            _dialogCreator.MessageBoxInfo(PKSimConstants.Warning.StaticInhibitionRemovedFromSimulationMapping(oldMappedInhibitionProcessList));
      }

      private void removeOldStaticInhibitionReferences(ProcessSelectionGroup processSelectionGroup, Compound compound, List<string> oldMappedInhibitionProcessList)
      {
         var allInhibitionProcesses = compound.AllProcesses<PartialProcess>().Where(x => x.InternalName.Contains("Inhibition"));
         foreach (var inhibitionProcess in allInhibitionProcesses)
         {
            var inhibitionSelection = processSelectionGroup.AllPartialProcesses()
               .FirstOrDefault(x => string.Equals(x.ProcessName, inhibitionProcess.Name));

            if (inhibitionSelection == null)
               continue;

            processSelectionGroup.RemovePartialProcessSelection(inhibitionSelection);
            oldMappedInhibitionProcessList.Add(inhibitionProcess.Name);
         }
      }

      private void createReactionBuildingBlockAndDiagramFor(Simulation simulation)
      {
         simulation.ReactionDiagramModel = _diagramModelFactory.Create();
         simulation.Reactions = _reactionBuildingBlockCreator.CreateFor(simulation);
      }

      private void updateFractionUnboundGroup(Simulation simulation)
      {
         var compoundName = simulation.CompoundNames.FirstOrDefault();
         if (string.IsNullOrEmpty(compoundName)) return;
         var fractionUnbound = simulation.Model.Root.EntityAt<IParameter>(compoundName, ConverterConstants.Parameter.FractionUnboundPlasma);
         fractionUnbound.GroupName = CoreConstants.Groups.FRACTION_UNBOUND_PLASMA;
      }

      private void moveMoleculeCalculationMethodsFromModelConfigurationToCompoundProperties(Simulation simulation)
      {
         if (simulation.IsImported)
            return;

         var modelProperties = simulation.ModelProperties;
         var compoundProperties = simulation.CompoundPropertiesList.First();

         moveMoleculeCalculationMethod(modelProperties, compoundProperties, CoreConstants.Category.DistributionCellular);
         moveMoleculeCalculationMethod(modelProperties, compoundProperties, CoreConstants.Category.DiffusionIntCell);
         moveMoleculeCalculationMethod(modelProperties, compoundProperties, CoreConstants.Category.DistributionInterstitial);
         moveMoleculeCalculationMethod(modelProperties, compoundProperties, CoreConstants.Category.IntestinalPermeability);
      }

      private void moveMoleculeCalculationMethod(ModelProperties modelProperties, CompoundProperties compoundProperties, string category)
      {
         var cm = modelProperties.CalculationMethodFor(category);
         if (cm == null) return;
         modelProperties.RemoveCalculationMethod(cm);
         compoundProperties.AddCalculationMethod(cm);
      }

      private void updateReferencesToBuildingBlocks(Simulation simulation)
      {
         //only one compound and one compound properties. Just set the reference
         var compound = simulation.Compounds.FirstOrDefault();
         var protocol = simulation.AllBuildingBlocks<Protocol>().FirstOrDefault();

         //possible for imported simulations
         if (compound == null)
         {
            simulation.Properties.ClearCompoundPropertiesList();
            return;
         }

         var compoundProperties = simulation.CompoundPropertiesList.FirstOrDefault();
         if (compoundProperties == null)
            return;

         compoundProperties.Compound = compound;
         compoundProperties.ProtocolProperties.Protocol = protocol;

         compoundProperties.Processes.AllProcessSelectionGroups.Each(x => updateCompoundNameInProcess(x, compound));
      }

      private void updateCompoundNameInProcess(ProcessSelectionGroup processSelectionGroup, Compound compound)
      {
         processSelectionGroup.AllPartialProcesses().Each(p => p.CompoundName = compound.Name);
         processSelectionGroup.AllSystemicProcesses().Each(p => p.CompoundName = compound.Name);
      }

      public void Visit(IndividualSimulation individualSimulation)
      {
         Visit((Simulation) individualSimulation);
         convertAucIVCache(individualSimulation);
         _converted = true;
      }

      private void convertAucIVCache(IndividualSimulation individualSimulation)
      {
         var value = individualSimulation.AucIVFor(DUMMY_COMPOUND_NAME);
         if (!value.HasValue)
            return;

         //use the real compoud name and set the value loaded previously
         var compoundName = individualSimulation.CompoundNames.First();
         individualSimulation.ClearPKCache();
         individualSimulation.AddCompoundPK(new CompoundPK {CompoundName = compoundName, AucIV = value});
      }

      public void Visit(Simulation simulation)
      {
         convertSimulation(simulation);
         _converted = true;
      }

      public void Visit(Compound compound)
      {
         convertCompound(compound);
         _converted = true;
      }

      private void convertCompound(Compound compound)
      {
         if (compound == null) return;
         addDefaultCalculationMethodsTo(compound);
         updatePartialProcessKineticAccordingToTemplate(compound);
      }

      private void updatePartialProcessKineticAccordingToTemplate(Compound compound)
      {
         foreach (var partialProcess in compound.AllProcesses<PartialProcess>().ToList())
         {
            if (partialProcess.InternalName.Contains("Hill"))
               continue;

            //remove the old partial process that needs to be updated
            compound.RemoveChild(partialProcess);

            var templateProcess = _compoundProcessRepository.All<PartialProcess>().FindByName(partialProcess.InternalName);

            bool wasInhibitionProcess = false;
            //process does not exist anymore such as Inhibition. convert to non inhibition
            if (templateProcess == null)
            {
               var processInternalName = partialProcess.InternalName.Replace("CompetitiveInhibition_", string.Empty);
               templateProcess = _compoundProcessRepository.All<PartialProcess>().FindByName(processInternalName);
               wasInhibitionProcess = true;
            }

            if (templateProcess == null)
               continue;

            //This updates all local properites from the parital process such as molecule name etc..
            var cloneDbProcess = _cloner.Clone(templateProcess);
            cloneDbProcess.UpdatePropertiesFrom(partialProcess, _cloner);
            compound.Add(cloneDbProcess);

            if (wasInhibitionProcess)
            {
               cloneDbProcess.Description = PKSimConstants.Warning.StaticInhibitionRemovedFromApplication(templateProcess.Description);
               cloneDbProcess.InternalName = templateProcess.InternalName;
            }

            foreach (var parameter in partialProcess.AllParameters())
            {
               var newParameter = cloneDbProcess.Parameter(parameter.Name);
               if (newParameter == null)
                  continue;

               //make sure we have the same parameter id as before to ensure smooth update commit
               newParameter.Id = parameter.Id;
               _parameterSetUpdater.UpdateValue(parameter, newParameter);
               //this needs to be done after udpdate value as the Update value also sets the origin 
               newParameter.Origin.ParameterId = parameter.Origin.ParameterId;
            }
         }
      }

      private void addDefaultCalculationMethodsTo(Compound compound)
      {
         _compoundCalculationMethodCategoryRepository.All().Each(c => compound.AddCalculationMethod(c.DefaultItem));
      }

      private void convertIndividual(Individual individual)
      {
         //possible for imported simulation where individual is not defined
         if (individual == null) return;

         _individualCalculationMethodsUpdater.AddMissingCalculationMethodsTo(individual);
         _individualUpdater.AddScalingExponentForFluidFlowTo(individual);

         updateLiverStructure(individual);
         updateIsLiverZonatedValue(individual);
         updateLiverExpressions(individual);
         updateOntogenyFactorVisibility(individual);
         removeVolumePlasmaParameter(individual.Organism);
         _neighborhoodFinalizer.SetNeighborsIn(individual);
      }

      private void removeVolumePlasmaParameter(IContainer container)
      {
         var parameter = container.Parameter(CoreConstants.Parameters.VOLUME_PLASMA);
         if (parameter == null) return;

         container.RemoveChild(parameter);
      }

      private void updateOntogenyFactorVisibility(Individual individual)
      {
         foreach (var plasmaOntogenyFactoryName in CoreConstants.Parameters.AllPlasmaProteinOntogenyFactors)
         {
            individual.Organism.Parameter(plasmaOntogenyFactoryName).Visible = true;
         }

         foreach (var molecule in individual.AllDefinedMolecules())
         {
            updateOntogenyFactorParameter(molecule.OntogenyFactorParameter);
            updateOntogenyFactorParameter(molecule.OntogenyFactorGIParameter);
         }
      }

      private void updateOntogenyFactorParameter(IParameter ontogenyFactorParameter)
      {
         ontogenyFactorParameter.Visible = true;
         ontogenyFactorParameter.GroupName = CoreConstants.Groups.ONTOGENY_FACTOR;
      }

      private void updateLiverExpressions(Individual individual)
      {
         individual.AllMolecules().Each(updateLiverMoleculeExpression);
      }

      private void updateLiverMoleculeExpression(IndividualMolecule molecule)
      {
         var liverContainer = molecule.ExpressionContainer(CoreConstants.Organ.Liver);
         if (liverContainer == null) return;

         LIVER_ZONES.Each(z =>
         {
            var zoneContainer = _cloner.Clone(liverContainer).WithName(z);
            zoneContainer.OrganPath.Add(z);
            zoneContainer.ContainerName = z;
            molecule.Add(zoneContainer);
         });

         molecule.RemoveChild(liverContainer);
      }

      private void updateIsLiverZonatedValue(Individual individual)
      {
         var liver = liverIn(individual);
         liver.Parameter(CoreConstants.Parameters.IS_LIVER_ZONATED).Value = 0;
      }

      private void updateLiverStructure(Individual individual)
      {
         var templateIndividual = _individualFactory.CreateAndOptimizeFor(individual.OriginData);
         var originalLiver = liverIn(individual);
         var newLiver = _cloner.Clone(liverIn(templateIndividual));

         individual.Organism.RemoveChild(originalLiver);
         individual.Organism.Add(newLiver);

         updateContainerParameterValues(originalLiver, newLiver);

         addMissingNeighborhoods(individual, templateIndividual);
         removeOldNeighbhorhoods(individual);
      }

      private void addMissingNeighborhoods(Individual individual, Individual templateIndividual)
      {
         foreach (var templateNeighborhood in templateIndividual.Neighborhoods.Children)
         {
            if (individual.Neighborhoods.ContainsName(templateNeighborhood.Name))
               continue;

            individual.Neighborhoods.Add(_cloner.Clone(templateNeighborhood));
         }
      }

      private void removeOldNeighbhorhoods(Individual individual)
      {
         var oldNeighborhoods = new[]
         {
            "ArterialBlood_bc_Liver_bc",
            "ArterialBlood_pls_Liver_pls",
            "Liver_bc_VenousBlood_bc",
            "Liver_end_Liver_ecl",
            "Liver_int_VenousBlood_pls",
            "Liver_cell_Gallbladder",
            "Liver_pls_VenousBlood_pls",
            "PortalVein_bc_Liver_bc",
            "PortalVein_pls_Liver_pls",
            "Liver_cell_Lumen_duo"
         };


         foreach (var neighborHoodName in oldNeighborhoods)
         {
            var neighborHood = individual.Neighborhoods.GetSingleChildByName<INeighborhood>(neighborHoodName);
            if (neighborHood == null)
               continue;

            individual.Neighborhoods.RemoveChild(neighborHood);
         }
      }

      private void updateContainerParameterValues(IContainer originalContainer, IContainer newContainer)
      {
         var allOriginalParameters = _containerTask.CacheAllChildren<IParameter>(originalContainer);
         var allNewParameters = _containerTask.CacheAllChildren<IParameter>(newContainer);

         foreach (var parameterKeyValue in allOriginalParameters.KeyValues)
         {
            var newParameter = allNewParameters[parameterKeyValue.Key];
            if (newParameter == null)
               continue;

            var originalParameter = parameterKeyValue.Value;

            if (originalParameter.IsNamed(Constants.Distribution.PERCENTILE))
            {
               newParameter.Value = originalParameter.Value;
               continue;
            }

            if (!originalParameter.ValueDiffersFromDefault())
               continue;

            newParameter.Value = originalParameter.Value;
         }
      }

      private IContainer liverIn(Individual individual)
      {
         return individual.Organism.Container(CoreConstants.Organ.Liver);
      }

      public void Visit(Individual individual)
      {
         convertIndividual(individual);
         _converted = true;
      }

      public void Visit(Population population)
      {
         convertIndividual(population.FirstIndividual);
         _converted = true;
      }
   }
}