using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationCompoundParameterAlternativeSelectionPresenter : ContextSpecification<ISimulationCompoundParameterAlternativesSelectionPresenter>
   {
      protected ISimulationToSimulationCompoundParameterMappingDTOMapper _mapper;
      protected ISimulationCompoundParameterAlternativesSelectionView _view;
      private ILazyLoadTask _lazyLoadTask;
      protected Compound _compound;
      protected Simulation _simulation;

      protected override void Context()
      {
         _view = A.Fake<ISimulationCompoundParameterAlternativesSelectionView>();
         _mapper = A.Fake<ISimulationToSimulationCompoundParameterMappingDTOMapper>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         sut = new SimulationCompoundParameterAlternativesSelectionPresenter(_view, _lazyLoadTask, _mapper);

         _compound = A.Fake<Compound>();
         _simulation = A.Fake<Simulation>();
      }
   }

   public class When_the_simulation_compound_setup_presenter_is_asked_to_edit_a_simulation : concern_for_SimulationCompoundParameterAlternativeSelectionPresenter
   {
      private SimulationCompoundParameterMappingDTO _simParameterMappingDTO;

      protected override void Context()
      {
         base.Context();
         _simParameterMappingDTO = new SimulationCompoundParameterMappingDTO();
         A.CallTo(() => _mapper.MapFrom(_simulation, _compound)).Returns(_simParameterMappingDTO);
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation,_compound);
      }

      [Observation]
      public void should_set_the_availale_mapping_into_the_view()
      {
         A.CallTo(() => _view.BindTo(_simParameterMappingDTO.AllParameterGroups())).MustHaveHappened();
      }
   }

   public class When_the_simulation_compound_parameter_alternative_presenter_is_told_to_save_the_edited_compound_for_the_active_simulation : concern_for_SimulationCompoundParameterAlternativeSelectionPresenter
   {
      private CompoundProperties _compoundProperties;

      protected override void Context()
      {
         base.Context();
         _compoundProperties = A.Fake<CompoundProperties>();
         var alternative1 = A.Fake<ParameterAlternative>();
         var alternative2 = A.Fake<ParameterAlternative>();
         var compoundParameterSelection1 = new CompoundParameterSelectionDTO(A.Fake<ParameterAlternativeGroup>());
         compoundParameterSelection1.SelectedAlternative = alternative1;
         var compoundParameterSelection2 = new CompoundParameterSelectionDTO(A.Fake<ParameterAlternativeGroup>());
         compoundParameterSelection2.SelectedAlternative = alternative2;
         var simulationCompoundMappingDTO = new SimulationCompoundParameterMappingDTO();
         simulationCompoundMappingDTO.Add(compoundParameterSelection1);
         simulationCompoundMappingDTO.Add(compoundParameterSelection2);
         A.CallTo(() => _mapper.MapFrom(_simulation, _compound)).Returns(simulationCompoundMappingDTO);

         A.CallTo(() => _simulation.CompoundPropertiesFor(_compound)).Returns(_compoundProperties);
         sut.EditSimulation(_simulation, _compound);


      }

      protected override void Because()
      {
         sut.SaveConfiguration();
      }

      [Observation]
      public void should_erase_the_previous_parameter_mapping()
      {
         A.CallTo(() => _compoundProperties.ClearGroupMapping()).MustHaveHappened();
      }

      [Observation]
      public void should_add_one_compound_parameter_selection_for_each_selected_alternative()
      {
         //one for each process
         A.CallTo(() => _compoundProperties.AddCompoundGroupSelection(A<CompoundGroupSelection>.Ignored)).MustHaveHappenedTwiceExactly();
      }
   }
}