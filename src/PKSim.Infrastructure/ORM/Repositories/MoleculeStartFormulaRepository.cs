using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class MoleculeStartFormulaRepository : StartableRepository<IMoleculeStartFormula>, IMoleculeStartFormulaRepository
   {
      private readonly IFlatMoleculeStartFormulaRepository _flatMoleculeStartFormulasRepository;
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IList<IMoleculeStartFormula> _moleculeStartFormulas;

      public MoleculeStartFormulaRepository(IFlatMoleculeStartFormulaRepository flatMoleculeStartFormulasRepository,
                                            IFlatContainerRepository flatContainerRepository)
      {
         _flatMoleculeStartFormulasRepository = flatMoleculeStartFormulasRepository;
         _flatContainerRepository = flatContainerRepository;
         _moleculeStartFormulas = new List<IMoleculeStartFormula>();
      }

      public override IEnumerable<IMoleculeStartFormula> All()
      {
         Start();
         return _moleculeStartFormulas;
      }

      protected override void DoStart()
      {
         foreach (var flatStartFormula in _flatMoleculeStartFormulasRepository.All())
         {
            var moleculePath = _flatContainerRepository.ContainerPathFrom(flatStartFormula.Id);
            moleculePath.Add(flatStartFormula.MoleculeName);

            var startFormula = new MoleculeStartFormula(moleculePath, flatStartFormula.CalculationMethod,
                                                        flatStartFormula.Rate);
            _moleculeStartFormulas.Add(startFormula);
         }
      }

      public RateKey RateKeyFor(ObjectPath moleculePath, ModelProperties modelProperties)
      {
         Start();

         foreach (var startFormula in _moleculeStartFormulas)
         {
            if (startFormula.MoleculePath.Equals(moleculePath) &&
                modelProperties.ContainsCalculationMethod(startFormula.RateKey.CalculationMethod))
               return startFormula.RateKey;
         }

         return null;
      }
   }
}