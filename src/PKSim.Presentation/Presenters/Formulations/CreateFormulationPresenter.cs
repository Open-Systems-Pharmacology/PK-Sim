using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Formulations;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.Presenters.Formulations
{
   public interface ICreateFormulationPresenter : IFormulationPresenter, ICreateBuildingBlockPresenter<Formulation>
   {
      IPKSimCommand CreateFormulation(string applicationRoute);
   }

   public class CreateFormulationPresenter : AbstractSubPresenterContainerPresenter<ICreateFormulationView, ICreateFormulationPresenter, IFormulationItemPresenter>, ICreateFormulationPresenter
   {
      private readonly IBuildingBlockPropertiesMapper _propertiesMapper;
      private readonly IObjectBaseDTOFactory _buildingBlockDTOFactory;
      private ObjectBaseDTO _formulationPropertiesDTO;

      public CreateFormulationPresenter(ICreateFormulationView view, ISubPresenterItemManager<IFormulationItemPresenter> subPresenterItemManager,
         IBuildingBlockPropertiesMapper propertiesMapper, IObjectBaseDTOFactory buildingBlockDTOFactory, IDialogCreator dialogCreator)
         : base(view, subPresenterItemManager, FormulationItems.All, dialogCreator)
      {
         _propertiesMapper = propertiesMapper;
         _buildingBlockDTOFactory = buildingBlockDTOFactory;
      }

      public Formulation Formulation => formulationSettingsPresenter.Formulation;

      private IFormulationSettingsPresenter formulationSettingsPresenter => _subPresenterItemManager.PresenterAt(FormulationItems.Settings);

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         formulationSettingsPresenter.FormulationTypeChanged += formulationTypeChanged;
      }

      private void formulationTypeChanged(string formulationType)
      {
         _macroCommand.Clear();
      }

      public IPKSimCommand CreateFormulation(string applicationRoute)
      {
         _formulationPropertiesDTO = _buildingBlockDTOFactory.CreateFor<Formulation>();
         _view.BindToProperties(_formulationPropertiesDTO);
         formulationSettingsPresenter.EditFormulationFor(applicationRoute);
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         formulationSettingsPresenter.SaveFormulation();
         _propertiesMapper.MapProperties(_formulationPropertiesDTO, Formulation);

         return _macroCommand;
      }

      public IPKSimCommand Create()
      {
         return CreateFormulation(string.Empty);
      }

      public Formulation BuildingBlock => Formulation;

      public override void ViewChanged()
      {
         base.ViewChanged();
         View.OkEnabled = CanClose;
      }
   }
}