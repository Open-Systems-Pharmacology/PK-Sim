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
      /// <summary>
      /// Are we in "Edit" mode ore "Creation" mode. 
      /// </summary>
      bool IsEditMode { get; set; }
      void LoadExpressionFromDatabaseQuery();
   }

   public class ExpressionProfileMoleculesPresenter : AbstractSubPresenter<IExpressionProfileMoleculesView, IExpressionProfileMoleculesPresenter>, IExpressionProfileMoleculesPresenter
   {
      private readonly IExpressionProfileFactory _expressionProfileFactory;
      private readonly IApplicationController _applicationController;
      private readonly IExpressionProfileToExpressionProfileDTOMapper _expressionProfileDTOMapper;
      private readonly IExpressionProfileProteinDatabaseTask _expressionProfileProteinDatabaseTask;
      private readonly IExpressionProfileUpdater _expressionProfileUpdater;
      private readonly IMoleculeParameterTask _moleculeParameterTask;
      private IIndividualMoleculeExpressionsPresenter _moleculeExpressionsPresenter;
      private ExpressionProfileDTO _expressionProfileDTO;
      private ExpressionProfile _expressionProfile;
      private bool _isEditMode = false;


      public ExpressionProfileMoleculesPresenter(
         IExpressionProfileMoleculesView view,
         IExpressionProfileFactory expressionProfileFactory,
         IApplicationController applicationController,
         IExpressionProfileToExpressionProfileDTOMapper expressionProfileDTOMapper,
         IExpressionProfileProteinDatabaseTask expressionProfileProteinDatabaseTask,
         IExpressionProfileUpdater expressionProfileUpdater,
         IMoleculeParameterTask moleculeParameterTask) : base(view)
      {
         _expressionProfileFactory = expressionProfileFactory;
         _applicationController = applicationController;
         _expressionProfileDTOMapper = expressionProfileDTOMapper;
         _expressionProfileProteinDatabaseTask = expressionProfileProteinDatabaseTask;
         _expressionProfileUpdater = expressionProfileUpdater;
         _moleculeParameterTask = moleculeParameterTask;
      }

      public void Edit(ExpressionProfile expressionProfile)
      {
         _expressionProfile = expressionProfile;
         _expressionProfileDTO = _expressionProfileDTOMapper.MapFrom(expressionProfile);
         _view.BindTo(_expressionProfileDTO);
         activateMoleculeExpressionPresenter();
      }

      public void Save()
      {
         _expressionProfile.Category = _expressionProfileDTO.Category;
         _expressionProfileUpdater.UpdateMoleculeName(_expressionProfile, _expressionProfileDTO.MoleculeName);
      }

      public void SpeciesChanged()
      {
         _expressionProfileFactory.UpdateSpecies(_expressionProfile, _expressionProfileDTO.Species);
         refreshExpression();
      }

      public bool IsEditMode
      {
         get => _isEditMode;
         set
         {
            _isEditMode = value;
            //Edit vs Create. We can only change naming settings when CREATING an Expression Profile
            if(_isEditMode)
               _view.DisableSettings();

         }
      }

      
      public void LoadExpressionFromDatabaseQuery()
      {
         if (!_expressionProfileProteinDatabaseTask.CanQueryProteinExpressionsFor(_expressionProfile))
            throw new OSPSuiteException(PKSimConstants.Error.NoProteinExpressionDatabaseAssociatedTo(_expressionProfile.Species.Name));

         var queryResults = _expressionProfileProteinDatabaseTask.QueryDatabase(_expressionProfile, _expressionProfileDTO.MoleculeName);
         if (queryResults == null)
            return;

         if (!IsEditMode)
         {
            _expressionProfileDTO.MoleculeName = queryResults.ProteinName;
            _moleculeParameterTask.SetDefaultFor(_expressionProfile, _expressionProfileDTO.MoleculeName);
         }

         AddCommand(_expressionProfileUpdater.UpdateExpressionFromQuery(_expressionProfile, queryResults));

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