using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using PKSim.Core.Model;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface IParameterValuePresenter : ICommandCollectorPresenter
   {
      void SetParameterValue(IParameterDTO parameterDTO, double valueInGuiUnit);
      void SetParameterUnit(IParameterDTO parameterDTO, Unit displayUnit);
      void SetParameterPercentile(IParameterDTO parameterDTO, double percentileInPercent);
      void SetParameterValueOrigin(IParameterDTO parameterDTO, ValueOrigin valueOrigin);
   }

   public interface IEditParameterPresenter : IParameterValuePresenter
   {
      /// <summary>
      ///    Returns true if the parameter can be edited otherwise false
      /// </summary>
      bool CanEditParameter(IParameterDTO parameterDTO);

      /// <summary>
      ///    Reset the given parameter
      /// </summary>
      void ResetParameter(IParameterDTO parameterDTO);

      /// <summary>
      ///    Returns true if the parameter was set by the user
      /// </summary>
      bool IsSetByUser(IParameterDTO parameterDTO);
   }

   public abstract class EditParameterPresenter<TView, TPresenter> : AbstractCommandCollectorPresenter<TView, TPresenter>, IEditParameterPresenter
      where TView : IView<TPresenter>
      where TPresenter : IEditParameterPresenter
   {
      private readonly IEditParameterPresenterTask _editParameterPresenterTask;

      protected EditParameterPresenter(TView view, IEditParameterPresenterTask editParameterPresenterTask) : base(view)
      {
         _editParameterPresenterTask = editParameterPresenterTask;
      }

      public virtual void SetParameterPercentile(IParameterDTO parameterDTO, double percentileInPercent)
      {
         _editParameterPresenterTask.SetParameterPercentile(this, parameterDTO, percentileInPercent);
      }

      public void SetParameterValueOrigin(IParameterDTO parameterDTO, ValueOrigin valueOrigin)
      {
         _editParameterPresenterTask.SetParameterValueOrigin(this, parameterDTO, valueOrigin);
      }

      public virtual void EditTableFor(IParameterDTO parameterDTO)
      {
         _editParameterPresenterTask.EditTableFor(this, parameterDTO);
      }

      public virtual void SetParameterValue(IParameterDTO parameterDTO, double valueInGuiUnit)
      {
         _editParameterPresenterTask.SetParameterValue(this, parameterDTO, valueInGuiUnit);
      }

      public virtual void SetParameterUnit(IParameterDTO parameterDTO, Unit displayUnit)
      {
         _editParameterPresenterTask.SetParameterUnit(this, parameterDTO, displayUnit);
      }

      protected virtual IParameter ParameterFrom(IParameterDTO parameterDTO)
      {
         return parameterDTO.Parameter;
      }

      protected virtual IEnumerable<IParameter> ParametersFrom(IEnumerable<IParameterDTO> parametersDTO)
      {
         return parametersDTO.Select(x => x.Parameter);
      }

      public virtual void SetFavorite(IParameterDTO parameterDTO, bool isFavorite)
      {
         _editParameterPresenterTask.SetParameterFavorite(parameterDTO, isFavorite);
      }

      public bool CanEditParameter(IParameterDTO parameterDTO)
      {
         if (parameterDTO.Parameter.Editable)
            return true;

         //table parameter are always editable
         return parameterDTO.FormulaType == FormulaType.Table;
      }

      public void ResetParameter(IParameterDTO parameterDTO)
      {
         _editParameterPresenterTask.ResetParameter(this, parameterDTO);
      }

      public bool IsSetByUser(IParameterDTO parameterDTO)
      {
         if (parameterDTO.Parameter == null)
            return false;

         return parameterDTO.Parameter.ValueDiffersFromDefault();
      }
   }
}