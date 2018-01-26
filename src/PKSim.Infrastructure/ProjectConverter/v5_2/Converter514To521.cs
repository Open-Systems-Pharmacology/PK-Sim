using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using ISimulationPersistableUpdater = PKSim.Core.Services.ISimulationPersistableUpdater;

namespace PKSim.Infrastructure.ProjectConverter.v5_2
{
   public class Converter514To521 : IObjectConverter,
      IVisitor<RandomPopulation>,
      IVisitor<PopulationSimulation>,
      IVisitor<Individual>,
      IVisitor<IndividualSimulation>,
      IVisitor<Compound>,
      IVisitor<AdvancedProtocol>,
      IVisitor<SimpleProtocol>,
      IVisitor<PKSimEvent>,
      IVisitor<Formulation>,
      IVisitor<DataRepository>,
      IVisitor<PKSimProject>,
      IVisitor<OriginData>
   {
      private readonly IParameterValuesCacheConverter _parameterValuesCacheConverter;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly ICompoundConverter _compoundConverter;
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly ICloner _cloner;
      private readonly IFormulaAndDimensionConverter _formulaAndDimensionConverter;
      private readonly ICalculationMethodRepository _calculationMethodRepository;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly ISimulationPersistableUpdater _simulationPersistableUpdater;
      private readonly IIndividualCalculationMethodsUpdater _individualCalculationMethodsUpdater;

      public Converter514To521(IParameterValuesCacheConverter parameterValuesCacheConverter, IEntityPathResolver entityPathResolver, ICompoundConverter compoundConverter,
         IDefaultIndividualRetriever defaultIndividualRetriever, ICloner cloner, IFormulaAndDimensionConverter formulaAndDimensionConverter,
         ICalculationMethodRepository calculationMethodRepository, IDimensionRepository dimensionRepository, ISimulationPersistableUpdater simulationPersistableUpdater,
         IIndividualCalculationMethodsUpdater individualCalculationMethodsUpdater)
      {
         _parameterValuesCacheConverter = parameterValuesCacheConverter;
         _entityPathResolver = entityPathResolver;
         _compoundConverter = compoundConverter;
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _cloner = cloner;
         _formulaAndDimensionConverter = formulaAndDimensionConverter;
         _calculationMethodRepository = calculationMethodRepository;
         _dimensionRepository = dimensionRepository;
         _simulationPersistableUpdater = simulationPersistableUpdater;
         _individualCalculationMethodsUpdater = individualCalculationMethodsUpdater;
      }

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V5_1_4;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         this.Visit(objectToConvert);
         //Almost each object is converted with this conversion step => true
         return (ProjectVersions.V5_2_1, true);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         //Always convert dimensions
         _formulaAndDimensionConverter.ConvertDimensionIn(element);

         if (element.Name == "PopulationSimulation")
         {
            convertPopulationSettings(element);
            convertParameterCache(element);
         }
         else if (element.Name == "RandomPopulation")
         {
            convertParameterCache(element);
         }
         else if (element.Name == CoreConstants.Serialization.SummaryChart)
         {
            //this is not taken care of by default in our generic logic
            convertAxisDimension(element);
         }

         return (ProjectVersions.V5_2_1, true);
      }

      private void convertAxisDimension(XElement element)
      {
         //retrieve all elements with an attribute dimension
         var allDimensionAttributes = from child in element.DescendantsAndSelf()
            where child.HasAttributes
            let attr = child.Attribute(Constants.Serialization.Attribute.Dimension) ?? child.Attribute("dimension")
            where attr != null
            select attr;

         foreach (var dimensionAttribute in allDimensionAttributes)
         {
            if (string.Equals(dimensionAttribute.Value, "Concentration"))
               dimensionAttribute.SetValue(CoreConstants.Dimension.MASS_CONCENTRATION);
         }
      }

      private void convertParameterCache(XElement element)
      {
         _parameterValuesCacheConverter.Convert(element);
      }

