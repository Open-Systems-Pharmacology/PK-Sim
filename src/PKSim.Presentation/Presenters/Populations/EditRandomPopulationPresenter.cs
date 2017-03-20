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
         : base(view, subPresenterItemManager, RamdomPopulationItems.All)
      {
      }

      protected override ISubPresenterItem<IPopulationSettingsPresenter<RandomPopulation>> SettingPresenterItem
      {
         get { return RamdomPopulationItems.Settings; }
      }

      protected override ISubPresenterItem<IPopulationAdvancedParameterDistributionPresenter> DistributionPresenterItem
      {
         get { return RamdomPopulationItems.ParameterDistribution; }
      }

      protected override ISubPresenterItem<IPopulationAdvancedParametersPresenter> AdvancedParameterPresenterItem
      {
         get { return RamdomPopulationItems.AdvancedParameters; }
      }

      protected override ISubPresenterItem<IPopulationMoleculesPresenter> MoleculesPresenterItem
      {
         get { return RamdomPopulationItems.Molecules; }
      }
   }
}