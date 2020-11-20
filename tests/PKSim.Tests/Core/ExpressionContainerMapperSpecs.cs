using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Core.Snapshots.Services;
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
      protected IOSPLogger _logger;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _transportContainerUpdater = A.Fake<ITransportContainerUpdater>();
         _logger= A.Fake<IOSPLogger>();

         sut = new ExpressionContainerMapper(_parameterMapper, _transportContainerUpdater, _logger);

         _relativeExpressionParameter = DomainHelperForSpecs.ConstantParameterWithValue(0, isDefault:true).WithName(CoreConstants.Parameters.REL_EXP);
         _moleculeExpressionContainer = new MoleculeExpressionContainer().WithName("EXP");
         _moleculeExpressionContainer.Add(_relativeExpressionParameter);

         _transporterRelativeExpressionParameter = DomainHelperForSpecs.ConstantParameterWithValue(0, isDefault: true).WithName(CoreConstants.Parameters.REL_EXP);
         _transporterExpressionContainer = new TransporterExpressionContainer().WithName("TRANS");
         _transporterExpressionContainer.TransportDirection = TransportDirections.Influx;

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
      public void should_return_null()
      {
         _snapshot.ShouldBeNull();
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
         _snapshot = new ExpressionContainer {Name = "Plasma", Value = 5};
         _expressionContainerMapperContext.Molecule = _enzyme;
         _expressionContainerMapperContext.ExpressionParameters = new Cache<string, IParameter>{{ _snapshot.Name, _relativeExpressionParameter }};
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
         _transporterRelativeExpressionParameter.Value = 5;
         _transporterRelativeExpressionParameter.IsDefault = false;
         _snapshot = await sut.MapToSnapshot(_transporterExpressionContainer);
         _snapshot.MembraneLocation = MembraneLocation.Basolateral;
         _expressionContainerMapperContext.Molecule = _transporter;
         _expressionContainerMapperContext.ExpressionParameters = new Cache<string, IParameter> { { _snapshot.Name, _transporterRelativeExpressionParameter } };

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
   }
}