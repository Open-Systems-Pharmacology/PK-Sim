using PKSim.Core.Model;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface IEditImportPopulationPresenter : IEditPopulationPresenter<ImportPopulation>
   {
   }

   public class EditImportPopulationPresenter : EditPopulationPresenter<IEditImportPopulationView, IEditImportPopulationPresenter, ImportPopulation>, IEditImportPopulationPresenter
   {
      public EditImportPopulationPresenter(IEditImportPopulationView view, ISubPresenterItemManager<IPopulationItemPresenter> subPresenterItemManager)
         : base(view, subPresenterItemManager, ImportPopulationItems.All)
      {
      }

      protected override ISubPresenterItem<IPopulationSettingsPresenter<ImportPopulation>> SettingPresenterItem
      {
         get { return ImportPopulationItems.ImportSettings; }
      }

      protected override ISubPresenterItem<IPopulationAdvancedParameterDistributionPresenter> DistributionPresenterItem
      {
         get { return ImportPopulationItems.ParameterDistribution; }
      }

      protected override ISubPresenterItem<IPopulationAdvancedParametersPresenter> AdvancedParameterPresenterItem
      {
         get { return ImportPopulationItems.AdvancedParameters; }
      }

      protected override ISubPresenterItem<IPopulationMoleculesPresenter> MoleculesPresenterItem
      {
         get { return ImportPopulationItems.Molecules; }
      }
   }
}