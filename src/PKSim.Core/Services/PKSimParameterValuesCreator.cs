using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IPKSimParameterValuesCreator
   {
      ParameterValuesBuildingBlock CreateFor(Simulation simulation);
   }

   public class PKSimParameterValuesCreator : IPKSimParameterValuesCreator
   {
      private readonly IParameterValuesCreator _parameterValuesCreator;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IEntityPathResolver _entityPathResolver;
      private ParameterValuesBuildingBlock _defaultValues;

      public PKSimParameterValuesCreator(
         IParameterValuesCreator parameterValuesCreator,
         IObjectBaseFactory objectBaseFactory,
         IEntityPathResolver entityPathResolver)
      {
         _parameterValuesCreator = parameterValuesCreator;
         _objectBaseFactory = objectBaseFactory;
         _entityPathResolver = entityPathResolver;
      }

      public ParameterValuesBuildingBlock CreateFor(Simulation simulation)
      {
         try
         {
            //default default parameter start values matrix
            _defaultValues = _objectBaseFactory.Create<ParameterValuesBuildingBlock>();
            var individual = simulation.Individual;

            //set the relative expression values for each molecule undefined molecule of the individual (other will be done in expression profile)
            individual.AllUndefinedMolecules().Each(molecule => updateMoleculeParametersValues(molecule, individual));

            updateSimulationParameters(simulation);
            return _defaultValues.WithName(simulation.Name);
         }
         finally
         {
            _defaultValues = null;
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

      private ParameterValue trySetValue(IParameter parameter)
      {
         var parameterValue = getOrCreateParameterValueFor(parameter);
         parameterValue.Value = parameter.Value;
         return parameterValue;
      }

      private ParameterValue getOrCreateParameterValueFor(IParameter parameter)
      {
         var parameterPath = _entityPathResolver.ObjectPathFor(parameter);
         var parameterValue = _defaultValues[parameterPath];
         if (parameterValue != null)
            return parameterValue;

         parameterValue = _parameterValuesCreator.CreateParameterValue(parameterPath, parameter);
         _defaultValues.Add(parameterValue);

         return parameterValue;
      }
   }
}