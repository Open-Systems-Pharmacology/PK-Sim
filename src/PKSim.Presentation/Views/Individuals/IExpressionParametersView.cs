using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IExpressionParametersView<TExpressionParameterDTO> : IView<IExpressionParametersPresenter<TExpressionParameterDTO>>
      where TExpressionParameterDTO : ExpressionParameterDTO
   {
      void BindTo(IEnumerable<TExpressionParameterDTO> expressionParameters);
      bool EmphasisRelativeExpressionParameters { get; set; }
   }
}