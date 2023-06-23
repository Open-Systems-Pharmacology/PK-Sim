using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class ModelContainerMoleculeRepository : StartableRepository<string>, IModelContainerMoleculeRepository
   {
      private readonly IFlatModelContainerMoleculeRepository _flatModelContainerMoleculeRepo;
      private readonly IFlatModelRepository _flatModelRepo;
      private readonly IFlatContainerRepository _flatContainerRepo;
      private readonly ICache<string, List<string>> _moleculesForModel;

      private readonly ICache<ModelContainerMoleculeKey, FlatModelContainerMolecule> _mcmProperties; 

      public ModelContainerMoleculeRepository(IFlatModelContainerMoleculeRepository flatModelContainerMoleculeRepo,
                                              IFlatModelRepository flatModelRepo,
                                              IFlatContainerRepository flatContainerRepo)
      {
         _flatModelContainerMoleculeRepo = flatModelContainerMoleculeRepo;
         _flatModelRepo = flatModelRepo;
         _flatContainerRepo = flatContainerRepo;
         _moleculesForModel=new Cache<string, List<string>>();
         _mcmProperties = new Cache<ModelContainerMoleculeKey, FlatModelContainerMolecule>();
      }

      protected override void DoStart()
      {
         // first, cache all (static) molecules used in model (besides DRUG)
         foreach (var flatModel in _flatModelRepo.All())
         {
            var modelName = flatModel.Id;

            var molecules = (from mcm in _flatModelContainerMoleculeRepo.All()
                                       where mcm.Model.Equals(modelName)
                                       where mcm.IsPresent
                                       where !mcm.Molecule.Equals(CoreConstants.Molecule.Drug)
                                       select mcm.Molecule).Distinct().ToList();

            _moleculesForModel.Add(modelName, molecules);
         }

         // second, cache MCM-info by {Model, Container, Molecule}
         foreach (var flatMCM in _flatModelContainerMoleculeRepo.All())
         {
            var key = new ModelContainerMoleculeKey(flatMCM.Model, flatMCM.Id, flatMCM.Molecule);
            _mcmProperties.Add(key, flatMCM); 
         }
      }

      public override IEnumerable<string> All()
      {
         Start();
         return new List<string>(); //dummy implementation, just because required by the interface
      }

      public IReadOnlyList<string> MoleculeNamesWithoutDrug(string modelName)
      {
         Start();
         return _moleculesForModel[modelName];
      }

      public IReadOnlyList<string> MoleculeNamesIncludingDrug(string modelName)
      {
         Start();

         var moleculeNames = MoleculeNamesWithoutDrug(modelName).ToList();
         moleculeNames.Add(CoreConstants.Molecule.Drug);

         return moleculeNames;
      }

      public bool IsPresent(string modelName, int containerId, string moleculeName)
      {
         Start();

         var key = new ModelContainerMoleculeKey(modelName, containerId, moleculeName);

         // if no entry found, return true for DRUG and false for all other molecules
         if (!_mcmProperties.Contains(key))
            return moleculeName.Equals(CoreConstants.Molecule.Drug); 

         return _mcmProperties[key].IsPresent;
      }

      public bool IsPresent(string modelName, ObjectPath containerPath, string moleculeName)
      {
         Start();

         var container = _flatContainerRepo.ContainerFrom(containerPath.ToString());
         return IsPresent(modelName, container.Id, moleculeName);
      }

      public bool NegativeValuesAllowed(string modelName, ObjectPath containerPath, string moleculeName)
      {
         Start();

         var container = _flatContainerRepo.ContainerFrom(containerPath.ToString());
         return negativeValuesAllowed(modelName, container.Id, moleculeName);
      }

      private bool negativeValuesAllowed(string modelName, int containerId, string moleculeName)
      {
         var key = new ModelContainerMoleculeKey(modelName, containerId, moleculeName);

         // if no entry found, return false
         if (!_mcmProperties.Contains(key))
            return false;

         return _mcmProperties[key].NegativeValuesAllowed;
      }
   }

   class ModelContainerMoleculeKey
   {
      public CompositeKey ModelAndMolecule { get; private set; }
      public int ContainerId { get; private set; }

      public ModelContainerMoleculeKey(string modelName, int containerId, string moleculeName)
      {
         ModelAndMolecule=new CompositeKey(modelName,moleculeName);
         ContainerId = containerId;
      }

      public override bool Equals(object obj)
      {
         var key = obj as ModelContainerMoleculeKey;

         return (key != null)
                && Equals(key.ModelAndMolecule, ModelAndMolecule)
                && Equals(key.ContainerId, ContainerId);
      }

      public override int GetHashCode()
      {
         return ModelAndMolecule.GetHashCode() + ContainerId;
      }
   }
}
