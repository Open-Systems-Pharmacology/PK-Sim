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
      protected ValueOrigin _previousValueOrigin;
      protected IParameter _buildingBlockParameter;

      protected override void Context()
      {
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _parameter.Id = "ParameterId";
         _parameter.Origin.ParameterId = "BuildingBlockInSimulationParameter";
         _parameter.Origin.BuilingBlockId = "BuildingBlockId";
         _parameter.Origin.SimulationId = "SimulationId";
         _parameter.BuildingBlockType = PKSimBuildingBlockType.Formulation;

         _valueOrigin = new ValueOrigin
         {
            Description = "Hello",
            Method = ValueOriginDeterminationMethods.InVitro,
            Source = ValueOriginSources.Other
         };

         _previousValueOrigin = new ValueOrigin
         {
            Description = "OldValueOrigin",
            Method = ValueOriginDeterminationMethods.InVivo,
            Source = ValueOriginSources.ParameterIdentification
         };

         _parameter.ValueOrigin.UpdateFrom(_previousValueOrigin);

         sut = new UpdateParameterValueOriginCommand(_parameter, _valueOrigin);

         _buildingBlockParameter = DomainHelperForSpecs.ConstantParameterWithValue(20);
         _buildingBlockParameter.Id = _parameter.Origin.ParameterId;

         _context = A.Fake<IExecutionContext>();
         A.CallTo(() => _context.Get<IParameter>(_parameter.Id)).Returns(_parameter);
         A.CallTo(() => _context.Get<IParameter>(_buildingBlockParameter.Id)).Returns(_buildingBlockParameter);
      }
   }

   public class When_executing_the_update_parameter_value_origin_command : concern_for_UpdateParameterValueOriginCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_update_the_value_origin_of_the_parameter()
      {
         _parameter.ValueOrigin.ShouldBeEqualTo(_valueOrigin);
      }

      [Observation]
      public void should_update_the_value_origin_of_the_associated_building_block_parameter_if_available()
      {
         _buildingBlockParameter.ValueOrigin.ShouldBeEqualTo(_valueOrigin);
      }

      [Observation]
      public void should_update_the_command_type_and_description()
      {
         sut.Description.ShouldNotBeEmpty();
      }
   }

   public class When_executin_the_inverse_of_the_update_parameter_value_origin_command : concern_for_UpdateParameterValueOriginCommand
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_context);
      }

      [Observation]
      public void should_have_reset_the_value_origin_to_its_original_value()
      {
         _parameter.ValueOrigin.ShouldBeEqualTo(_previousValueOrigin);
      }

      [Observation]
      public void should_have_reset_the_value_origin_of_the_associated_building_block_parameter_if_available()
      {
         _buildingBlockParameter.ValueOrigin.ShouldBeEqualTo(_previousValueOrigin);
      }
   }
}