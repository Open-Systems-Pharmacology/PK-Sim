using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Core;
using BuildingBlockRemovedEvent = PKSim.Core.Events.BuildingBlockRemovedEvent;

namespace PKSim.Presentation
{
   public abstract class concern_for_CloseSubjectPresenterInvoker<TBuildingBlock> : ContextSpecification<ICloseSubjectPresenterInvoker> where TBuildingBlock : class, IPKSimBuildingBlock
   {
      protected TBuildingBlock _buildingBlock;
      protected PKSimProject _project;
      protected IApplicationController _applicationController;

      protected override void Context()
      {
         _applicationController = A.Fake<IApplicationController>();
         sut = new CloseSubjectPresenterInvoker(_applicationController);

         _project = A.Fake<PKSimProject>();
         _buildingBlock = A.Fake<TBuildingBlock>();
      }

      protected override void Because()
      {
         sut.Handle(new BuildingBlockRemovedEvent(_buildingBlock, _project));
      }
   }

   public class When_the_close_subject_presenter_invoker_is_being_notified_that_an_observed_data_is_being_deleted : concern_for_CloseSubjectPresenterInvoker<Individual>
   {
      private DataRepository _dataRepository;

      protected override void Because()
      {
         _dataRepository= A.Fake<DataRepository>();   
         sut.Handle(new ObservedDataRemovedEvent(_dataRepository, _project));
      }

      [Observation]
      public void should_close_any_open_editor_for_the_repository()
      {
         A.CallTo(() => _applicationController.Close(_dataRepository)).MustHaveHappened();
      }
   }

   public class When_the_close_building_block_presenter_invoker_is_being_notified_that_an_individual_is_deleted : concern_for_CloseSubjectPresenterInvoker<Individual>
   {
      [Observation]
      public void should_close_any_edit_individual_presenter_for_that_individual()
      {
         A.CallTo(() => _applicationController.Close(_buildingBlock)).MustHaveHappened();
      }
   }

   public class When_the_close_building_block_presenter_invoker_is_being_notified_that_a_compound_is_deleted : concern_for_CloseSubjectPresenterInvoker<Compound>
   {
      [Observation]
      public void should_close_any_edit_compound_presenter_for_that_compound()
      {
         A.CallTo(() => _applicationController.Close(_buildingBlock)).MustHaveHappened();
      }
   }

   public class When_the_close_building_block_presenter_invoker_is_being_notified_that_a_protocol_is_deleted : concern_for_CloseSubjectPresenterInvoker<Protocol>
   {
      [Observation]
      public void should_close_any_edit_protocol_presenter_for_that_protocol()
      {
         A.CallTo(() => _applicationController.Close(_buildingBlock)).MustHaveHappened();
      }
   }

   public class When_the_close_building_block_presenter_invoker_is_being_notified_that_an_imported_population_is_being_deleted : concern_for_CloseSubjectPresenterInvoker<ImportPopulation>
   {
      [Observation]
      public void should_close_any_edit_presenter_for_that_random_population()
      {
         A.CallTo(() => _applicationController.Close(_buildingBlock)).MustHaveHappened();
      }
   }

   public class When_the_close_building_block_presenter_invoker_is_being_notified_that_a_simulation_is_deleted : concern_for_CloseSubjectPresenterInvoker<Simulation>
   {
      [Observation]
      public void should_close_any_edit_presenter_for_that_simulation()
      {
         A.CallTo(() => _applicationController.Close(_buildingBlock)).MustHaveHappened();
      }
   }
}