using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Individuals;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualProteinExpressionsPresenter : IIndividualMoleculeExpressionsPresenter
   {
   }

   public abstract class IndividualProteinExpressionsPresenter<TProtein, TSimulationSubject> :
      AbstractCommandCollectorPresenter<IIndividualProteinExpressionsView, IIndividualProteinExpressionsPresenter>,
      IIndividualProteinExpressionsPresenter
      where TProtein : IndividualProtein
      where TSimulationSubject : ISimulationSubject
   {
      private readonly IIndividualProteinToIndividualProteinDTOMapper _individualProteinMapper;
      private readonly IIndividualMoleculePropertiesPresenter<TSimulationSubject> _moleculePropertiesPresenter;
      private readonly IExpressionLocalizationPresenter<TSimulationSubject> _expressionLocalizationPresenter;
      private readonly IExpressionParametersPresenter _expressionParametersPresenter;
      protected TProtein _protein;
      private IndividualProteinDTO _proteinDTO;
      public ISimulationSubject SimulationSubject { get; set; }


      protected IndividualProteinExpressionsPresenter(
         IIndividualProteinExpressionsView view,
         IIndividualProteinToIndividualProteinDTOMapper individualProteinMapper,
         IIndividualMoleculePropertiesPresenter<TSimulationSubject> moleculePropertiesPresenter,
         IExpressionLocalizationPresenter<TSimulationSubject> expressionLocalizationPresenter,
         IExpressionParametersPresenter expressionParametersPresenter)
         : base(view)
      {
         _individualProteinMapper = individualProteinMapper;
         _moleculePropertiesPresenter = moleculePropertiesPresenter;
         _expressionLocalizationPresenter = expressionLocalizationPresenter;
         _expressionParametersPresenter = expressionParametersPresenter;
         _expressionLocalizationPresenter.LocalizationChanged += (o, e) => onLocalizationChanged();
         AddSubPresenters(_moleculePropertiesPresenter, _expressionLocalizationPresenter, _expressionParametersPresenter);
         view.AddMoleculePropertiesView(_moleculePropertiesPresenter.View);
         view.AddLocalizationView(_expressionLocalizationPresenter.View);
         view.AddExpressionParametersView(_expressionParametersPresenter.View);

       
      }

      private void onLocalizationChanged() => rebind();

      private void rebind()
      {
         if (_proteinDTO == null)
            return;

         _proteinDTO.AllExpressionParameters.Each(x => { x.Visible = IsParameterVisible(x); });

         _expressionParametersPresenter.Edit(_proteinDTO.AllExpressionParameters);
      }

      //Internal to allow for testing 
      internal bool IsParameterVisible(ExpressionParameterDTO expressionParameterDTO)
      {
         var parameter = expressionParameterDTO.Parameter;

         //global surrogate parameters depending on settings
         if (string.Equals(expressionParameterDTO.GroupName, CoreConstants.Groups.VASCULAR_SYSTEM))
         {
            switch (parameter.Name)
            {
               case REL_EXP_BLOOD_CELLS:
                  return _protein.InBloodCells;
               case FRACTION_EXPRESSED_BLOOD_CELLS:
               case FRACTION_EXPRESSED_BLOOD_CELLS_MEMBRANE:
                  return _protein.IsBloodCellsMembrane && _protein.IsBloodCellsIntracellular;
               case REL_EXP_VASCULAR_ENDOTHELIUM:
                  return _protein.InVascularEndothelium;
               case FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME:
                  return _protein.IsVascEndosome && (_protein.IsVascMembraneTissueSide || _protein.IsVascMembranePlasmaSide);
               case FRACTION_EXPRESSED_VASC_ENDO_TISSUE_SIDE:
                  return _protein.IsVascMembraneTissueSide && (_protein.IsVascEndosome || _protein.IsVascMembranePlasmaSide);
               case FRACTION_EXPRESSED_VASC_ENDO_PLASMA_SIDE:
                  return _protein.IsVascMembranePlasmaSide && (_protein.IsVascEndosome || _protein.IsVascMembraneTissueSide);
            }
         }

         //tissue location. Rel exp visible if we are expressing it
         switch (parameter.Name)
         {
            case REL_EXP:
               return _protein.InTissue;
            case FRACTION_EXPRESSED_INTRACELLULAR:
            case FRACTION_EXPRESSED_INTERSTITIAL:
               return _protein.IsIntracellular && _protein.IsInterstitial;
         }

         return true;
      }

      public bool OntogenyVisible
      {
         set => _moleculePropertiesPresenter.OntogenyVisible = value;
      }

      public bool MoleculeParametersVisible
      {
         set => _moleculePropertiesPresenter.MoleculeParametersVisible = value;
      }

      public void ActivateMolecule(IndividualMolecule molecule) => Activate(molecule.DowncastTo<TProtein>());

      protected virtual void Activate(TProtein protein)
      {
         _protein = protein;
         _proteinDTO = _individualProteinMapper.MapFrom(protein, SimulationSubject);
         rebind();
         _moleculePropertiesPresenter.Edit(protein, SimulationSubject.DowncastTo<TSimulationSubject>());
         _expressionLocalizationPresenter.Edit(protein, SimulationSubject.DowncastTo<TSimulationSubject>());
      }
   }
}