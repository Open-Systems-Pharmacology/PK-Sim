using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Presenters.Parameters;
using OSPSuite.Presentation.Views.Parameters;
using PKSim.Assets;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IEditTableSolubilityParameterPresenter : IEditTableParameterPresenter
   {
   }

   public class EditTableSolubilityParameterPresenter : EditTableParameterPresenter<ITableSolubilityParameterPresenter>, IEditTableSolubilityParameterPresenter
   {
      public EditTableSolubilityParameterPresenter(IEditTableParameterView view, ITableSolubilityParameterPresenter tableParameterPresenter, IFullPathDisplayResolver fullPathDisplayResolver, ISimpleChartPresenter chartPresenter) : base(view, tableParameterPresenter, fullPathDisplayResolver, chartPresenter)
      {
         tableParameterPresenter.ImportToolTip = PKSimConstants.UI.ImportSolubilityTable;
         tableParameterPresenter.Description = PKSimConstants.UI.ImportSolubilityTableDescription;
      }
   }
}