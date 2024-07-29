using OSPSuite.Presentation.Presenters.Parameters;
using OSPSuite.Presentation.Views.Parameters;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.Presenters.Formulations
{
   public interface ITableFormulationParameterPresenter : ITableParameterPresenter
   {
   }

   public class TableFormulationParameterPresenter : Parameters.TableParameterPresenter<ITableParameterView>, ITableFormulationParameterPresenter
   {
      public TableFormulationParameterPresenter(ITableParameterView view, IParameterTask parameterTask, IFormulaFactory formulaFactory, ICloner cloner, IFormulationTask formulationTask) :
         base(view, parameterTask, formulaFactory, cloner, formulationTask.ImportTableFormula)
      {
      }
   }
}