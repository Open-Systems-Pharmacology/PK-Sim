using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using FakeItEasy;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_SetParameterPercentileCommand : ContextSpecification<IPKSimReversibleCommand>
   {
      protected double _percentile;
      protected IDistributedParameter _parameter;
      protected IExecutionContext _executionContext;
      protected double _oldPercentile;

      protected override void Context()
      {
         _parameter = DomainHelperForSpecs.NormalDistributedParameter();
         _oldPercentile = _parameter.Percentile;
         _parameter.Id="tralala";
         _executionContext = A.Fake<IExecutionContext>();
         A.CallTo(() => _executionContext.Get<IParameter>(_parameter.Id)).Returns(_parameter);
         A.CallTo(() => _executionContext.BuildingBlockContaining(_parameter)).Returns(A.Fake<IPKSimBuildingBlock>());
         _percentile = 0.4;
         _parameter.IsFixedValue = true;

         sut = new SetParameterPercentileCommand(_parameter, _percentile);
      }
   }

   
   public class When_executing_the_set_parameter_percentile_command : concern_for_SetParameterPercentileCommand
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_set_the_given_percentile_as_percentile_value_for_the_parameter()
      {
         _parameter.Percentile.ShouldBeEqualTo(_percentile);
      }
   }

   
   public class When_executing_the_set_parameter_percentile_inverse_command : concern_for_SetParameterPercentileCommand
   {
      private IReversibleCommand<IExecutionContext> _inverseCommand;

      protected override void Context()
      {
         base.Context();
         sut.Execute(_executionContext);
         sut.RestoreExecutionData(_executionContext);
         _inverseCommand = sut.InverseCommand(_executionContext);
      }

      protected override void Because()
      {
         _inverseCommand.Execute(_executionContext);
      }

      [Observation]
      public void should_set_the_given_percentile_as_percentile_value_for_the_parameter()
      {
         _parameter.Percentile.ShouldBeEqualTo(_oldPercentile);
      }
   }
}