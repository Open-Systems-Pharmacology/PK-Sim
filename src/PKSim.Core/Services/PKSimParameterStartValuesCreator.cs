using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using static PKSim.Core.CoreConstants.CalculationMethod;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Core.Services
{
   public interface IPKSimParameterStartValuesCreator
   {
      IParameterStartValuesBuildingBlock CreateFor(IBuildConfiguration buildConfiguration, Simulation simulation);
   }

   public class PKSimParameterStartValuesCreator : IPKSimParameterStartValuesCreator
   {
      private readonly IParameterStartValuesCreator _parameterStartValuesCreator;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IEntityPathResolver _entityPathResolver;
      private IParameterStartValuesBuildingBlock _defaultStartValues;

      public PKSimParameterStartValuesCreator(IParameterStartValuesCreator parameterStartValuesCreator,
         IFormulaFactory formulaFactory,
         IEntityPathResolver entityPathResolver)
      {
         _parameterStartValuesCreator = parameterStartValuesCreator;
         _formulaFactory = formulaFactory;
         _entityPathResolver = entityPathResolver;
      }

      public IParameterStartValuesBuildingBlock CreateFor(IBuildConfiguration buildConfiguration, Simulation simulation)
      {
         try
         {
            //default default parameter start values matrix
            var spatialStructure = buildConfiguration.SpatialStructure;
            var molecules = buildConfiguration.Molecules;
            _defaultStartValues = _parameterStartValuesCreator.CreateFrom(spatialStructure, molecules);
            var individual = simulation.Individual;

            //set the relative expression values for each molecule defined in individual
            individual.AllMolecules().Each(molecule => updateMoleculeParametersValues(molecule, individual, simulation));

            updateSimulationParameters(simulation);

            return _defaultStartValues.WithName(simulation.Name);
         }
         finally
         {
            _defaultStartValues = null;
         }
      }

      private void updateSimulationParameters(Simulation simulation)
      {
         //this is only required if the simulation already has a model. That means that we should update the PSV with any
         //simulation parameters that might have been updated by the user
         if (simulation.Model == null)
            return;

         var allSimulationParameters = simulation.Model.Root.GetAllChildren<IParameter>(isChangedSimulationParameter);
         allSimulationParameters.Each(p =>
         {
            var psv = trySetValue(p);
            //Ensure that the formula will not become a constant after clone
            psv.OverrideFormulaWithValue = false;
         });
      }

      private bool isChangedSimulationParameter(IParameter parameter)
      {
         return parameter.BuildingBlockType.Is(PKSimBuildingBlockType.Simulation)
                && parameter.ValueDiffersFromDefault();
      }

      private void updateMoleculeParametersValues(IndividualMolecule molecule, Individual individual, Simulation simulation)
      {
         var allMoleculeParameters = individual.AllMoleculeParametersFor(molecule);
         var modelConfiguration = simulation.ModelConfiguration;
         allMoleculeParameters.Each(p => setParameter(p, modelConfiguration.ModelName == CoreConstants.Model.FOUR_COMP));
      }

      private void setParameter(IParameter parameter, bool isSmallMolecule)
      {
         //We do not generate start values for endosome parameters when dealing with a small molecule model
         if (parameter.HasAncestorNamed(CoreConstants.Compartment.ENDOSOME) && isSmallMolecule)
            return;

         //We do not generate value for this parameter by default. Exit
         var parameterStartValue = getStartValueFor(parameter);
         if (parameterStartValue == null)
            return;

         if (parameter.Formula.IsExplicit())
         {
            var formula = _formulaFactory.RateFor(EXPRESSION_PARAMETERS, parameter.Formula.Name, _defaultStartValues.FormulaCache);
            parameterStartValue.Formula = formula;
            //There is a formula, make sure we use it. We set this flag to false to ensure that the formula will not be replaced with a constant formula
            parameterStartValue.OverrideFormulaWithValue = false;
         }

         if (parameter.IsConstantParameter())
         {
            parameterStartValue.StartValue = parameter.Value;
            //we reset the formula to null to ensure that the start value will be used when constructing the simulation
            parameterStartValue.Formula = null;
         }
      }

      private IParameterStartValue trySetValue(IParameter parameter)
      {
         var parameterStartValue = getOrCreateStartValueFor(parameter);
         parameterStartValue.StartValue = parameter.Value;
         return parameterStartValue;
      }

      private IParameterStartValue getOrCreateStartValueFor(IParameter parameter)
      {
         var parameterPath = _entityPathResolver.ObjectPathFor(parameter);
         var parameterStartValue = _defaultStartValues[parameterPath];
         if (parameterStartValue != null)
            return parameterStartValue;

         parameterStartValue = _parameterStartValuesCreator.CreateParameterStartValue(parameterPath, parameter);
         _defaultStartValues.Add(parameterStartValue);

         return parameterStartValue;
      }

      private IParameterStartValue getStartValueFor(IParameter parameter)
      {
         var parameterPath = _entityPathResolver.ObjectPathFor(parameter);
         return _defaultStartValues[parameterPath];
      }
   }
}