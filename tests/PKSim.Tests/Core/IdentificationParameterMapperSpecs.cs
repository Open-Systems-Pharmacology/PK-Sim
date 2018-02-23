using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_IdentificationParameterMapper : ContextSpecificationAsync<IdentificationParameterMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected IdentificationParameter _identificationParameter;
      protected Snapshots.IdentificationParameter _result;
      private ISimulation _simulation;
      private IParameter _parameter1;
      private IParameter _parameter2;
      private IParameter _startValueParameter;
      protected Parameter _snapshotStartValueParameter;
      protected ParameterSelection _parameterSelection1;
      protected ParameterSelection _parameterSelection2;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         sut = new IdentificationParameterMapper(_parameterMapper);

         _identificationParameter = new IdentificationParameter
         {
            IsFixed = true,
            UseAsFactor = true,
            Scaling = Scalings.Linear
         };


         _startValueParameter = DomainHelperForSpecs.ConstantParameterWithValue().WithName(Constants.Parameters.START_VALUE);

         _identificationParameter.Add(_startValueParameter);

         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue().WithName("P1");
         _parameter2 = DomainHelperForSpecs.ConstantParameterWithValue().WithName("P2");
         _simulation = A.Fake<Simulation>().WithName("S");
         _simulation.Model.Root = new Container {_parameter1, _parameter2};

         _identificationParameter.Scaling = Scalings.Linear;
         _parameterSelection1 = new ParameterSelection(_simulation, _parameter1.Name);
         _parameterSelection2 = new ParameterSelection(_simulation, _parameter2.Name);
         _identificationParameter.AddLinkedParameter(_parameterSelection1);
         _identificationParameter.AddLinkedParameter(_parameterSelection2);

         _snapshotStartValueParameter = new Parameter();
         A.CallTo(() => _parameterMapper.MapToSnapshot(_startValueParameter)).Returns(_snapshotStartValueParameter);
         return _completed;
      }
   }

   public class When_mapping_an_indentification_parameter_to_snapshot : concern_for_IdentificationParameterMapper
   {
      protected override async Task Because()
      {
         _result = await sut.MapToSnapshot(_identificationParameter);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _result.IsFixed.ShouldBeEqualTo(_identificationParameter.IsFixed);
         _result.UseAsFactor.ShouldBeEqualTo(_identificationParameter.UseAsFactor);
         _result.Scaling.ShouldBeEqualTo(_identificationParameter.Scaling);
      }

      [Observation]
      public void should_map_underlying_parameters()
      {
         _result.Parameters.ShouldContain(_snapshotStartValueParameter);
      }

      [Observation]
      public void should_return_one_entry_for_each_linked_parameter()
      {
         _result.LinkedParameters.ShouldContain(_parameterSelection1.FullQuantityPath, _parameterSelection2.FullQuantityPath);
      }
   }
}