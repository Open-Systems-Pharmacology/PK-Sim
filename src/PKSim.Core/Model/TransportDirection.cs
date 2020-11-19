using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Utility.Collections;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public enum TransportDirectionId
   {
      None,
      Influx,
      Efflux,
      PgpLike,
      CellsBiDirectional,
      Excretion,
      PlasmaToInterstitial,
      InterstitialToPlasma,
      VascEndoBiDirectional
   }


   public static class TransportDirections
   {
      private static readonly Cache<TransportDirectionId, TransportDirection> _allTransportDirections = new Cache<TransportDirectionId, TransportDirection>(x=>x.TransportDirectionId);
      public static TransportDirection None = create(TransportDirectionId.None, PKSimConstants.TransportDirection.None, PKSimConstants.TransportDirection.None,ApplicationIcons.EmptyIcon);
      public static TransportDirection Influx = create(TransportDirectionId.Influx, PKSimConstants.TransportDirection.Influx, PKSimConstants.TransportDirection.InfluxDescription, ApplicationIcons.Influx);
      public static TransportDirection Efflux = create(TransportDirectionId.Efflux, PKSimConstants.TransportDirection.Efflux, PKSimConstants.TransportDirection.EffluxDescription, ApplicationIcons.Efflux);
      public static TransportDirection CellsBiDirectional = create(TransportDirectionId.CellsBiDirectional, PKSimConstants.TransportDirection.CellsBiDirectional, PKSimConstants.TransportDirection.CellsBiDirectionalDescription, ApplicationIcons.Refresh);
      public static TransportDirection PgpLike = create(TransportDirectionId.PgpLike, PKSimConstants.TransportDirection.PgpLike, PKSimConstants.TransportDirection.PgpLikeDescription, ApplicationIcons.Pgp);
      public static TransportDirection Excretion = create(TransportDirectionId.Excretion, PKSimConstants.TransportDirection.Excretion, PKSimConstants.TransportDirection.Excretion, ApplicationIcons.Excretion);
      public static TransportDirection PlasmaToInterstitial = create(TransportDirectionId.PlasmaToInterstitial, PKSimConstants.TransportDirection.PlasmaToInterstitial, PKSimConstants.TransportDirection.PlasmaToInterstitialDescription, ApplicationIcons.Plasma);
      public static TransportDirection InterstitialToPlasma = create(TransportDirectionId.InterstitialToPlasma, PKSimConstants.TransportDirection.InterstitialToPlasma, PKSimConstants.TransportDirection.InterstitialToPlasmaDescription, ApplicationIcons.Interstitial);
      public static TransportDirection VascEndoBiDirectional = create(TransportDirectionId.VascEndoBiDirectional, PKSimConstants.TransportDirection.VascEndoBiDirectional, PKSimConstants.TransportDirection.VascEndoBiDirectionalDescription, ApplicationIcons.Refresh);


      public static TransportDirection ById(TransportDirectionId transportDirectionId)
      {
         return _allTransportDirections[transportDirectionId];
      }

      private static TransportDirection create(TransportDirectionId transportDirectionId, string displayName, string description, ApplicationIcon applicationIcon)
      {
         var transportDirection = new TransportDirection(transportDirectionId, displayName, description,  applicationIcon);
         _allTransportDirections.Add(transportDirection);
         return transportDirection;
      }

      public static IReadOnlyCollection<TransportDirection> All() => _allTransportDirections;
   }
   public class TransportDirection
   {
      public TransportDirectionId TransportDirectionId { get; }
      public string DisplayName { get; }
      public string Description { get; }
      public ApplicationIcon Icon { get; }

      public TransportDirection(TransportDirectionId transportDirectionId, string displayName, string description, ApplicationIcon icon)
      {
         TransportDirectionId = transportDirectionId;
         DisplayName = displayName;
         Description = description;
         Icon = icon;
      }

      public override string ToString() => DisplayName;
   }
}