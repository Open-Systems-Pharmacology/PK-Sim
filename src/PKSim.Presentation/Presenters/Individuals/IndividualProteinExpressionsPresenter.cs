using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualProteinExpressionsPresenter : IIndividualMoleculeExpressionsPresenter
   {
      void SetExpressionParameterValue(IParameterDTO expressionParameterDTO, double value);
      bool ShowInitialConcentration { get; set; }
   }

   public abstract class IndividualProteinExpressionsPresenter<TProtein, TSimulationSubject> :
      EditParameterPresenter<IIndividualProteinExpressionsView, IIndividualProteinExpressionsPresenter>,
      IIndividualProteinExpressionsPresenter
      where TProtein : IndividualProtein
      where TSimulationSubject : ISimulationSubject
   {
      private readonly IIndividualProteinToIndividualProteinDTOMapper _individualProteinMapper;
      private readonly IIndividualMoleculePropertiesPresenter<TSimulationSubject> _moleculePropertiesPresenter;
      private readonly IExpressionLocalizationPresenter<TSimulationSubject> _expressionLocalizationPresenter;
      protected TProtein _protein;
      private IndividualProteinDTO _proteinDTO;
      private bool _showInitialConcentration;
      public ISimulationSubject SimulationSubject { get; set; }

      public bool ShowInitialConcentration
      {
         get => _showInitialConcentration;
         set
         {
            _showInitialConcentration = value;
            rebind();
         }
      }

      protected IndividualProteinExpressionsPresenter(
         IIndividualProteinExpressionsView view,
         IEditParameterPresenterTask parameterTask,
         IIndividualProteinToIndividualProteinDTOMapper individualProteinMapper,
         IIndividualMoleculePropertiesPresenter<TSimulationSubject> moleculePropertiesPresenter,
         IExpressionLocalizationPresenter<TSimulationSubject> expressionLocalizationPresenter)
         : base(view, parameterTask)
      {
         _individualProteinMapper = individualProteinMapper;
         _moleculePropertiesPresenter = moleculePropertiesPresenter;
         _expressionLocalizationPresenter = expressionLocalizationPresenter;
         _expressionLocalizationPresenter.LocalizationChanged += (o, e) => onLocalizationChanged();
         AddSubPresenters(_moleculePropertiesPresenter, _expressionLocalizationPresenter);
         view.AddMoleculePropertiesView(_moleculePropertiesPresenter.View);
         view.AddLocalizationView(_expressionLocalizationPresenter.View);

         //TODO probably in preferences
         _showInitialConcentration = true;
      }

      private void onLocalizationChanged()
      {
         rebind();
      }

      private void rebind()
      {
         if (_proteinDTO == null)
            return;

         updateParametersVisibility();
         normalizeExpressionValues();
         _view.BindTo(_proteinDTO.AllExpressionParameters.Where(x => x.Visible));
      }

      private void updateParametersVisibility()
      {
         if (_protein == null)
            return;

         _proteinDTO.AllExpressionParameters.Each(x => { x.Visible = isParameterVisible(x); });
      }

      private bool isParameterVisible(ExpressionParameterDTO expressionParameterDTO)
      {
         var parameter = expressionParameterDTO.Parameter;
         //initial concentration always visible
         if (parameter.IsNamed(INITIAL_CONCENTRATION))
            return ShowInitialConcentration;

         //global surrogate parameters depending on settings
         if (string.Equals(parameter.Parameter.GroupName, CoreConstants.Groups.VASCULAR_SYSTEM))
         {
            switch (parameter.Name)
            {
               case REL_EXP_BLOOD_CELLS:
                  return _protein.InBloodCells;
               case FRACTION_EXPRESSED_BLOOD_CELLS:
               case FRACTION_EXPRESSED_BLOOD_CELLS_MEMBRANE:
                  return _protein.IsBloodCellsMembrane && _protein.IsBloodCellsIntracellular;
               case REL_EXP_VASC_ENDO:
                  return _protein.InVascularEndothelium;
               case FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME:
                  return _protein.IsVascEndosome;
               case FRACTION_EXPRESSED_VASC_ENDO_BASOLATERAL:
                  return _protein.IsVascMembraneBasolateral;
               case FRACTION_EXPRESSED_VASC_ENDO_APICAL:
                  return _protein.IsVascMembraneApical;
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

      public void ActivateMolecule(IndividualMolecule molecule)
      {
         Activate(molecule.DowncastTo<TProtein>());
      }

      public void SetExpressionParameterValue(IParameterDTO expressionParameterDTO, double value)
      {
         SetParameterValue(expressionParameterDTO, value);
         var parameter = expressionParameterDTO.Parameter;
         if (!parameter.IsExpression())
            return;

         normalizeExpressionValues();
      }

      private void normalizeExpressionValues()
      {
         var allExpressionParameters = _proteinDTO.AllExpressionParameters.Where(x => x.Parameter.Parameter.IsExpression()).ToList();
         var max = allExpressionParameters.Select(x => x.Value).Max();

         allExpressionParameters.Each(x => x.NormalizedExpression = max == 0 ? 0 : x.Value / max);
      }

      protected virtual void Activate(TProtein protein)
      {
         _protein = protein;
         _proteinDTO = _individualProteinMapper.MapFrom(SimulationSubject, protein);
         rebind();
         _moleculePropertiesPresenter.Edit(protein, SimulationSubject.DowncastTo<TSimulationSubject>());
         _expressionLocalizationPresenter.Edit(protein, SimulationSubject.DowncastTo<TSimulationSubject>());
         _moleculePropertiesPresenter.RefreshView();
      }
   }
}