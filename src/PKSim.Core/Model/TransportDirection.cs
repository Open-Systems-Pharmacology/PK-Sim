namespace PKSim.Core.Model
{
   public enum TransportDirectionId
   {
      None,
      InfluxPlasmaToInterstitial,
      EffluxInterstitialToPlasma,
      InfluxPlasmaToBloodCells,
      EffluxBloodCellsToPlasma,
      InfluxInterstitialToIntracellular,
      EffluxIntracellularToInterstitial,
      InfluxLumenToMucosaIntracellular,
      EffluxMucosaIntracellularToLumen,
      ExcretionKidney,
      ExcretionLiver,
      PgpIntracellularToInterstitial,
      PgpMucosaIntracellularToLumen,
      EffluxBrainTissueToInterstitial,
      InfluxBrainInterstitialToTissue,
      PgpBrainTissueToInterstitial,
      InfluxBrainPlasmaToInterstitial,
      EffluxBrainInterstitialToPlasma,
      PgpBrainInterstitialToPlasma
   }

   public class TransportDirection
   {
      public TransportDirectionId Id { get; set; }
      public string DisplayName { get; set; }
      public string Description { get; set; }
      public string Icon { get; set; }
      public bool Global { get; set; }
   }
}