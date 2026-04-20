using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Snapshots;
using OSPSuite.R.Domain;
using PKSim.Assets;
using PKSim.Core;
using PKSim.R.Exchange;
using static PKSim.Assets.PKSimConstants.UI;

namespace PKSim.R;

internal class CreateExchangeIndividual : ContextForStaticIntegration
{
   private IndividualCharacteristics _individualCharacteristics;
   private string _result;

   protected override void Context()
   {
      base.Context();
      _individualCharacteristics = new IndividualCharacteristics
      {
         Species = CoreConstants.Species.HUMAN,
         Population = CoreConstants.Population.ICRP,
         Age = new Parameter
         {
            Value = 30,
            Unit = "year(s)",
         },
         Weight = new Parameter
         {
            Value = 75,
            Unit = "kg",
         },
         Height = new Parameter
         {
            Value = 175,
            Unit = "cm",
         },
         Gender = CoreConstants.Gender.MALE
      };
   }

   protected override void Because()
   {
      _result = BuildingBlockCreator.CreateIndividual(_individualCharacteristics);
   }

   [Observation]
   public void an_exchangeable_string_is_created()
   {
      _result.ShouldNotBeNull();
   }
}

internal class CreateExchangeMetabolizingEnzyme : ContextForStaticIntegration
{
   private string _result;

   protected override void Because()
   {
      _result = BuildingBlockCreator.CreateExpressionProfile(MetabolizingEnzyme, "CYP3A4", CoreConstants.Species.HUMAN, "Healthy");
   }

   [Observation]
   public void an_exchangeable_string_is_created()
   {
      _result.ShouldNotBeNull();
   }
}

internal class CreateExchangeExpressionProfileWithInvalidSpecies : ContextForStaticIntegration
{
   [Observation]
   public void should_throw_an_argument_exception_with_species_not_found_message()
   {
      var message = string.Empty;
      The.Action(() =>
      {
         try
         {
            BuildingBlockCreator.CreateExpressionProfile(MetabolizingEnzyme, "CYP3A4", "InvalidSpecies", "Healthy");
         }
         catch (Exception e)
         {
            message = e.Message;
            throw;
         }
      }).ShouldThrowAn<ArgumentException>();
      message.ShouldBeEqualTo(PKSimConstants.Error.CouldNotFindValidSpecies("InvalidSpecies"));
   }
}

internal class CreateExchangeTransportProtein : ContextForStaticIntegration
{
   private string _result;

   protected override void Because()
   {
      _result = BuildingBlockCreator.CreateExpressionProfile(TransportProtein, "CYP3A4", CoreConstants.Species.HUMAN, "Healthy");
   }

   [Observation]
   public void an_exchangeable_string_is_created()
   {
      _result.ShouldNotBeNull();
   }

   internal class CreateExchangeProteinBindingPartner : ContextForStaticIntegration
   {
      private string _result;

      protected override void Because()
      {
         _result = BuildingBlockCreator.CreateExpressionProfile(ProteinBindingPartner, "CYP3A4", CoreConstants.Species.HUMAN, "Healthy");
      }

      [Observation]
      public void an_exchangeable_string_is_created()
      {
         _result.ShouldNotBeNull();
      }
   }
}