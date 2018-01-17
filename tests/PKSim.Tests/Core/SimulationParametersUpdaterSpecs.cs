using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationParametersUpdater : ContextSpecification<ISimulationParametersUpdater>
   {
      protected IEntityPathResolver _entityPathResolver;
      protected IParameterSetUpdater _parameterSetUpdater;

      protected override void Context()
      {
         _entityPathResolver= A.Fake<IEntityPathResolver>();
         _parameterSetUpdater= A.Fake<IParameterSetUpdater>();
         sut = new SimulationParametersUpdater(_parameterSetUpdater,_entityPathResolver);
      }
   }

   public class When_starting_the_reconciliation_of_parmaeters_in_a_simulation : concern_for_SimulationParametersUpdater
   {
      private Simulation _sourceSimulation;
      private Simulation _targetSimulation;
      private ValidationResult _result;
      private IParameter _sourceP1;
      private IParameter _sourceP2;
      private IParameter _sourceP3;
      private IParameter _targetP1;
      private IParameter _targetP4;

      protected override void Context()
      {
         base.Context();
         _sourceSimulation= A.Fake<Simulation>();
         _targetSimulation = A.Fake<Simulation>();
         _sourceP1= A.Fake<IParameter>();
         _sourceP2 = A.Fake<IParameter>();
         _sourceP3 = A.Fake<IParameter>();
         _targetP1 = A.Fake<IParameter>();
         _targetP4 = A.Fake<IParameter>();
         A.CallTo(() => _sourceSimulation.ParametersOfType(PKSimBuildingBlockType.Simulation)).Returns(new[] { _sourceP1, _sourceP2, _sourceP3 });
         A.CallTo(() => _targetSimulation.ParametersOfType(PKSimBuildingBlockType.Simulation)).Returns(new[] { _targetP1, _targetP4});

         A.CallTo(() => _entityPathResolver.PathFor(_sourceP1)).Returns("P1");
         A.CallTo(() => _entityPathResolver.PathFor(_sourceP2)).Returns("P2");
         A.CallTo(() => _entityPathResolver.PathFor(_sourceP3)).Returns("P3");
         A.CallTo(() => _entityPathResolver.PathFor(_targetP1)).Returns("P1");
         A.CallTo(() => _entityPathResolver.PathFor(_targetP4)).Returns("P4");

         //this parameter was changed by the user and does not exist anymore
         _sourceP2.Formula = new ExplicitFormula();
         _sourceP2.Editable = true;
         _sourceP2.IsFixedValue = true;

         //this parameter was NOT changed by the user and does not exist anymore
         _sourceP3.Formula = new ExplicitFormula();
         _sourceP3.IsFixedValue = false;
      }

      protected override void Because()
      {
         _result = sut.ReconciliateSimulationParametersBetween(_sourceSimulation, _targetSimulation);
      }

      [Observation]
      public void should_update_all_parameters_from_the_source_simulation_into_the_target_simulation()
      {
         A.CallTo(() => _parameterSetUpdater.UpdateValues(A<PathCache<IParameter>>._,A<PathCache<IParameter>>._, true)).MustHaveHappened();  
      }

      [Observation]
      public void should_return_a_validation_message_for_each_parameter_not_found_in_the_source_simulation_that_was_overwritten_by_the_user()
      {
         _result.Messages.Count().ShouldBeEqualTo(1);
         _result.Messages.ElementAt(0).Object.ShouldBeEqualTo(_sourceP2);
      }
   }
}	