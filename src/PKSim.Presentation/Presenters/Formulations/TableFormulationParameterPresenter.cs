using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation.Presenters.Formulations
{
   public interface ITableFormulationParameterPresenter : ITableParameterPresenter
   {
   }

   public class TableFormulationParameterPresenter : TableParameterPresenter<ITableParameterView>, ITableFormulationParameterPresenter
   {
      public TableFormulationParameterPresenter(ITableParameterView view, IParameterTask parameterTask, IFormulaFactory formulaFactory, ICloner cloner, IFormulationTask formulationTask) :
         base(view, parameterTask, formulaFactory, cloner, formulationTask.ImportTableFormula)
      {
      }
   }
}