      private void convertPopulationSettings(XElement element)
      {
         //here we need to convert the nodes for a population simulation
         var allSelectedQuantities = element.Descendants("SelectedQuantities").FirstOrDefault();
         if (allSelectedQuantities == null)
            return;

         var populationSettings = new XElement("PopulationSimulationSettings");
         populationSettings.AddAttribute("dynamicName", "Settings");
         var newSelectedQuantities = new XElement("SelectedQuantities");
         populationSettings.Add(newSelectedQuantities);

         foreach (var moleculeSelection in allSelectedQuantities.Descendants("MoleculeSelection").ToList())
         {
            var quantitySelection = new XElement("QuantitySelection");
            quantitySelection.AddAttribute(CoreConstants.Serialization.Attribute.Name, moleculeSelection.GetAttribute(CoreConstants.Serialization.Attribute.Name));
            quantitySelection.AddAttribute(CoreConstants.Serialization.Attribute.Path, moleculeSelection.GetAttribute(CoreConstants.Serialization.Attribute.Path));

            newSelectedQuantities.Add(quantitySelection);
         }
         element.Add(populationSettings);
      }

      public void Visit(RandomPopulation randomPopulation)
      {
         Visit(randomPopulation.FirstIndividual);
         performCommonConversion(randomPopulation);
         _parameterValuesCacheConverter.Convert(randomPopulation);
      }

      public void Visit(PopulationSimulation populationSimulation)
      {
         convertSimulation(populationSimulation);
         _parameterValuesCacheConverter.Convert(populationSimulation);
      }

      public void Visit(IndividualSimulation simulation)
      {
         convertSimulation(simulation);
         convertIndividualSimulationSettings(simulation);
      }

      private void convertSimulation(Simulation simulation)
      {
         foreach (var buildingBlock in simulation.AllBuildingBlocks<IPKSimBuildingBlock>())
         {
            this.Visit(buildingBlock);
         }
         performCommonConversion(simulation);
         updateOutputSchema(simulation);
         updateContainerType(simulation);
         updateModelConfiguration(simulation);
         //special parameter that need conversion
         convertSimulationParameters(simulation);
         convertIndividualParameters(simulation.Model.Root.Container(Constants.ORGANISM));
      }

      private void updateContainerType(Simulation simulation)
      {
         var compound = simulation.BuildingBlock<Compound>();
         var allContainers = simulation.Model.Root.GetAllChildren<IContainer>(x => x.IsNamed(compound.Name));
         foreach (var container in allContainers)
         {
            container.ContainerType = ContainerType.Molecule;
         }
      }

      private void convertIndividualParameters(IContainer organism)
      {
         organism.Parameter(ConverterConstants.Parameter.A_to_V_bc).Value *= 10;
         organism.Parameter(ConverterConstants.Parameter.A_to_V_bc).Dimension = _dimensionRepository.DimensionByName(CoreConstants.Dimension.InversedLength);

         organism.Parameter(ConverterConstants.Parameter.PARAM_k_SA).Value /= 100;
         organism.Parameter(ConverterConstants.Parameter.PARAM_k_SA).Dimension = _dimensionRepository.DimensionByName(CoreConstants.Dimension.InversedLength);
      }

      private void convertSimulationParameters(Simulation simulation)
      {
         var applications = simulation.Model.Root.Container(Constants.APPLICATIONS);

         if (applications != null)
         {
            var allParticleFactors = applications.GetAllChildren<IParameter>(x => x.IsNamed(CoreConstants.Parameter.NUMBER_OF_PARTICLES_FACTOR));
            foreach (var allParticleFactor in allParticleFactors)
            {
               allParticleFactor.Value *= 1000;
            }
         }

         var allPEndothelialParameters = simulation.Model.Root.GetAllChildren<IParameter>(x => x.IsNamed(ConverterConstants.Parameter.P_endothelial));
         foreach (var parameter in allPEndothelialParameters)
         {
            if (parameter.Formula.IsExplicit())
               continue;

            if (parameter.DefaultValue != null)
               continue;

            //default value for all P_endotehlial in dm/min
            parameter.DefaultValue = 10;
            if (ValueComparer.AreValuesEqual(parameter.DefaultValue.Value, parameter.Value, CoreConstants.DOUBLE_RELATIVE_EPSILON))
               return;

            parameter.IsFixedValue = true;
         }
      }

      private void updateOutputSchema(Simulation simulation)
      {
         foreach (var interval in simulation.OutputSchema.Intervals)
         {
            interval.Parameter(ConverterConstants.Parameter.EndTime).Name = Constants.Parameters.END_TIME;
            interval.Parameter(Constants.Parameters.RESOLUTION).IsFixedValue = false;
         }
      }

