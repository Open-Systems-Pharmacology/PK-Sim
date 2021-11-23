using System;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Exceptions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters.ExpressionProfiles
{
   public interface IExpressionProfileMoleculesPresenter : IExpressionProfileItemPresenter
   {
      void SpeciesChanged();
      void CategoryChanged();
      void MoleculeNameChanged();
      void DisableSettings();
      void LoadExpressionFromDatabaseQuery();
   }

   public class ExpressionProfileMoleculesPresenter : AbstractSubPresenter<IExpressionProfileMoleculesView, IExpressionProfileMoleculesPresenter>, IExpressionProfileMoleculesPresenter
   {
      private readonly IExpressionProfileFactory _expressionProfileFactory;
      private readonly IApplicationController _applicationController;
      private readonly IExpressionProfileToExpressionProfileDTOMapper _expressionProfileDTOMapper;
      private readonly IEditMoleculeTask<Individual> _editMoleculeTask;
      private readonly IExpressionProfileUpdater _expressionProfileUpdater;
      private IIndividualMoleculeExpressionsPresenter _moleculeExpressionsPresenter;
      private ExpressionProfileDTO _expressionProfileDTO;
      private ExpressionProfile _expressionProfile;

      public ExpressionProfileMoleculesPresenter(
         IExpressionProfileMoleculesView view,
         IExpressionProfileFactory expressionProfileFactory,
         IApplicationController applicationController,
         IExpressionProfileToExpressionProfileDTOMapper expressionProfileDTOMapper,
         IEditMoleculeTask<Individual> editMoleculeTask,
         IExpressionProfileUpdater expressionProfileUpdater) : base(view)
      {
         _expressionProfileFactory = expressionProfileFactory;
         _applicationController = applicationController;
         _expressionProfileDTOMapper = expressionProfileDTOMapper;
         _editMoleculeTask = editMoleculeTask;
         _expressionProfileUpdater = expressionProfileUpdater;
      }

      public void Edit(ExpressionProfile expressionProfile)
      {
         _expressionProfile = expressionProfile;
         _expressionProfileDTO = _expressionProfileDTOMapper.MapFrom(expressionProfile);
         _view.BindTo(_expressionProfileDTO);
         activateMoleculeExpressionPresenter();
      }

      public void SpeciesChanged()
      {
         _expressionProfileFactory.UpdateSpecies(_expressionProfile, _expressionProfileDTO.Species);
         refreshExpression();
      }

      public void CategoryChanged()
      {
         _expressionProfile.Category = _expressionProfileDTO.Category;
      }

      public void MoleculeNameChanged()
      {
         _expressionProfileUpdater.UpdateMoleculeName(_expressionProfile, _expressionProfileDTO.MoleculeName);
      }

      public void DisableSettings() => _view.DisableSettings();

      public void LoadExpressionFromDatabaseQuery()
      {
         if (!_editMoleculeTask.CanQueryProteinExpressionsFor(_expressionProfile.Individual))
            throw new OSPSuiteException(PKSimConstants.Error.NoProteinExpressionDatabaseAssociatedTo(_expressionProfile.Species.Name));

         AddCommand(_editMoleculeTask.EditMolecule(_expressionProfile.Molecule, _expressionProfile.Individual, _expressionProfileDTO.MoleculeName));
         refreshExpression();
      }

      private void refreshExpression()
      {
         _moleculeExpressionsPresenter.SimulationSubject = _expressionProfile.Individual;
         _moleculeExpressionsPresenter.ActivateMolecule(_expressionProfile.Molecule);
      }

      private void activateMoleculeExpressionPresenter()
      {
         _moleculeExpressionsPresenter = presenterFor(_expressionProfile.Molecule);
         AddSubPresenters(_moleculeExpressionsPresenter);
         _moleculeExpressionsPresenter.InitializeWith(CommandCollector);
         _moleculeExpressionsPresenter.StatusChanged += OnStatusChanged;
         _view.AddExpressionView(_moleculeExpressionsPresenter.BaseView);
         refreshExpression();
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _moleculeExpressionsPresenter = null;
      }

      private IIndividualMoleculeExpressionsPresenter presenterFor(IndividualMolecule molecule)
      {
         switch (molecule)
         {
            case IndividualOtherProtein _:
               return _applicationController.Start<IIndividualOtherProteinExpressionsPresenter<Individual>>();
            case IndividualEnzyme _:
               return _applicationController.Start<IIndividualEnzymeExpressionsPresenter<Individual>>();
            case IndividualTransporter _:
               return _applicationController.Start<IIndividualTransporterExpressionsPresenter<Individual>>();
            default:
               throw new ArgumentOutOfRangeException(nameof(molecule));
         }
      }
   }
}