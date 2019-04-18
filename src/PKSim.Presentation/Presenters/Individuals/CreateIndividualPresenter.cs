using OSPSuite.Core.Services;
using PKSim.Core.Commands;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface ICreateIndividualPresenter : IWizardPresenter, IIndividualPresenter, ICreateBuildingBlockPresenter<PKSim.Core.Model.Individual>
   {
      void CreateIndividual();
   }

   public class CreateIndividualPresenter : PKSimWizardPresenter<ICreateIndividualView, ICreateIndividualPresenter, IIndividualItemPresenter>, ICreateIndividualPresenter
   {
      private ObjectBaseDTO _individualPropertiesDTO;
      private readonly IBuildingBlockPropertiesMapper _propertiesMapper;
      private readonly IObjectBaseDTOFactory _buildingBlockDTOFactory;

      public CreateIndividualPresenter(ICreateIndividualView view, ISubPresenterItemManager<IIndividualItemPresenter> subPresenterItemManager, IDialogCreator dialogCreator,
                                       IBuildingBlockPropertiesMapper propertiesMapper, IObjectBaseDTOFactory buildingBlockDTOFactory)
         : base(view, subPresenterItemManager,IndividualItems.All, dialogCreator)
      {
         _propertiesMapper = propertiesMapper;
         _buildingBlockDTOFactory = buildingBlockDTOFactory;
         AllowQuickFinish = true;
      }

      public IPKSimCommand Create()
      {
         _individualPropertiesDTO = _buildingBlockDTOFactory.CreateFor<PKSim.Core.Model.Individual>();
         _view.BindToProperties(_individualPropertiesDTO);

         PresenterAt(IndividualItems.Settings).PrepareForCreating();
         _view.EnableControl(IndividualItems.Settings);
         SetWizardButtonEnabled(IndividualItems.Settings);
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         updateIndividualProperties();
         return _macroCommand;
      }

      protected override bool HasData()
      {
         return PresenterAt(IndividualItems.Settings).IndividualCreated;
      }

      public PKSim.Core.Model.Individual BuildingBlock => Individual;

      private void updateIndividualProperties()
      {
         _propertiesMapper.MapProperties(_individualPropertiesDTO, Individual);
      }

      protected override void UpdateControls(int indexThatWillHaveFocus)
      {
         UpdateViewStatus();
         _view.NextEnabled = PresenterAt(IndividualItems.Settings).CanClose && indexThatWillHaveFocus != IndividualItems.Expression.Index;
         _view.OkEnabled = CanClose;
         _view.SetControlEnabled(IndividualItems.Expression, PresenterAt(IndividualItems.Settings).IndividualCreated);
         _view.SetControlEnabled(IndividualItems.Parameters, PresenterAt(IndividualItems.Settings).IndividualCreated);
      }

      public override bool CanClose => base.CanClose && PresenterAt(IndividualItems.Settings).IndividualCreated;

      public void CreateIndividual()
      {
         if (PresenterAt(IndividualItems.Settings).IndividualCreated) return;
         //reset commands before generating a new individual
         _macroCommand.Clear();
         PresenterAt(IndividualItems.Settings).CreateIndividual();
         if (Individual == null) return;
         PresenterAt(IndividualItems.Parameters).EditIndividual(Individual);
         PresenterAt(IndividualItems.Expression).EditIndividual(Individual);
      }

      public override void WizardNext(int previousIndex)
      {
         if (previousIndex == IndividualItems.Settings.Index)
         {
            CreateIndividual();
            if (Individual == null) return;
         }

         base.WizardNext(previousIndex);
      }

      public PKSim.Core.Model.Individual Individual
      {
         get { return PresenterAt(IndividualItems.Settings).Individual; }
      }
   }
}