      public void Visit(Individual individual)
      {
         _individualCalculationMethodsUpdater.AddMissingCalculationMethodsTo(individual);

         performCommonConversion(individual);
         Visit(individual.OriginData);

         foreach (var transporter in individual.AllMolecules<IndividualTransporter>().SelectMany(x => x.AllExpressionsContainers()))
         {
            var oldProcessNames = transporter.ProcessNames.ToList();
            transporter.ClearProcessNames();
            foreach (var processName in oldProcessNames)
            {
               transporter.AddProcessName(processName.Replace("_MM", ""));
            }
         }

         //change container type for molecules
         foreach (var molecule in individual.AllMolecules())
         {
            molecule.ContainerType = ContainerType.Molecule;
            updateOntogeny(molecule);
         }

         //special parameters that need conversion by hand
         convertIndividualParameters(individual.Organism);
         convertIndividualForTwoPore(individual);

         var human = _defaultIndividualRetriever.DefaultHuman();

         individual.Organism.Add(clone(human.Organism, CoreConstants.Parameter.ONTOGENY_FACTOR_ALBUMIN));
         individual.Organism.Add(clone(human.Organism, CoreConstants.Parameter.ONTOGENY_FACTOR_AGP));
         individual.Organism.Add(clone(human.Organism, CoreConstants.Parameter.PLASMA_PROTEIN_SCALE_FACTOR));


         if (!individual.IsHuman)
         {
            individual.OriginData.AddCalculationMethod(_calculationMethodRepository.FindByName("MucosaVolume_Animals"));
         }
         else
         {
            individual.OriginData.AddCalculationMethod(_calculationMethodRepository.FindByName("MucosaVolume_Human"));
            individual.OriginData.GestationalAge = CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS;
            individual.OriginData.GestationalAgeUnit = CoreConstants.Units.Weeks;

            var gestationalAge = human.Organism.Parameter(CoreConstants.Parameter.GESTATIONAL_AGE);

            if (!individual.Organism.ContainsName(CoreConstants.Parameter.GESTATIONAL_AGE))
               individual.Organism.Add(_cloner.Clone(gestationalAge));
         }
      }

      private void updateModelConfiguration(Simulation simulation)
      {
         var modelProperties = simulation.ModelProperties;
         var individual = simulation.Individual;
         if (individual.IsHuman)
            modelProperties.AddCalculationMethod(_calculationMethodRepository.FindByName("MucosaVolume_Human"));
         else
            modelProperties.AddCalculationMethod(_calculationMethodRepository.FindByName("MucosaVolume_Animals"));
      }

      public void Visit(OriginData originData)
      {
         if (originData.WeightUnit == null)
            originData.WeightUnit = CoreConstants.Units.Kg;

         if (originData.SpeciesPopulation.IsHeightDependent)
         {
            //automatic conversion does not work for height
            originData.Height = originData.Height / 10;
            originData.BMI = originData.BMI / 100;

            if (originData.HeightUnit == null)
               originData.HeightUnit = CoreConstants.Units.cm;

            if (originData.BMIUnit == null)
               originData.BMIUnit = CoreConstants.Units.KgPerM2;
         }

         if (originData.SpeciesPopulation.IsAgeDependent)
         {
            if (originData.AgeUnit == null)
               originData.AgeUnit = CoreConstants.Units.Years;
         }
      }

      private void updateOntogeny(IndividualMolecule molecule)
      {
         var liver = molecule.ExpressionContainer(CoreConstants.Organ.Liver);
         if (liver == null)
            return;

         var ontogenyFactorLiver = liver.Parameter(CoreConstants.Parameter.ONTOGENY_FACTOR);
         if (ontogenyFactorLiver != null)
            molecule.Add(ontogenyFactorLiver);

         IParameter ontogenyFactorDuodenum = null;
         var duodenum = molecule.ExpressionContainer(CoreConstants.Compartment.Duodenum);
         if (duodenum != null)
            ontogenyFactorDuodenum = duodenum.Parameter(CoreConstants.Parameter.ONTOGENY_FACTOR);

         //remove old ontogeny factor parameter in container
         foreach (var expressionContainer in molecule.AllExpressionsContainers())
         {
            var ontogenyFactor = expressionContainer.Parameter(CoreConstants.Parameter.ONTOGENY_FACTOR);
            if (ontogenyFactor != null)
               expressionContainer.RemoveChild(ontogenyFactor);
         }

         if (ontogenyFactorDuodenum != null)
         {
            ontogenyFactorDuodenum.Name = CoreConstants.Parameter.ONTOGENY_FACTOR_GI;
            molecule.Add(ontogenyFactorDuodenum);
         }
      }

