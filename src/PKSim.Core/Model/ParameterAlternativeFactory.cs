using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core.Model
{
   public interface IParameterAlternativeFactory
   {
      ParameterAlternative CreateAlternativeFor(ParameterAlternativeGroup compoundParameterGroup);
      ParameterAlternative CreateDefaultAlternativeFor(ParameterAlternativeGroup compoundParameterGroup);
   }

   public class ParameterAlternativeFactory : IParameterAlternativeFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly ICloner _cloner;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly ICoreUserSettings _userSettings;

      public ParameterAlternativeFactory(IObjectBaseFactory objectBaseFactory, ICloner cloner,
         ISpeciesRepository speciesRepository, ICoreUserSettings userSettings)
      {
         _objectBaseFactory = objectBaseFactory;
         _cloner = cloner;
         _speciesRepository = speciesRepository;
         _userSettings = userSettings;
      }

      public ParameterAlternative CreateAlternativeFor(ParameterAlternativeGroup compoundParameterGroup)
      {
         var alternative = createAlternativeFor(compoundParameterGroup);

         //for alternative that are not the default one, and if the groups requires it, we reset the formula to a constant value
         if (groupHasCalculatedAlternative(compoundParameterGroup))
         {
            alternative.AllParameters().Each(p => p.Formula = _objectBaseFactory.Create<ConstantFormula>().WithValue(0));
         }
         return alternative;
      }

      public ParameterAlternative CreateDefaultAlternativeFor(ParameterAlternativeGroup compoundParameterGroup)
      {
         var alternative = createAlternativeFor(compoundParameterGroup);

         alternative.Name = groupHasCalculatedAlternative(compoundParameterGroup)
            ? PKSimConstants.UI.CalculatedAlernative
            : defaultAlternativeNameFor(compoundParameterGroup);
         alternative.IsDefault = true;
         return alternative;
      }

      private string defaultAlternativeNameFor(ParameterAlternativeGroup compoundParameterGroup)
      {
         if (string.Equals(compoundParameterGroup.Name, CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND))
            return _userSettings.DefaultFractionUnboundName;

         if (string.Equals(compoundParameterGroup.Name, CoreConstants.Groups.COMPOUND_LIPOPHILICITY))
            return _userSettings.DefaultLipophilicityName;

         if (string.Equals(compoundParameterGroup.Name, CoreConstants.Groups.COMPOUND_SOLUBILITY))
            return _userSettings.DefaultSolubilityName;

         return PKSimConstants.UI.DefaultAlternative;
      }

      private ParameterAlternative createAlternativeFor(ParameterAlternativeGroup compoundParameterGroup)
      {
         ParameterAlternative alternative;

         if (groupHasAlternativeWithSpecies(compoundParameterGroup))
         {
            var newParamGroupAlternative = _objectBaseFactory.Create<ParameterAlternativeWithSpecies>();
            newParamGroupAlternative.Species = _speciesRepository.DefaultSpecies;
            alternative = newParamGroupAlternative;
         }
         else
         {
            alternative = _objectBaseFactory.Create<ParameterAlternative>();
         }

         alternative.Name = string.Empty;

         foreach (var param in compoundParameterGroup.TemplateParameters)
            alternative.Add(_cloner.Clone(param));

         return alternative;
      }

      private bool groupHasCalculatedAlternative(ParameterAlternativeGroup group)
      {
         return CoreConstants.Groups.GroupsWithCalculatedAlternative.Contains(group.Name);
      }

      private bool groupHasAlternativeWithSpecies(ParameterAlternativeGroup group)
      {
         return CoreConstants.Groups.GroupsWithAlternativeAndSpecies.Contains(group.Name);
      }
   }
}