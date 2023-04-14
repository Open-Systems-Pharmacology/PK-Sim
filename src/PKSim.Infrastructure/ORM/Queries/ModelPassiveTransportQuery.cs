using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ORM.Queries
{
   public class ModelPassiveTransportQuery : IModelPassiveTransportQuery
   {
      private readonly ICloneManagerForBuildingBlock _cloneManager;
      private readonly IModelPassiveTransportMoleculeNameRepository _modelPassiveTransportMoleculeNameRepo;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IPassiveTransportRepository _passiveTransportRepo;

      public ModelPassiveTransportQuery(IObjectBaseFactory objectBaseFactory,
         IPassiveTransportRepository passiveTransportRepo,
         ICloneManagerForBuildingBlock cloneManager,
         IModelPassiveTransportMoleculeNameRepository modelPassiveTransportMoleculeNameRepo)
      {
         _objectBaseFactory = objectBaseFactory;
         _passiveTransportRepo = passiveTransportRepo;
         _cloneManager = cloneManager;
         _modelPassiveTransportMoleculeNameRepo = modelPassiveTransportMoleculeNameRepo;
      }

      public PassiveTransportBuildingBlock AllPassiveTransportsFor(Simulation simulation)
      {
         var passiveTransportBuilderCollection = _objectBaseFactory.Create<PassiveTransportBuildingBlock>()
            .WithName(simulation.Name);

         var modelProperties = simulation.ModelProperties;
         var compoundNames = simulation.AllBuildingBlocks<Compound>().AllNames().ToList();
         foreach (var transportTemplate in _passiveTransportRepo.AllFor(modelProperties))
         {
            var transport = _cloneManager.Clone(transportTemplate, passiveTransportBuilderCollection.FormulaCache);

            setupTransportMoleculeNames(transport, simulation.ModelProperties, compoundNames);

            passiveTransportBuilderCollection.Add(transport);
         }

         return passiveTransportBuilderCollection;
      }

      private void setupTransportMoleculeNames(PKSimTransport transport, ModelProperties modelProperties, IReadOnlyList<string> compoundNames)
      {
         string modelName = modelProperties.ModelConfiguration.ModelName;
         string transportName = transport.Name;

         var moleculeNames = _modelPassiveTransportMoleculeNameRepo.MoleculeNamesFor(modelName, transportName, compoundNames);

         if (moleculeNames == null)
            return; //no special molecule settings for this transport

         transport.MoleculeList.Update(moleculeNames);
      }
   }
}