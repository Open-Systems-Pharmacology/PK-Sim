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
   public interface IPKSimInitialConditionsCreator
   {
      InitialConditionsBuildingBlock CreateFor(SpatialStructure spatialStructure, IReadOnlyList<MoleculeBuilder> molecules, Simulation simulation);
      InitialConditionsBuildingBlock CreateFor(SpatialStructure spatialStructure, MoleculeBuilder molecule, Simulation simulation);
   }

   public class PKSimInitialConditionsCreator : IPKSimInitialConditionsCreator
   {
      private readonly IInitialConditionsCreator _initialConditionsCreator;
      private readonly IMoleculeStartFormulaRepository _moleculeStartFormulaRepository;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IModelContainerMoleculeRepository _modelContainerMoleculeRepository;
      private readonly IEntityPathResolver _entityPathResolver;

      public PKSimInitialConditionsCreator(
         IInitialConditionsCreator initialConditionsCreator,
         IMoleculeStartFormulaRepository moleculeStartFormulaRepository,
         IFormulaFactory formulaFactory,
         IModelContainerMoleculeRepository modelContainerMoleculeRepository,
         IEntityPathResolver entityPathResolver)
      {
         _initialConditionsCreator = initialConditionsCreator;
         _moleculeStartFormulaRepository = moleculeStartFormulaRepository;
         _formulaFactory = formulaFactory;
         _modelContainerMoleculeRepository = modelContainerMoleculeRepository;
         _entityPathResolver = entityPathResolver;
      }

      public InitialConditionsBuildingBlock CreateFor(SpatialStructure spatialStructure, IReadOnlyList<MoleculeBuilder> molecules, Simulation simulation)
      {
         //default molecule start values matrix
         var compounds = simulation.Compounds;
         var individual = simulation.Individual;
         var defaultInitialConditions = _initialConditionsCreator.CreateFrom(spatialStructure, molecules);

         //set available start formulas for molecules
         setStartFormulasForStaticMolecules(defaultInitialConditions, simulation, compounds);

         foreach (var molecule in individual.AllMolecules())
         {
            var allMoleculesObjectPath = moleculesInvolvedInExpression(individual, molecule, simulation.CompoundPropertiesList);
            //path involved expression might not exist in the start values structure=>hence check that they are not null
            var allAvailableInitialConditions = allMoleculesObjectPath.Select(objectPath => defaultInitialConditions[objectPath]).Where(msv => msv != null);
            //the one found should be set to present
            allAvailableInitialConditions.Each(msv => msv.IsPresent = true);
         }

         return defaultInitialConditions;
      }

      public InitialConditionsBuildingBlock CreateFor(SpatialStructure spatialStructure, MoleculeBuilder molecule, Simulation simulation)
      {
         return CreateFor(spatialStructure, new List<MoleculeBuilder> { molecule }, simulation);
      }

      private IEnumerable<ObjectPath> moleculesInvolvedInExpression(Individual individual, IndividualMolecule molecule,
         IReadOnlyList<CompoundProperties> compoundPropertiesList)
      {
         foreach (var container in individual.AllPhysicalContainersWithMoleculeFor(molecule))
         {
            var containerPath = _entityPathResolver.ObjectPathFor(container);

            foreach (var compoundProperties in compoundPropertiesList)
            {
               foreach (var moleculeName in compoundProperties.Processes.AllInducedMoleculeNames(molecule))
               {
                  yield return containerPath.Clone<ObjectPath>().AndAdd(moleculeName);
               }
            }
         }
      }

      private void setStartFormulasForStaticMolecules(InitialConditionsBuildingBlock defaultInitialConditions, Simulation simulation, IEnumerable<Compound> compounds)
      {
         var modelName = simulation.ModelConfiguration.ModelName;
         //get the names of molecules that are static (e.g. not enzymes, metabolites, etc.)
         // (e.g. FcRn in 2pores-model)
         var staticMoleculeNames = _modelContainerMoleculeRepository.MoleculeNamesIncludingDrug(modelName);
         var compoundNames = compounds.AllNames();

         foreach (var initialCondition in defaultInitialConditions)
         {
            var dbMoleculeName = getDbMoleculeName(initialCondition.MoleculeName, compoundNames);

            if (staticMoleculeNames.Contains(dbMoleculeName))
            {
               initialCondition.IsPresent = _modelContainerMoleculeRepository.IsPresent(modelName, initialCondition.ContainerPath, dbMoleculeName);
               initialCondition.NegativeValuesAllowed = _modelContainerMoleculeRepository.NegativeValuesAllowed(modelName, initialCondition.ContainerPath, dbMoleculeName);
            }
            else
            {
               initialCondition.IsPresent = false;
               initialCondition.NegativeValuesAllowed = false;
            }

            var moleculeDbPath = initialCondition.ContainerPath.Clone<ObjectPath>();

            moleculeDbPath.Add(dbMoleculeName);

            var rateKey = _moleculeStartFormulaRepository.RateKeyFor(moleculeDbPath, simulation.ModelProperties);

            if (rateKey == null)
               continue; //no start formula available

            //set molecule start formula
            initialCondition.Formula = _formulaFactory.RateFor(rateKey, defaultInitialConditions.FormulaCache);
         }
      }

      private static string getDbMoleculeName(string moleculeName, IReadOnlyList<string> compoundNames)
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