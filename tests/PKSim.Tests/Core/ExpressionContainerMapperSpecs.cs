using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Core.Snapshots.Services;
using Individual = PKSim.Core.Model.Individual;
using OriginData = PKSim.Core.Model.OriginData;
using static OSPSuite.Core.Domain.Constants.Parameters;

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
      protected Individual _individual;
      protected ExpressionContainerMapperContext _expressionContainerMapperContext;
      protected IOSPSuiteLogger _logger;
      protected TransporterExpressionContainer _bloodCellsExpressionContainer;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _logger = A.Fake<IOSPSuiteLogger>();

         sut = new ExpressionContainerMapper(_parameterMapper, _logger);

         _relativeExpressionParameter =
            DomainHelperForSpecs.ConstantParameterWithValue(0, isDefault: true).WithName(REL_EXP);
         _moleculeExpressionContainer = new MoleculeExpressionContainer().WithName("EXP");
         _moleculeExpressionContainer.Add(_relativeExpressionParameter);

         _transporterRelativeExpressionParameter =
            DomainHelperForSpecs.ConstantParameterWithValue(0, isDefault: true).WithName(REL_EXP);
         _transporterExpressionContainer = new TransporterExpressionContainer().WithName("TRANS");
         _transporterExpressionContainer.TransportDirection = TransportDirectionId.InfluxInterstitialToIntracellular;

         _bloodCellsExpressionContainer = new TransporterExpressionContainer().WithName(CoreConstants.Compartment.BLOOD_CELLS);
         _bloodCellsExpressionContainer.TransportDirection = TransportDirectionId.BiDirectionalBloodCellsPlasma;

         _transporterExpressionContainer.Add(_transporterRelativeExpressionParameter);

         _individual = new Individual {OriginData = new OriginData {Species = new Species().WithName("Human")}};
         _expressionContainerMapperContext = new ExpressionContainerMapperContext(new SnapshotContext())
         {
         };

         _enzyme = new IndividualEnzyme {_moleculeExpressionContainer};
         _transporter = new IndividualTransporter {_transporterExpressionContainer, _bloodCellsExpressionContainer};
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

   public class When_mapping_a_surrogate_compartment : concern_for_ExpressionContainerMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_bloodCellsExpressionContainer);
      }

      [Observation]
      public void should_set_the_name_to_the_name_of_the_container()
      {
         _snapshot.Name.ShouldBeEqualTo(_bloodCellsExpressionContainer.Name);
      }

      [Observation]
      public void should_set_the_compartment_name_to_null()
      {
         _snapshot.CompartmentName.ShouldBeNull();
      }

      [Observation]
      public void should_update_the_transport_direction()
      {
         _snapshot.TransportDirection.ShouldBeEqualTo(_bloodCellsExpressionContainer.TransportDirection);
      }
   }

   public class when_updating_a_undefined_expression_container_from_snapshot : concern_for_ExpressionContainerMapper
   {
      protected override Task Because()
      {
         return sut.MapToModel(null, _expressionContainerMapperContext);
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
         _snapshot = new ExpressionContainer {Name = "Plasma", Value = 5};
         _expressionContainerMapperContext.Molecule = _enzyme;
         _expressionContainerMapperContext.ExpressionParameters = new Cache<string, IParameter> {{_snapshot.Name, _relativeExpressionParameter}};
      }

      protected override Task Because()
      {
         return sut.MapToModel(_snapshot, _expressionContainerMapperContext);
      }

      [Observation]
      public void should_update_the_expression_container_with_the_expected_value()
      {
         A.CallTo(() => _parameterMapper.MapToModel(_snapshot, A<ParameterSnapshotContext>.That.Matches(x => x.Parameter == _relativeExpressionParameter))).MustHaveHappened();
      }
   }

   public class When_updating_a_transporter_expression_container_from_snapshot : concern_for_ExpressionContainerMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _transporterRelativeExpressionParameter.Value = 5;
         _transporterRelativeExpressionParameter.IsDefault = false;
         _snapshot = await sut.MapToSnapshot(_transporterExpressionContainer);
         _snapshot.MembraneLocation = MembraneLocation.Basolateral;
         _snapshot.Value = 10;
         _expressionContainerMapperContext.Molecule = _transporter;
         _expressionContainerMapperContext.ExpressionParameters = new Cache<string, IParameter>
            {{_snapshot.Name, _transporterRelativeExpressionParameter}};
         _expressionContainerMapperContext.MoleculeExpressionContainers = new[] {_transporterExpressionContainer};
      }

      protected override Task Because()
      {
         return sut.MapToModel(_snapshot, _expressionContainerMapperContext);
      }

      [Observation]
      public void should_update_the_expression_container_with_the_expected_value()
      {
         A.CallTo(() => _parameterMapper.MapToModel(_snapshot, A<ParameterSnapshotContext>.That.Matches(x => x.Parameter == _transporterRelativeExpressionParameter))).MustHaveHappened();
      }
   }
}