using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Core.Snapshots.Services;

namespace PKSim.IntegrationTests
{
   [Category("Snapshot Integration")]
   public abstract class ContextWithLoadedSnapshot : ContextForIntegration<ISnapshotMapper>
   {
      private ISnapshotTask _snapshotTask;
      protected PKSimProject _project;

      public void LoadSnapshot(string snapshotFileName, bool isFullPath = false)
      {
         var snapshotFile = isFullPath ? snapshotFileName : DomainHelperForSpecs.DataFilePathFor($"{snapshotFileName}.json");
         _snapshotTask = IoC.Resolve<ISnapshotTask>();
         _project = _snapshotTask.LoadProjectFromSnapshotFileAsync(snapshotFile).Result;
      }

      public TBuildingBlock FindByName<TBuildingBlock>(string name) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         var bb = _project.All<TBuildingBlock>().FindByName(name);
         return bb;
      }

      public TBuildingBlock First<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return _project.All<TBuildingBlock>().FirstOrDefault();
      }

      public List<TBuildingBlock> All<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return _project.All<TBuildingBlock>().ToList();
      }
   }
}