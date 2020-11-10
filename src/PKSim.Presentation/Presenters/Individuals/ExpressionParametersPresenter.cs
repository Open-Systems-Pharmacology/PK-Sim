using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IExpressionParametersPresenter:IPresenter<IExpressionParametersView>, IEditParameterPresenter
   {
      void Edit(IReadOnlyList<ExpressionParameterDTO> expressionParameters);
      void SetExpressionParameterValue(IParameterDTO expressionParameterDTO, double value);

   }

   public class ExpressionParametersPresenter : EditParameterPresenter<IExpressionParametersView, IExpressionParametersPresenter>, IExpressionParametersPresenter
   {
      private IReadOnlyList<ExpressionParameterDTO> _expressionParameters;

      public ExpressionParametersPresenter(
         IExpressionParametersView view, 
         IEditParameterPresenterTask editParameterPresenterTask) : base(view, editParameterPresenterTask)
      {
      }

      private void rebind()
      {
         normalizeExpressionValues();
         _view.BindTo(_expressionParameters.Where(x => x.Visible));
      }

      public void Edit(IReadOnlyList<ExpressionParameterDTO> expressionParameters)
      {
         _expressionParameters = expressionParameters;
         rebind();
      }

      private void normalizeExpressionValues()
      {
         var allExpressionParameters = _expressionParameters.Where(x => x.Parameter.Parameter.IsExpression()).ToList();
         var max = allExpressionParameters.Select(x => x.Value).Max();

         allExpressionParameters.Each(x => x.NormalizedExpression = max == 0 ? 0 : x.Value / max);
      }

      public void SetExpressionParameterValue(IParameterDTO expressionParameterDTO, double value)
      {
         SetParameterValue(expressionParameterDTO, value);
         var parameter = expressionParameterDTO.Parameter;
         if (!parameter.IsExpression())
            return;

         normalizeExpressionValues();
      }

   }
}