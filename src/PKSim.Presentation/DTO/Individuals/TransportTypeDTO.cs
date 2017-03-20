using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.DTO.Individuals
{
   public static class TransportTypes
   {
      private static readonly ICache<TransportType,TransportTypeDTO> _allTransporterTypes = new Cache<TransportType, TransportTypeDTO>(x=>x.TransportType);

      public static TransportTypeDTO Influx = addTransporterType(TransportType.Influx, PKSimConstants.UI.Influx, ApplicationIcons.Influx);
      public static TransportTypeDTO Efflux = addTransporterType(TransportType.Efflux, PKSimConstants.UI.Efflux, ApplicationIcons.Efflux);
      public static TransportTypeDTO PgpLike = addTransporterType(TransportType.PgpLike, PKSimConstants.UI.PgpLike,ApplicationIcons.Pgp);

      private static TransportTypeDTO addTransporterType(TransportType transporterType, string displayName, ApplicationIcon icon)
      {
         var transporterTypeDTO = new TransportTypeDTO(transporterType, displayName, icon);
         _allTransporterTypes.Add(transporterTypeDTO);
         return transporterTypeDTO;
      }

      public static TransportTypeDTO By(TransportType transporterType)
      {
         return _allTransporterTypes[transporterType];
      }

      public static IEnumerable<TransportTypeDTO> All()
      {
         return _allTransporterTypes;
      }
   }

   public class TransportTypeDTO
   {
      public TransportType TransportType { get; private set; }
      public string DisplayName { get; private  set; }
      public ApplicationIcon Icon { get; private set; }

      public TransportTypeDTO(TransportType transporterType, string displayName, ApplicationIcon icon)
      {
         TransportType = transporterType;
         DisplayName = displayName;
         Icon = icon;
      }

      public override string ToString()
      {
         return DisplayName;
      }
   }
}