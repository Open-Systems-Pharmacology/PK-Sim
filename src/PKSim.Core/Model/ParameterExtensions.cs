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

         if (parameter.IsExpressionNorm())
            return true;

         return parameter.NameIsOneOf(CoreConstants.Parameter.REL_EXP, CoreConstants.Parameter.REL_EXP_BLOOD_CELL,
            CoreConstants.Parameter.REL_EXP_PLASMA, CoreConstants.Parameter.REL_EXP_VASC_ENDO);
      }

      public static bool IsIndividualMolecule(this IParameter parameter)
      {
         if (parameter.IsExpression())
            return true;

         if (CoreConstants.Parameter.OntogenyFactors.Contains(parameter.Name))
            return true;

         if (parameter.IsNamed(CoreConstants.Parameter.REFERENCE_CONCENTRATION))
            return true;

         return false;
      }

      public static bool IsStructural(this IParameter parameter)
      {
         return CoreConstants.Parameter.ParticleDistributionStructuralParameters.Contains(parameter.Name);
      }

      public static bool IsExpressionNorm(this IParameter parameter)
      {
         if (parameter == null) return false;
         return parameter.NameIsOneOf(CoreConstants.Parameter.REL_EXP_NORM, CoreConstants.Parameter.REL_EXP_BLOOD_CELL_NORM,
            CoreConstants.Parameter.REL_EXP_PLASMA_NORM, CoreConstants.Parameter.REL_EXP_VASC_ENDO_NORM);
      }

      public static bool IsOrganVolume(this IParameter parameter)
      {
         return parameter.IsNamed(Constants.Parameters.VOLUME) &&
                parameter.ParentContainer.IsAnImplementationOf<Organ>();
      }

      public static bool CanBeDisplayedInAllView(this IParameter parameter)
      {
         return !parameter.IsExpressionNorm();
      }

      public static TParameter WithInfo<TParameter>(this TParameter parameter, ParameterInfo info) where TParameter : IParameter
      {
         parameter.Info = info;
         return parameter;
      }

      public static bool NeedsDefault(this IParameter parameter)
      {
         if (parameter.NameIsOneOf(CoreConstants.Parameter.AllDistributionParameters))
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

      //TODO Use ValueOrigin state when implemented. It should be != than PKSim default and probably mereged with method above
      // This is just a dymmy method for now to satisfy requalification project
      public static bool ParameterHasChanged(this IParameter parameter)
      {
         var canBeEdited = parameter.Visible && parameter.Formula.IsConstant();
         return parameter.ValueDiffersFromDefault() || canBeEdited && parameter.Value != 0;
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