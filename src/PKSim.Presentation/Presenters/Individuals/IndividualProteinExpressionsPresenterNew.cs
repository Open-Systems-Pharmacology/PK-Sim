using System;
using System.Collections.Generic;
using OSPSuite.Utility;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualMoleculeExpressionsPresenterNew : IEditParameterPresenter
   {
      bool OntogenyVisible { set; }
      bool MoleculeParametersVisible { set; }
      ISimulationSubject SimulationSubject { get; set; }
      void ActivateMolecule(IndividualMolecule molecule);
      void SetRelativeExpression(ExpressionContainerDTO expressionContainerDTO, double value);
   }

   public interface IIndividualProteinExpressionsPresenterNew : IIndividualMoleculeExpressionsPresenterNew
   {
      IEnumerable<TissueLocation> AllTissueLocations();
      IEnumerable<IntracellularVascularEndoLocation> AllIntracellularVascularEndoLocations();
      void TissueLocationChanged(TissueLocation tissueLocation);
      void MembraneLocationChanged(MembraneLocation membraneLocation);
      void IntracellularLocationVascularEndoChanged(IntracellularVascularEndoLocation vascularEndoLocation);
      IEnumerable<MembraneLocation> AllMembraneLocation();
      string DisplayFor(TissueLocation tissueLocation);
      string IconFor(TissueLocation tissueLocation);
   }

   public abstract class IndividualProteinExpressionsPresenterNew<TProtein, TSimulationSubject> : EditParameterPresenter<IIndividualProteinExpressionsViewNew, IIndividualProteinExpressionsPresenterNew>, IIndividualProteinExpressionsPresenterNew
      where TProtein : IndividualProtein
      where TSimulationSubject : ISimulationSubject
   {
      protected readonly IMoleculeExpressionTask<TSimulationSubject> _moleculeExpressionTask;
      private readonly IIndividualProteinToIndividualProteinDTOMapper _individualProteinMapper;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IIndividualMoleculePropertiesPresenter<TSimulationSubject> _moleculePropertiesPresenter;
      protected TProtein _protein;
      private readonly Action<Object> _updateLocationVisibilityHandler;
      private IndividualProteinDTO _proteinDTO;
      public ISimulationSubject SimulationSubject { get; set; }

      protected IndividualProteinExpressionsPresenterNew(
         IIndividualProteinExpressionsViewNew view, 
         IEditParameterPresenterTask parameterTask,
         IMoleculeExpressionTask<TSimulationSubject> moleculeExpressionTask,
         IIndividualProteinToIndividualProteinDTOMapper individualProteinMapper,
         IRepresentationInfoRepository representationInfoRepository, 
         IIndividualMoleculePropertiesPresenter<TSimulationSubject> moleculePropertiesPresenter)
         : base(view, parameterTask)
      {
         _moleculeExpressionTask = moleculeExpressionTask;
         _individualProteinMapper = individualProteinMapper;
         _representationInfoRepository = representationInfoRepository;
         _moleculePropertiesPresenter = moleculePropertiesPresenter;
         AddSubPresenters(_moleculePropertiesPresenter);
         // _updateLocationVisibilityHandler = o => updateLocationSelectionVisibility();
         view.AddMoleculePropertiesView(_moleculePropertiesPresenter.View);
      }

      public bool OntogenyVisible
      {
         set => _moleculePropertiesPresenter.OntogenyVisible = value;
      }

      public bool MoleculeParametersVisible
      {
         set => _moleculePropertiesPresenter.MoleculeParametersVisible = value;
      }

      public void ActivateMolecule(IndividualMolecule molecule)
      {
         Activate(molecule.DowncastTo<TProtein>());
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         clearReferences();
      }

      public void SetRelativeExpression(ExpressionContainerDTO expressionContainerDTO, double value)
      {
         AddCommand(_moleculeExpressionTask.SetRelativeExpressionFor(_protein, expressionContainerDTO.RelativeExpressionParameter.Parameter, value));
      }

      // private void updateLocationSelectionVisibility()
      // {
      //    _view.IntracellularVascularEndoLocationVisible = (_protein.TissueLocation == TissueLocation.Intracellular);
      //    _view.LocationOnVascularEndoVisible = (_protein.TissueLocation == TissueLocation.ExtracellularMembrane);
      // }

      public IEnumerable<TissueLocation> AllTissueLocations()
      {
         return EnumHelper.AllValuesFor<TissueLocation>();
      }

      public IEnumerable<IntracellularVascularEndoLocation> AllIntracellularVascularEndoLocations()
      {
         return EnumHelper.AllValuesFor<IntracellularVascularEndoLocation>();
      }

      public string DisplayFor(TissueLocation tissueLocation)
      {
         return _representationInfoRepository.DisplayNameFor(RepresentationObjectType.CONTAINER, tissueLocation.ToString());
      }

      public string IconFor(TissueLocation tissueLocation)
      {
         return _representationInfoRepository.InfoFor(RepresentationObjectType.CONTAINER, tissueLocation.ToString()).IconName;
      }

      public void TissueLocationChanged(TissueLocation tissueLocation)
      {
         AddCommand(_moleculeExpressionTask.SetTissueLocationFor(_protein, tissueLocation));
      }

      public void MembraneLocationChanged(MembraneLocation membraneLocation)
      {
         AddCommand(_moleculeExpressionTask.SetMembraneLocationFor(_protein, membraneLocation));
      }

      public void IntracellularLocationVascularEndoChanged(IntracellularVascularEndoLocation vascularEndoLocation)
      {
         AddCommand(_moleculeExpressionTask.SetIntracellularVascularEndoLocation(_protein, vascularEndoLocation));
      }

      public IEnumerable<MembraneLocation> AllMembraneLocation()
      {
         return new[] {MembraneLocation.Apical, MembraneLocation.Basolateral};
      }

      protected virtual void Activate(TProtein protein)
      {
         clearReferences();
         _protein = protein;
         _proteinDTO = _individualProteinMapper.MapFrom(SimulationSubject, protein);
         _view.BindTo(_proteinDTO);
         _moleculePropertiesPresenter.Edit(protein, SimulationSubject.DowncastTo<TSimulationSubject>());
         _protein.Changed += _updateLocationVisibilityHandler;
         // updateLocationSelectionVisibility();
         _moleculePropertiesPresenter.RefreshView();
      }

      private void clearReferences()
      {
         if (_protein != null)
            _protein.Changed -= _updateLocationVisibilityHandler;

         // _proteinDTO?.ClearReferences();
      }
   }
}