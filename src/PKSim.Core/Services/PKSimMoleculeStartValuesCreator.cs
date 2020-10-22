using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IPKSimMoleculeStartValuesCreator
   {
      IMoleculeStartValuesBuildingBlock CreateFor(IBuildConfiguration buildConfiguration, Simulation simulation);
   }

   public class PKSimMoleculeStartValuesCreator : IPKSimMoleculeStartValuesCreator
   {
      private readonly IMoleculeStartValuesCreator _moleculeStartValuesCreator;
      private readonly IObjectPathFactory _objectPathFactory;
      private readonly IMoleculeStartFormulaRepository _moleculeStartFormulaRepository;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IModelContainerMoleculeRepository _modelContainerMoleculeRepository;

      public PKSimMoleculeStartValuesCreator(
         IMoleculeStartValuesCreator moleculeStartValuesCreator,
         IObjectPathFactory objectPathFactory,
         IMoleculeStartFormulaRepository moleculeStartFormulaRepository,
         IFormulaFactory formulaFactory,
         IModelContainerMoleculeRepository modelContainerMoleculeRepository)
      {
         _moleculeStartValuesCreator = moleculeStartValuesCreator;
         _objectPathFactory = objectPathFactory;
         _moleculeStartFormulaRepository = moleculeStartFormulaRepository;
         _formulaFactory = formulaFactory;
         _modelContainerMoleculeRepository = modelContainerMoleculeRepository;
      }

      public IMoleculeStartValuesBuildingBlock CreateFor(IBuildConfiguration buildConfiguration, Simulation simulation)
      {
         //default molecule start values matrix
         var compounds = simulation.Compounds;
         var individual = simulation.Individual;
         var defaultStartValues = _moleculeStartValuesCreator.CreateFrom(buildConfiguration.SpatialStructure, buildConfiguration.Molecules);

         //set available start formulas for molecules
         setStartFormulasForStaticMolecules(defaultStartValues, simulation, compounds);

         foreach (var molecule in individual.AllMolecules())
         {
            var allMoleculesObjectPath = moleculesInvolvedInExpression(individual, molecule, simulation.CompoundPropertiesList);
            //path involved expression might not exist in the start values structure=>hence check that they are not null
            var allAvailableStartValues = allMoleculesObjectPath.Select(objectPath => defaultStartValues[objectPath]).Where(msv => msv != null);
            //the one found should be set to present
            allAvailableStartValues.Each(msv => msv.IsPresent = true);
         }

         return defaultStartValues.WithName(simulation.Name);
      }

      private IEnumerable<IObjectPath> moleculesInvolvedInExpression(Individual individual, IndividualMolecule molecule,
         IReadOnlyList<CompoundProperties> compoundPropertiesList)
      {
         foreach (var expressionContainer in individual.AllMoleculeContainersFor(molecule))
         {
            var containerPath = _objectPathFactory.CreateAbsoluteObjectPath(expressionContainer);

            foreach (var compoundProperties in compoundPropertiesList)
            {
               foreach (var moleculeName in compoundProperties.Processes.AllInducedMoleculeNames(molecule))
               {
                  yield return containerPath.Clone<IObjectPath>().AndAdd(moleculeName);
               }
            }
         }
      }

      private void setStartFormulasForStaticMolecules(IMoleculeStartValuesBuildingBlock defaultStartValues, Simulation simulation, IEnumerable<Compound> compounds)
      {
         var modelName = simulation.ModelConfiguration.ModelName;
         //get the names of molecules that are static (e.g. not enzymes, metabolites, etc.)
         // (e.g. FcRn in 2pores-model)
         var staticMoleculeNames = _modelContainerMoleculeRepository.MoleculeNamesIncludingDrug(modelName);
         var compoundNames = compounds.AllNames().ToList();

         foreach (var moleculeStartValue in defaultStartValues)
         {
            var dbMoleculeName = getDbMoleculeName(moleculeStartValue.MoleculeName, compoundNames);

            if (staticMoleculeNames.Contains(dbMoleculeName))
            {
               moleculeStartValue.IsPresent = _modelContainerMoleculeRepository.IsPresent(modelName, moleculeStartValue.ContainerPath, dbMoleculeName);
               moleculeStartValue.NegativeValuesAllowed = _modelContainerMoleculeRepository.NegativeValuesAllowed(modelName, moleculeStartValue.ContainerPath, dbMoleculeName);
            }
            else
            {
               moleculeStartValue.IsPresent = false;
               moleculeStartValue.NegativeValuesAllowed = false;
            }

            var moleculeDbPath = moleculeStartValue.ContainerPath.Clone<IObjectPath>();

            moleculeDbPath.Add(dbMoleculeName);

            var rateKey = _moleculeStartFormulaRepository.RateKeyFor(moleculeDbPath, simulation.ModelProperties);

            if (rateKey == null)
               continue; //no start formula available

            //set molecule start formula
            moleculeStartValue.Formula = _formulaFactory.RateFor(rateKey, defaultStartValues.FormulaCache);
         }
      }

      private static string getDbMoleculeName(string moleculeName, List<string> compoundNames)
      {
         if (compoundNames.Contains(moleculeName))
            return CoreConstants.Molecule.Drug;

         foreach (var compoundName in compoundNames)
         {
            if (string.Equals(moleculeName, CoreConstants.Molecule.DrugFcRnComplexName(compoundName)))
               return CoreConstants.Molecule.DrugFcRnComplexTemplate;
         }

         return moleculeName;
      }
   }
}