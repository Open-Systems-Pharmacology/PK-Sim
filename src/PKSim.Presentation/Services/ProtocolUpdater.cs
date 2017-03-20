using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Services;

namespace PKSim.Presentation.Services
{
   public class ProtocolUpdater : IProtocolUpdater
   {
      private readonly ISimpleProtocolToSchemaMapper _schemaMapper;
      private readonly IDialogCreator _dialogCreator;

      public ProtocolUpdater(ISimpleProtocolToSchemaMapper schemaMapper, IDialogCreator dialogCreator)
      {
         _schemaMapper = schemaMapper;
         _dialogCreator = dialogCreator;
      }

      public void UpdateProtocol(Protocol sourceProtocol, Protocol targetProtocol)
      {
         //update protocol id so that they stay the same (copy object base properties)
         updateProtocolProperties(sourceProtocol, targetProtocol);

         //Structure change should only be propagated if the target is an advacned protocol 
         var simpleProtocol = sourceProtocol as SimpleProtocol;
         var advancedProtocol = targetProtocol as AdvancedProtocol;

         if (simpleProtocol == null) return;
         if (advancedProtocol == null) return;


         var schemas = _schemaMapper.MapFrom(simpleProtocol).ToList();
         if (!schemas.Any()) return;

         advancedProtocol.RemoveAllSchemas();

         schemas.Each(advancedProtocol.AddSchema);
      }

      public bool ValidateSwitchFrom(Protocol sourceProtocol)
      {
         var simpleProtocol = sourceProtocol as SimpleProtocol;
         if (simpleProtocol == null)
            return true;

         if (!simpleProtocol.ApplicationType.UserDefined)
            return true;

         _dialogCreator.MessageBoxError(PKSimConstants.Error.CannotSwitchToAdvancedProtocolWhenUsingUserDefinedAppplication);
         return false;
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