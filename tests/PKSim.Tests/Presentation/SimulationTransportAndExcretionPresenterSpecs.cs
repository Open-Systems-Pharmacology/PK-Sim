using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;


namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationTransportAndExcretionPresenter : ContextSpecification<ISimulationCompoundTransportAndExcretionPresenter>
   {
      protected IPartialProcessRetriever _partialProcessRetriever;
      protected ISimulationCompoundProcessView<TransportPartialProcess, SimulationPartialProcessSelectionDTO> _view;
      protected Simulation _simulation;
      protected Compound _compound;
      protected IList<SimulationPartialProcess> _selectedSimulationPrtialProcess;
      protected CompoundProperties _compoundProperties;

      protected override void Context()
      {
         _partialProcessRetriever = A.Fake<IPartialProcessRetriever>();
         _view = A.Fake<ISimulationCompoundProcessView<TransportPartialProcess, SimulationPartialProcessSelectionDTO>>();
         _selectedSimulationPrtialProcess = new List<SimulationPartialProcess>();
         _compound = new Compound();
         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         _compoundProperties = new CompoundProperties{Compound = _compound};
         _simulation.Properties.AddCompoundProperties(_compoundProperties);
         A.CallTo(_partialProcessRetriever).WithReturnType<IEnumerable<SimulationPartialProcess>>().Returns(_selectedSimulationPrtialProcess);

         sut = new SimulationCompoundTransportAndExcretionPresenter(_view, _partialProcessRetriever);
      }
   }

   public class When_the_simulation_transport_presenter_is_checking_the_validity_of_a_configuration_containing_a_GFR_and_plasma_clearance : concern_for_SimulationTransportAndExcretionPresenter
   {
      protected override void Context()
      {
         base.Context();

         _compound.AddProcess(new SystemicProcess {Name = "PLS", InternalName = CoreConstants.Process.KIDNEY_CLEARANCE, SystemicProcessType = SystemicProcessTypes.Renal});
         _compound.AddProcess(new SystemicProcess {Name = "GFR", InternalName = "GFR", SystemicProcessType = SystemicProcessTypes.GFR});
      }

      protected override void Because()
      {
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void should_show_the_warning_field()
      {
         _view.WarningVisible.ShouldBeTrue();
      }

      [Observation]
      public void should_display_the_accurate_warning_message()
      {
         _view.Warning.ShouldBeEqualTo(PKSimConstants.Warning.RenalAndGFRSelected);
      }
   }

   public class When_the_simulation_transport_presenter_is_checking_the_validity_of_a_configuration_containing_a_GFR_and_TS_clearance : concern_for_SimulationTransportAndExcretionPresenter
   {
      protected override void Context()
      {
         base.Context();

         _compound.AddProcess(new SystemicProcess {Name = "TS", InternalName = "TS", SystemicProcessType = SystemicProcessTypes.Renal});
         _compound.AddProcess(new SystemicProcess {Name = "GFR", InternalName = "GFR", SystemicProcessType = SystemicProcessTypes.GFR});
      }

      protected override void Because()
      {
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void should_hide_the_warning_field()
      {
         _view.WarningVisible.ShouldBeFalse();
      }
   }

   public class When_the_simulation_transport_presenter_is_checking_the_validity_of_a_configuration_containing_only_a_GFR_clearance : concern_for_SimulationTransportAndExcretionPresenter
   {
      protected override void Context()
      {
         base.Context();

         _compound.AddProcess(new SystemicProcess {Name = "GFR", InternalName = "GFR", SystemicProcessType = SystemicProcessTypes.GFR});
      }

      protected override void Because()
      {
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void should_hide_the_warning_field()
      {
         _view.WarningVisible.ShouldBeFalse();
      }
   }

   public class When_the_simulation_transport_presenter_is_checking_the_validity_of_a_configuration_containing_only_a_plasma_clearance : concern_for_SimulationTransportAndExcretionPresenter
   {
      protected override void Context()
      {
         base.Context();
         _compound.AddProcess(new SystemicProcess {Name = "PLS", InternalName = CoreConstants.Process.KIDNEY_CLEARANCE, SystemicProcessType = SystemicProcessTypes.Renal});
      }

      protected override void Because()
      {
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void should_hide_the_warning_field()
      {
         _view.WarningVisible.ShouldBeFalse();
      }
   }

   public class When_the_simulation_transport_presenter_is_checking_the_validity_of_a_configuration_containing_no_processes : concern_for_SimulationTransportAndExcretionPresenter
   {
      protected override void Because()
      {
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void should_hide_the_warning_field()
      {
         _view.WarningVisible.ShouldBeFalse();
      }
   }
}