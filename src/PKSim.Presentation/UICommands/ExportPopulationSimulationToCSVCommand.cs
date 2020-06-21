using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.UICommands;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.UICommands
{
   public abstract class ExportVectorialParameterContainerToCSVCommand<T> : ActiveObjectUICommand<T> where T : class, IWithName
   {
      private readonly IApplicationController _applicationController;

      protected ExportVectorialParameterContainerToCSVCommand(IActiveSubjectRetriever activeSubjectRetriever, IApplicationController applicationController) : base(activeSubjectRetriever)
      {
         _applicationController = applicationController;
      }

      protected FileSelection SelectCSVFile()
      {
         using (var presenter = _applicationController.Start<ISelectFilePresenter>())
         {
            return presenter.SelectFile(PKSimConstants.UI.ExportPopulationToCSVTitle, Constants.Filter.CSV_FILE_FILTER, CoreConstants.DefaultPopulationExportNameFor(Subject.Name), Constants.DirectoryKey.POPULATION);
         }
      }
   }

   public class ExportPopulationToCSVCommand : ExportVectorialParameterContainerToCSVCommand<Population>
   {
      private readonly IPopulationExportTask _populationExportTask;

      public ExportPopulationToCSVCommand(IPopulationExportTask populationExportTask, IApplicationController applicationController, IActiveSubjectRetriever activeSubjectRetriever) : base(activeSubjectRetriever, applicationController)
      {
         _populationExportTask = populationExportTask;
      }

      protected override void PerformExecute()
      {
         _populationExportTask.ExportToCSV(Subject, SelectCSVFile());
      }
   }

   public class ExportPopulationSimulationToCSVCommand : ExportVectorialParameterContainerToCSVCommand<PopulationSimulation>
   {
      private readonly IPopulationExportTask _populationExportTask;

      public ExportPopulationSimulationToCSVCommand(IPopulationExportTask populationExportTask, IApplicationController applicationController, IActiveSubjectRetriever activeSubjectRetriever)
         : base(activeSubjectRetriever, applicationController)
      {
         _populationExportTask = populationExportTask;
      }

      protected override void PerformExecute()
      {
         _populationExportTask.ExportToCSV(Subject, SelectCSVFile());
      }
   }
}