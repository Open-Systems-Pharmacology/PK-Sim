using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualScalingTask : ContextSpecification<IIndividualScalingTask>
   {
      protected Individual _originIndividual;
      protected Individual _targetIndividual;
      protected IScalingMethodTask _scalingMethodTask;
      protected IEntityPathResolver _entityPathResolver;
      private IContainerTask _containerTask;

      protected override void Context()
      {
         _originIndividual = new Individual();
         _originIndividual.OriginData = new OriginData();
         _targetIndividual = new Individual();
         _scalingMethodTask = A.Fake<IScalingMethodTask>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _containerTask = new ContainerTask(A.Fake<IObjectBaseFactory>(), _entityPathResolver);
         sut = new IndividualScalingTask(_scalingMethodTask, _containerTask);
      }
   }

   public class When_retrieving_the_list_of_parameters_used_for_scaling_between_two_individuals : concern_for_IndividualScalingTask
   {
      private IParameter _targetParam1;
      private IParameter _targetParam2;
      private IParameter _targetParam3;
      private IParameter _originRelExpParam;
      private IParameter _originRefConcParam;

      private IParameter _originParam1;
      private IParameter _originParam2;
      private IParameter _notMatchingOriginParam1;
      private IParameter _notMatchingOriginParam2;

      private List<ParameterScaling> _allParameterScalings;

      protected override void Context()
      {
         base.Context();
         _targetParam1 = A.Fake<IParameter>().WithName("P1");
         _targetParam2 = A.Fake<IParameter>().WithName("P2");
         _targetParam3 = A.Fake<IParameter>().WithName("P3");
         _originRelExpParam = A.Fake<IParameter>().WithName(CoreConstants.Parameter.RelExp);
         _originRefConcParam = A.Fake<IParameter>().WithName(CoreConstants.Parameter.REFERENCE_CONCENTRATION);

         _originParam1 = A.Fake<IParameter>().WithName(_targetParam1.Name);
         _originParam2 = A.Fake<IParameter>().WithName(_targetParam2.Name);
         _notMatchingOriginParam1 = A.Fake<IParameter>().WithName("not match1");
         _notMatchingOriginParam2 = A.Fake<IParameter>().WithName("not match2");

         var pathParam1 = "_pathParam1";
         var pathParam2 = "_pathParam2";
         var pathParam3 = "_pathParam3";

         A.CallTo(() => _entityPathResolver.PathFor(_targetParam1)).Returns(pathParam1);
         A.CallTo(() => _entityPathResolver.PathFor(_originParam1)).Returns(pathParam1);
         A.CallTo(() => _entityPathResolver.PathFor(_targetParam2)).Returns(pathParam2);
         A.CallTo(() => _entityPathResolver.PathFor(_originParam2)).Returns(pathParam2);
         A.CallTo(() => _entityPathResolver.PathFor(_targetParam3)).Returns(pathParam3);
         A.CallTo(() => _entityPathResolver.PathFor(_notMatchingOriginParam1)).Returns("_notMatchingParam1");
         A.CallTo(() => _entityPathResolver.PathFor(_notMatchingOriginParam2)).Returns("_notMatchingParam2");

         _targetParam1.Value = 5;
         _targetParam1.DefaultValue = 7;
         _originParam1.Value = 6;

         _originParam1.IsFixedValue = true;
         _originParam1.Visible = true;
         _originParam1.Editable = true;

         _targetParam2.Visible = false;
         _targetParam3.Visible = true;
         _originRelExpParam.Visible = true;

         _targetIndividual.Add(_targetParam1);
         _targetIndividual.Add(_targetParam2);
         _targetIndividual.Add(_targetParam3);

         _originIndividual.Add(_originParam1);
         _originIndividual.Add(_originParam2);
         _originIndividual.Add(_notMatchingOriginParam1);
         _originIndividual.Add(_notMatchingOriginParam2);
         _originIndividual.Add(_originRelExpParam);
         _originIndividual.Add(_originRefConcParam);
      }

      protected override void Because()
      {
         _allParameterScalings = sut.AllParameterScalingsFrom(_originIndividual, _targetIndividual).ToList();
      }

      [Observation]
      public void should_return_one_parameter_scaling_for_each_visible_parameter_from_the_target_individual_for_which_the_scaling_is_actually_allowed()
      {
         //only one parameter that need scaling, since one is not visible, the other one is not matched and the last one is a relative expression parameter
         //and the last one is a reference concentration parameter
         _allParameterScalings.Count.ShouldBeEqualTo(1);
         _allParameterScalings.Any(x => x.SourceParameter.IsNamed(_targetParam1.Name)).ShouldBeTrue();
      }

      [Observation]
      public void should_leverage_the_scaling_method_task_to_retrieve_the_default_scaling_method_for_the_parameter_scaling()
      {
         foreach (var parameterScaling in _allParameterScalings)
         {
            A.CallTo(() => _scalingMethodTask.DefaultMethodFor(parameterScaling)).MustHaveHappened();
         }
      }

      [Observation]
      public void should_not_return_the_reference_concentration_parameter()
      {
         _allParameterScalings.Any(x => x.SourceParameter.IsNamed(CoreConstants.Parameter.REFERENCE_CONCENTRATION)).ShouldBeFalse();
      }

      [Observation]
      public void should_not_return_the_expression_parameters()
      {
         _allParameterScalings.Any(x => x.SourceParameter.IsNamed(CoreConstants.Parameter.RelExp)).ShouldBeFalse();
      }

      [Observation]
      public void should_not_return_the_hiddent_parameters()
      {
         _allParameterScalings.Any(x => x.SourceParameter.IsNamed(_targetParam2.Name)).ShouldBeFalse();
      }
   }

   public class When_asked_to_perform_the_parameter_scaling : concern_for_IndividualScalingTask
   {
      private ParameterScaling _parameterScaling1;
      private ParameterScaling _parameterScaling2;
      private ICommand _scalingCommand1;
      private ICommand _scalingCommand2;
      private IEnumerable<ICommand> _result;

      protected override void Context()
      {
         base.Context();
         _parameterScaling1 = A.Fake<ParameterScaling>();
         _parameterScaling2 = A.Fake<ParameterScaling>();
         _scalingCommand1 = A.Fake<IPKSimCommand>();
         _scalingCommand2 = A.Fake<IPKSimCommand>();
         A.CallTo(() => _parameterScaling1.Scale()).Returns(_scalingCommand1);
         A.CallTo(() => _parameterScaling2.Scale()).Returns(_scalingCommand2);
      }

      protected override void Because()
      {
         _result = sut.PerformScaling(new[] {_parameterScaling1, _parameterScaling2});
      }

      [Observation]
      public void should_iterate_over_all_defined_parameter_scalings_and_perform_the_scaling_on_the_target_individual()
      {
         A.CallTo(() => _parameterScaling1.Scale()).MustHaveHappened();
         A.CallTo(() => _parameterScaling2.Scale()).MustHaveHappened();
      }

      [Observation]
      public void should_return_a_macro_command_contain_one_command_for_each_parameter_scaling()
      {
         _result.ShouldNotBeNull();
         _result.All().ShouldOnlyContain(_scalingCommand1, _scalingCommand2);
      }
   }
}