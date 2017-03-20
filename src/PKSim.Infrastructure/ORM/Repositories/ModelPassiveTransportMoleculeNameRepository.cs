using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class ModelPassiveTransportMoleculeNameRepository : StartableRepository<MoleculeList>, IModelPassiveTransportMoleculeNameRepository
   {
      private readonly IFlatModelPassiveTransportMoleculeNameRepository _flatModelPassiveTransportMoleculeNameRepo;
      private readonly ICache<CompositeKey, MoleculeList> _transportMoleculeNames;

      public ModelPassiveTransportMoleculeNameRepository(IFlatModelPassiveTransportMoleculeNameRepository flatModelPassiveTransportMoleculeNameRepo)
      {
         _flatModelPassiveTransportMoleculeNameRepo = flatModelPassiveTransportMoleculeNameRepo;
         _transportMoleculeNames = new Cache<CompositeKey, MoleculeList> {OnMissingKey = x => null};
      }

      protected override void DoStart()
      {
         foreach (var flatTransportMoleculeName in _flatModelPassiveTransportMoleculeNameRepo.All())
         {
            var key = keyFrom(flatTransportMoleculeName.Model, flatTransportMoleculeName.Transport);

            if (!_transportMoleculeNames.Contains(key))
               _transportMoleculeNames.Add(key, new MoleculeList());

            var moleculeNames = _transportMoleculeNames[key];

            if (flatTransportMoleculeName.ShouldTransport)
               moleculeNames.AddMoleculeName(flatTransportMoleculeName.Molecule);
            else
               moleculeNames.AddMoleculeNameToExclude(flatTransportMoleculeName.Molecule);

            if (moleculeNames.MoleculeNames.Any())
               moleculeNames.ForAll = false;
         }
      }

      public override IEnumerable<MoleculeList> All()
      {
         Start();
         return _transportMoleculeNames;
      }

      public MoleculeList MoleculeNamesFor(string model, string transportName, IReadOnlyList<string> compoundNames)
      {
         Start();

         var key = keyFrom(model, transportName);

         if (!_transportMoleculeNames.Contains(key))
            return null; //no molecule names defined for this transport

         var moleculeNames = _transportMoleculeNames[key].Clone();

         var fcComplexNames = compoundNames.Select(CoreConstants.Molecule.DrugFcRnComplexName).ToList();
         moleculeNames.ReplaceMoleculeName(CoreConstants.Molecule.DrugFcRnComplexTemplate, fcComplexNames);

         return moleculeNames;
      }

      private CompositeKey keyFrom(string model, string transportName)
      {
         return new CompositeKey(model, transportName);
      }
   }
}