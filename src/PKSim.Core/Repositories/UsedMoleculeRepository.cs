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

      public UsedMoleculeRepository(IPKSimProjectRetriever projectRetriever)
      {
         _projectRetriever = projectRetriever;
      }

      public IEnumerable<string> All()
      {
         return allMoleculesDefinedInIndividuals()
            .Union(allMoleculesDefinedInCompounds())
            .Union(allExpressionProfiles())
            .Distinct()
            .OrderBy(x => x);
      }

      private IEnumerable<string> allMoleculesDefinedInCompounds()
      {
         return allLoadedBuildingBlocks<Compound>()
            .SelectMany(comp => comp.AllProcesses<PartialProcess>())
            .Select(proc => proc.MoleculeName);
      }

      private IEnumerable<string> allMoleculesDefinedInIndividuals()
      {
         return allLoadedBuildingBlocks<Individual>()
            .SelectMany(x => x.AllMolecules())
            .Select(x => x.Name);
      }

      private IEnumerable<string> allExpressionProfiles()
      {
         return allLoadedBuildingBlocks<ExpressionProfile>()
            .Select(x => x.MoleculeName);
      }

      private IEnumerable<TBuildingBlock> allLoadedBuildingBlocks<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return _projectRetriever.Current.All<TBuildingBlock>(x => x.IsLoaded);
      }
   }
}