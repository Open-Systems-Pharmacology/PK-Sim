using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICalculatedParameterValuePresenter : IPresenter<ICalculatedParameterValueView>, ICommandCollectorPresenter
   {
      void Edit(IEnumerable<IParameter> calculatedParameters);
      string Description { set; }
   }

   public class CalculatedParameterValuePresenter : AbstractCommandCollectorPresenter<ICalculatedParameterValueView, ICalculatedParameterValuePresenter>, ICalculatedParameterValuePresenter
   {
      private readonly IMultiParameterEditPresenter _multiParameterEditPresenter;

      public CalculatedParameterValuePresenter(ICalculatedParameterValueView view, IMultiParameterEditPresenter multiParameterEditPresenter) : base(view)
      {
         _multiParameterEditPresenter = multiParameterEditPresenter;
         _multiParameterEditPresenter.IsSimpleEditor = true;

         _view.SetParameterView(_multiParameterEditPresenter.BaseView);
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _multiParameterEditPresenter.InitializeWith(commandCollector);
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _multiParameterEditPresenter.ReleaseFrom(eventPublisher);
      }

      public void Edit(IEnumerable<IParameter> calculatedParameters)
      {
         _multiParameterEditPresenter.Edit(calculatedParameters);
      }

      public string Description
      {
         set => _view.Description = value;
      }
   }
}