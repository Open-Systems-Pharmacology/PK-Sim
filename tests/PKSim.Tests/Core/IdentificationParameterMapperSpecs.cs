using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_IdentificationParameterMapper : ContextSpecificationAsync<IdentificationParameterMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected IdentificationParameter _identificationParameter;
      protected Snapshots.IdentificationParameter _snapshot;
      private Simulation _simulation;
      private IParameter _parameter1;
      private IParameter _parameter2;
      private IParameter _startValueParameter;
      protected Parameter _snapshotStartValueParameter;
      protected ParameterSelection _parameterSelection1;
      protected ParameterSelection _parameterSelection2;
      protected IIdentificationParameterFactory _identificationParameterFactory;
      protected ILogger _logger;
      protected ParameterIdentificationContext _parameterIdentificationContext;
      private ParameterIdentification _parameterIdentification;
      private PKSimProject _project;
      protected IIdentificationParameterTask _identificationParameterTask;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _identificationParameterFactory= A.Fake<IIdentificationParameterFactory>();
         _logger= A.Fake<ILogger>();
         _identificationParameterTask = A.Fake<IIdentificationParameterTask>();
         sut = new IdentificationParameterMapper(_parameterMapper, _identificationParameterFactory, _identificationParameterTask,  _logger);

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

         _project =new PKSimProject();
         _project.AddBuildingBlock(_simulation);
         _parameterIdentification = new ParameterIdentification();
         _parameterIdentificationContext = new ParameterIdentificationContext(_parameterIdentification, _project);
         return _completed;
      }
   }

   public class When_mapping_an_indentification_parameter_to_snapshot : concern_for_IdentificationParameterMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_identificationParameter);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.IsFixed.ShouldBeEqualTo(_identificationParameter.IsFixed);
         _snapshot.UseAsFactor.ShouldBeEqualTo(_identificationParameter.UseAsFactor);
         _snapshot.Scaling.ShouldBeEqualTo(_identificationParameter.Scaling);
      }

      [Observation]
      public void should_map_underlying_parameters()
      {
         _snapshot.Parameters.ShouldContain(_snapshotStartValueParameter);
      }

      [Observation]
      public void should_return_one_entry_for_each_linked_parameter()
      {
         _snapshot.LinkedParameters.ShouldContain(_parameterSelection1.FullQuantityPath, _parameterSelection2.FullQuantityPath);
      }
   }

   public class When_mapping_an_identification_parameter_snapshot_to_identification_parameter : concern_for_IdentificationParameterMapper
   {
      private IdentificationParameter _newParameterIdentification;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_identificationParameter);
         A.CallTo(() => _identificationParameterFactory.CreateFor(A<IEnumerable<ParameterSelection>>._, _parameterIdentificationContext.ParameterIdentification)).Returns(_identificationParameter);
      }

      protected override async Task Because()
      {
         _newParameterIdentification = await sut.MapToModel(_snapshot, _parameterIdentificationContext);
      }

      [Observation]
      public void should_return_an_identification_parameter_with_the_expected_properties()
      {
         _newParameterIdentification.IsFixed.ShouldBeEqualTo(_identificationParameter.IsFixed);
         _newParameterIdentification.UseAsFactor.ShouldBeEqualTo(_identificationParameter.UseAsFactor);
         _newParameterIdentification.Scaling.ShouldBeEqualTo(_identificationParameter.Scaling);
      }

      [Observation]
      public void should_call_the_update_range_method_if_the_identification_parameter_is_using_factor()
      {
         A.CallTo(() => _identificationParameterTask.UpdateParameterRange(_newParameterIdentification)).MustHaveHappened();
      }
   }
}