      private void convertIndividualForTwoPore(Individual individual)
      {
         var endogenous = individual.Organism.Organ(CoreConstants.Organ.EndogenousIgG);

         //already converted
         if (endogenous.ContainsName(ConverterConstants.Parameter.Kass_FcRn_ligandEndo)) return;
         var defaultIndividual = _defaultIndividualRetriever.DefaultIndividualFor(individual.OriginData.SpeciesPopulation);
         var defaultEndogenous = defaultIndividual.Organism.Organ(CoreConstants.Organ.EndogenousIgG);

         endogenous.Add(clone(defaultEndogenous, ConverterConstants.Parameter.Kass_FcRn_ligandEndo));
         endogenous.Add(clone(defaultEndogenous, ConverterConstants.Parameter.Kd_FcRn_LigandEndo));
         endogenous.Add(clone(defaultEndogenous, ConverterConstants.Parameter.Kd_FcRn_ligandEndo_pls_int));

         var endosome = endogenous.Compartment(ConverterConstants.ContainerName.IGG_ENDOSOME);
         endosome.Name = CoreConstants.Compartment.Endosome;

         endogenous.Compartment(CoreConstants.Compartment.Endosome)
            .Add(clone(defaultEndogenous.Compartment(CoreConstants.Compartment.Endosome), ConverterConstants.Parameter.Start_concentration_FcRn_endosome));

         endogenous.Compartment(CoreConstants.Compartment.Plasma)
            .Add(clone(defaultEndogenous.Compartment(CoreConstants.Compartment.Plasma), ConverterConstants.Parameter.Start_concentration_endogenous_plasma));


         foreach (var container in individual.Organism.GetAllChildren<IContainer>(x => x.IsNamed(ConverterConstants.ContainerName.ENDOSOMAL_CLEARANCE)))
         {
            if (container.ParentContainer.IsNamed(CoreConstants.Organ.EndogenousIgG))
               continue;

            container.ParentContainer.RemoveChild(container);
         }

         var endsomeClearance = clone(defaultIndividual.Organism, ConverterConstants.ContainerName.ENDOSOMAL_CLEARANCE) as IContainer;
         individual.Organism.Add(endsomeClearance);

         foreach (var neighborhood in individual.Neighborhoods.GetChildren<INeighborhood>(neighborhoodIsEndosomalClearance))
         {
            neighborhood.SecondNeighbor = endsomeClearance;
         }
      }

      private bool neighborhoodIsEndosomalClearance(INeighborhood neighborhood)
      {
         //name of neighborhood is for instance "SmallIntestine_end_SmallIntestine_ecl"
         return Regex.Matches(neighborhood.Name, @"(.*)_end_\1_ecl").Count > 0;
      }

      private IEntity clone(IContainer container, string childName)
      {
         return _cloner.Clone(container.GetSingleChildByName(childName));
      }

      private void convertIndividualSimulationSettings(IndividualSimulation simulation)
      {
         _simulationPersistableUpdater.ResetPersistable(simulation);
         foreach (var observer in simulation.All<IObserver>().Where(x => x.Persistable))
         {
            simulation.OutputSelections.AddOutput(new QuantitySelection(_entityPathResolver.PathFor(observer), observer.QuantityType));
         }
      }

      public void Visit(Compound compound)
      {
         performCommonConversion(compound);
         _compoundConverter.Convert(compound);
      }

      private void performCommonConversion(IPKSimBuildingBlock buildingBlock)
      {
         _formulaAndDimensionConverter.ConvertDimensionIn(buildingBlock);
      }

      public void Visit(AdvancedProtocol advancedProtocol)
      {
         performCommonConversion(advancedProtocol);
      }

      public void Visit(SimpleProtocol simpleProtocol)
      {
         performCommonConversion(simpleProtocol);
         var parameter = simpleProtocol.Parameter(ConverterConstants.Parameter.EndTime);
         parameter.Name = Constants.Parameters.END_TIME;
      }

      public void Visit(PKSimEvent pkSimEvent)
      {
         performCommonConversion(pkSimEvent);
      }

      public void Visit(Formulation formulation)
      {
         performCommonConversion(formulation);
      }

      public void Visit(DataRepository dataRepository)
      {
         _formulaAndDimensionConverter.ConvertDimensionIn(dataRepository);
      }

      public void Visit(PKSimProject project)
      {
         foreach (var observedData in project.AllObservedData)
         {
            Visit(observedData);
         }
      }
   }
}