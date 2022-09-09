using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using static PKSim.Core.CoreConstants.Compartment;
using static PKSim.Core.CoreConstants.Organ;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_TransportContainerUpdater : ContextForIntegration<ITransportContainerUpdater>
   {
      protected IExpressionProfileFactory _expressionProfileFactory;
      protected ISpeciesRepository _speciesRepository;
      protected Species _humanSpecies;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _expressionProfileFactory = IoC.Resolve<IExpressionProfileFactory>();
         _speciesRepository = IoC.Resolve<ISpeciesRepository>();
         _humanSpecies = _speciesRepository.FindByName(CoreConstants.Species.HUMAN);
      }
   }

   public class When_creating_a_transporter_for_SLCO1B1 : concern_for_TransportContainerUpdater
   {
      private ExpressionProfile _expressionProfile;
      private Individual _individual;
      private IndividualMolecule _molecule;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _expressionProfile = _expressionProfileFactory.Create<IndividualTransporter>(_humanSpecies, "SLCO1B1");
         (_molecule, _individual) = _expressionProfile;
      }

      protected override void Because()
      {
         sut.SetDefaultSettingsForTransporter(_individual, _molecule.DowncastTo<IndividualTransporter>(), _molecule.Name);
      }

      [Observation]
      public void should_have_created_an_influx_transporter_with_expression_basolateral_in_liver()
      {
         var fractionExpressedBasolateralLiver = _individual.Organism.EntityAt<IParameter>(LIVER, PERICENTRAL, INTERSTITIAL, "SLCO1B1", FRACTION_EXPRESSED_BASOLATERAL);
         fractionExpressedBasolateralLiver.Value.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_created_an_influx_transporter_with_expression_apical_in_kidney()
      {
         var fractionExpressedBasolateralKidney = _individual.Organism.EntityAt<IParameter>(KIDNEY, INTERSTITIAL, "SLCO1B1", FRACTION_EXPRESSED_BASOLATERAL);
         fractionExpressedBasolateralKidney.Value.ShouldBeEqualTo(0);
      }

   }
}