using PKSim.Core.Model;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface IEditRandomPopulationPresenter : IEditPopulationPresenter<RandomPopulation>
   {
   }

   public class EditRandomPopulationPresenter : EditPopulationPresenter<IEditRandomPopulationView, IEditRandomPopulationPresenter, RandomPopulation>, IEditRandomPopulationPresenter
   {
      public EditRandomPopulationPresenter(IEditRandomPopulationView view, ISubPresenterItemManager<IPopulationItemPresenter> subPresenterItemManager)
         : base(view, subPresenterItemManager, RandomPopulationItems.All)
      {
      }

      protected override ISubPresenterItem<IPopulationSettingsPresenter<RandomPopulation>> SettingPresenterItem
      {
         get { return RandomPopulationItems.Settings; }
      }

      protected override ISubPresenterItem<IPopulationAdvancedParameterDistributionPresenter> DistributionPresenterItem
      {
         get { return RandomPopulationItems.ParameterDistribution; }
      }

      protected override ISubPresenterItem<IPopulationAdvancedParametersPresenter> AdvancedParameterPresenterItem
      {
         get { return RandomPopulationItems.AdvancedParameters; }
      }

      protected override ISubPresenterItem<IPopulationMoleculesPresenter> MoleculesPresenterItem
      {
         get { return RandomPopulationItems.Molecules; }
      }
   }
}