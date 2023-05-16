using System.Collections.Generic;
using NPOI.SS.Formula.Functions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.DiseaseStates;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.DiseaseStates;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters.ExpressionProfiles
{
   public interface ICreateExpressionProfilePresenter : ICreateBuildingBlockPresenter<ExpressionProfile>, IExpressionProfilePresenter
   {
      ExpressionProfile ExpressionProfile { get; }
      void SpeciesChanged();
      IPKSimCommand Create<TMolecule>() where TMolecule : IndividualMolecule;
   }

   public class CreateExpressionProfilePresenter : AbstractDisposableCommandCollectorPresenter<ICreateExpressionProfileView, ICreateExpressionProfilePresenter>, ICreateExpressionProfilePresenter
   {
      private readonly IExpressionProfileFactory _expressionProfileFactory;
      private readonly IExpressionProfileToExpressionProfileDTOMapper _expressionProfileDTOMapper;
      private readonly IMoleculeParameterTask _moleculeParameterTask;
      private ExpressionProfileDTO _expressionProfileDTO;
      private readonly IDialogCreator _dialogCreator;
      private readonly IDiseaseStateSelectionPresenter _diseaseStateSelectionPresenter;
      private readonly IDiseaseStateRepository _diseaseStateRepository;
      private readonly IDiseaseStateUpdater _diseaseStateUpdater;
      public ExpressionProfile ExpressionProfile { get; private set; }

      public CreateExpressionProfilePresenter(ICreateExpressionProfileView view,
         IExpressionProfileFactory expressionProfileFactory,
         IExpressionProfileToExpressionProfileDTOMapper expressionProfileDTOMapper,
         IMoleculeParameterTask moleculeParameterTask,
         IDialogCreator dialogCreator,
         IDiseaseStateSelectionPresenter diseaseStateSelectionPresenter, 
         IDiseaseStateRepository diseaseStateRepository, 
         IDiseaseStateUpdater diseaseStateUpdater) : base(view)
      {
         _expressionProfileFactory = expressionProfileFactory;
         _expressionProfileDTOMapper = expressionProfileDTOMapper;
         _moleculeParameterTask = moleculeParameterTask;
         _dialogCreator = dialogCreator;
         _diseaseStateSelectionPresenter = diseaseStateSelectionPresenter;
         _diseaseStateRepository = diseaseStateRepository;
         _diseaseStateUpdater = diseaseStateUpdater;
         _diseaseStateSelectionPresenter.Initialize(PKSimConstants.UI.DiseaseState, showDescription: false);
         _view.AddDiseaseStateView(_diseaseStateSelectionPresenter.View);
         AddSubPresenters(_diseaseStateSelectionPresenter);
      }

      public IPKSimCommand Create<TMolecule>() where TMolecule : IndividualMolecule
      {
         //Just for edit
         ExpressionProfile = _expressionProfileFactory.Create<TMolecule>();
         _expressionProfileDTO = _expressionProfileDTOMapper.MapFrom(ExpressionProfile);
         _view.Caption = PKSimConstants.UI.CreateExpressionProfile;
         _view.BindTo(_expressionProfileDTO);
         _diseaseStateSelectionPresenter.Edit(_expressionProfileDTO.DiseaseState);
         SpeciesChanged();
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         //we create a new one with all new features
         ExpressionProfile = _expressionProfileFactory.Create<TMolecule>(_expressionProfileDTO.Species, _expressionProfileDTO.MoleculeName);
         ExpressionProfile.Category = _expressionProfileDTO.Category;
         _diseaseStateUpdater.UpdateOriginDataFromDiseaseState(ExpressionProfile.Individual.OriginData, _expressionProfileDTO.DiseaseState);
         _moleculeParameterTask.SetDefaultFor(ExpressionProfile);

         //Action 
         return new PKSimMacroCommand();
      }

      public void SpeciesChanged()
      {
         var allDiseaseStates = allDiseaseStatesFor(_expressionProfileDTO.Species, ExpressionProfile.Molecule.MoleculeType);
         _diseaseStateSelectionPresenter.AllDiseaseStates = allDiseaseStates;
         _view.UpdateDiseaseStateVisibility(allDiseaseStates.HasAtLeastTwo());
      }

      private IReadOnlyList<DiseaseState> allDiseaseStatesFor(Species species, QuantityType moleculeType)
      {
         var list = new List<DiseaseState> {_diseaseStateRepository.HealthyState};
         list.AddRange(_diseaseStateRepository.AllForExpressionProfile(species, moleculeType));
         return list;
      }

      public IPKSimCommand Create() => Create<IndividualEnzyme>();

      public ExpressionProfile BuildingBlock => ExpressionProfile;

      public override void ViewChanged()
      {
         base.ViewChanged();
         View.OkEnabled = CanClose;
      }

      public void Save()
      {
         //We have a slightly different behavior for expression profile as the name is a composite name and we need to validate the object after the fact
         var rules = _expressionProfileDTO.Validate();
         if (rules.IsEmpty)
         {
            _view.CloseView();
            return;
         }

         _dialogCreator.MessageBoxInfo(rules.Message);
      }
   }
}