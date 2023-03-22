using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IPKSimParameterStartValuesCreator
   {
      ParameterStartValuesBuildingBlock CreateFor(SimulationConfiguration simulationConfiguration, Simulation simulation);
   }

   public class PKSimParameterStartValuesCreator : IPKSimParameterStartValuesCreator
   {
      private readonly IParameterStartValuesCreator _parameterStartValuesCreator;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IEntityPathResolver _entityPathResolver;
      private ParameterStartValuesBuildingBlock _defaultStartValues;

      public PKSimParameterStartValuesCreator(
         IParameterStartValuesCreator parameterStartValuesCreator,
         IObjectBaseFactory objectBaseFactory,
         IEntityPathResolver entityPathResolver)
      {
         _parameterStartValuesCreator = parameterStartValuesCreator;
         _objectBaseFactory = objectBaseFactory;
         _entityPathResolver = entityPathResolver;
      }

      public ParameterStartValuesBuildingBlock CreateFor(SimulationConfiguration simulationConfiguration, Simulation simulation)
      {
         try
         {
            //default default parameter start values matrix
            _defaultStartValues = _objectBaseFactory.Create<ParameterStartValuesBuildingBlock>();
            var individual = simulation.Individual;

            //set the relative expression values for each molecule undefined molecule of the individual (other will be done in expression profile)
            individual.AllUndefinedMolecules().Each(molecule => updateMoleculeParametersValues(molecule, individual));

            updateSimulationParameters(simulation);
            return _defaultStartValues.WithName(simulation.Name);
         }
         finally
         {
            _defaultStartValues = null;
         }
      }

      private void updateMoleculeParametersValues(IndividualMolecule molecule, Individual individual)
      {
         var allMoleculeParameters = individual.AllMoleculeParametersFor(molecule);
         allMoleculeParameters.Each(p => trySetValue(p));
      }

      private void updateSimulationParameters(Simulation simulation)
      {
         //this is only required if the simulation already has a model. That means that we should update the PSV with any
         //simulation parameters that might have been updated by the user
         if (simulation.Model == null)
            return;

         //TODO: Ensure that the formula will not become a constant after clone
         //THis was done with  psv.OverrideFormulaWithValue = false;
         var allSimulationParameters = simulation.Model.Root.GetAllChildren<IParameter>(isChangedSimulationParameter);
         allSimulationParameters.Each(p => trySetValue(p));
      }

      private bool isChangedSimulationParameter(IParameter parameter)
      {
         return parameter.BuildingBlockType.Is(PKSimBuildingBlockType.Simulation)
                && parameter.ValueDiffersFromDefault();
      }

      private ParameterStartValue trySetValue(IParameter parameter)
      {
         var parameterStartValue = getOrCreateStartValueFor(parameter);
         parameterStartValue.StartValue = parameter.Value;
         return parameterStartValue;
      }

      private ParameterStartValue getOrCreateStartValueFor(IParameter parameter)
      {
         var parameterPath = _entityPathResolver.ObjectPathFor(parameter);
         var parameterStartValue = _defaultStartValues[parameterPath];
         if (parameterStartValue != null)
            return parameterStartValue;

         parameterStartValue = _parameterStartValuesCreator.CreateParameterStartValue(parameterPath, parameter);
         _defaultStartValues.Add(parameterStartValue);

         return parameterStartValue;
      }
   }
}