using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Core.Services
{
   public interface IIndividualScalingTask
   {
      IEnumerable<ParameterScaling> AllParameterScalingsFrom(Individual originIndividual, Individual targetIndividual);
      IEnumerable<ICommand> PerformScaling(IEnumerable<ParameterScaling> parameterScalings);
   }

   public class IndividualScalingTask : IIndividualScalingTask
   {
      private readonly IScalingMethodTask _scalingMethodTask;
      private readonly IContainerTask _containerTask;

      public IndividualScalingTask(IScalingMethodTask scalingMethodTask, IContainerTask containerTask)
      {
         _scalingMethodTask = scalingMethodTask;
         _containerTask = containerTask;
      }

      public IEnumerable<ParameterScaling> AllParameterScalingsFrom(Individual originIndividual, Individual targetIndividual)
      {
         //only show parameter for scaling that are visble parameters
         var allTargetParameters = _containerTask.CacheAllChildren<IParameter>(targetIndividual);

         //default individul based on origin indivudal (used to retrieve default value)
         var allOriginParameters = _containerTask.CacheAllChildrenSatisfying<IParameter>(originIndividual, scalingAllowedFor);

         //return parameter scaling for all parameters existing in the source and target compartment
         return from originParameter in allOriginParameters.KeyValues
            let targetParameter = allTargetParameters[originParameter.Key]
            where scalingRequiredFor(originParameter.Value, targetParameter)
            select parameterScalingFor(originParameter.Value, targetParameter);
      }

      private bool scalingRequiredFor(IParameter originParameter, IParameter targetParameter)
      {
         if (targetParameter == null)
            return false;

         //parameter was marked as changed by user
         if (originParameter.IsFixedValue)
            return true;

         //value is not constant and was not fixed. Nothing to do by default
         if (!originParameter.Formula.IsConstant())
            return false;

         //origin parameter is a constant value that was not changed, return false
         return !ValueComparer.AreValuesEqual(originParameter.Value, originParameter.DefaultValue.GetValueOrDefault(originParameter.Value));
      }

      private bool scalingAllowedFor(IParameter parameter)
      {
         //editable parameter could not have been changed by user
         if (!parameter.Editable)
            return false;

         //same goes for visible parameters
         if (!parameter.Visible)
            return false;

         if (parameter.IsExpressionOrOntogenyFactor() || parameter.NameIsOneOf(AllPlasmaProteinOntogenyFactors.Union(AllPlasmaProteinOntogenyFactorTables))) 
            return false;

         //other conditions
         return true;
      }

      public IEnumerable<ICommand> PerformScaling(IEnumerable<ParameterScaling> parameterScalings)
      {
         var macroCommand = new List<ICommand>();
         parameterScalings.Each(item => macroCommand.Add(item.Scale()));
         return macroCommand;
      }

      private ParameterScaling parameterScalingFor(IParameter sourceParameter, IParameter targetParameter)
      {
         var parameterScaling = new ParameterScaling(sourceParameter, targetParameter);
         parameterScaling.ScalingMethod = _scalingMethodTask.DefaultMethodFor(parameterScaling);
         return parameterScaling;
      }
   }
}