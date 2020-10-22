using System;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
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
      private ISpatialStructure _spatialStructure;
      private IParameterStartValuesBuildingBlock _defaultStartValues;
      private IFormulaCache _formulaCache;

      public PKSimParameterStartValuesCreator(
         IParameterStartValuesCreator parameterStartValuesCreator, 
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
            _spatialStructure = buildConfiguration.SpatialStructure;
            _defaultStartValues = _parameterStartValuesCreator.CreateFrom(_spatialStructure, buildConfiguration.Molecules);
            _formulaCache = _defaultStartValues.FormulaCache;
            var individual = simulation.Individual;

            //set the relative expression values for each molecule defined in individual
            foreach (var molecule in individual.AllMolecules())
            {
               updateMoleculeParametersValues(molecule, individual);
            }
            
            updateSimulationParameters(simulation);

            return _defaultStartValues.WithName(simulation.Name);
         }
         finally
         {
            _spatialStructure = null;
            _defaultStartValues = null;
            _formulaCache = null;
         }
      }

      private void updateSimulationParameters(Simulation simulation)
      {
         //this is only required if the simulation already has a model. That means that we should update the PSV with any
         //simulation parameters that might have been updated by the user
         if (simulation.Model == null) return;
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

      private void updateMoleculeParametersValues(IndividualMolecule molecule, Individual individual)
      {
         var allProteinParameters = individual.AllMoleculeContainersFor(molecule).SelectMany(x => x.AllParameters()).ToList();
         allProteinParameters.Each(x => setParameter(x));
      }

      private IParameterStartValue setParameter(IParameter parameter)
      {
         var parameterPath = _entityPathResolver.ObjectPathFor(parameter);
         var parameterStartValue =_defaultStartValues[parameterPath] ?? _parameterStartValuesCreator.CreateParameterStartValue(parameterPath, parameter);

         if (parameter.Formula.IsExplicit())
         {
            var formula = _formulaFactory.RateFor(CoreConstants.CalculationMethod.EXPRESSION_PARAMETERS, parameter.Formula.Name, _formulaCache);
            parameterStartValue.Formula = formula;
            //There if a formula, make sure we use it. This flag will be overwritten if the value was set by user
            //TODO CHECK
            parameterStartValue.OverrideFormulaWithValue = false;
         }

         if (parameter.IsConstantParameter())
         {
            parameterStartValue.StartValue = parameter.Value;
            //TODO CHECK
            parameterStartValue.OverrideFormulaWithValue = true;
         }

         _defaultStartValues[parameterPath] = parameterStartValue;
         return parameterStartValue;
      }


      private IParameterStartValue trySetValue(IParameter parameter)
      {
         var parameterPath = _entityPathResolver.ObjectPathFor(parameter);
         if (_defaultStartValues[parameterPath] != null)
            return trySetValue(parameterPath, parameter);

         var parameterStartValue = _parameterStartValuesCreator.CreateParameterStartValue(parameterPath, parameter);
         _defaultStartValues.Add(parameterStartValue);
         return parameterStartValue;
      }

      private IParameterStartValue trySetValue(IObjectPath parameterPath, IParameter parameter)
      {
         return trySetValue(parameterPath, parameter.Value);
      }

      private IParameterStartValue trySetValue(IObjectPath objectPath, double value)
      {
         var parameterStartValue = _defaultStartValues[objectPath];
         if (parameterStartValue == null)
            throw new Exception($"Parameter not found : {objectPath}");

         parameterStartValue.StartValue = value;
         return parameterStartValue;
      }

   }
}