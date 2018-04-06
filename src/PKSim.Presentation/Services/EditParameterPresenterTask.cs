using OSPSuite.Core.Commands.Core;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.Services
{
   public interface IEditParameterPresenterTask
   {
      void SetParameterValue(ICommandCollector presenter, IParameterDTO parameterDTO, double valueInGuiUnit);
      void SetParameterUnit(ICommandCollector presenter, IParameterDTO parameterDTO, Unit displayUnit);
      void SetParameterPercentile(ICommandCollector presenter, IParameterDTO parameterDTO, double percentileInPercent);
      void SetParameterValueOrigin(ICommandCollector presenter, IParameterDTO parameterDTO, ValueOrigin valueOrigin);
      void SetParameterName(ICommandCollector presenter, IParameterDTO parameterDTO, string name);
      void EditTableFor(ICommandCollector presenter, IParameterDTO parameterDTO);
      void SetParameterFavorite(IParameterDTO parameterDTO, bool isFavorite);
   }

   public class EditParameterPresenterTask : IEditParameterPresenterTask
   {
      private readonly IParameterTask _parameterTask;
      private readonly IApplicationController _applicationController;

      public EditParameterPresenterTask(IParameterTask parameterTask, IApplicationController applicationController)
      {
         _parameterTask = parameterTask;
         _applicationController = applicationController;
      }

      public virtual void SetParameterPercentile(ICommandCollector presenter, IParameterDTO parameterDTO, double percentileInPercent)
      {
         presenter.AddCommand(_parameterTask.SetParameterPercentile(ParameterFrom(parameterDTO), percentileInPercent / 100));
      }

      public void SetParameterValueOrigin(ICommandCollector presenter, IParameterDTO parameterDTO, ValueOrigin valueOrigin)
      {
         presenter.AddCommand(_parameterTask.SetParameterValueOrigin(ParameterFrom(parameterDTO), valueOrigin));
      }

      public void SetParameterName(ICommandCollector presenter, IParameterDTO parameterDTO, string name)
      {
         presenter.AddCommand(_parameterTask.SetParameterName(ParameterFrom(parameterDTO), name));
      }

      public void EditTableFor(ICommandCollector presenter, IParameterDTO parameterDTO)
      {
         using (var tablePresenter = _applicationController.Start<IEditTableParameterPresenter>())
         {
            var parameter = ParameterFrom(parameterDTO);
            if (!tablePresenter.Edit(parameter))
               return;

            presenter.AddCommand(_parameterTask.SetParameterFomula(parameter, tablePresenter.EditedFormula));
         }
      }

      public void SetParameterFavorite(IParameterDTO parameterDTO, bool isFavorite)
      {
         parameterDTO.IsFavorite = isFavorite;
         _parameterTask.SetParameterFavorite(ParameterFrom(parameterDTO), isFavorite);
      }

      public virtual void SetParameterValue(ICommandCollector presenter, IParameterDTO parameterDTO, double valueInGuiUnit)
      {
         presenter.AddCommand(_parameterTask.SetParameterDisplayValue(ParameterFrom(parameterDTO), valueInGuiUnit));
      }

      public virtual void SetParameterUnit(ICommandCollector presenter, IParameterDTO parameterDTO, Unit displayUnit)
      {
         presenter.AddCommand(_parameterTask.SetParameterUnit(ParameterFrom(parameterDTO), displayUnit));
      }

      public virtual IParameter ParameterFrom(IParameterDTO parameterDTO)
      {
         return parameterDTO.Parameter;
      }
   }
}