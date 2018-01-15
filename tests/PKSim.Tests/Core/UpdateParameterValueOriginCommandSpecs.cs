using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_UpdateParameterValueOriginCommand : ContextSpecification<UpdateParameterValueOriginCommand>
   {
      protected IParameter _parameter;
      protected ValueOrigin _valueOrigin;
      protected IExecutionContext _context;

      protected override void Context()
      {
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _valueOrigin = new ValueOrigin
         {
            Description = "Hello",
            Method = ValueOriginDeterminationMethods.InVitroAssay,
            Source = ValueOriginSources.Other
         };

         sut = new UpdateParameterValueOriginCommand(_parameter, _valueOrigin);

         _context = A.Fake<IExecutionContext>();
      }
   }

   public class When_executing_the_update_parameter_value_origin_command : concern_for_UpdateParameterValueOriginCommand
   {
      private IParameter _buildingBlockParameter;

      protected override void Context()
      {
         base.Context();
         _buildingBlockParameter= A.Fake<IParameter>();
         _parameter.Origin.ParameterId = "BuildingBlockInSimulationParameter";
         A.CallTo(() => _context.Get<IParameter>(_parameter.Origin.ParameterId)).Returns(_buildingBlockParameter);
      }
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_update_the_value_origin_of_the_parameter()
      {
         _parameter.ValueOrigin.IsIdenticalTo(_valueOrigin).ShouldBeTrue();
      }

      [Observation]
      public void should_update_the_value_origin_of_the_associated_building_block_parameter_if_available()
      {
         _buildingBlockParameter.ValueOrigin.IsIdenticalTo(_valueOrigin).ShouldBeTrue();
      }

      [Observation]
      public void should_update_the_command_type_and_description()
      {
         sut.Description.ShouldNotBeEmpty();
      }
   }
}	