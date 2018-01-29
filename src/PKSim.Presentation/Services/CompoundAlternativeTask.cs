using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.Core;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Compounds;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Presentation.Services
{
   public class CompoundAlternativeTask : ICompoundAlternativeTask
   {
      private readonly IParameterAlternativeFactory _parameterAlternativeFactory;
      private readonly IApplicationController _applicationController;
      private readonly IExecutionContext _executionContext;
      private readonly ICompoundFactory _compoundFactory;
      private readonly IEntityTask _entityTask;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IParameterTask _parameterTask;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public CompoundAlternativeTask(IParameterAlternativeFactory parameterAlternativeFactory, IApplicationController applicationController,
         IExecutionContext executionContext, ICompoundFactory compoundFactory, IEntityTask entityTask,
         IFormulaFactory formulaFactory, IParameterTask parameterTask, IBuildingBlockRepository buildingBlockRepository)
      {
         _parameterAlternativeFactory = parameterAlternativeFactory;
         _applicationController = applicationController;
         _executionContext = executionContext;
         _compoundFactory = compoundFactory;
         _entityTask = entityTask;
         _formulaFactory = formulaFactory;
         _parameterTask = parameterTask;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public ICommand AddParameterGroupAlternativeTo(ParameterAlternativeGroup compoundParameterGroup)
      {
         var newAlternative = _parameterAlternativeFactory.CreateAlternativeFor(compoundParameterGroup);

         using (var presenter = _applicationController.Start<IParameterAlternativeNamePresenter>())
         {
            //canceled by user - nothing to do
            if (!presenter.Edit(compoundParameterGroup))
               return new PKSimEmptyCommand();

            newAlternative.Name = presenter.Name;

            return AddParameterGroupAlternativeTo(compoundParameterGroup, newAlternative);
         }
      }

      public ICommand AddParameterGroupAlternativeTo(ParameterAlternativeGroup compoundParameterGroup, ParameterAlternative parameterAlternative)
      {
         return new AddParameterAlternativeCommand(parameterAlternative, compoundParameterGroup, _executionContext).Run(_executionContext);
      }

      public ICommand RemoveParameterGroupAlternative(ParameterAlternativeGroup parameterGroup, ParameterAlternative parameterAlternative)
      {
         if (parameterAlternative.IsDefault)
            throw new CannotDeleteDefaultParameterAlternativeException();

         if (parameterGroup.AllAlternatives.Count() == 1)
            throw new CannotDeleteParameterAlternativeException();

         return new RemoveParameterAlternativeCommand(parameterAlternative, parameterGroup, _executionContext).Run(_executionContext);
      }

      public ICommand RenameParameterAlternative(ParameterAlternative parameterAlternative)
      {
         return _entityTask.StructuralRename(parameterAlternative);
      }

      public ICommand SetAlternativeParameterValue(IParameter parameter, double valueInDisplayUnit)
      {
         if (simulationsAreUsingAlternativeContaining(parameter))
            return _parameterTask.SetParameterDisplayValue(parameter, valueInDisplayUnit);

         return _parameterTask.SetParameterDisplayValueWithoutBuildingBlockChange(parameter, valueInDisplayUnit);
      }

      public ICommand SetAlternativeParameterUnit(IParameter parameter, Unit newUnit)
      {
         if (simulationsAreUsingAlternativeContaining(parameter))
            return _parameterTask.SetParameterUnit(parameter, newUnit);

         return _parameterTask.SetParameterUnitWithoutBuildingBlockChange(parameter, newUnit);
      }

      private bool simulationsAreUsingAlternativeContaining(IParameter parameter)
      {
         var alternative = parameter.ParentContainer as ParameterAlternative;
         return alternative != null && simulationsAreUsingAlternative(alternative);
      }

      private bool simulationsAreUsingAlternative(ParameterAlternative alternative)
      {
         var allCompoundGroupSelectionForAlternativeGroup = _buildingBlockRepository.All<Simulation>()
            .SelectMany(x => x.CompoundPropertiesList)
            .SelectMany(x => x.CompoundGroupSelections
               .Where(groupSel => string.Equals(groupSel.GroupName, alternative.GroupName))
            );

         return allCompoundGroupSelectionForAlternativeGroup.Any(x => string.Equals(x.AlternativeName, alternative.Name));
      }

      public ICommand UpdateValueOrigin(ParameterAlternative parameterAlternative, ValueOrigin newValueOrigin)
      {
         var allNonDefaultParameters = parameterAlternative.AllParameters(x => !x.IsDefault);

         var shouldChangeBuildingBlockVersion = simulationsAreUsingAlternative(parameterAlternative);
         var updateValueOriginCommand = new UpdateParametersValueOriginCommand(allNonDefaultParameters, newValueOrigin, shouldChangeBuildingBlockVersion);

         return updateValueOriginCommand.Run(_executionContext);
      }

      public IEnumerable<IParameter> PermeabilityValuesFor(Compound compound)
      {
         return permeabilityParametersFor(compound, CoreConstants.Parameters.PERMEABILITY);
      }

      public IEnumerable<IParameter> IntestinalPermeabilityValuesFor(Compound compound)
      {
         return permeabilityParametersFor(compound, CoreConstants.Parameters.SPECIFIC_INTESTINAL_PERMEABILITY);
      }

      public ICommand SetDefaultAlternativeFor(ParameterAlternativeGroup parameterGroup, ParameterAlternative parameterAlternative)
      {
         return new SetDefaultAlternativeParameterCommand(parameterGroup, parameterAlternative, _executionContext).Run(_executionContext);
      }

      public ICommand SetSpeciesForAlternative(ParameterAlternativeWithSpecies parameterAlternative, Species species)
      {
         return new SetSpeciesInSpeciesDependentEntityCommand(parameterAlternative, species, _executionContext).Run(_executionContext);
      }

      public TableFormula SolubilityTableForPh(ParameterAlternative solubilityAlternative, Compound compound)
      {
         //Sol(pH) = ref_Solubility * Solubility_Factor (ref_pH) / Solubility_Factor(pH) 
         //Solubility_pKa_pH_Factor

         var refPh = solubilityAlternative.Parameter(CoreConstants.Parameters.REFERENCE_PH);
         var refSolubility = solubilityAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH);
         var gainPerCharge = solubilityAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_GAIN_PER_CHARGE);
         var refSolubilityValue = refSolubility.Value;

         var formula = _formulaFactory.CreateTableFormula()
            .WithName(PKSimConstants.UI.Solubility)
            .InitializedWith(PKSimConstants.UI.pH, PKSimConstants.UI.Solubility, refPh.Dimension, refSolubility.Dimension);

         compound.Parameter(CoreConstants.Parameters.REFERENCE_PH).Value = refPh.Value;
         compound.Parameter(CoreConstants.Parameters.SOLUBILITY_GAIN_PER_CHARGE).Value = gainPerCharge.Value;

         double solFactorRefpH = compound.Parameter(CoreConstants.Parameters.SOLUBILITY_P_KA__P_H_FACTOR).Value;
         var allPh = new List<double>();
         int ph = 0;
         while (ph <= 13)
         {
            allPh.AddRange(new[] {ph, ph + 0.5});
            ph++;
         }
         allPh.Add(14);
         foreach (var pH in allPh)
         {
            compound.Parameter(CoreConstants.Parameters.REFERENCE_PH).Value = pH;
            double solFactorAtpH = compound.Parameter(CoreConstants.Parameters.SOLUBILITY_P_KA__P_H_FACTOR).Value;
            formula.AddPoint(pH, refSolubilityValue * solFactorRefpH / solFactorAtpH);
         }

         return formula;
      }

      private IEnumerable<IParameter> permeabilityParametersFor(Compound compound, string permeabilityParameterName)
      {
         //create a temp compound from the compound factory
         //retrieve the lipophilicty alternatives
         var lipophilictyGroup = compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_LIPOPHILICITY);
         foreach (var alternative in lipophilictyGroup.AllAlternatives)
         {
            var tempCompound = _compoundFactory.Create();
            tempCompound.Parameter(CoreConstants.Parameters.IS_SMALL_MOLECULE).Value = compound.Parameter(CoreConstants.Parameters.IS_SMALL_MOLECULE).Value;
            tempCompound.Parameter(CoreConstants.Parameters.EFFECTIVE_MOLECULAR_WEIGHT).Value = compound.Parameter(CoreConstants.Parameters.EFFECTIVE_MOLECULAR_WEIGHT).Value;
            tempCompound.Parameter(CoreConstants.Parameters.LIPOPHILICITY).Value = alternative.Parameter(CoreConstants.Parameters.LIPOPHILICITY).Value;
            var permParameter = tempCompound.Parameter(permeabilityParameterName);
            permParameter.Editable = false;
            permParameter.Name = alternative.Name;
            yield return permParameter;
         }
      }
   }
}