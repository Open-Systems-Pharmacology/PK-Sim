using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Services;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SwapSimulationCommand : ContextSpecification<SwapSimulationCommand>
   {
      protected IExecutionContext _context;
      protected Simulation _oldSimulation;
      protected Simulation _newSimulation;
      protected PKSimProject _project;
      protected ISimulationCommandDescriptionBuilder _simulationDifferenceBuilder;
      protected ISimulationReferenceUpdater _simulationReferenceUpdater;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _oldSimulation = new IndividualSimulation().WithName("Old");
         _newSimulation = new IndividualSimulation().WithName("New");
         _project =new PKSimProject();
         _simulationDifferenceBuilder = A.Fake<ISimulationCommandDescriptionBuilder>();
         _simulationReferenceUpdater = A.Fake<ISimulationReferenceUpdater>();
         A.CallTo(() => _context.CurrentProject).Returns(_project);
         A.CallTo(() => _context.Resolve<ISimulationCommandDescriptionBuilder>()).Returns(_simulationDifferenceBuilder);
         A.CallTo(() => _context.Resolve<ISimulationReferenceUpdater>()).Returns(_simulationReferenceUpdater);
         var reportPart = new ReportPart();
         reportPart.AddToContent("toto");
         A.CallTo(() => _simulationDifferenceBuilder.BuildDifferenceBetween(_oldSimulation, _newSimulation)).Returns(reportPart);
         sut = new SwapSimulationCommand(_oldSimulation, _newSimulation, _context);

         _project.AddBuildingBlock(_oldSimulation);
      }
   }

   public class When_executing_the_swap_simulation_and_the_simulation_is_used_by_a_sensitivity_analysis : concern_for_SwapSimulationCommand
   {
      private SensitivityAnalysis _sensitivityAnalysis;

      protected override void Context()
      {
         base.Context();
         _sensitivityAnalysis = new SensitivityAnalysis {Simulation = _oldSimulation};
         _project.AddSensitivityAnalysis(_sensitivityAnalysis);
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void the_analysis_should_reference_the_new_simulation()
      {
         _sensitivityAnalysis.Simulation.ShouldBeEqualTo(_newSimulation);
      }
   }

   public class When_executing_the_swap_simulation_command_and_the_removed_simulation_is_classified : concern_for_SwapSimulationCommand
   {
      protected override void Context()
      {
         base.Context();
         var classifiable = new Classification().WithId(_oldSimulation.Id);
         _project.AddClassifiable(classifiable);
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void the_project_must_be_updated_with_the_simulation_correctly_classified_according_to_the_old_simulation_classification()
      {
         _project.AllClassifiables.OfType<ClassifiableSimulation>().Select(x=>x.Simulation).ShouldContain(_newSimulation);
      }
   }

   public class When_executing_the_swap_simulation_command : concern_for_SwapSimulationCommand
   {
      private ParameterIdentification _parameterIdentification;

      protected override void Context()
      {
         base.Context();
         _parameterIdentification = A.Fake<ParameterIdentification>();
         _project.AddParameterIdentification(_parameterIdentification);
         _parameterIdentification.IsLoaded = true;
         A.CallTo(() => _parameterIdentification.UsesSimulation(_oldSimulation)).Returns(true);
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void parameter_identifications_should_have_the_reference_to_the_new_simulation()
      {
         A.CallTo(() => _simulationReferenceUpdater.SwapSimulationInParameterAnalysables(_oldSimulation, _newSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_remove_the_old_simulation_from_the_project()
      {
         _project.All<Simulation>().ShouldNotContain(_oldSimulation);
      }

      [Observation]
      public void should_added_the_new_simulation_to_the_project()
      {
         _project.All<Simulation>().ShouldContain(_newSimulation);
      }

      [Observation]
      public void should_have_updated_the_extended_description_with_the_difference_between_the_two_simulation()
      {
         sut.ExtendedDescription.Contains("toto").ShouldBeTrue();
      }
   }


   public class The_inverse_of_the_swap_simulation_command : concern_for_SwapSimulationCommand
   {
      private IReversibleCommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_a_swap_simulation_command()
      {
         _result.ShouldBeAnInstanceOf<SwapSimulationCommand>();
      }

      [Observation]
      public void should_have_beeen_marked_as_inverse_for_the_swap_simulation_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}