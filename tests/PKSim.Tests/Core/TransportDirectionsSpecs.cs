using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using static OSPSuite.Core.Domain.Constants.Compartment;
using static PKSim.Core.CoreConstants.Compartment;
using static PKSim.Core.CoreConstants.Organ;

namespace PKSim.Core
{
   public abstract class concern_for_TransportDirections : StaticContextSpecification
   {
      protected Container _brain_cell;
      protected Container _liver_cell;
      protected Container _kidney;
      protected Container _mucosaDuodenumCell;
      protected Container _mucosa;
      protected Container _liver_int;
      protected Container _bone_cell;
      protected Container _kidney_cell;
      protected Container _kidney_int;
      protected Container _mucosaDuodenumInt;
      protected TransporterExpressionContainer _transporterExpressionContainer;
      protected Container _brain_pls;

      protected override void Context()
      {
         _mucosa = new Container().WithName(MUCOSA);
         _brain_cell = create(BRAIN, INTRACELLULAR);
         _brain_pls = create(BRAIN, PLASMA);
         _liver_cell = create(PERICENTRAL, INTRACELLULAR, LIVER);
         _liver_int = create(PERICENTRAL, INTERSTITIAL, LIVER);
         _bone_cell = create(BONE, INTRACELLULAR);
         _kidney_cell = create(KIDNEY, INTRACELLULAR);
         _kidney_int = create(KIDNEY, INTERSTITIAL);
         _mucosaDuodenumCell = create(DUODENUM, INTRACELLULAR, MUCOSA);
         _mucosaDuodenumInt = create(DUODENUM, INTERSTITIAL, MUCOSA);

         _transporterExpressionContainer = new TransporterExpressionContainer
            {TransportDirection = TransportDirectionId.InfluxInterstitialToIntracellular};
      }

      private Container create(string organName, string compartmentName, string groupingName = null)
      {
         var compartment = new Container().WithName(compartmentName)
            .WithParentContainer(new Container().WithName(organName));

         if (groupingName != null)
            compartment.ParentContainer.WithParentContainer(new Container().WithName(groupingName));

         return compartment;
      }
   }

   public class When_retrieving_the_default_transport_direction_for_a_given_transport_type : concern_for_TransportDirections
   {
      [Observation]
      public void should_return_the_expected_direction_for_tissue()
      {
         _transporterExpressionContainer.WithParentContainer(_bone_cell);
         TransportDirections.DefaultDirectionFor(TransportType.Influx, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.InfluxInterstitialToIntracellular);

         TransportDirections.DefaultDirectionFor(TransportType.BiDirectional, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.BiDirectionalInterstitialIntracellular);
         
         TransportDirections.DefaultDirectionFor(TransportType.Efflux, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.EffluxIntracellularToInterstitial);
      }

      [Observation]
      public void should_return_the_expected_direction_for_brain()
      {
         _transporterExpressionContainer.WithParentContainer(_brain_pls);
         TransportDirections.DefaultDirectionFor(TransportType.Influx, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.InfluxBrainPlasmaToInterstitial);

         TransportDirections.DefaultDirectionFor(TransportType.BiDirectional, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.BiDirectionalBrainPlasmaInterstitial);

         TransportDirections.DefaultDirectionFor(TransportType.Efflux, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.EffluxBrainInterstitialToPlasma);
         
         _transporterExpressionContainer.WithParentContainer(_brain_cell);
         TransportDirections.DefaultDirectionFor(TransportType.Influx, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.InfluxBrainInterstitialToTissue);

         TransportDirections.DefaultDirectionFor(TransportType.BiDirectional, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.BiDirectionalBrainInterstitialTissue);
         
         TransportDirections.DefaultDirectionFor(TransportType.Efflux, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.EffluxBrainTissueToInterstitial);
      }

      [Observation]
      public void should_return_the_expected_direction_for_organ_with_lumen()
      {
         _transporterExpressionContainer.WithParentContainer(_mucosaDuodenumInt);
         TransportDirections.DefaultDirectionFor(TransportType.Influx, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.InfluxInterstitialToIntracellular);

         TransportDirections.DefaultDirectionFor(TransportType.BiDirectional, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.BiDirectionalInterstitialIntracellular);
         
         TransportDirections.DefaultDirectionFor(TransportType.Efflux, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.EffluxIntracellularToInterstitial);

         _transporterExpressionContainer.WithParentContainer(_mucosaDuodenumCell);
         TransportDirections.DefaultDirectionFor(TransportType.Influx, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.InfluxLumenToMucosaIntracellular);

         TransportDirections.DefaultDirectionFor(TransportType.BiDirectional, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.BiDirectionalLumenMucosaIntracellular);

         TransportDirections.DefaultDirectionFor(TransportType.Efflux, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.EffluxMucosaIntracellularToLumen);
      }

      [Observation]
      public void should_return_the_expected_direction_for_liver_and_kidney()
      {
         _transporterExpressionContainer.WithParentContainer(_liver_cell);
         _transporterExpressionContainer.TransportDirection = TransportDirectionId.ExcretionLiver;
         TransportDirections.DefaultDirectionFor(TransportType.Influx, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.ExcretionLiver);

         TransportDirections.DefaultDirectionFor(TransportType.Efflux, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.ExcretionLiver);

         _transporterExpressionContainer.WithParentContainer(_kidney_cell);
         _transporterExpressionContainer.TransportDirection = TransportDirectionId.ExcretionKidney;
         TransportDirections.DefaultDirectionFor(TransportType.Influx, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.ExcretionKidney);
         
         TransportDirections.DefaultDirectionFor(TransportType.Efflux, _transporterExpressionContainer)
            .ShouldBeEqualTo(TransportDirectionId.ExcretionKidney);
      }
   }
}