using System;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ORM.Queries
{
   public class ModelObserverQuery : IModelObserverQuery
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IObserverBuilderRepository _observerBuilderRepository;
      private readonly ICloneManagerForBuildingBlock _cloneManager;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IObjectPathFactory _objectPathFactory;
      private readonly IInteractionTask _interactionTask;

      private const string TOTAL_DRUG_MASS_ALIAS = "TotalDrugMass";

      public ModelObserverQuery(IObjectBaseFactory objectBaseFactory, IObserverBuilderRepository observerBuilderRepository,
         ICloneManagerForBuildingBlock cloneManager, IDimensionRepository dimensionRepository, IObjectPathFactory objectPathFactory,
         IInteractionTask interactionTask)
      {
         _objectBaseFactory = objectBaseFactory;
         _observerBuilderRepository = observerBuilderRepository;
         _cloneManager = cloneManager;
         _dimensionRepository = dimensionRepository;
         _objectPathFactory = objectPathFactory;
         _interactionTask = interactionTask;
      }

      public IObserverBuildingBlock AllObserversFor(IMoleculeBuildingBlock moleculeBuildingBlock, Simulation simulation)
      {
         var observerBuildingBlock = _objectBaseFactory.Create<IObserverBuildingBlock>().WithName(simulation.Name);
         addStandardObserversTo(simulation, observerBuildingBlock, moleculeBuildingBlock);

         addSimulationObservers(simulation, observerBuildingBlock);

         //now add dynamic observers for molecules needing one
         createFractionExcretedToUrineObserver(simulation, observerBuildingBlock);
         createFractionOfDoseExcretedToBileObserver(simulation, observerBuildingBlock);
         createFractionOfDoseExcretedToFecesObserver(simulation, observerBuildingBlock);

         //add fraction observers for metabolite and complex
         createFractionOfDoseObserver(simulation, observerBuildingBlock);

         foreach (var compound in simulation.Compounds)
         {
            foreach (var specificBinding in simulation.CompoundPropertiesFor(compound).Processes.SpecificBindingSelection.AllEnabledProcesses())
            {
               var complexName = specificBinding.ProductName(CoreConstants.Molecule.Complex);
               var protein = moleculeBuildingBlock[specificBinding.MoleculeName];
               var complex = moleculeBuildingBlock[complexName];

               createReceptorOccupancyObserver(observerBuildingBlock, protein, complex);
            }
         }

         return observerBuildingBlock;
      }

      private void addSimulationObservers(Simulation simulation, IObserverBuildingBlock observerBuildingBlock)
      {
         simulation.AllBuildingBlocks<ObserverSet>()
            .SelectMany(x => x.Observers)
            .Each(observerBuildingBlock.Add);
      }

      private void addStandardObserversTo(Simulation simulation, IObserverBuildingBlock observerBuildingBlock, IMoleculeBuildingBlock moleculeBuildingBlock)
      {
         var allMoleculesNeedingConcentrationObservers = moleculeBuildingBlock.Where(concentrationIsNeeded).Select(x => x.Name).ToList();
         var compoundNames = simulation.CompoundNames;

         foreach (var observerTemplate in _observerBuilderRepository.AllFor(simulation.ModelProperties))
         {
            if (isDrugFcRnObserver(observerTemplate))
            {
               //special case: protein model observer for drug and its FcRn complex. 
               //Observer must be created separately for every drug because complexname can not be referenced in generic way by now
               //E.g.: for drug C1, Observer is f("C1", "C1-FcRn_Complex"))
               foreach (var compoundName in compoundNames)
               {
                  var observer = _cloneManager.Clone(observerTemplate.ObserverBuilder, observerBuildingBlock.FormulaCache);
                  observer.Name = CoreConstants.Observer.ObserverNameFrom(observer.Name, compoundName);

                  observer.Formula.ReplaceKeywordsInObjectPaths(
                     new[] {CoreConstants.Molecule.DrugFcRnComplexTemplate},
                     new[] {CoreConstants.Molecule.DrugFcRnComplexName(compoundName)}
                  );

                  observer.AddMoleculeName(compoundName);

                  observerBuildingBlock.Add(observer);
               }
            }
            else
            {
               var observer = _cloneManager.Clone(observerTemplate.ObserverBuilder, observerBuildingBlock.FormulaCache);

               compoundNames.Each(observer.AddMoleculeName);

               if (observer.IsConcentration())
                  allMoleculesNeedingConcentrationObservers.Each(observer.AddMoleculeName);

               observerBuildingBlock.Add(observer);
            }
         }
      }

      private bool isDrugFcRnObserver(IPKSimObserverBuilder observerTemplate)
      {
         var templateFormula = observerTemplate.ObserverBuilder.Formula;
         return templateFormula.ObjectPaths.Any(p => p.Contains(CoreConstants.Molecule.DrugFcRnComplexTemplate));
      }

      private bool concentrationIsNeeded(IMoleculeBuilder moleculeBuilder)
      {
         //concentration for all dynamic species but metabolite
         return moleculeBuilder.QuantityType.Is(QuantityType.Protein) ||
                moleculeBuilder.QuantityType.Is(QuantityType.Complex);
      }

      private void createFractionOfDoseObserver(Simulation simulation, IObserverBuildingBlock observerBuildingBlock)
      {
         simulation.Compounds.Each(c => createFractionOfDoseObserverForCompound(simulation, observerBuildingBlock, c));
      }

      private void createFractionOfDoseObserverForCompound(Simulation simulation, IObserverBuildingBlock observerBuildingBlock, Compound compound)
      {
         var compoundProperties = simulation.CompoundPropertiesFor(compound);
         var processes = compoundProperties.Processes;

         if (!compoundProperties.IsAdministered)
            return;

         if (_interactionTask.IsMetabolite(compound, simulation))
            return;

         //always add complex molecules if any
         var moleculeNames = processes.SpecificBindingSelection.AllEnabledPartialProcesses().Select(x => x.ProductName(CoreConstants.Molecule.Complex)).ToList();

         //Add sink metabolites
         var enzymaticProcessProductNames = processes.MetabolizationSelection.AllEnabledPartialProcesses().OfType<EnzymaticProcessSelection>()
            .Where(x => x.IsSink)
            .Select(x => x.ProductName()).ToList();

         moleculeNames.AddRange(enzymaticProcessProductNames);

         if (!moleculeNames.Any())
            return;

         //Local fraction of dose observer defined per instance of the metabolization or specific binding process
         var observerName = CoreConstants.Observer.ObserverNameFrom(CoreConstants.Observer.FRACTION_OF_DOSE, compound.Name);
         var fractionObserver = createAmountFractionObserver(observerName, observerBuildingBlock, () => getFractionOfDoseFormula(observerBuildingBlock, compound));
         moleculeNames.Each(fractionObserver.AddMoleculeName);

         //Add liver zone specific observers based on this observer
         addLiverZoneObserversBasedOn(fractionObserver, observerBuildingBlock, compound);

         //Global fraction of dose observers. One per complex or binding process defined globally under organism
         var totalFractionObserverName = CoreConstants.Observer.ObserverNameFrom(CoreConstants.Observer.TOTAL_FRACTION_OF_DOSE, compound.Name);
         var totalFractionObserver = createContainerFractionObserver(totalFractionObserverName, observerBuildingBlock, () => getTotalFractionOfDoseFormula(observerBuildingBlock, compound, observerName));
         totalFractionObserver.ContainerCriteria = Create.Criteria(x => x.With(Constants.ORGANISM));
         moleculeNames.Each(totalFractionObserver.AddMoleculeName);
      }

      private void createReceptorOccupancyObserver(IObserverBuildingBlock observerBuildingBlock, IMoleculeBuilder protein, IMoleculeBuilder complex)
      {
         var observerName = CoreConstants.Observer.ObserverNameFrom(CoreConstants.Observer.RECEPTOR_OCCUPANCY, complex.Name);
         var observer = createAmountFractionObserver(observerName, observerBuildingBlock, () => getReceptorOccupancyFormula(observerBuildingBlock, protein, complex));
         observer.AddMoleculeName(complex.Name);
      }

      private void createFractionExcretedToUrineObserver(Simulation simulation, IObserverBuildingBlock observerBuildingBlock)
      {
         createSimpleFractionObserver(simulation, observerBuildingBlock, CoreConstants.Observer.FRACTION_EXCRETED_TO_URINE, CoreConstants.Compartment.URINE);
      }

      private void createFractionOfDoseExcretedToBileObserver(Simulation simulation, IObserverBuildingBlock observerBuildingBlock)
      {
         createSimpleFractionObserver(simulation, observerBuildingBlock, CoreConstants.Observer.FRACTION_EXCRETED_TO_BILE, CoreConstants.Organ.Gallbladder);
      }

      private void createFractionOfDoseExcretedToFecesObserver(Simulation simulation, IObserverBuildingBlock observerBuildingBlock)
      {
         createSimpleFractionObserver(simulation, observerBuildingBlock, CoreConstants.Observer.FRACTION_EXCRETED_TO_FECES, CoreConstants.Compartment.FECES);
      }

      private void createSimpleFractionObserver(Simulation simulation, IObserverBuildingBlock observerBuildingBlock, string observerName, string criteria)
      {
         var observer = createAmountFractionObserver(observerName, observerBuildingBlock, () => getFractionFormula(observerBuildingBlock));
         var compoundNames = simulation.Compounds.Where(x => !_interactionTask.IsMetabolite(x, simulation)).AllNames().ToList();
         compoundNames.Each(observer.AddMoleculeName);
         observer.ContainerCriteria = Create.Criteria(x => x.With(criteria));
      }

      private void addLiverZoneObserversBasedOn(IObserverBuilder observerBuilder, IObserverBuildingBlock observerBuildingBlock, Compound compound)
      {
         CoreConstants.Compartment.LiverCompartments.Each(compartment => addLiverZoneCompartmentObserver(observerBuilder, compartment, observerBuildingBlock, compound));
      }

      private void addLiverZoneCompartmentObserver(IObserverBuilder observerBuilder, string compartment, IObserverBuildingBlock observerBuildingBlock, Compound compound)
      {
         var observerName = CoreConstants.CompositeNameFor(observerBuilder.Name, CoreConstants.Organ.Liver, compartment);
         if (observerBuildingBlock.ExistsByName(observerName))
            return;

         var observer = _objectBaseFactory.Create<IContainerObserverBuilder>()
            .WithName(observerName)
            .WithDimension(_dimensionRepository.Fraction);

         observer.ForAll = false;
         observerBuilder.MoleculeNames().Each(observer.AddMoleculeName);

         observer.ContainerCriteria = Create.Criteria(x => x.With(CoreConstants.Organ.Liver)
            .And.With(compartment)
            .And.Not(CoreConstants.Compartment.Pericentral)
            .And.Not(CoreConstants.Compartment.Periportal));

         var formula = _objectBaseFactory.Create<ExplicitFormula>()
            .WithName(observerName)
            .WithFormulaString("(M_periportal + M_pericentral)/" + TOTAL_DRUG_MASS_ALIAS)
            .WithDimension(_dimensionRepository.Fraction);

         formula.AddObjectPath(createZoneAmountPath(compartment, CoreConstants.Compartment.Periportal, "M_periportal"));
         formula.AddObjectPath(createZoneAmountPath(compartment, CoreConstants.Compartment.Pericentral, "M_pericentral"));
         formula.AddObjectPath(createTotalDrugMassObjectPath(compound.Name));

         observer.Formula = formula;
         observerBuildingBlock.Add(observer);
         observerBuildingBlock.AddFormula(formula);
      }

      private IFormulaUsablePath createZoneAmountPath(string compartment, string zone, string alias)
      {
         return _objectPathFactory.CreateFormulaUsablePathFrom(Constants.ORGANISM, CoreConstants.Organ.Liver, zone, compartment, ObjectPathKeywords.MOLECULE)
            .WithAlias(alias)
            .WithDimension(_dimensionRepository.Amount);
      }

      private IContainerObserverBuilder createContainerFractionObserver(string observerName, IObserverBuildingBlock observerBuildingBlock, Func<IFormula> getFormula)
      {
         var observer = _objectBaseFactory.Create<IContainerObserverBuilder>()
            .WithName(observerName)
            .WithDimension(_dimensionRepository.Fraction)
            .WithFormula(getFormula());

         observerBuildingBlock.Add(observer);
         return observer;
      }

      private IAmountObserverBuilder createAmountFractionObserver(string observerName, IObserverBuildingBlock observerBuildingBlock, Func<IFormula> getFormula)
      {
         var observer = _objectBaseFactory.Create<IAmountObserverBuilder>()
            .WithName(observerName)
            .WithDimension(_dimensionRepository.Fraction)
            .WithFormula(getFormula());

         observer.ContainerCriteria.Add(new MatchAllCondition());

         observerBuildingBlock.Add(observer);
         return observer;
      }

      private IFormula getFractionFormula(IObserverBuildingBlock observerBuildingBlock)
      {
         return getFractionFormula(observerBuildingBlock, "FractionObserver", ObjectPathKeywords.MOLECULE);
      }

      private IFormula getFractionOfDoseFormula(IObserverBuildingBlock observerBuildingBlock, Compound compound)
      {
         var formulaName = $"FractionOfDose_{compound.Name}";
         return getFractionFormula(observerBuildingBlock, formulaName, compound.Name);
      }

      private IFormula getTotalFractionOfDoseFormula(IObserverBuildingBlock observerBuildingBlock, Compound compound, string fractionOfDoseObserverName)
      {
         var formulaName = $"TotalFractionOfDose_{compound.Name}";

         if (observerBuildingBlock.FormulaCache.Contains(formulaName))
            return observerBuildingBlock.FormulaCache[formulaName];

         var sumFormula = _objectBaseFactory.Create<SumFormula>()
            .WithName(formulaName)
            .WithDimension(_dimensionRepository.Fraction);

         //This will collect all observer named 'fractionOfDoseObserverName' for the molecule where the observer is defined.
         sumFormula.Criteria = Create.Criteria(x => x.With(fractionOfDoseObserverName).And.InContainer(ObjectPathKeywords.MOLECULE));
         sumFormula.Variable = "F";
         sumFormula.FormulaString = sumFormula.VariablePattern;

         return sumFormula;
      }

      private IFormula getFractionFormula(IObserverBuildingBlock observerBuildingBlock, string formulaName, string pathToDrugMass)
      {
         if (observerBuildingBlock.FormulaCache.Contains(formulaName))
            return observerBuildingBlock.FormulaCache[formulaName];

         var fractionFormula = _objectBaseFactory.Create<ExplicitFormula>()
            .WithName(formulaName)
            .WithFormulaString($"{TOTAL_DRUG_MASS_ALIAS}>0 ? M/{TOTAL_DRUG_MASS_ALIAS} : 0")
            .WithDimension(_dimensionRepository.Fraction);

         fractionFormula.AddObjectPath(_objectPathFactory.CreateFormulaUsablePathFrom(ObjectPath.PARENT_CONTAINER)
            .WithAlias("M")
            .WithDimension(_dimensionRepository.Amount));

         fractionFormula.AddObjectPath(createTotalDrugMassObjectPath(pathToDrugMass));

         observerBuildingBlock.AddFormula(fractionFormula);
         return fractionFormula;
      }

      private IFormulaUsablePath createTotalDrugMassObjectPath(string pathToDrugMass)
      {
         return _objectPathFactory.CreateFormulaUsablePathFrom(pathToDrugMass, CoreConstants.Parameters.TOTAL_DRUG_MASS)
            .WithAlias(TOTAL_DRUG_MASS_ALIAS)
            .WithDimension(_dimensionRepository.Amount);
      }

      private IFormula getReceptorOccupancyFormula(IObserverBuildingBlock observerBuildingBlock, IMoleculeBuilder protein, IMoleculeBuilder complex)
      {
         string receptorOccupancyObserver = $"ReceptorOccupancyObserver_{complex.Name}";
         if (observerBuildingBlock.FormulaCache.Contains(receptorOccupancyObserver))
            return observerBuildingBlock.FormulaCache[receptorOccupancyObserver];

         var receptorOccupancy = _objectBaseFactory.Create<ExplicitFormula>()
            .WithName(receptorOccupancyObserver)
            .WithFormulaString("Complex/(Protein + Complex)")
            .WithDimension(_dimensionRepository.Fraction);


         receptorOccupancy.AddObjectPath(_objectPathFactory.CreateFormulaUsablePathFrom(ObjectPath.PARENT_CONTAINER)
            .WithAlias("Complex")
            .WithDimension(_dimensionRepository.Amount));

         receptorOccupancy.AddObjectPath(_objectPathFactory.CreateFormulaUsablePathFrom(ObjectPath.PARENT_CONTAINER, ObjectPath.PARENT_CONTAINER, protein.Name)
            .WithAlias("Protein")
            .WithDimension(_dimensionRepository.Amount));

         observerBuildingBlock.FormulaCache.Add(receptorOccupancy);

         //reference to protein concentration has to be added for each receptor
         return receptorOccupancy;
      }
   }
}