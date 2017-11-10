using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using Individual = PKSim.Core.Model.Individual;

namespace PKSim.Core
{
   public abstract class concern_for_ExpressionContainerMapper : ContextSpecificationAsync<ExpressionContainerMapper>
   {
      protected MoleculeExpressionContainer _moleculeExpressionContainer;
      protected TransporterExpressionContainer _transporterExpressionContainer;
      protected IndividualMolecule _enzyme;
      protected IParameter _relativeExpressionParameter;
      protected IParameter _transporterRelativeExpressionParameter;
      protected ExpressionContainer _snapshot;
      protected ParameterMapper _parameterMapper;
      protected IndividualTransporter _transporter;
      protected ITransportContainerUpdater _transportContainerUpdater;
      protected Individual _individual;
      protected ExpressionContainerMapperContext _expressionContainerMapperContext;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _transportContainerUpdater = A.Fake<ITransportContainerUpdater>();
         sut = new ExpressionContainerMapper(_parameterMapper, _transportContainerUpdater);

         _relativeExpressionParameter = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameter.REL_EXP);
         _moleculeExpressionContainer = new MoleculeExpressionContainer().WithName("EXP");
         _moleculeExpressionContainer.Add(_relativeExpressionParameter);

         _transporterRelativeExpressionParameter = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameter.REL_EXP);
         _transporterExpressionContainer = new TransporterExpressionContainer().WithName("TRANS");
         _transporterExpressionContainer.MembraneLocation = MembraneLocation.Apical;

         _transporterExpressionContainer.Add(_transporterRelativeExpressionParameter);

         _individual = new Individual {OriginData = new  Model.OriginData {Species = new Species().WithName("Human")}};

         _expressionContainerMapperContext = new ExpressionContainerMapperContext
         {
            SimulationSubject = _individual
         };

         _enzyme = new IndividualEnzyme {_moleculeExpressionContainer};
         _transporter = new IndividualTransporter {_transporterExpressionContainer};
         return _completed;
      }
   }

   public class When_mapping_a_molecule_expression_container_with_a_default_value_to_snapshot : concern_for_ExpressionContainerMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_moleculeExpressionContainer);
      }

      [Observation]
      public void should_return_null()
      {
         _snapshot.ShouldBeNull();
      }
   }

   public class When_mapping_a_transport_molecule_expression_container_with_a_default_value_to_snapshot : concern_for_ExpressionContainerMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_transporterExpressionContainer);
      }

      [Observation]
      public void should_return_an_expression_container_containing_only_the_membrane_location()
      {
         _snapshot.Value.ShouldBeNull();
         _snapshot.MembraneLocation.ShouldBeEqualTo(_transporterExpressionContainer.MembraneLocation);
      }
   }

   public class When_mappign_a_expression_container_with_a_changed_value : concern_for_ExpressionContainerMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _relativeExpressionParameter.Value = 5;
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_moleculeExpressionContainer);
      }

      [Observation]
      public void should_update_the_parameter_value_from_expression_parameter()
      {
         A.CallTo(() => _parameterMapper.UpdateSnapshotFromParameter(_snapshot, _relativeExpressionParameter)).MustHaveHappened();
      }

      [Observation]
      public void should_have_set_the_name_of_the_expression_container_to_the_name_of_the_container()
      {
         _snapshot.Name.ShouldBeEqualTo(_moleculeExpressionContainer.Name);
      }
   }

   public class when_updating_a_undefined_expression_container_from_snapshot : concern_for_ExpressionContainerMapper
   {
      protected override Task Because()
      {
         return sut.MapToModel(null, new ExpressionContainerMapperContext());
      }

      [Observation]
      public void should_not_do_anything()
      {
      }
   }

   public class When_updating_an_molecule_expression_container_from_snapshot : concern_for_ExpressionContainerMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _relativeExpressionParameter.Value = 5;
         _snapshot = await sut.MapToSnapshot(_moleculeExpressionContainer);
         _expressionContainerMapperContext.Molecule = _enzyme;
      }

      protected override Task Because()
      {
         return sut.MapToModel(_snapshot, _expressionContainerMapperContext);
      }

      [Observation]
      public void should_update_the_expression_container_with_the_expected_value()
      {
         A.CallTo(() => _parameterMapper.MapToModel(_snapshot, _relativeExpressionParameter)).MustHaveHappened();
      }
   }


   public class When_updating_a_transporter_expression_container_from_snapshot : concern_for_ExpressionContainerMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_transporterExpressionContainer);
         _snapshot.MembraneLocation = MembraneLocation.Basolateral;
         _expressionContainerMapperContext.Molecule = _transporter;
      }

      protected override Task Because()
      {
         return sut.MapToModel(_snapshot, _expressionContainerMapperContext);
      }

      [Observation]
      public void should_update_the_expression_container_with_the_expected_value()
      {
         A.CallTo(() => _parameterMapper.MapToModel(_snapshot, _transporterRelativeExpressionParameter)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_transporter_container_properties()
      {
         A.CallTo(() => _transportContainerUpdater.UpdateTransporterFromTemplate(_transporterExpressionContainer, _individual.Species.Name, _snapshot.MembraneLocation.Value, _transporter.TransportType)).MustHaveHappened();
      }
   }
}