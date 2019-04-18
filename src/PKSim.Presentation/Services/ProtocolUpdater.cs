using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.Services
{
   public class ProtocolUpdater : IProtocolUpdater
   {
      private readonly ISimpleProtocolToSchemaMapper _schemaMapper;

      public ProtocolUpdater(ISimpleProtocolToSchemaMapper schemaMapper)
      {
         _schemaMapper = schemaMapper;
      }

      public void UpdateProtocol(Protocol sourceProtocol, Protocol targetProtocol)
      {
         //update protocol id so that they stay the same (copy object base properties)
         updateProtocolProperties(sourceProtocol, targetProtocol);

         //Structure change should only be propagated if the target is an advanced protocol 
         var simpleProtocol = sourceProtocol as SimpleProtocol;
         var advancedProtocol = targetProtocol as AdvancedProtocol;

         if (simpleProtocol == null) return;
         if (advancedProtocol == null) return;


         var schemas = _schemaMapper.MapFrom(simpleProtocol).ToList();
         if (!schemas.Any()) return;

         advancedProtocol.RemoveAllSchemas();

         schemas.Each(advancedProtocol.AddSchema);
      }

      private void updateProtocolProperties(Protocol sourceProtocol, Protocol targetProtocol)
      {
         if (targetProtocol == null || sourceProtocol == null) return;
         targetProtocol.Id = sourceProtocol.Id;
         targetProtocol.Name = sourceProtocol.Name;
         targetProtocol.Description = sourceProtocol.Description;
      }
   }
}