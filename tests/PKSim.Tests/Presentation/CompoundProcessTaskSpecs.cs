using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundProcessTask : ContextSpecification<ICompoundProcessTask>
   {
      protected IExecutionContext _executionContext;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         sut = new CompoundProcessTask(_executionContext);
      }
   }

   public class When_creating_a_process_from_template : concern_for_CompoundProcessTask
   {
      private Compound _compound;
      private SystemicProcess _template;
      private CompoundProcess _result;
      private SystemicProcess _clone;
      private IParameter _p1;
      private IParameter _p2;
      private ParameterAlternative _fuAlternative;
      private readonly string _anotherParameter = "TOTO";

      protected override void Context()
      {
         base.Context();
         _compound = new Compound();
         var fuGroup = new ParameterAlternativeGroup().WithName(CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND);
         _fuAlternative = new ParameterAlternative().WithName("MyFu");
         _fuAlternative.Add(DomainHelperForSpecs.ConstantParameterWithValue(0.2).WithName(CoreConstants.Parameters.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE));
         _fuAlternative.IsDefault = true;
         fuGroup.AddAlternative(_fuAlternative);
         _compound.AddParameterAlternativeGroup(fuGroup);
         _template = new SystemicProcess();
         _p1 = DomainHelperForSpecs.ConstantParameterWithValue(0.5, isDefault: true).WithName(CoreConstants.Parameters.FRACTION_UNBOUND_EXPERIMENT);
         _p2 = DomainHelperForSpecs.ConstantParameterWithValue(0.9, isDefault: true).WithName(_anotherParameter);
         _template.Add(_p1);
         _template.Add(_p2);
         _clone = new SystemicProcess {DomainHelperForSpecs.ConstantParameterWithValue(0.5).WithName(CoreConstants.Parameters.FRACTION_UNBOUND_EXPERIMENT)};

         A.CallTo(() => _executionContext.Clone(_template)).Returns(_clone);
      }

      protected override void Because()
      {
         _result = sut.CreateProcessFromTemplate(_template, _compound);
      }

      [Observation]
      public void should_return_a_clone_from_the_given_template()
      {
         _result.ShouldBeEqualTo(_clone);
      }

      [Observation]
      public void should_have_set_the_compound_specific_parameter_using_the_defined_value_if_available()
      {
         _result.Parameter(CoreConstants.Parameters.FRACTION_UNBOUND_EXPERIMENT).Value.ShouldBeEqualTo(_fuAlternative.Parameter(CoreConstants.Parameters.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE).Value);
      }

      [Observation]
      public void should_have_set_the_is_default_flag_for_updated_parameters_to_false()
      {
         _result.Parameter(CoreConstants.Parameters.FRACTION_UNBOUND_EXPERIMENT).IsDefault.ShouldBeFalse();
      }
   }
}