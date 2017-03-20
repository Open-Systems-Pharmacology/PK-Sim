using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface IImportPopulationPresenter : ICreatePopulationPresenter<ImportPopulation>
   {
   }

   public class ImportPopulationPresenter : CreatePopulationPresenter<IImportPopulationView,IImportPopulationPresenter,ImportPopulation>, IImportPopulationPresenter
   {
      public ImportPopulationPresenter(IImportPopulationView view, ISubPresenterItemManager<IPopulationItemPresenter> subPresenterItemManager, 
         IDialogCreator dialogCreator, IBuildingBlockPropertiesMapper propertiesMapper, IObjectBaseDTOFactory buildingBlockDTOFactory, 
         IBuildingBlockRepository buildingBlockRepository) : base(view, subPresenterItemManager, ImportPopulationItems.All, dialogCreator, propertiesMapper, buildingBlockDTOFactory, buildingBlockRepository)
      {
      }

      protected override IPopulationSettingsPresenter<ImportPopulation> RetrieveSettingsPresenter()
      {
         return PresenterAt(ImportPopulationItems.ImportSettings);
      }

      protected override ISubPresenterItem SettingPresenterItem => ImportPopulationItems.ImportSettings;

      protected override ISubPresenterItem<IPopulationAdvancedParametersPresenter> AdvancedParametersPresenterItem => ImportPopulationItems.AdvancedParameters;

      protected override ISubPresenterItem<IPopulationMoleculesPresenter> MoleculesPresenterItem => ImportPopulationItems.Molecules;

      protected override ISubPresenterItem<IPopulationAdvancedParameterDistributionPresenter> DistributionPresenterItem => ImportPopulationItems.ParameterDistribution;
   }
}