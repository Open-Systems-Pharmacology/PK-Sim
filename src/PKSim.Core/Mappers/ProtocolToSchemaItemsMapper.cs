using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Mappers
{
   public interface IProtocolToSchemaItemsMapper : IMapper<Protocol, IReadOnlyList<SchemaItem>>
   {
   }

   public class ProtocolToSchemaItemsMapper : IProtocolToSchemaItemsMapper,
                                              IVisitor<AdvancedProtocol>,
                                              IVisitor<SimpleProtocol>
   {
      private readonly ISimpleProtocolToSchemaMapper _simpleProtocolToSchemaMapper;
      private readonly ICloner _cloneManager;
      private IEnumerable<SchemaItem> _allSchemaItems;

      public ProtocolToSchemaItemsMapper(ISimpleProtocolToSchemaMapper simpleProtocolToSchemaMapper, ICloner cloneManager)
      {
         _simpleProtocolToSchemaMapper = simpleProtocolToSchemaMapper;
         _cloneManager = cloneManager;
      }

      public IReadOnlyList<SchemaItem> MapFrom(Protocol protocol)
      {
         this.Visit(protocol);
         var resultingList = _allSchemaItems.OrderBy(si => si.StartTime.Value).ToList();
         resultingList.Each(schemaItem => { schemaItem.StartTime.DisplayUnit = protocol.TimeUnit; });
         _allSchemaItems = null;
         return resultingList;
      }

      public void Visit(AdvancedProtocol advancedProtocol)
      {
         fillSchemaItemListFrom(advancedProtocol.AllSchemas);
      }

      private void fillSchemaItemListFrom(IEnumerable<Schema> allSchemas)
      {
         _allSchemaItems = from schema in allSchemas
                           from schemaItem in schema.ExpandedSchemaItems(_cloneManager)
                           select schemaItem;
      }

      public void Visit(SimpleProtocol simpleProtocol)
      {
         //we need to create a protocol according to the predefined schema
         fillSchemaItemListFrom(_simpleProtocolToSchemaMapper.MapFrom(simpleProtocol));
      }
   }
}