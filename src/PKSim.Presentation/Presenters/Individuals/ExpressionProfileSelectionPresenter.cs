using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IExpressionProfileSelectionPresenter : IDisposablePresenter
   {
      /// <summary>
      ///    return true if the user confirms the creation of a molecule for the given individual otherwise false
      /// </summary>
      /// <param name="simulationSubject">simulationSubject for which a molecule should be created</param>
      ExpressionProfile SelectExpressionProfile<TMolecule>(ISimulationSubject simulationSubject) where TMolecule : IndividualMolecule;

      IEnumerable<ExpressionProfile> AllExpressionProfiles();
      void CreateExpressionProfile();
      Task LoadExpressionProfileAsync();
   }

   public class ExpressionProfileSelectionPresenter : AbstractDisposablePresenter<IExpressionProfileSelectionView, IExpressionProfileSelectionPresenter>, IExpressionProfileSelectionPresenter
   {
      private readonly IMoleculePropertiesMapper _moleculePropertiesMapper;
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly IExpressionProfileTask _expressionProfileTask;
      private readonly ExpressionProfileSelectionDTO _expressionProfileSelectionDTO;
      private Type _moleculeType;
      private Action _createExpressionProfileAction;
      private Func<Task>_loadExpressionProfileAsync;
      private IReadOnlyCollection<ExpressionProfile> _allExpressionProfilesForMoleculeType;
      private ISimulationSubject _simulationSubject;

      public ExpressionProfileSelectionPresenter(
         IExpressionProfileSelectionView view, 
         IMoleculePropertiesMapper moleculePropertiesMapper,
         IBuildingBlockRepository buildingBlockRepository,
         IExpressionProfileTask expressionProfileTask)
         : base(view)
      {
         _moleculePropertiesMapper = moleculePropertiesMapper;
         _buildingBlockRepository = buildingBlockRepository;
         _expressionProfileTask = expressionProfileTask;
         _expressionProfileSelectionDTO = new ExpressionProfileSelectionDTO();
      }

      public ExpressionProfile SelectExpressionProfile<TMolecule>(ISimulationSubject simulationSubject) where TMolecule : IndividualMolecule
      {
         _moleculeType = typeof(TMolecule);
         _simulationSubject = simulationSubject;
         _createExpressionProfileAction = () =>
         {
            _expressionProfileTask.AddForMoleculeToProject<TMolecule>();
            refreshExpressionProfilesForMolecule();
         };

         _loadExpressionProfileAsync = async () =>
         {
            await _expressionProfileTask.LoadSingleFromTemplateAsync();
            refreshExpressionProfilesForMolecule();
         };

         refreshExpressionProfilesForMolecule();

         var moleculeDisplay = _moleculePropertiesMapper.MoleculeDisplayFor<TMolecule>();
         _view.Caption = PKSimConstants.UI.AddMolecule(moleculeDisplay);
         _view.ApplicationIcon = _moleculePropertiesMapper.MoleculeIconFor<TMolecule>();
         _expressionProfileSelectionDTO.AllExistingMolecules = simulationSubject.AllMolecules().AllNames();
         _view.BindTo(_expressionProfileSelectionDTO);
         _view.Display(); 
         if (_view.Canceled)
            return null;

         return _expressionProfileSelectionDTO.ExpressionProfile;
      }

      private void refreshExpressionProfilesForMolecule()
      {
         _allExpressionProfilesForMoleculeType = _buildingBlockRepository.All<ExpressionProfile>(canSelect);
         _view.RefreshList();
         //Pre selection
         _expressionProfileSelectionDTO.ExpressionProfile = _allExpressionProfilesForMoleculeType.FirstOrDefault();
      }

      private bool canSelect(ExpressionProfile expressionProfile) =>
         expressionProfile.Molecule.IsAnImplementationOf(_moleculeType)
         && Equals(expressionProfile.Species, _simulationSubject.Species)
         && !_simulationSubject.Uses(expressionProfile);

      public IEnumerable<ExpressionProfile> AllExpressionProfiles() => _allExpressionProfilesForMoleculeType;

      public void CreateExpressionProfile() => _createExpressionProfileAction();

      public Task LoadExpressionProfileAsync() => _loadExpressionProfileAsync();

      public override void ViewChanged()
      {
         base.ViewChanged();
         _view.OkEnabled = CanClose;
      }
   }
}