using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public class concern_for_SimulationWithParticlesFormulation : concern_for_IndividualSimulation
   {
      protected Formulation _formulation;
      protected Organ _lumen;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound.Name = "C1";

         _protocol = DomainFactoryForSpecs.CreateStandardOralProtocol();
         _formulation = DomainFactoryForSpecs.CreateParticlesFormulation(numberOfBins:2);
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol, _formulation) as IndividualSimulation;

         //disable precipitation
         var moleculePropertiesContainer = _simulation.Model.Root.Container("C1");
         moleculePropertiesContainer.Parameter(CoreConstants.Parameter.PRECIPITATED_DRUG_SOLUBLE).Value = 1;

         //disable intestinal absorption and luminal flow to feces for easier mass balance checks
         moleculePropertiesContainer.Parameter(CoreConstants.Parameter.INTESTINAL_PERMEABILITY).Value = 0;
         //_lumen = _simulation.Model. .Root.Organ("xxx");

         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         simulationEngine.Run(_simulation);
      }

      [Observation]
      public void TEST_NAME()
      {
         
      }
   }
}	