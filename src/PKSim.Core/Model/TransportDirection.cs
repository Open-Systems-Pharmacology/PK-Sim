using OSPSuite.Core.Domain;
using PKSim.Core.Extensions;
using PKSim.Core.Model.Extensions;
using static PKSim.Core.Model.TransportDirectionId;

namespace PKSim.Core.Model
{
   public enum TransportDirectionId
   {
      None,
      InfluxPlasmaToInterstitial,
      EffluxInterstitialToPlasma,
      BiDirectionalPlasmaInterstitial,

      InfluxPlasmaToBloodCells,
      EffluxBloodCellsToPlasma,
      BiDirectionalBloodCellsPlasma,

      InfluxInterstitialToIntracellular,
      EffluxIntracellularToInterstitial,
      BiDirectionalInterstitialIntracellular,
      PgpIntracellularToInterstitial,

      InfluxLumenToMucosaIntracellular,
      EffluxMucosaIntracellularToLumen,
      BiDirectionalLumenMucosaIntracellular,
      PgpMucosaIntracellularToLumen,

      ExcretionKidney,
      ExcretionLiver,

      InfluxBrainInterstitialToTissue,
      EffluxBrainTissueToInterstitial,
      BiDirectionalBrainInterstitialTissue,
      PgpBrainTissueToInterstitial,

      InfluxBrainPlasmaToInterstitial,
      EffluxBrainInterstitialToPlasma,
      BiDirectionalBrainPlasmaInterstitial,
      PgpBrainInterstitialToPlasma
   }

   public static class TransportDirections
   {
      public static readonly TransportDirectionId[] PLASMA_DIRECTIONS =
      {
         InfluxPlasmaToInterstitial,
         EffluxInterstitialToPlasma,
         BiDirectionalPlasmaInterstitial
      };

      public static readonly TransportDirectionId[] BLOOD_CELLS_DIRECTIONS =
      {
         InfluxPlasmaToBloodCells,
         EffluxBloodCellsToPlasma,
         BiDirectionalBloodCellsPlasma
      };

      public static readonly TransportDirectionId[] MUCOSA_DIRECTIONS =
      {
         InfluxLumenToMucosaIntracellular,
         EffluxMucosaIntracellularToLumen,
         BiDirectionalLumenMucosaIntracellular,
         PgpMucosaIntracellularToLumen
      };

      public static readonly TransportDirectionId[] TISSUE_DIRECTIONS =
      {
         InfluxInterstitialToIntracellular,
         EffluxIntracellularToInterstitial,
         BiDirectionalInterstitialIntracellular,
         PgpIntracellularToInterstitial
      };

      public static readonly TransportDirectionId[] BRAIN_TISSUE_DIRECTIONS =
      {
         InfluxBrainInterstitialToTissue,
         EffluxBrainTissueToInterstitial,
         BiDirectionalBrainInterstitialTissue,
         PgpBrainTissueToInterstitial
      };

      public static readonly TransportDirectionId[] BRAIN_BBB_DIRECTIONS =
      {
         InfluxBrainPlasmaToInterstitial,
         EffluxBrainInterstitialToPlasma,
         BiDirectionalBrainPlasmaInterstitial,
         PgpBrainInterstitialToPlasma
      };

      public static TransportDirectionId DefaultVascularEndotheliumDirectionFor(TransportType transportType)
      {
         switch (transportType)
         {
            case TransportType.Influx:
               return InfluxPlasmaToInterstitial;
            case TransportType.BiDirectional:
               return BiDirectionalPlasmaInterstitial;
            default:
               return EffluxInterstitialToPlasma;
         }
      }

      public static TransportDirectionId DefaultBloodCellsDirectionFor(TransportType transportType)
      {
         switch (transportType)
         {
            case TransportType.Influx:
               return InfluxPlasmaToBloodCells;
            case TransportType.BiDirectional:
               return BiDirectionalBloodCellsPlasma;
            default:
               return EffluxBloodCellsToPlasma;
         }
      }

      public static TransportDirectionId DefaultMucosaDirectionFor(TransportType transportType)
      {
         switch (transportType)
         {
            case TransportType.Influx:
               return InfluxLumenToMucosaIntracellular;
            case TransportType.BiDirectional:
               return BiDirectionalLumenMucosaIntracellular;
            case TransportType.PgpLike:
               return PgpMucosaIntracellularToLumen;
            default:
               return EffluxMucosaIntracellularToLumen;
         }
      }

      public static TransportDirectionId DefaultTissueDirectionFor(TransportType transportType)
      {
         switch (transportType)
         {
            case TransportType.Influx:
               return InfluxInterstitialToIntracellular;
            case TransportType.BiDirectional:
               return BiDirectionalInterstitialIntracellular;
            case TransportType.PgpLike:
               return PgpIntracellularToInterstitial;
            default:
               return EffluxIntracellularToInterstitial;
         }
      }

      public static TransportDirectionId DefaultBrainTissueDirectionFor(TransportType transportType)
      {
         switch (transportType)
         {
            case TransportType.Influx:
               return InfluxBrainInterstitialToTissue;
            case TransportType.BiDirectional:
               return BiDirectionalBrainInterstitialTissue;
            case TransportType.PgpLike:
               return PgpBrainTissueToInterstitial;
            default:
               return EffluxBrainTissueToInterstitial;
         }
      }

      public static TransportDirectionId DefaultBloodBrainBarrierDirectionFor(TransportType transportType)
      {
         switch (transportType)
         {
            case TransportType.Influx:
               return InfluxBrainPlasmaToInterstitial;
            case TransportType.BiDirectional:
               return BiDirectionalBrainPlasmaInterstitial;
            case TransportType.PgpLike:
               return PgpBrainInterstitialToPlasma;
            default:
               return EffluxBrainInterstitialToPlasma;
         }
      }

      public static TransportDirectionId DefaultDirectionFor(TransportType transportType, TransporterExpressionContainer expressionContainer)
      {
         if (expressionContainer.TransportDirection == None)
            return None;

         if (expressionContainer.Name.IsBloodCells())
            return DefaultBloodCellsDirectionFor(transportType);

         if (expressionContainer.Name.IsVascularEndothelium())
            return DefaultVascularEndotheliumDirectionFor(transportType);

         var organ = expressionContainer.LogicalContainer;
         if (organ.IsBrain())
            return expressionContainer.CompartmentName.IsPlasma()
               ? DefaultBloodBrainBarrierDirectionFor(transportType)
               : DefaultBrainTissueDirectionFor(transportType);

         if (organ.IsOrganWithLumen())
         {
            if (expressionContainer.CompartmentName.IsInterstitial())
               return DefaultTissueDirectionFor(transportType);

            //Organ with lumen not in mucosa=>Only one option (excretion
            return organ.IsInMucosa() ? DefaultMucosaDirectionFor(transportType) : expressionContainer.TransportDirection;
         }

         return DefaultTissueDirectionFor(transportType);
      }
   }

   public class TransportDirection
   {
      public TransportDirectionId Id { get; set; }
      public string DisplayName { get; set; }
      public string Description { get; set; }
      public string Icon { get; set; }
   }
}