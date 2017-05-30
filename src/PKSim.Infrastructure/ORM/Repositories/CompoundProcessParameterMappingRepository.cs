using System;
using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class CompoundProcessParameterMappingRepository : StartableRepository<ICompoundProcessParameterMapping>, ICompoundProcessParameterMappingRepository
   {
      private readonly IFlatCompoundProcessParameterMappingRepository _flatParameterMappingRepo;
      private readonly IFlatContainerRepository _flatContainerRepo;
      private readonly ICache<CompositeKey, ICompoundProcessParameterMapping> _parameterMappings;

      public CompoundProcessParameterMappingRepository(IFlatCompoundProcessParameterMappingRepository flatParameterMappingRepo, IFlatContainerRepository flatContainerRepo)
      {
         _flatParameterMappingRepo = flatParameterMappingRepo;
         _flatContainerRepo = flatContainerRepo;
         _parameterMappings = new Cache<CompositeKey, ICompoundProcessParameterMapping>(pm => keyFor(pm.ProcessName, pm.ParameterName));
      }

      public override IEnumerable<ICompoundProcessParameterMapping> All()
      {
         Start();
         return _parameterMappings;
      }

      protected override void DoStart()
      {
         foreach (var flatParameterMapping in _flatParameterMappingRepo.All())
         {
            ICompoundProcessParameterMapping mapping = new CompoundProcessParameterMapping();

            mapping.ProcessName = flatParameterMapping.ProcessName;
            mapping.ParameterName = flatParameterMapping.ParameterName;
            mapping.MappedParameterPath = _flatContainerRepo.ContainerPathFrom(flatParameterMapping.ContainerId);
            mapping.MappedParameterPath.Add(flatParameterMapping.ContainerParameterName);

            _parameterMappings.Add(mapping);
         }
      }

      public IObjectPath MappedParameterPathFor(string compoundProcessName, string processParameterName)
      {
         Start();

         var key = keyFor(compoundProcessName, processParameterName);

         if (!_parameterMappings.Contains(key))
            throw new ArgumentException(PKSimConstants.Error.CompoundProcessParameterMappingNotAvailable(compoundProcessName, processParameterName));

         return _parameterMappings[key].MappedParameterPath;
      }

      public bool HasMappedParameterFor(string compoundProcessName, string processParameterName)
      {
         Start();
         return _parameterMappings.Contains(keyFor(compoundProcessName, processParameterName));
      }

      private CompositeKey keyFor(string compoundProcessName, string processParameterName)
      {
         return new CompositeKey(compoundProcessName, processParameterName);
      }
   }
}