using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class CompoundPropertiesTeXBuilder : OSPSuiteTeXBuilder<CompoundProperties>
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly ICompoundAlternativeTask _compoundAlternativeTask;
      private readonly ITeXBuilderRepository _builderRepository;

      public CompoundPropertiesTeXBuilder(IRepresentationInfoRepository representationInfoRepository, ICompoundAlternativeTask compoundAlternativeTask, ITeXBuilderRepository builderRepository)
      {
         _representationInfoRepository = representationInfoRepository;
         _compoundAlternativeTask = compoundAlternativeTask;
         _builderRepository = builderRepository;
      }

      public override void Build(CompoundProperties compoundProperties, OSPSuiteTracker buildTracker)
      {
         var objectsToReport = new List<object>();
         var compoundConfig = new TablePart(PKSimConstants.UI.Parameter, PKSimConstants.UI.AlternativeInCompound, PKSimConstants.UI.Value, PKSimConstants.UI.Unit)
            {
               Caption = PKSimConstants.UI.CompoundConfiguration
            };
         compoundConfig.Types[PKSimConstants.UI.Value] = typeof (double);

         var compound = compoundProperties.Compound;
         foreach (var alternativeSelection in compoundProperties.CompoundGroupSelections)
         {
            var parameterName = _representationInfoRepository.DisplayNameFor(RepresentationObjectType.GROUP, alternativeSelection.GroupName);
            var parameter = getParameterForAlternative(compound, compoundProperties, alternativeSelection);
            compoundConfig.AddIs(parameterName, alternativeSelection.AlternativeName, ParameterMessages.DisplayValueFor(parameter, numericalDisplayOnly: true), ParameterMessages.DisplayUnitFor(parameter));
         }

         objectsToReport.Add(buildTracker.CreateRelativeStructureElement(PKSimConstants.UI.CompoundConfiguration, 2));
         objectsToReport.Add(compoundConfig);

         objectsToReport.Add(compoundProperties.AllCalculationMethods().Where(cm => cm.Category.IsOneOf(CoreConstants.Category.DistributionCellular, CoreConstants.Category.DistributionInterstitial, CoreConstants.Category.DiffusionIntCell)));

         _builderRepository.Report(objectsToReport, buildTracker);
      }



      private IParameter getParameterForAlternative(Compound compound, CompoundProperties compoundProperties, CompoundGroupSelection alternativeSelection)
      {
         if (compound == null)
            return null;

         var alternativeGroup = compound.ParameterAlternativeGroup(alternativeSelection.GroupName);
         if (alternativeGroup == null)
            return null;

         var alternative = alternativeGroup.AlternativeByName(alternativeSelection.AlternativeName);
         if (alternative == null)
            return null;

         var parameter = alternative.AllVisibleParameters().FirstOrDefault();
         if (parameter == null)
            return null;

         if (!CoreConstants.Groups.GroupsWithCalculatedAlternative.Contains(alternativeSelection.GroupName))
            return parameter;

         //this is a calculated alternative. We need to retrieve the value depending on the selected lipophilicity
         IEnumerable<IParameter> allParameters;
         if (alternativeGroup.Name == CoreConstants.Groups.COMPOUND_PERMEABILITY)
            allParameters = _compoundAlternativeTask.PermeabilityValuesFor(compound);
         else
            allParameters = _compoundAlternativeTask.IntestinalPermeabilityValuesFor(compound);

         var lipophilicityName = compoundProperties.CompoundGroupSelections
            .Where(x => x.GroupName == CoreConstants.Groups.COMPOUND_LIPOPHILICITY)
            .Select(x => x.AlternativeName).FirstOrDefault();

         return allParameters.FirstOrDefault(x => x.IsNamed(lipophilicityName));
      }
   }
}