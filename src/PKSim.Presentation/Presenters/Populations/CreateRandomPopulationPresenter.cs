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
         : base(view, subPresenterItemManager, RandomPopulationItems.All, dialogCreator, propertiesMapper, buildingBlockDTOFactory, buildingBlockRepository)
      {
      }

      protected override IPopulationSettingsPresenter<RandomPopulation> RetrieveSettingsPresenter()
      {
         return PresenterAt(RandomPopulationItems.Settings);
      }

      protected override ISubPresenterItem SettingPresenterItem => RandomPopulationItems.Settings;

      protected override ISubPresenterItem<IPopulationAdvancedParametersPresenter> AdvancedParametersPresenterItem => RandomPopulationItems.AdvancedParameters;

      protected override ISubPresenterItem<IPopulationMoleculesPresenter> MoleculesPresenterItem => RandomPopulationItems.Molecules;

      protected override ISubPresenterItem<IPopulationAdvancedParameterDistributionPresenter> DistributionPresenterItem => RandomPopulationItems.ParameterDistribution;
   }
}