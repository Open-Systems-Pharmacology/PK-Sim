using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;

namespace PKSim.Core
{
   public abstract class concern_for_AddPKAnalysesToSimulationCommand : ContextSpecification<AddPKAnalysesToSimulationCommand>
   {
      protected List<QuantityPKParameter> _pkParameters;
      protected PopulationSimulation _populationSimulation;
      protected string _fileName;

      protected override void Context()
      {
         _pkParameters = new List<QuantityPKParameter>();
         _populationSimulation = A.Fake<PopulationSimulation>();
         _fileName = "ABC";
         sut = new AddPKAnalysesToSimulationCommand(_populationSimulation, _pkParameters, _fileName);
      }
   }

   public class When_executing_the_add_pk_analysis_to_simulation_command : concern_for_AddPKAnalysesToSimulationCommand
   {
      private IExecutionContext _context;
      private PopulationSimulationPKAnalyses _pkAnalysis;
      private QuantityPKParameter _pkParameter1;
      private QuantityPKParameter _pkParameter2;

      protected override void Context()
      {
         base.Context();
         _context = A.Fake<IExecutionContext>();
         _pkParameter1 = A.Fake<QuantityPKParameter>();
         A.CallTo(() => _pkParameter1.Id).Returns("Id1");
         _pkParameter2 = A.Fake<QuantityPKParameter>();
         A.CallTo(() => _pkParameter2.Id).Returns("Id2");
         _pkParameters.Add(_pkParameter1);
         _pkParameters.Add(_pkParameter2);
         _pkAnalysis = new PopulationSimulationPKAnalyses();
         _populationSimulation.PKAnalyses = _pkAnalysis;
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_have_added_the_pk_parameters_to_the_existing_pk_parameters()
      {
         _pkAnalysis.All().ShouldContain(_pkParameter1, _pkParameter2);
      }

      [Observation]
      public void should_notify_that_the_simulation_results_were_updated()
      {
         A.CallTo(() => _context.PublishEvent(A<SimulationResultsUpdatedEvent>._)).MustHaveHappened();
      }

      [Observation]
      public void should_use_the_file_name_in_the_command_description()
      {
         sut.Description.Contains(_fileName).ShouldBeTrue();
      }
   }
}