using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Populations
{
   public static class RandomPopulationItems
   {
      public static readonly PopulationItem<IRandomPopulationSettingsPresenter> Settings = new PopulationItem<IRandomPopulationSettingsPresenter>();
      public static readonly PopulationItem<IPopulationMoleculesPresenter> Molecules = new PopulationItem<IPopulationMoleculesPresenter>();
      public static readonly PopulationItem<IPopulationAdvancedParametersPresenter> AdvancedParameters = new PopulationItem<IPopulationAdvancedParametersPresenter>();
      public static readonly PopulationItem<IPopulationAdvancedParameterDistributionPresenter> ParameterDistribution = new PopulationItem<IPopulationAdvancedParameterDistributionPresenter>();
      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> { Settings, Molecules, AdvancedParameters, ParameterDistribution };
   }

   public static class ImportPopulationItems
   {
      public static readonly PopulationItem<IImportPopulationSettingsPresenter> ImportSettings = new PopulationItem<IImportPopulationSettingsPresenter>();
      public static readonly PopulationItem<IPopulationMoleculesPresenter> Molecules = new PopulationItem<IPopulationMoleculesPresenter>();
      public static readonly PopulationItem<IPopulationAdvancedParametersPresenter> AdvancedParameters = new PopulationItem<IPopulationAdvancedParametersPresenter>();
      public static readonly PopulationItem<IPopulationAdvancedParameterDistributionPresenter> ParameterDistribution = new PopulationItem<IPopulationAdvancedParameterDistributionPresenter>();
      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> { ImportSettings, Molecules,  AdvancedParameters, ParameterDistribution };
   }

   public class PopulationItem< TPopulationItemPresenter> : SubPresenterItem<TPopulationItemPresenter> where TPopulationItemPresenter : IPopulationItemPresenter
   {
   }
}