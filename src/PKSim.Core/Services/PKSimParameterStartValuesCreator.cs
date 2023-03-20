using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IPKSimParameterStartValuesCreator
   {
      ParameterStartValuesBuildingBlock CreateFor(IBuildConfiguration buildConfiguration, Simulation simulation);
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

      public ParameterStartValuesBuildingBlock CreateFor(IBuildConfiguration buildConfiguration, Simulation simulation)
      {
         //default default parameter start values matrix
         _defaultStartValues = _objectBaseFactory.Create<ParameterStartValuesBuildingBlock>();
         updateSimulationParameters(simulation);
         return _defaultStartValues.WithName(simulation.Name);
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