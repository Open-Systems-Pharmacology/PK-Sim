using System.Data;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IParameterValuesDebugPresenter : IPresenter<IParameterDebugView>, IDisposablePresenter
   {
      void ShowParametersFor(IPKSimBuildingBlock buildingBlock);
   }

   public class ParameterValuesDebugPresenter : AbstractDisposablePresenter<IParameterDebugView, IParameterValuesDebugPresenter>, IParameterValuesDebugPresenter
   {
      private readonly IParametersReportCreator _parametersReportCreator;
      private const string BUILDING_BLOCK_TYPE = "Building Block Type";

      public ParameterValuesDebugPresenter(IParameterDebugView view, IParametersReportCreator parametersReportCreator)
         : base(view)
      {
         _parametersReportCreator = parametersReportCreator;
      }

      public void ShowParametersFor(IPKSimBuildingBlock buildingBlock)
      {
         _view.Caption = $"All Parameters for BuildingBlock {buildingBlock.Name}";
         var allParameters = buildingBlock.GetAllChildren<IParameter>();
         var dataTable = _parametersReportCreator.ExportParametersToTable(allParameters, configureTable, addParameterInfo);
         _view.BindTo(dataTable);
         _view.Display();
      }

      private void addParameterInfo(IParameter parameter, DataRow dataRow)
      {
         dataRow[BUILDING_BLOCK_TYPE] = parameter.BuildingBlockType;
      }

      private void configureTable(DataTable dataTable)
      {
         dataTable.AddColumn(BUILDING_BLOCK_TYPE);
      }
   }
}