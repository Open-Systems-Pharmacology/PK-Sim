using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;

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
         var distributedParameter = parameter as IDistributedParameter;
         if (distributedParameter == null) return;
         distributedParameter.Percentile = percentile;
      }

      public static bool IsExpression(this IParameter parameter)
      {
         if (parameter == null)
            return false;

         return parameter.NameIsOneOf(CoreConstants.Parameters.REL_EXP, CoreConstants.Parameters.REL_EXP_BLOOD_CELL,
            CoreConstants.Parameters.REL_EXP_PLASMA, CoreConstants.Parameters.REL_EXP_VASC_ENDO);
      }

      public static bool IsExpressionOrOntogenyFactor(this IParameter parameter)
      {
         if (parameter.IsExpression())
            return true;

         if (CoreConstants.Parameters.OntogenyFactors.Contains(parameter.Name))
            return true;

         return false;
      }

      public static bool IsIndividualMolecule(this IParameter parameter)
      {
         return IsExpressionOrOntogenyFactor(parameter) || IsIndividualMoleculeGlobal(parameter);
      }

      public static bool IsIndividualMoleculeGlobal(this IParameter parameter) => CoreConstants.Parameters.AllGlobalMoleculeParameters.Contains(parameter.Name);

      public static bool IsStructural(this IParameter parameter)
      {
         return CoreConstants.Parameters.ParticleDistributionStructuralParameters.Contains(parameter.Name);
      }

      public static bool IsOrganVolume(this IParameter parameter)
      {
         return parameter.IsNamed(Constants.Parameters.VOLUME) &&
                parameter.ParentContainer.IsAnImplementationOf<Organ>();
      }

      public static TParameter WithInfo<TParameter>(this TParameter parameter, ParameterInfo info) where TParameter : IParameter
      {
         parameter.Info = info;
         return parameter;
      }

      public static bool NeedsDefault(this IParameter parameter)
      {
         if (parameter.NameIsOneOf(CoreConstants.Parameters.AllDistributionParameters))
            return false;

         if (!parameter.BuildingBlockType.IsOneOf(PKSimBuildingBlockType.Individual, PKSimBuildingBlockType.Population, PKSimBuildingBlockType.Simulation))
            return false;

         if (parameter.Formula == null)
            return false;

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
      ///    Returns the factor with which the value was changed from current vlaue
      /// </summary>
      public static double ScaleFactor(this IParameter parameter)
      {
         if (!parameter.ValueDiffersFromDefault())
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

      public static IReadOnlyList<IParameter> AllRelatedRelativeExpressionParameters(this IParameter relativeExpressionParameter)
      {
         var moleculeContainer = relativeExpressionParameter.ParentContainer;
         var rootContainer = relativeExpressionParameter.RootContainer;
         return rootContainer.GetAllChildren<IParameter>(x => string.Equals(x.GroupName, CoreConstants.Groups.RELATIVE_EXPRESSION))
            .Where(x => x.BuildingBlockType == PKSimBuildingBlockType.Individual)
            .Where(x => moleculeContainer.IsNamed(x.ParentContainer.Name))
            .Where(x => x.IsExpression())
            .Where(x => x.Formula.IsConstant()).ToList();
      }
   }
}