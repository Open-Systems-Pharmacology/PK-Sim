using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;

namespace PKSim.IntegrationTests
{
   [Category("ProjectConverter")]
   public abstract class ContextWithLoadedProject<T> : ContextForIntegration<T>
   {
      protected ILazyLoadTask _lazyLoadTask;
      protected PKSimProject _project;
      protected IWorkspace _workspace;

      public void LoadProject(string projectFileName, bool isFullPath = false)
      {
         var projectFile = isFullPath ? projectFileName : DomainHelperForSpecs.DataFilePathFor($"{projectFileName}.pksim5");
         var workspacePersistor = IoC.Resolve<IWorkspacePersistor>();
         _workspace = IoC.Resolve<IWorkspace>();
         _lazyLoadTask = IoC.Resolve<ILazyLoadTask>();
         workspacePersistor.LoadSession(_workspace, projectFile);
         _project = _workspace.Project;
      }

      protected override void Context()
      {
      }

      public TBuildingBlock FindByName<TBuildingBlock>(string name) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         var bb = _project.All<TBuildingBlock>().FindByName(name);
         Load(bb);
         return bb;
      }

      public TBuildingBlock First<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         var bb = _project.All<TBuildingBlock>().FirstOrDefault();
         if (bb == null)
            return null;

         Load(bb);
         return bb;
      }

      protected void Load<TBuildingBlock>(TBuildingBlock bb) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         _lazyLoadTask.Load(bb);
      }

      public List<TBuildingBlock> All<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return _project.All<TBuildingBlock>().ToList();
      }

      public DataRepository FirstObservedData()
      {
         return _project.AllObservedData.First();
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         UnloadProject();
      }

      protected void UnloadProject()
      {
         var registrationTask = IoC.Resolve<IRegistrationTask>();
         registrationTask.UnregisterProject(_project);
      }
   }
}