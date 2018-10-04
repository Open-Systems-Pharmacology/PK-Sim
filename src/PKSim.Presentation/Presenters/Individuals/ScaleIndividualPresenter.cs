using PKSim.Assets;
using OSPSuite.Core.Services;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IScaleIndividualPresenter : IIndividualPresenter, IWizardPresenter
   {
      IPKSimCommand ScaleIndividual(Individual individualToScale);
      void PerformScaling();
      void CreateIndividual();
   }

   public class ScaleIndividualPresenter : PKSimWizardPresenter<IScaleIndividualView, IScaleIndividualPresenter, IIndividualItemPresenter>, IScaleIndividualPresenter
   {
      private readonly IIndividualExpressionsUpdater _individualExpressionsUpdater;
      private readonly IObjectBaseDTOFactory _objectBaseDTOFactory;
      private readonly IBuildingBlockPropertiesMapper _propertiesMapper;
      private readonly ICloner _cloner;
      private Individual _individualToScale;
      private ObjectBaseDTO _scaleIndividualPropertiesDTO;

      public ScaleIndividualPresenter(IScaleIndividualView view, ISubPresenterItemManager<IIndividualItemPresenter> subPresenterItemManager, IDialogCreator dialogCreator,
         IIndividualExpressionsUpdater individualExpressionsUpdater, IObjectBaseDTOFactory objectBaseDTOFactory,
         IBuildingBlockPropertiesMapper propertiesMapper, ICloner cloner)
         : base(view, subPresenterItemManager, ScaleIndividualItems.All, dialogCreator)
      {
         _individualExpressionsUpdater = individualExpressionsUpdater;
         _objectBaseDTOFactory = objectBaseDTOFactory;
         _propertiesMapper = propertiesMapper;
         _cloner = cloner;
         AllowQuickFinish = true;
      }

      public IPKSimCommand ScaleIndividual(Individual individualToScale)
      {
         _individualToScale = _cloner.Clone(individualToScale);
         _scaleIndividualPropertiesDTO = _objectBaseDTOFactory.CreateFor<Individual>();
         _scaleIndividualPropertiesDTO.Name = _individualToScale.Name;
         _view.BindToProperties(_scaleIndividualPropertiesDTO);
         PresenterAt(ScaleIndividualItems.Settings).PrepareForScaling(_individualToScale);
         _view.EnableControl(ScaleIndividualItems.Settings);
         SetWizardButtonEnabled(ScaleIndividualItems.Settings);
         _view.Caption = PKSimConstants.UI.ScaleIndividual(_individualToScale.Name);
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         updateIndividualProperties();
         return _macroCommand;
      }

      private void updateIndividualProperties()
      {
         _propertiesMapper.MapProperties(_scaleIndividualPropertiesDTO, Individual);
      }

      public Individual Individual => PresenterAt(ScaleIndividualItems.Settings).Individual;

      public override void WizardNext(int previousIndex)
      {
         if (previousIndex == ScaleIndividualItems.Settings.Index)
         {
            CreateIndividual();
         }
         if (previousIndex == ScaleIndividualItems.Scaling.Index)
         {
            PerformScaling();
         }
         base.WizardNext(previousIndex);
      }

      public void CreateIndividual()
      {
         if (PresenterAt(ScaleIndividualItems.Settings).IndividualCreated) return;
         PresenterAt(ScaleIndividualItems.Settings).CreateIndividual();
         _individualExpressionsUpdater.Update(_individualToScale, Individual);
         PresenterAt(ScaleIndividualItems.Scaling).ConfigureScaling(_individualToScale, Individual);

         //remove all previous command from the history
         _macroCommand.Clear();
      }

      protected override void UpdateControls(int indexThatWillHaveFocus)
      {
         var scalingEnabled = PresenterAt(ScaleIndividualItems.Settings).IndividualCreated;
         var editControlsEnabled = scalingEnabled && PresenterAt(ScaleIndividualItems.Scaling).ScalingPerformed;
         _view.OkEnabled = CanClose && editControlsEnabled;
         _view.NextEnabled = PresenterAt(ScaleIndividualItems.Settings).CanClose && indexThatWillHaveFocus != ScaleIndividualItems.Expressions.Index;
         _view.SetControlEnabled(ScaleIndividualItems.Scaling, scalingEnabled);
         _view.SetControlEnabled(ScaleIndividualItems.Expressions, editControlsEnabled);
         _view.SetControlEnabled(ScaleIndividualItems.Parameters, editControlsEnabled);
      }

      public void PerformScaling()
      {
         if (PresenterAt(ScaleIndividualItems.Scaling).ScalingPerformed) return;
         PresenterAt(ScaleIndividualItems.Scaling).PerformScaling();
         PresenterAt(ScaleIndividualItems.Parameters).EditIndividual(Individual);
         PresenterAt(ScaleIndividualItems.Expressions).EditIndividual(Individual);
      }
   }
}