using System;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters.ExpressionProfiles
{
   public interface IExpressionProfileMoleculesPresenter : IExpressionProfileItemPresenter
   {
      void SpeciesChanged();
      void Save();
      void DisableSettings();
   }

   public class ExpressionProfileMoleculesPresenter : AbstractSubPresenter<IExpressionProfileMoleculesView, IExpressionProfileMoleculesPresenter>, IExpressionProfileMoleculesPresenter
   {
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IUsedMoleculeRepository _usedMoleculeRepository;
      private readonly IExpressionProfileFactory _expressionProfileFactory;
      private readonly IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      private readonly IApplicationController _applicationController;
      private IIndividualMoleculeExpressionsPresenter _moleculeExpressionsPresenter;
      private ExpressionProfileDTO _expressionProfileDTO;
      private ExpressionProfile _expressionProfile;

      public ExpressionProfileMoleculesPresenter(
         IExpressionProfileMoleculesView view,
         ISpeciesRepository speciesRepository,
         IUsedMoleculeRepository usedMoleculeRepository,
         IExpressionProfileFactory expressionProfileFactory,
         IMoleculeExpressionTask<Individual> moleculeExpressionTask,
         IApplicationController applicationController) : base(view)
      {
         _speciesRepository = speciesRepository;
         _usedMoleculeRepository = usedMoleculeRepository;
         _expressionProfileFactory = expressionProfileFactory;
         _moleculeExpressionTask = moleculeExpressionTask;
         _applicationController = applicationController;
      }

      public void SpeciesChanged()
      {
         _expressionProfileFactory.UpdateSpecies(_expressionProfile, _expressionProfileDTO.Species);
         refreshExpression();
      }

      public void Save()
      {
         _moleculeExpressionTask.RenameMolecule(_expressionProfile.Molecule, _expressionProfileDTO.MoleculeName, _expressionProfile.Individual);
         _expressionProfile.Category = _expressionProfileDTO.Category;

      }

      public void DisableSettings() => _view.DisableSettings();

      private void refreshExpression()
      {
         _moleculeExpressionsPresenter.SimulationSubject = _expressionProfile.Individual;
         _moleculeExpressionsPresenter.ActivateMolecule(_expressionProfile.Molecule);
      }

      public void Edit(ExpressionProfile expressionProfile)
      {
         _expressionProfile = expressionProfile;
         _expressionProfileDTO = dtoFrom(expressionProfile);
         _view.BindTo(_expressionProfileDTO);
         activateMoleculeExpressionPresenter();
         OnStatusChanged();
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

      private ExpressionProfileDTO dtoFrom(ExpressionProfile expressionProfile)
      {
         var moleculeName = expressionProfile.MoleculeName == ExpressionProfile.DUMMY_MOLECULE_NAME ? "" : expressionProfile.MoleculeName;
         return new ExpressionProfileDTO
         {
            Species = expressionProfile.Species,
            Category = expressionProfile.Category,
            MoleculeName = moleculeName,
            AllMolecules = _usedMoleculeRepository.All(),
            AllSpecies = _speciesRepository.All(),
            //TODO
            MoleculeType = expressionProfile.Molecule.MoleculeType.ToString()
         };
      }


   }
}