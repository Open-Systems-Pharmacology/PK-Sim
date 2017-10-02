using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationSelectionForComparisonPresenter : ContextSpecification<ISimulationSelectionForComparisonPresenter>
   {
      private IBuildingBlockRepository _buildingBlockRepository;
      protected ISimulationSelectionForComparisonView _view;
      protected SimulationComparisonSelectionDTO _dto;
      protected PopulationSimulation _sim1;
      protected PopulationSimulation _sim2;
      protected ILazyLoadTask _lazyLoadTask;
      protected PopulationSimulationComparison _populationSimulationComparison;

      protected override void Context()
      {
         _view = A.Fake<ISimulationSelectionForComparisonView>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _populationSimulationComparison = new PopulationSimulationComparison();
         sut = new SimulationSelectionForComparisonPresenter(_view, _buildingBlockRepository, _lazyLoadTask);

         A.CallTo(() => _view.BindTo(A<SimulationComparisonSelectionDTO>._))
            .Invokes(x => _dto = x.GetArgument<SimulationComparisonSelectionDTO>(0));

         _sim1 = new PopulationSimulation().WithId("1").WithName("Sim1");
         _sim2 = new PopulationSimulation().WithId("2").WithName("Sim2");
         A.CallTo(() => _buildingBlockRepository.All<PopulationSimulation>()).Returns(new[] {_sim1, _sim2});
      }
   }

   public class When_starting_the_simulation_selection_process_in_order_to_compare_simulations : concern_for_SimulationSelectionForComparisonPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(false);
         _populationSimulationComparison.AddSimulation(_sim1);
         _populationSimulationComparison.AddSimulation(_sim2);
      }

      protected override void Because()
      {
         sut.Edit(_populationSimulationComparison);
      }

      [Observation]
      public void should_retrieve_all_population_simulation_simulations_and_display_them_so_that_the_user_can_select_them()
      {
         _dto.AllSimulations.Count().ShouldBeEqualTo(2);
         _dto.AllSimulations.ElementAt(0).Simulation.ShouldBeEqualTo(_sim1);
         _dto.AllSimulations.ElementAt(1).Simulation.ShouldBeEqualTo(_sim2);
      }

      [Observation]
      public void shoud_load_the_selected_simulations()
      {
         A.CallTo(() => _lazyLoadTask.Load(_sim1)).MustHaveHappened();
         A.CallTo(() => _lazyLoadTask.Load(_sim2)).MustHaveHappened();
      }

      [Observation]
      public void should_return_a_population_simulation_comparison_referencing_the_selected_simulations()
      {
         _populationSimulationComparison.AllSimulations.ShouldOnlyContain(_sim1, _sim2);
      }
   }

   public class When_retrieving_the_list_of_all_symbols_available_for_the_comparison : concern_for_SimulationSelectionForComparisonPresenter
   {
      [Observation]
      public void should_return_all_predefined_symbols()
      {
         sut.AllSymbols().ShouldOnlyContain(EnumHelper.AllValuesFor<Symbols>());
      }
   }

   public class When_retrieving_the_available_simulations_that_can_be_defined_as_reference_in_a_comparison : concern_for_SimulationSelectionForComparisonPresenter
   {
      private List<Simulation> _allSimulations;

      protected override void Because()
      {
         _allSimulations = sut.AvailableSimulations().ToList();
      }

      [Observation]
      public void should_return_all_population_simulatiions_and_the_no_selection_entry()
      {
         _allSimulations.Count.ShouldBeEqualTo(3);
         _allSimulations[0].ShouldBeAnInstanceOf<NullSimulation>();
         _allSimulations.ShouldContain(_sim1, _sim2);
      }
   }

   public class When_notified_that_the_reference_simulation_has_changed : concern_for_SimulationSelectionForComparisonPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Edit(_populationSimulationComparison);
         _dto.Reference = _sim1;
      }

      protected override void Because()
      {
         sut.ReferenceSimulationChanged();
      }

      [Observation]
      public void should_marked_the_simulation_has_used_in_the_comparison()
      {
         _dto.AllSimulations.First(x => x.Simulation == _sim1).Selected.ShouldBeTrue();
      }
   }
}