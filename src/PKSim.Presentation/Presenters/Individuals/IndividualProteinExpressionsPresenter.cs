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
      bool ShowInitialConcentration { get; set; }
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

         //TODO probably in preferences
         _showInitialConcentration = false;
      }

      private void onLocalizationChanged() => rebind();

      private void rebind()
      {
         if (_proteinDTO == null)
            return;

         _proteinDTO.AllExpressionParameters.Each(x => { x.Visible = isParameterVisible(x); });

         _expressionParametersPresenter.EmphasisRelativeExpressionParameters = ShowInitialConcentration;
         _expressionParametersPresenter.Edit(_proteinDTO.AllExpressionParameters);
      }

      private bool isParameterVisible(ExpressionParameterDTO expressionParameterDTO)
      {
         var parameter = expressionParameterDTO.Parameter;
         //initial concentration always visible
         if (parameter.IsNamed(INITIAL_CONCENTRATION))
            return ShowInitialConcentration;

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

      public void ActivateMolecule(IndividualMolecule molecule) => Activate(molecule.DowncastTo<TProtein>());

      protected virtual void Activate(TProtein protein)
      {
         _protein = protein;
         _proteinDTO = _individualProteinMapper.MapFrom(protein, SimulationSubject);
         _view.Bind();
         rebind();
         _moleculePropertiesPresenter.Edit(protein, SimulationSubject.DowncastTo<TSimulationSubject>());
         _expressionLocalizationPresenter.Edit(protein, SimulationSubject.DowncastTo<TSimulationSubject>());
         _moleculePropertiesPresenter.RefreshView();
      }
   }
}