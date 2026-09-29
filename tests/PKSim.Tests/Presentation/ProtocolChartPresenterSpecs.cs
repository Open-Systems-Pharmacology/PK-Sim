using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using OSPSuite.Core;
using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Protocols;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Views.Protocols;

namespace PKSim.Presentation
{
   public abstract class concern_for_ProtocolChartPresenter : ContextSpecification<IProtocolChartPresenter>
   {
      protected IProtocolChartView _view;
      protected IProtocolToSchemaItemsMapper _schemaItemsMapper;
      protected ISchemaItemToSchemaItemDTOMapper _schemaItemDTOMapper;
      protected IDimensionRepository _dimensionRepository;
      protected Compound _compound;
      protected Protocol _protocol;
      protected SchemaItem _administrationSchemaItem;
      protected SchemaItem _eventSchemaItem;
      protected ICache<Compound, Protocol> _protocols;

      protected override void Context()
      {
         _view = A.Fake<IProtocolChartView>();
         _schemaItemsMapper = A.Fake<IProtocolToSchemaItemsMapper>();
         _schemaItemDTOMapper = A.Fake<ISchemaItemToSchemaItemDTOMapper>();
         _dimensionRepository = A.Fake<IDimensionRepository>();

         A.CallTo(() => _dimensionRepository.Time).Returns(A.Fake<IDimension>());

         _compound = new Compound().WithName("DRUG");
         _protocol = A.Fake<Protocol>();
         A.CallTo(() => _protocol.EndTime).Returns(60);
         A.CallTo(() => _protocol.TimeUnit).Returns(new Unit("min", 1, 0));

         _administrationSchemaItem = new SchemaItem {ApplicationType = ApplicationTypes.Intravenous};
         _eventSchemaItem = new SchemaItem {ApplicationType = ApplicationTypes.Event, EventKey = "EVENT_1"};
         A.CallTo(() => _schemaItemsMapper.MapFrom(_protocol)).Returns(new[] {_administrationSchemaItem, _eventSchemaItem});

         A.CallTo(() => _schemaItemDTOMapper.MapFrom(A<SchemaItem>._))
            .ReturnsLazily(x => DomainHelperForSpecs.SchemaItemDTO(x.GetArgument<SchemaItem>(0).ApplicationType));

         _protocols = new Cache<Compound, Protocol> {{_compound, _protocol}};

         sut = new ProtocolChartPresenter(_view, _schemaItemsMapper, _schemaItemDTOMapper, _dimensionRepository,
            A.Fake<OSPSuite.Core.IApplicationSettings>(), A.Fake<IDialogCreator>());
      }
   }

   public class When_plotting_a_protocol_whose_event_placeholder_is_left_unmapped : concern_for_ProtocolChartPresenter
   {
      protected override void Because()
      {
         var unmappedEventKeys = new Cache<Compound, IReadOnlyList<string>> {{_compound, new[] {"EVENT_1"}}};
         sut.PlotProtocols(_protocols, unmappedEventKeys);
      }

      [Observation]
      public void should_not_plot_the_event_that_applies_no_event()
      {
         A.CallTo(() => _schemaItemDTOMapper.MapFrom(_eventSchemaItem)).MustNotHaveHappened();
      }

      [Observation]
      public void should_still_plot_the_administration()
      {
         A.CallTo(() => _schemaItemDTOMapper.MapFrom(_administrationSchemaItem)).MustHaveHappened();
      }
   }

   public class When_plotting_a_protocol_whose_event_placeholder_is_mapped : concern_for_ProtocolChartPresenter
   {
      protected override void Because()
      {
         sut.PlotProtocols(_protocols);
      }

      [Observation]
      public void should_plot_the_event()
      {
         A.CallTo(() => _schemaItemDTOMapper.MapFrom(_eventSchemaItem)).MustHaveHappened();
      }
   }
}
