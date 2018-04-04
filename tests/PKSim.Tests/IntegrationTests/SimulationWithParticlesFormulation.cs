using PKSim.Core.Model;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public class concern_for_SimulationWithParticlesFormulationSpecs : concern_for_IndividualSimulation
   {
      protected Formulation _formulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _protocol = DomainFactoryForSpecs.CreateStandardOralProtocol();
         _formulation = DomainFactoryForSpecs.CreateParticlesFormulation(numberOfBins:1);

         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol, _formulation) as IndividualSimulation;
      }
   }
}	