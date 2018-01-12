using System;
using System.Collections.Generic;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;

using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Protocols;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Protocols
{
   public interface IProtocolItemPresenter : ISubPresenter, IListener, IEditParameterPresenter
   {
      IEnumerable<ApplicationType> AllApplications();
      IEnumerable<string> AllFormulationKeys();
      void EditProtocol(Protocol protocol);
      event Action StatusChanging;
   }

   public abstract class ProtocolItemPresenter<TView, TPresenter> : AbstractSubPresenter<TView, TPresenter>, IProtocolItemPresenter
      where TView : IView<TPresenter>, IProtocolView
      where TPresenter : IProtocolItemPresenter
   {
      protected readonly IProtocolTask _protocolTask;
      private readonly IParameterTask _parameterTask;
      public event Action StatusChanging = delegate { };

      protected ProtocolItemPresenter(TView view, IProtocolTask protocolTask, IParameterTask parameterTask)
         : base(view)
      {
         _protocolTask = protocolTask;
         _parameterTask = parameterTask;
      }

      public abstract IEnumerable<ApplicationType> AllApplications();

      public IEnumerable<string> AllFormulationKeys()
      {
         return _protocolTask.AllFormulationKey();
      }

      public void SetParameterValue(IParameterDTO parameterDTO, double newValue)
      {
         AddCommand(_parameterTask.SetParameterDisplayValue(ParameterFrom(parameterDTO), newValue));
      }

      public void SetParameterUnit(IParameterDTO parameterDTO, Unit newUnit)
      {
         AddCommand(_parameterTask.SetParameterUnit(ParameterFrom(parameterDTO), newUnit));
      }

      public void SetParameterPercentile(IParameterDTO parameterDTO, double percentileInPercent)
      {
         AddCommand(_parameterTask.SetParameterPercentile(ParameterFrom(parameterDTO), percentileInPercent));
      }

      public void SetParameterValueOrigin(IParameterDTO parameterDTO, ValueOrigin valueOrigin)
      {
         AddCommand(_parameterTask.SetParameterValueOrigin(ParameterFrom(parameterDTO), valueOrigin));
      }

      public abstract void EditProtocol(Protocol protocol);

      protected IParameter ParameterFrom(IParameterDTO parameterDTO)
      {
         return parameterDTO.Parameter;
      }

      public override void ViewChanged()
      {
         //We use the changing event in that case since the StatusChanged Event triggers a refresh of the plot
         StatusChanging();
      }
   }
}