using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface ICreateRandomPopulationPresenter : ICreatePopulationPresenter<RandomPopulation>
   {
   }

   public class CreateRandomPopulationPresenter : CreatePopulationPresenter<ICreateRandomPopulationView, ICreateRandomPopulationPresenter, RandomPopulation>, ICreateRandomPopulationPresenter
   {
      public CreateRandomPopulationPresenter(ICreateRandomPopulationView view, ISubPresenterItemManager<IPopulationItemPresenter> subPresenterItemManager, IDialogCreator dialogCreator,
                                             IBuildingBlockPropertiesMapper propertiesMapper, IObjectBaseDTOFactory buildingBlockDTOFactory,
                                             IBuildingBlockRepository buildingBlockRepository)
         : base(view, subPresenterItemManager, RamdomPopulationItems.All, dialogCreator, propertiesMapper, buildingBlockDTOFactory, buildingBlockRepository)
      {
      }

      protected override IPopulationSettingsPresenter<RandomPopulation> RetrieveSettingsPresenter()
      {
         return PresenterAt(RamdomPopulationItems.Settings);
      }

      protected override ISubPresenterItem SettingPresenterItem
      {
         get { return RamdomPopulationItems.Settings; }
      }

      protected override ISubPresenterItem<IPopulationAdvancedParametersPresenter> AdvancedParametersPresenterItem
      {
         get { return RamdomPopulationItems.AdvancedParameters; }
      }

      protected override ISubPresenterItem<IPopulationMoleculesPresenter> MoleculesPresenterItem
      {
         get { return RamdomPopulationItems.Molecules; }
      }

      protected override ISubPresenterItem<IPopulationAdvancedParameterDistributionPresenter> DistributionPresenterItem
      {
         get { return RamdomPopulationItems.ParameterDistribution; }
      }
   }
}