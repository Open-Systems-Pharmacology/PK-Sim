using PKSim.Core.Model;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation.Presenters;

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
   }

   public class IndividualMoleculePropertiesPresenter<TSimulationSubject> : EditParameterPresenter<IIndividualMoleculePropertiesView, IIndividualMoleculePropertiesPresenter>, IIndividualMoleculePropertiesPresenter<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      private readonly IOntogenySelectionPresenter<TSimulationSubject> _ontogenySelectionPresenter;
      private readonly IIndividualMoleculeToMoleculePropertiesDTOMapper _mapper;

      public IndividualMoleculePropertiesPresenter(IIndividualMoleculePropertiesView view, IEditParameterPresenterTask editParameterPresenterTask,
         IOntogenySelectionPresenter<TSimulationSubject> ontogenySelectionPresenter, IIndividualMoleculeToMoleculePropertiesDTOMapper mapper)
         : base(view, editParameterPresenterTask)
      {
         _ontogenySelectionPresenter = ontogenySelectionPresenter;
         _mapper = mapper;
         AddSubPresenters(_ontogenySelectionPresenter);
         view.AddOntogenyView(_ontogenySelectionPresenter.View);
      }

      public bool OntogenyVisible
      {
         set { View.OntogenyVisible = value; }
      }

      public bool MoleculeParametersVisible
      {
         set { View.MoleculeParametersVisible = value; }
      }

      public void Edit(IndividualMolecule molecule, TSimulationSubject simulationSubject)
      {
         _ontogenySelectionPresenter.Edit(molecule, simulationSubject);
         _view.BindTo(_mapper.MapFrom(molecule));
      }

      public void RefreshView()
      {
         _ontogenySelectionPresenter.RefreshView();
      }
   }
}