using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.Extensions;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Core.Model
{
   public static class ParameterExtensions
   {
      public static double GetPercentile(this IParameter parameter)
      {
         var distributedParameter = parameter as IDistributedParameter;
         return distributedParameter?.Percentile ?? 0;
      }

      public static void SetPercentile(this IParameter parameter, double percentile)
      {
         if (!(parameter is IDistributedParameter distributedParameter))
            return;

         distributedParameter.Percentile = percentile;
      }

      public static bool IsIndividualMoleculeGlobal(this IParameter parameter) =>
         CoreConstants.Parameters.AllGlobalMoleculeParameters.Contains(parameter.Name);

      public static bool IsExpressionOrOntogenyFactor(this IParameter parameter)
      {
         if (parameter.IsExpression())
            return true;

         if (OntogenyFactors.Contains(parameter.Name))
            return true;

         return false;
      }

      public static bool IsExpressionProfile(this IParameter parameter)
      {
         return parameter.IsExpression() ||
                IsIndividualMoleculeGlobal(parameter) ||
                parameter.IsNamed(INITIAL_CONCENTRATION) ||
                parameter.Name.StartsWith(FRACTION_EXPRESSED_PREFIX);
      }

      public static bool IsStructural(this IParameter parameter)
      {
         return ParticleDistributionStructuralParameters.Contains(parameter.Name);
      }

      public static bool IsOrganVolume(this IParameter parameter)
      {
         return parameter.IsNamed(Constants.Parameters.VOLUME) &&
                parameter.ParentContainer.IsAnImplementationOf<Organ>();
      }

      public static TParameter WithInfo<TParameter>(this TParameter parameter, ParameterInfo info) where TParameter : IParameter
      {
         parameter.Info.UpdatePropertiesFrom(info);
         return parameter;
      }

      public static bool NeedsDefault(this IParameter parameter)
      {
         if (parameter.NameIsOneOf(AllDistributionParameters))
            return false;

         if (!parameter.BuildingBlockType.IsOneOf(PKSimBuildingBlockType.Individual, PKSimBuildingBlockType.Population,
                PKSimBuildingBlockType.Simulation))
            return false;

         if (parameter.Formula == null)
            return false;

         if (!parameter.IsDefault)
            return false;

         // Default only for constant parameter or distribute parameters
         return parameter.Formula.IsConstant() || parameter.Formula.IsDistributed();
      }

      public static bool ValueDiffersFromDefault(this IParameter parameter)
      {
         if (!parameter.Editable)
            return false;

         if (parameter.Formula.IsExplicit() || parameter.Formula.IsDistributed())
            return parameter.IsFixedValue;

         if (parameter.DefaultValue == null)
            return false;

         return !ValueComparer.AreValuesEqual(parameter.Value, parameter.DefaultValue.Value, CoreConstants.DOUBLE_RELATIVE_EPSILON);
      }

      public static bool ShouldExportToSnapshot(this IParameter parameter)
      {
         if (parameter == null)
            return false;

         //For a molecule, we export all global parameters to ensure that they do not get out of sync when loading from snapshot 
         if (parameter.IsIndividualMoleculeGlobal())
            return true;

         if (parameter.IsDefault)
            return false;

         return parameter.ValueIsDefined();
      }

      /// <summary>
      ///    Returns <c>true</c> if the value can be computed and is not NaN otherwise <c>false</c>
      /// </summary>
      public static bool ValueIsDefined(this IParameter parameter)
      {
         if (!ValueIsComputable(parameter))
            return false;

         return !double.IsNaN(parameter.Value);
      }

      /// <summary>
      ///    Returns <c>true</c> if the value can be computed otherwise <c>false</c>
      /// </summary>
      public static bool ValueIsComputable(this IParameter parameter)
      {
         try
         {
            //let's compute the value. Exception will be thrown if value cannot be calculated
            var v = parameter.Value;
            return true;
         }
         catch (Exception)
         {
            //this is a parameter that cannot be evaluated and should not be exported
            return false;
         }
      }

      /// <summary>
      ///    Returns the factor with which the value was changed from current value
      /// </summary>
      public static double ScaleFactor(this IParameter parameter)
      {
         if (parameter == null || !parameter.ValueDiffersFromDefault())
            return 1;

         //What should be done here?
         var defaultValue = parameter.DefaultValue.GetValueOrDefault(0);
         if (defaultValue == 0)
            return 1;

         return parameter.Value / defaultValue;
      }

      public static bool CanBeDefinedAsAdvanced(this IParameter parameter)
      {
         if (parameter == null)
            return false;

         return parameter.CanBeVariedInPopulation && !parameter.IsChangedByCreateIndividual;
      }

      public static IReadOnlyList<IParameter> AllGlobalMoleculeParameters(this IReadOnlyList<IParameter> parameters)
      {
         return new[]
         {
            parameters.FindByName(REFERENCE_CONCENTRATION),
            parameters.FindByName(HALF_LIFE_LIVER),
            parameters.FindByName(HALF_LIFE_INTESTINE)
         }.Where(x => x != null).ToList();
      }

      public static IReadOnlyList<IParameter> AllExpressionParameters(this IReadOnlyList<IParameter> parameters)
      {
         return parameters.Except(AllGlobalMoleculeParameters(parameters)).ToList();
      }

      public static void ScaleDistributionBasedOn(this IDistributedParameter currentParameter, IDistributedParameter baseParameter)
      {
         ScaleDistributionBasedOn(currentParameter, baseParameter?.ScaleFactor());
      }

      public static void ScaleDistributionBasedOn(this IDistributedParameter parameter, double? factor)
      {
         var factorValue = factor.GetValueOrDefault(1);
         if (factorValue == 1)
            return;

         parameter.MeanParameter.Value *= factorValue;

         if (parameter.Formula.DistributionType() == DistributionTypes.Normal)
            parameter.DeviationParameter.Value *= factorValue;

         parameter.IsFixedValue = false;
      }
   }

   public static class ExpressionParameterExtensions
   {
      public static string ContainerNameForRelativeExpressionParameter(this ExpressionParameter expressionParameter)
      {
         if (expressionParameter.HasGlobalExpressionName())
            return CoreConstants.ContainerName.GlobalExpressionContainerNameFor(expressionParameter.Name);

         var objectPath = expressionParameter.Path;
         var containerName = grandParent(objectPath);
         var key = Equals(containerName, CoreConstants.Compartment.INTRACELLULAR) ? greatGrandParent(objectPath) : containerName;
         if (expressionParameter.ContainerPath.Contains(CoreConstants.Organ.LUMEN))
            key = CoreConstants.ContainerName.LumenSegmentNameFor(key);

         return key;
      }

      private static string greatGrandParent(ObjectPath objectPath)
      {
         return ancestorFrom(objectPath, generations: 4);
      }

      private static string grandParent(ObjectPath objectPath)
      {
         return ancestorFrom(objectPath, generations: 3);
      }

      private static string ancestorFrom(ObjectPath objectPath, int generations)
      {
         if (objectPath.Count >= generations)
            return objectPath[objectPath.Count - generations];

         return objectPath.FirstOrDefault();
      }
   }
}