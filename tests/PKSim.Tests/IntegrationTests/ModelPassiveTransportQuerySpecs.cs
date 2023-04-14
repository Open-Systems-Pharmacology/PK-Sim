using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ModelPassiveTransportQuery : ContextForSimulationIntegration<IModelPassiveTransportQuery>
   {
   }

   public class When_retrieving_the_passive_transport_processes_defined_for_a_given_model_configuration : concern_for_ModelPassiveTransportQuery
   {
      private PassiveTransportBuildingBlock _passiveTransports;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulation = DomainFactoryForSpecs.CreateDefaultSimulation();
      }

      protected override void Because()
      {
         _passiveTransports = sut.AllPassiveTransportsFor(_simulation);
      }

      [Observation]
      public void should_return_the_predefined_processes_for_the_model_and_the_calculation_methods()
      {
         _passiveTransports.Count().ShouldBeGreaterThan(0);
      }
   }

   public class When_retrieving_the_passive_transport_processes_for_4Comp_model : concern_for_ModelPassiveTransportQuery
   {
      private PassiveTransportBuildingBlock _passiveTransports;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulation = DomainFactoryForSpecs.CreateDefaultSimulationForModel(CoreConstants.Model.FOUR_COMP);
      }

      protected override void Because()
      {
         _passiveTransports = sut.AllPassiveTransportsFor(_simulation);
      }

      [Observation]
      public void moleculenames_lists_for_all_transports_should_be_empty()
      {
         foreach (var passiveTransport in _passiveTransports)
         {
            passiveTransport.MoleculeNames().Count().ShouldBeEqualTo(0);
            passiveTransport.MoleculeNamesToExclude().Count().ShouldBeEqualTo(0);
            passiveTransport.ForAll.ShouldBeTrue();
         }
      }
   }

   public class When_retrieving_the_passive_transport_processes_for_protein_model : concern_for_ModelPassiveTransportQuery
   {
      private PassiveTransportBuildingBlock _passiveTransports;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulation = DomainFactoryForSpecs.CreateDefaultSimulationForModel(CoreConstants.Model.TWO_PORES);
      }

      protected override void Because()
      {
         _passiveTransports = sut.AllPassiveTransportsFor(_simulation);
      }

      [Observation]
      public void should_set_molecule_names_for_twopores_transport_link()
      {
         var twoPoresTransportLink = _passiveTransports.First(t => t.Name.Equals("TwoPoresTransportLink"));
         twoPoresTransportLink.MoleculeNamesToExclude().ShouldContain(CoreConstants.Molecule.FcRn,
                                                                      CoreConstants.Molecule.LigandEndoComplex);
      }

      [Observation]
      public void should_set_molecule_names_for_NetMassTransfer_InterstitialToEndosomal_FcRn()
      {
         var transport = _passiveTransports.First(t => t.Name.Equals("NetMassTransfer_InterstitialToEndosomal_FcRn"));
         transport.MoleculeNames().ShouldContain(CoreConstants.Molecule.FcRn);
         transport.ForAll.ShouldBeFalse();
      }
   }
}