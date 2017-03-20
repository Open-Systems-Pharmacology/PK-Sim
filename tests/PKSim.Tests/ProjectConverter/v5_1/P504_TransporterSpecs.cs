using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v5_1;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_1
{
   public abstract class concern_for_P504_Transporter : ContextWithLoadedProject<Converter50To513>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("P504_Transporter");
      }
   }

   public class When_converting_the_project_P504_Transporter : concern_for_P504_Transporter
   {
      private Simulation _simulation;
      private Individual _individual;

      protected override void Context()
      {
         _simulation = First<Simulation>();
         _individual = First<Individual>();
      }

      [Observation]
      public void should_have_converted_the_transporter_type_for_the_transporter_and_set_it_to_the_type_the_most_often_used()
      {
         assertTransporterHaveTheRightTransportType(_individual);
      }

      [Observation]
      public void should_have_converted_the_transporter_type_for_the_transporter_in_the_simulation_as_well()
      {
         assertTransporterHaveTheRightTransportType(_simulation.BuildingBlock<Individual>());
      }

      private void assertTransporterHaveTheRightTransportType(Individual individual)
      {
         var influx = individual.MoleculeByName<IndividualTransporter>("T1-Eff");
         influx.TransportType.ShouldBeEqualTo(TransportType.Efflux);

         var efflux = individual.MoleculeByName<IndividualTransporter>("T2-Inf");
         efflux.TransportType.ShouldBeEqualTo(TransportType.Influx);

         var pgp = individual.MoleculeByName<IndividualTransporter>("T3-pgp");
         pgp.TransportType.ShouldBeEqualTo(TransportType.PgpLike);
      }
   }
}