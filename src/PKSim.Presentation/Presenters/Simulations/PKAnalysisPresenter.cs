using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IPKAnalysisPresenter : IUnitsInColumnPresenter<string>, IPresenterWithSettings
   {
      string ToolTipFor(string pkParameterName);
      void ExportToExcel();
      string DisplayNameFor(string parameterName);
   }

   public abstract class PKAnalysisPresenter<TView, TPresenter> : AbstractSubPresenter<TView, TPresenter>, IPKAnalysisPresenter
      where TView : IView<TPresenter>, IPKAnalysisView
      where TPresenter : IPresenter
   {
      private readonly IPKParameterRepository _pkParameterRepository;
      private readonly IParameter _undefinedPKParameter;
      private DefaultPresentationSettings _settings;
      private readonly IPresentationSettingsTask _presentationSettingsTask;
      protected IGlobalPKAnalysisPresenter _globalPKAnalysisPresenter;

      protected PKAnalysisPresenter(TView view, IPKParameterRepository pkParameterRepository, IPresentationSettingsTask presentationSettingsTask, IGlobalPKAnalysisPresenter globalPKAnalysisPresenter)
         : base(view)
      {
         _pkParameterRepository = pkParameterRepository;
         _presentationSettingsTask = presentationSettingsTask;
         _undefinedPKParameter = new PKSimParameter { Dimension = Constants.Dimension.NO_DIMENSION };
         _settings = new DefaultPresentationSettings();
         _globalPKAnalysisPresenter = globalPKAnalysisPresenter;
         AddSubPresenters(_globalPKAnalysisPresenter);
         _view.AddGlobalPKAnalysisView(_globalPKAnalysisPresenter.View);
      }

      public void ChangeUnit(string pkParameterName, Unit newUnit)
      {
         changeUnit(pkParameterName, newUnit);
         BindToPKAnalysis();
      }

      private void changeUnit(string pkParameterName, Unit newUnit)
      {
         _settings.SetSetting(pkParameterName, newUnit.Name);
         foreach (var pkAnalysis in AllPKAnalyses)
         {
            var pkParameter = pkAnalysis.Parameter(pkParameterName);

            //possible when mixing different application protocols
            if (pkParameter != null)
               pkParameter.DisplayUnit = newUnit;
         }
      }

      protected abstract void BindToPKAnalysis();

      protected abstract IEnumerable<PKAnalysis> AllPKAnalyses { get; }

      public string ToolTipFor(string pkParameterName)
      {
         return _pkParameterRepository.DescriptionFor(pkParameterName);
      }

      public abstract void ExportToExcel();

      public string DisplayNameFor(string parameterName)
      {
         var displayName = _pkParameterRepository.DisplayNameFor(parameterName);
         return Constants.NameWithUnitFor(displayName, DisplayUnitFor(parameterName));
      }

      public IEnumerable<Unit> AvailableUnitsFor(string pkParameterName)
      {
         if (hasPKParameter(pkParameterName))
            return pkParameterFor(pkParameterName).Dimension.Units;

         return Enumerable.Empty<Unit>();
      }

      private IParameter pkParameterFor(string pkParameterName)
      {
         return AllPKAnalyses.Where(x => x.Parameter(pkParameterName) != null)
            .Select(x => x.Parameter(pkParameterName))
            .FirstOrDefault() ?? _undefinedPKParameter;
      }

      private bool hasPKParameter(string pkParameterName)
      {
         return !Equals(pkParameterFor(pkParameterName), _undefinedPKParameter);
      }

      public virtual void LoadSettingsForSubject(IWithId subject)
      {
         _settings = _presentationSettingsTask.PresentationSettingsFor<DefaultPresentationSettings>(this, subject);
      }

      public abstract string PresentationKey { get; }

      protected void LoadPreferredUnitsForPKAnalysis()
      {
         if (_settings == null)
            return;

         AllPKAnalyses.SelectMany(analysis => analysis.AllPKParameterNames).Distinct().Each(updateParameterUnit);
      }

      private void updateParameterUnit(string parameterName)
      {
         if (!hasPKParameter(parameterName))
            return;

         var currentDisplayUnit = DisplayUnitFor(parameterName).Name;
         var displayUnitToUse = _settings.GetSetting(parameterName, currentDisplayUnit);
         if (string.Equals(currentDisplayUnit, displayUnitToUse))
            return;

         changeUnit(parameterName, getUnit(parameterName, displayUnitToUse));
      }

      public Unit DisplayUnitFor(string pkParameterName)
      {
         return pkParameterFor(pkParameterName).DisplayUnit;
      }

      private Unit getUnit(string parameterName, string unit)
      {
         var dimension = pkParameterFor(parameterName).Dimension;
         return dimension.UnitOrDefault(unit);
      }
   }
}