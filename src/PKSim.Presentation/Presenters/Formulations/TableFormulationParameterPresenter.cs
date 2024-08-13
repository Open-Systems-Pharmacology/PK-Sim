using OSPSuite.Presentation.Views.Parameters;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.Presenters.Formulations
{
   public interface ITableFormulationParameterPresenter : ITableParameterPresenter
   {
   }

   public class TableFormulationParameterPresenter : Parameters.TableParameterPresenter<ITableFormulaView>, ITableFormulationParameterPresenter
   {
      public TableFormulationParameterPresenter(ITableFormulaView view, IParameterTask parameterTask, IFormulaFactory formulaFactory, ICloner cloner, IFormulationTask formulationTask) :
         base(view, parameterTask, formulaFactory, cloner, formulationTask.ImportTableFormula)
      {
      }
   }
}