using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;

namespace PKSim.Presentation.UICommands
{
   public class ExportToPDFCommand<T> : ObjectUICommand<T> where T : class
   {
      private readonly IApplicationController _applicationController;

      public ExportToPDFCommand(IApplicationController applicationController)
      {
         _applicationController = applicationController;
      }

      protected override void PerformExecute()
      {
         using (var reportingPresenter = _applicationController.Start<IReportingPresenter>())
         {
            reportingPresenter.CreateReport(Subject);
         }
      }
   }

   public class ExportSimulationAnalysisToPDFCommand : ExportToPDFCommand<ISimulationAnalysis>
   {
      public ExportSimulationAnalysisToPDFCommand(IApplicationController applicationController) : base(applicationController)
      {
      }
   }

   public class ExportChartToPDFCommand : ExportToPDFCommand<CurveChart>
   {
      public ExportChartToPDFCommand(IApplicationController applicationController)
         : base(applicationController)
      {
      }
   }

   public class ExportHistoryToPDFCommand : ExportToPDFCommand<IHistoryManager>
   {
      private readonly IWorkspace _workspace;

      public ExportHistoryToPDFCommand(IApplicationController applicationController, IWorkspace workspace) : base(applicationController)
      {
         _workspace = workspace;
      }

      protected override void PerformExecute()
      {
         Subject = _workspace.HistoryManager;
         base.PerformExecute();
      }
   }

   public class ExportActiveSimulationToPDFCommand : ExportToPDFCommand<Simulation>
   {
      private readonly IActiveSubjectRetriever _activeSubjectRetriever;

      public ExportActiveSimulationToPDFCommand(IApplicationController applicationController, IActiveSubjectRetriever activeSubjectRetriever) : base(applicationController)
      {
         _activeSubjectRetriever = activeSubjectRetriever;
      }

      protected override void PerformExecute()
      {
         Subject = _activeSubjectRetriever.Active<Simulation>();
         base.PerformExecute();
      }
   }

   public class ExportProjectToPDFCommand : ExportToPDFCommand<PKSimProject>
   {
      private readonly IPKSimProjectRetriever _projectRetriever;

      public ExportProjectToPDFCommand(IApplicationController applicationController, IPKSimProjectRetriever projectRetriever) : base(applicationController)
      {
         _projectRetriever = projectRetriever;
      }

      protected override void PerformExecute()
      {
         Subject = _projectRetriever.Current;
         base.PerformExecute();
      }
   }

   public class ExportCollectionToPDFCommand<T> : ExportToPDFCommand<IReadOnlyCollection<T>>
   {
      public ExportCollectionToPDFCommand(IApplicationController applicationController, IPKSimProjectRetriever projectRetriever) : base(applicationController)
      {
         var project = projectRetriever.Current;
         IEnumerable<T> all;
         if (typeof(T).IsAnImplementationOf<IPKSimBuildingBlock>())
            all = project.All<IPKSimBuildingBlock>().OfType<T>();

         else if (typeof(T).IsAnImplementationOf<DataRepository>())
            all = project.AllObservedData.Cast<T>();

         else if (typeof(T).IsAnImplementationOf<ISimulationComparison>())
            all = project.AllSimulationComparisons.Cast<T>();

         else
            all = Enumerable.Empty<T>();

         Subject = new ReadOnlyCollection<T>(all.ToList());
      }
   }
}