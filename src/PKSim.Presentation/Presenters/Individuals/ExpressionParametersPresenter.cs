using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IExpressionParametersPresenter<TExpressionParameterDTO> : IPresenter<IExpressionParametersView<TExpressionParameterDTO>>,
      IEditParameterPresenter where TExpressionParameterDTO : ExpressionParameterDTO
   {
      void Edit(IReadOnlyList<TExpressionParameterDTO> expressionParameters);
      bool ShowInitialConcentration { get; set; }
      void SetExpressionParameterValue(IParameterDTO expressionParameterDTO, double value);
      void DisableEdit();
   }

   public interface IExpressionParametersPresenter : IExpressionParametersPresenter<ExpressionParameterDTO>
   {
   }

   public abstract class ExpressionParametersPresenter<TExpressionParameterDTO> : EditParameterPresenter<
         IExpressionParametersView<TExpressionParameterDTO>,
         IExpressionParametersPresenter<TExpressionParameterDTO>>,
      IExpressionParametersPresenter<TExpressionParameterDTO> where TExpressionParameterDTO : ExpressionParameterDTO
   {
      private IReadOnlyList<TExpressionParameterDTO> _expressionParameters;
      private bool _showInitialConcentration;

      protected ExpressionParametersPresenter(
         IExpressionParametersView<TExpressionParameterDTO> view,
         IEditParameterPresenterTask editParameterPresenterTask) : base(view, editParameterPresenterTask)
      {
         _showInitialConcentration = false;
      }


      public bool ShowInitialConcentration
      {
         get => _showInitialConcentration;
         set
         {
            _showInitialConcentration = value;
            rebind();
         }
      }

      private void rebind()
      {
         normalizeExpressionValues();
         _view.EmphasisRelativeExpressionParameters = ShowInitialConcentration;

         var parametersToDisplay = _expressionParameters.Where(x=>parameterShouldBeDisplayed(x.Parameter)).ToList();
         _view.BindTo(parametersToDisplay);
      }

      private bool parameterShouldBeDisplayed(IParameterDTO parameter) => 
         parameter.IsNamed(INITIAL_CONCENTRATION) ? ShowInitialConcentration : parameter.Parameter.Visible;

      public void Edit(IReadOnlyList<TExpressionParameterDTO> expressionParameters)
      {
         _expressionParameters = expressionParameters;
         rebind();
      }

      private void normalizeExpressionValues()
      {
         var allExpressionParameters = _expressionParameters.Where(x => x.Parameter.Parameter.HasExpressionName()).ToList();
         var max = allExpressionParameters.Select(x => x.Value).Max();

         allExpressionParameters.Each(x => x.NormalizedExpression = max == 0 ? 0 : x.Value / max);
      }

      public void SetExpressionParameterValue(IParameterDTO expressionParameterDTO, double value)
      {
         SetParameterValue(expressionParameterDTO, value);
         var parameter = expressionParameterDTO.Parameter;
         if (!parameter.HasExpressionName())
            return;

         normalizeExpressionValues();
      }

      public void DisableEdit()
      {
         _view.ReadOnly = true;
      }
   }

   public class ExpressionParametersPresenter : ExpressionParametersPresenter<ExpressionParameterDTO>, IExpressionParametersPresenter
   {
      public ExpressionParametersPresenter(IExpressionParametersView<ExpressionParameterDTO> view,
         IEditParameterPresenterTask editParameterPresenterTask) : base(view, editParameterPresenterTask)
      {
      }
   }
}