using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualMoleculePropertiesPresenter : IPresenter<IIndividualMoleculePropertiesView>, IEditParameterPresenter
   {
      bool OntogenyVisible { set; }
      bool MoleculeParametersVisible { set; }
      void RefreshView();
   }

   public interface IIndividualMoleculePropertiesPresenter<TSimulationSubject> : IIndividualMoleculePropertiesPresenter where TSimulationSubject : ISimulationSubject
   {
      void Edit(IndividualMolecule molecule, TSimulationSubject simulationSubject);
      void DisableEdit();
   }

   public class IndividualMoleculePropertiesPresenter<TSimulationSubject> : EditParameterPresenter<IIndividualMoleculePropertiesView, IIndividualMoleculePropertiesPresenter>, IIndividualMoleculePropertiesPresenter<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      private readonly IOntogenySelectionPresenter _ontogenySelectionPresenter;
      private readonly IMultiParameterEditPresenter _moleculeParametersPresenter;

      public IndividualMoleculePropertiesPresenter(
         IIndividualMoleculePropertiesView view,
         IEditParameterPresenterTask editParameterPresenterTask,
         IOntogenySelectionPresenter ontogenySelectionPresenter,
         IMultiParameterEditPresenter moleculeParametersPresenter)
         : base(view, editParameterPresenterTask)
      {
         _ontogenySelectionPresenter = ontogenySelectionPresenter;
         _moleculeParametersPresenter = moleculeParametersPresenter;
         _moleculeParametersPresenter.IsSimpleEditor = true;

         AddSubPresenters(_ontogenySelectionPresenter, _moleculeParametersPresenter);

         view.AddOntogenyView(_ontogenySelectionPresenter.View);
         view.AddMoleculeParametersView(_moleculeParametersPresenter.View);
      }

      public bool OntogenyVisible
      {
         set => View.OntogenyVisible = value;
      }

      public bool MoleculeParametersVisible
      {
         set => View.MoleculeParametersVisible = value;
      }

      public void Edit(IndividualMolecule molecule, TSimulationSubject simulationSubject)
      {
         var parameters = new[] {molecule.ReferenceConcentration, molecule.HalfLifeLiver, molecule.HalfLifeIntestine};
         _moleculeParametersPresenter.Edit(parameters);
         _ontogenySelectionPresenter.Edit(molecule, simulationSubject);     
         RefreshView();
      }

      public void DisableEdit()
      {
         _ontogenySelectionPresenter.DisableEdit();
         _moleculeParametersPresenter.DisableEdit();
      }

      public void RefreshView()
      {
         _ontogenySelectionPresenter.RefreshView();
         _moleculeParametersPresenter.View.AdjustHeight();
         _view.AdjustHeight();
      }
   }
}