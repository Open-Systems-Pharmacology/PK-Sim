using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views;
using PKSim.Presentation.Views.Individuals;
using System.Collections.Generic;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface ICreateIndividualPresenter : IWizardPresenter, IIndividualPresenter, ICreateBuildingBlockPresenter<Individual>
   {
      void CreateIndividual();
   }


   public abstract class AbstractCreateIndividualPresenter :
      PKSimWizardPresenter<ICreateIndividualView, ICreateIndividualPresenter, IIndividualItemPresenter>,
      ICreateIndividualPresenter
   {
      protected ObjectBaseDTO _individualPropertiesDTO;
      protected readonly IBuildingBlockPropertiesMapper _propertiesMapper;
      protected readonly IObjectBaseDTOFactory _buildingBlockDTOFactory;

      protected AbstractCreateIndividualPresenter(ICreateIndividualView view, ISubPresenterItemManager<IIndividualItemPresenter> subPresenterItemManager,
         IDialogCreator dialogCreator,
         IBuildingBlockPropertiesMapper propertiesMapper, IObjectBaseDTOFactory buildingBlockDTOFactory, IReadOnlyList<ISubPresenterItem> subPresenterItems)
         : base(view, subPresenterItemManager, subPresenterItems, dialogCreator)
      {
         _propertiesMapper = propertiesMapper;
         _buildingBlockDTOFactory = buildingBlockDTOFactory;
         AllowQuickFinish = true;
      }

      public IPKSimCommand Create()
      {
         _individualPropertiesDTO = _buildingBlockDTOFactory.CreateFor<Individual>();
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

      public Individual BuildingBlock => Individual;

      private void updateIndividualProperties()
      {
         _propertiesMapper.MapProperties(_individualPropertiesDTO, Individual);
      }

      public override bool CanClose => base.CanClose && PresenterAt(IndividualItems.Settings).IndividualCreated;

      public virtual void CreateIndividual()
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

      public Individual Individual
      {
         get { return PresenterAt(IndividualItems.Settings).Individual; }
      }

   }

   public class CreateIndividualPresenter : AbstractCreateIndividualPresenter
   {
      public CreateIndividualPresenter(ICreateIndividualView view, ISubPresenterItemManager<IIndividualItemPresenter> subPresenterItemManager,
         IDialogCreator dialogCreator,
         IBuildingBlockPropertiesMapper propertiesMapper, IObjectBaseDTOFactory buildingBlockDTOFactory)
         : base(view, subPresenterItemManager, dialogCreator, propertiesMapper, buildingBlockDTOFactory, IndividualItems.All)
      { }

      protected override void UpdateControls(int indexThatWillHaveFocus)
      {
         UpdateViewStatus();
         _view.NextEnabled = PresenterAt(IndividualItems.Settings).CanClose && indexThatWillHaveFocus != IndividualItems.Expression.Index;
         _view.OkEnabled = CanClose;
         _view.SetControlEnabled(IndividualItems.Expression, PresenterAt(IndividualItems.Settings).IndividualCreated);
         _view.SetControlEnabled(IndividualItems.Parameters, PresenterAt(IndividualItems.Settings).IndividualCreated);
      }
   }
   }

//try moving to the Starter namespace
   public class CreateIndividualPresenterForMoBi : AbstractCreateIndividualPresenter
   {
      public CreateIndividualPresenterForMoBi(ICreateIndividualView view, ISubPresenterItemManager<IIndividualItemPresenter> subPresenterItemManager,
         IDialogCreator dialogCreator,
         IBuildingBlockPropertiesMapper propertiesMapper, IObjectBaseDTOFactory buildingBlockDTOFactory)
         : base(view, subPresenterItemManager, dialogCreator, propertiesMapper, buildingBlockDTOFactory, IndividualItems.AllExceptExpression)
      {}
      
      protected override void UpdateControls(int indexThatWillHaveFocus)
      {
         UpdateViewStatus();
         _view.NextEnabled = PresenterAt(IndividualItems.Settings).CanClose;
         _view.OkEnabled = CanClose;
         _view.SetControlEnabled(IndividualItems.Parameters, PresenterAt(IndividualItems.Settings).IndividualCreated);
   }

      public override void CreateIndividual()
      {
         if (PresenterAt(IndividualItems.Settings).IndividualCreated) return;
         //reset commands before generating a new individual
         _macroCommand.Clear();
         PresenterAt(IndividualItems.Settings).CreateIndividual();
         if (Individual == null) return;
         PresenterAt(IndividualItems.Parameters).EditIndividual(Individual);
      }
}

   
