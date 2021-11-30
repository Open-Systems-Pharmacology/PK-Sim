using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Repositories
{
   public interface IUsedMoleculeRepository : IRepository<string>
   {
   }

   public class UsedMoleculeRepository : IUsedMoleculeRepository
   {
      private readonly IPKSimProjectRetriever _projectRetriever;
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly IMoleculeParameterRepository _moleculeParameterRepository;

      public UsedMoleculeRepository(
         IPKSimProjectRetriever projectRetriever, 
         IOntogenyRepository ontogenyRepository,
         IMoleculeParameterRepository moleculeParameterRepository)
      {
         _projectRetriever = projectRetriever;
         _ontogenyRepository = ontogenyRepository;
         _moleculeParameterRepository = moleculeParameterRepository;
      }

      public IEnumerable<string> All()
      {
         //First add User defined molecules
         return allMoleculesDefinedInCompounds()
            .Union(allExpressionProfiles())
            .OrderBy(x => x)
            //Then predefined molecules
            .Union(allPredefinedMolecules())
            .Distinct();
      }

      private IEnumerable<string> allMoleculesDefinedInCompounds()
      {
         return allLoadedBuildingBlocks<Compound>()
            .SelectMany(comp => comp.AllProcesses<PartialProcess>())
            .Select(proc => proc.MoleculeName);
      }

      private IEnumerable<string> allExpressionProfiles()
      {
         return allLoadedBuildingBlocks<ExpressionProfile>().Select(x => x.MoleculeName);
      }

      private IEnumerable<string> allPredefinedMolecules()
      {
         //We use human as it has the most predefined molecules in the DB
         return _ontogenyRepository.AllFor(CoreConstants.Species.HUMAN)
            .Select(x => x.Name)
            .Union(_moleculeParameterRepository.All()
               .Select(x => x.MoleculeName))
            .OrderBy(x => x);
      }
      private IEnumerable<TBuildingBlock> allLoadedBuildingBlocks<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return _projectRetriever.Current.All<TBuildingBlock>(x => x.IsLoaded);
      }
   }
}