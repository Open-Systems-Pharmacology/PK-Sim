using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core
{
   public abstract class concern_for_ProtocolFactory : ContextSpecification<IProtocolFactory>
   {
      protected ISchemaFactory _schemaFactory;
      protected IObjectBaseFactory _objectBaseFactory;
      protected IParameterFactory _parameterFactory;
      protected IDimensionRepository _dimensionRepository;
      protected IDimension _timeDimension;
      protected IDimension _doseDimension;
      protected ISchemaItemParameterRetriever _schemaItemParameterRetriever;
      protected IDisplayUnitRetriever _displayUnitRetriever;
      protected Unit _defaultTimeUnit;

      protected override void Context()
      {
         _schemaFactory = A.Fake<ISchemaFactory>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _parameterFactory = A.Fake<IParameterFactory>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _schemaItemParameterRetriever = A.Fake<ISchemaItemParameterRetriever>();
         _displayUnitRetriever = A.Fake<IDisplayUnitRetriever>();
         sut = new ProtocolFactory(_objectBaseFactory, _schemaFactory, _parameterFactory, _dimensionRepository, _schemaItemParameterRetriever, _displayUnitRetriever);

         _timeDimension = A.Fake<IDimension>();
         _doseDimension = A.Fake<IDimension>();
         _defaultTimeUnit = A.Fake<Unit>();
         A.CallTo(() => _dimensionRepository.Mass).Returns(_doseDimension);
         A.CallTo(() => _dimensionRepository.Time).Returns(_timeDimension);
         A.CallTo(() => _displayUnitRetriever.PreferredUnitFor(_timeDimension)).Returns(_defaultTimeUnit);
      }
   }

   public class When_the_protocol_factory_is_asked_to_create_an_advanced_protocol : concern_for_ProtocolFactory
   {
      private AdvancedProtocol _protocol;
      private Protocol _result;

      protected override void Context()
      {
         base.Context();
         _protocol = new AdvancedProtocol();
         A.CallTo(() => _schemaFactory.CreateWithDefaultItem(ApplicationTypes.IntravenousBolus, _protocol)).Returns(A.Fake<Schema>());
         A.CallTo(() => _objectBaseFactory.Create<AdvancedProtocol>()).Returns(_protocol);
         A.CallTo(() => _objectBaseFactory.Create<IRootContainer>()).Returns(new RootContainer());
      }

      protected override void Because()
      {
         _result = sut.Create(ProtocolMode.Advanced);
      }

      [Observation]
      public void should_return_a_protocol_with_one_schema()
      {
         _result.ShouldBeEqualTo(_protocol);
         _protocol.AllSchemas.Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_set_the_default_unit_for_dose_and_time()
      {
         _result.TimeUnit.ShouldBeEqualTo(_defaultTimeUnit);
      }

      [Observation]
      public void the_created_protocol_shouuld_be_marked_as_loaded()
      {
         _result.IsLoaded.ShouldBeTrue();
      }
   }
}