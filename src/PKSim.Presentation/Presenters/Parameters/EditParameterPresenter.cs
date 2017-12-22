using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface IEditParameterPresenter : ICommandCollectorPresenter
   {
      void SetParameterValue(IParameterDTO parameterDTO, double valueInGuiUnit);
      void SetParameterUnit(IParameterDTO parameterDTO, Unit displayUnit);
      void SetParameterPercentile(IParameterDTO parameterDTO, double percentileInPercent);
      void SetParameterValueOrigin(IParameterDTO parameterDTO, ValueOrigin valueOrigin);
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

      public virtual void SetParameterName(IParameterDTO parameterDTO, string name)
      {
         _editParameterPresenterTask.SetParameterName(this, parameterDTO, name);
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
   }
}