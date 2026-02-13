using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Snapshots;
using OSPSuite.R.Domain;
using PKSim.Core;
using PKSim.R.Exchange;

namespace PKSim.R
{
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
}