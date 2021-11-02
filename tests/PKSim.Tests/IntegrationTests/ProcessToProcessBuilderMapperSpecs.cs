using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ProcessToProcessBuilderMapper : ContextWithLoadedProject<IProcessToProcessBuilderMapper>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Mucosa_MultipleTransportDirections");
      }
   }

   public class When_creating_a_simulation_using_transport_in_multiple_mucosa_directions : concern_for_ProcessToProcessBuilderMapper
   {
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         var ind = FindByName<Individual>("I1");
         var comp = FindByName<Compound>("C1");
         var prot = FindByName<Protocol>("IV");
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(ind, comp, prot);
      }

      [Observation]
      public void should_not_create_too_many_transporters()
      {
         var transportInLumenToMucosa = _simulation.Model.Neighborhoods.EntityAt<IContainer>("Lumen_lil_LowerIleum_cell", "C1", "T1-a");
         transportInLumenToMucosa.GetChildren<IContainer>(x => x.ContainerType == ContainerType.Transport).Count().ShouldBeEqualTo(1);
      }
   }
}