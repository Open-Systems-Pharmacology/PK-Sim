using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Mappers;
using PKSim.Infrastructure.ORM.Repositories;
using IMoleculeBuilderFactory = PKSim.Core.Model.IMoleculeBuilderFactory;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Infrastructure.Services
{
   public class MoleculeBuilderFactory : IMoleculeBuilderFactory
   {
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IParameterIdUpdater _parameterIdUpdater;
      private readonly IFlatMoleculeRepository _flatMoleculeRepository;
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IFlatMoleculeToMoleculeBuilderMapper _moleculeMapper;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly ICloner _cloner;
      private readonly IParameterFactory _parameterFactory;

      public MoleculeBuilderFactory(
         IParameterFactory parameterFactory,
         IParameterSetUpdater parameterSetUpdater,
         IObjectBaseFactory objectBaseFactory,
         IParameterIdUpdater parameterIdUpdater,
         IFlatMoleculeRepository flatMoleculeRepository,
         IParameterContainerTask parameterContainerTask,
         IFlatMoleculeToMoleculeBuilderMapper moleculeMapper,
         IDimensionRepository dimensionRepository,
         ICloner cloner)
      {
         _parameterFactory = parameterFactory;
         _parameterSetUpdater = parameterSetUpdater;
         _objectBaseFactory = objectBaseFactory;
         _parameterIdUpdater = parameterIdUpdater;
         _flatMoleculeRepository = flatMoleculeRepository;
         _parameterContainerTask = parameterContainerTask;
         _moleculeMapper = moleculeMapper;
         _dimensionRepository = dimensionRepository;
         _cloner = cloner;
      }

      public MoleculeBuilder Create(QuantityType moleculeType, IFormulaCache formulaCache)
      {
         var moleculeBuilder = createFor(moleculeType, formulaCache);
         moleculeBuilder.Dimension = _dimensionRepository.Amount;
         moleculeBuilder.DefaultStartFormula.Dimension = _dimensionRepository.Amount;
         addFormulaToCacheIfNecessary(moleculeBuilder.DefaultStartFormula, formulaCache);
         addConcentrationParameterTo(moleculeBuilder, formulaCache);
         return moleculeBuilder;
      }

      private MoleculeBuilder createFor(QuantityType moleculeType, IFormulaCache formulaCache)
      {
         switch (moleculeType)
         {
            case QuantityType.Drug:
               return _moleculeMapper.MapFrom(_flatMoleculeRepository.FindBy(QuantityType.Drug), formulaCache)
                  .WithIcon(ApplicationIcons.Drug.IconName);

            case QuantityType.Enzyme:
            case QuantityType.OtherProtein:
               return defaultProteinMoleculeFrom(moleculeType, formulaCache, QuantityType.Protein);
            case QuantityType.Transporter:
               return defaultProteinMoleculeFrom(QuantityType.Transporter, formulaCache, QuantityType.Transporter);
            case QuantityType.Metabolite:
            case QuantityType.Complex:
               return defaultReactionProduct(moleculeType, formulaCache);
            case QuantityType.Undefined:
               return defaultFloatingNonXenobioticMolecule(moleculeType, formulaCache);
            default:
               throw new ArgumentOutOfRangeException(nameof(moleculeType));
         }
      }

      private MoleculeBuilder defaultProteinMoleculeFrom(QuantityType moleculeType, IFormulaCache formulaCache, QuantityType templateType)
      {
         var molecule = _moleculeMapper.MapFrom(_flatMoleculeRepository.FindBy(templateType), formulaCache);
         molecule.QuantityType = moleculeType;
         molecule.IsXenobiotic = false;
         addDrugParametersTo(molecule, formulaCache);
         return molecule;
      }

      public MoleculeBuilder Create(Compound compound, CompoundProperties compoundProperties, InteractionProperties interactionProperties,
         IFormulaCache formulaCache)
      {
         var drug = Create(QuantityType.Drug, formulaCache).WithName(compound.Name);

         //first update all parameters defined in the molecule that are also in the compound and that DO NOT belong in one alternative
         _parameterSetUpdater.UpdateValuesByName(allSimpleParametersFrom(compound), drug.AllParameters());

         //once simple parameters have been set, set the alternative parameters
         updateAlternativeParameters(compound, compoundProperties, drug, formulaCache);

         //add interaction parameters
         addInteractionParameters(compound, drug, interactionProperties);

         _parameterIdUpdater.UpdateBuildingBlockId(drug.GetAllChildren<IParameter>(), compound);
         return drug;
      }

      private void addInteractionParameters(Compound compound, MoleculeBuilder drug, InteractionProperties interactionProperties)
      {
         foreach (var interactionProcess in compound.AllProcesses<InteractionProcess>())
         {
            if (!shouldGenerateInteractionContainer(interactionProperties, interactionProcess))
               continue;

            var interactionContainer = _objectBaseFactory.Create<InteractionContainer>()
               .WithIcon(interactionProcess.Icon)
               .WithName(interactionProcess.InternalName);

            //only add global parameters to the interaction container
            _parameterContainerTask.AddProcessBuilderParametersTo(interactionContainer);
            _parameterSetUpdater.UpdateValuesByName(interactionProcess, interactionContainer);
            interactionContainer.Name = interactionProcess.Name;
            drug.AddInteractionContainer(interactionContainer);
         }
      }

      private static bool shouldGenerateInteractionContainer(InteractionProperties interactionProperties, InteractionProcess interactionProcess)
      {
         return interactionProperties.Uses(interactionProcess);
      }

      private void updateAlternativeParameters(Compound compound, CompoundProperties compoundProperties, MoleculeBuilder drug,
         IFormulaCache formulaCache)
      {
         selectedAlternativesFor(compound, compoundProperties).Each(alternative => updateAlternativeParameters(alternative, drug, formulaCache));
      }

      private void updateAlternativeParameters(ParameterAlternative alternative, MoleculeBuilder drug, IFormulaCache formulaCache)
      {
         var allParameters = alternative.AllParameters().ToList();
         foreach (var alternativeParameter in allParameters)
         {
            //Parameters does not exist in drug?
            var drugParameter = drug.Parameter(alternativeParameter.Name);
            if (drugParameter == null) continue;

            if (alternativeParameter.Formula.IsTable())
            {
               var tableFormula = _cloner.Clone(alternativeParameter.Formula);
               formulaCache.Add(tableFormula);
               drugParameter.Formula = tableFormula;
            }

            //parameter is a rate. parameter in molecule should be readonly
            else if (!alternativeParameter.Formula.IsConstant())
               drugParameter.Editable = false;

            //target parameter is a rate and source parameter is constant
            else if (!drugParameter.Formula.IsConstant())
               drugParameter.Formula = _objectBaseFactory.Create<ConstantFormula>().WithValue(alternativeParameter.Value);

            _parameterSetUpdater.UpdateValue(alternativeParameter, drugParameter);

            //Default parameter Default and visible may not match database default and need to be set according to alternative parameter
            drugParameter.IsDefault = alternativeParameter.IsDefault;
            drugParameter.Visible = alternativeParameter.Visible;
         }
      }

      /// <summary>
      ///    Returns the parameters from the drug that cannot be defined as alternatives
      /// </summary>
      private IEnumerable<IParameter> allSimpleParametersFrom(Compound compound)
      {
         return compound.AllParameters(p => !CoreConstants.Groups.GroupsWithAlternative.Contains(p.GroupName));
      }

      private IEnumerable<ParameterAlternative> selectedAlternativesFor(Compound compound, CompoundProperties compoundProperties)
      {
         return compoundProperties.CompoundGroupSelections.Select(groupSelection => compound.ParameterAlternativeGroup(groupSelection.GroupName)
            .AlternativeByName(groupSelection.AlternativeName));
      }

      /// <summary>
      ///    add the given formula to the cache only if
      ///    1- the formula is not constant (constant formula are not registered in cache)
      ///    2- the formula was not registered already.
      /// </summary>
      /// <param name="formula">formula to add</param>
      /// <param name="formulaCache">formula cache</param>
      private void addFormulaToCacheIfNecessary(IFormula formula, IFormulaCache formulaCache)
      {
         if (formula.IsConstant()) return;
         if (formulaCache.Contains(formula.Id)) return;
         formulaCache.Add(formula);
      }

      private void addConcentrationParameterTo(MoleculeBuilder moleculeBuilder, IFormulaCache formulaCache)
      {
         var parameter = _parameterFactory.CreateConcentrationParameterIn(formulaCache);
         moleculeBuilder.AddParameter(parameter);
      }

      private MoleculeBuilder defaultReactionProduct(QuantityType moleculeType, IFormulaCache formulaCache)
      {
         var metabolite = _objectBaseFactory.Create<MoleculeBuilder>();
         addDrugParametersTo(metabolite, formulaCache);
         metabolite.QuantityType = moleculeType;
         metabolite.Name = moleculeType.ToString();
         metabolite.DefaultStartFormula = _objectBaseFactory.Create<ConstantFormula>().WithValue(0);
         metabolite.IsFloating = false;
         metabolite.IsXenobiotic = true;
         return metabolite;
      }

      private MoleculeBuilder defaultFloatingNonXenobioticMolecule(QuantityType moleculeType, IFormulaCache formulaCache)
      {
         var molecule = _objectBaseFactory.Create<MoleculeBuilder>();
         molecule.QuantityType = moleculeType;
         molecule.Name = moleculeType.ToString();
         molecule.DefaultStartFormula = _objectBaseFactory.Create<ConstantFormula>().WithValue(0);
         molecule.IsFloating = true;
         molecule.IsXenobiotic = false;
         return molecule;
      }

      private void addDrugParametersTo(MoleculeBuilder molecule, IFormulaCache formulaCache)
      {
         var allExistingParameters = molecule.AllParameters().ToList();
         molecule.Name = CoreConstants.Molecule.Drug;
         _parameterContainerTask.AddMoleculeParametersTo(molecule, formulaCache);
         foreach (var parameter in molecule.AllParameters().Where(p => !allExistingParameters.Contains(p)))
         {
            parameter.BuildingBlockType = PKSimBuildingBlockType.Simulation;
            parameter.Visible = false;
            parameter.IsDefault = true;
            parameter.CanBeVariedInPopulation = false;
            molecule.Add(parameter);
         }

         setDefaultParameterValues(molecule);

         //default value for floating in lumen should be 0
         molecule.Parameter(CoreConstants.Parameters.IS_FLOATING_IN_LUMEN).Value = 0;
      }

      private void setDefaultParameterValues(MoleculeBuilder molecule)
      {
         var parameters = molecule.AllParameters().Where(parameter => CoreConstants.Parameters.CompoundMustInputParameters.Contains(parameter.Name));
         parameters.Each(p =>
         {
            p.Value = double.NaN;
            p.IsDefault = true;
         });
      }
   }
}