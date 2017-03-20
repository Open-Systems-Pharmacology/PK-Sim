using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IParameterValuesDebugPresenter : IPresenter<IParameterDebugView>, IDisposablePresenter
   {
      void ShowParametersFor(IPKSimBuildingBlock buildingBlock);

   }
   public class ParameterValuesDebugPresenter : AbstractDisposablePresenter<IParameterDebugView, IParameterValuesDebugPresenter>, IParameterValuesDebugPresenter
   {
      private readonly IParametersReportCreator _parametersReportCreator;

      public ParameterValuesDebugPresenter(IParameterDebugView view, IParametersReportCreator parametersReportCreator)
         : base(view)
      {
         _parametersReportCreator = parametersReportCreator;
      }

      public void ShowParametersFor(IPKSimBuildingBlock buildingBlock)
      {
         _view.Caption = "All Parameters for BuidlingBlock {0}".FormatWith(buildingBlock.Name);
         var allParameters = buildingBlock.GetAllChildren<IParameter>();
         _view.BindTo(_parametersReportCreator.ExportParametersToTable(allParameters));
         _view.Display();
      }
   }
}