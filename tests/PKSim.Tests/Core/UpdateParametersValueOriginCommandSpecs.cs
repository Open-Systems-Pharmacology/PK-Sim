using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_UpdateParametersValueOriginCommand : ContextSpecification<UpdateParametersValueOriginCommand>
   {
      protected List<IParameter> _parameters;
      protected ValueOrigin _valueOrigin;
      protected IParameter _parameter1;
      protected IParameter _parameter2;
      protected IExecutionContext _context;

      protected override void Context()
      {
         _parameters = new List<IParameter>();
          _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _parameter2 = DomainHelperForSpecs.ConstantParameterWithValue(10);

         _valueOrigin = new ValueOrigin
         {
            Method = ValueOriginDeterminationMethods.Assumption,
            Source = ValueOriginSources.Other,
            Description = "Hello"
         };

         _context= A.Fake<IExecutionContext>();

         sut = new UpdateParametersValueOriginCommand(_parameters, _valueOrigin);
      }
   }

   public class When_executing_the_update_parameter_value_origin_command_for_a_set_of_parameters : concern_for_UpdateParametersValueOriginCommand
   {
      protected override void Context()
      {
         base.Context();
         _parameters.Add(_parameter1);
         _parameters.Add(_parameter2);
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_update_the_value_origin_for_all_parameters()
      {
         _parameter1.ValueOrigin.ShouldBeEqualTo(_valueOrigin);
         _parameter2.ValueOrigin.ShouldBeEqualTo(_valueOrigin);
      }

      [Observation]
      public void should_contain_one_command_for_each_parameter()
      {
         sut.Count.ShouldBeEqualTo(_parameters.Count);
      }

      [Observation]
      public void only_the_first_command_should_be_visible()
      {
         sut.All().First().Visible.ShouldBeTrue();
         //start at one so that we do not check the first command
         for (int i = 1; i < sut.Count; i++)
         {
            sut.All().ElementAt(i).Visible.ShouldBeFalse();
         }
      }
   }
}