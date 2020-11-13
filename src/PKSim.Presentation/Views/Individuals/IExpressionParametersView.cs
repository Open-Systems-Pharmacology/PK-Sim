using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IExpressionParametersView: IView<IExpressionParametersPresenter>
   {
      void BindTo(IEnumerable<ExpressionParameterDTO> expressionParameters);
      bool EmphasisRelativeExpressionParameters { get; set; }
   }
}