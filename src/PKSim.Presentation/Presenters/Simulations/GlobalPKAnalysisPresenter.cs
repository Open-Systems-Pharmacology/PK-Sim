using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IGlobalPKAnalysisPresenter : IPresenter<IGlobalPKAnalysisView>, IUnitsInColumnPresenter<string>, IPresenterWithSettings
   {
      void CalculatePKAnalysis(IReadOnlyList<Simulation> simulations);
      GlobalPKAnalysis GlobalPKAnalysis { get; }
      bool ShouldCalculateBioAvailability(string compoundName, string parameterName);
      bool ShouldCalculateDDIRatio(string compoundName, string parameterName);
      void CalculateBioAvailability(string compoundName);
      void CalculateDDIRatioFor(string compoundName);
      string DisplayNameFor(string parameterName);
      bool HasParameters();
   }

   public class GlobalPKAnalysisPresenter : AbstractSubPresenter<IGlobalPKAnalysisView, IGlobalPKAnalysisPresenter>, IGlobalPKAnalysisPresenter
   {
      private readonly IGlobalPKAnalysisTask _globalPKAnalysisTask;
      private readonly IGlobalPKAnalysisToGlobalPKAnalysisDTOMapper _globalPKAnalysisDTOMapper;
      private readonly IHeavyWorkManager _heavyWorkManager;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private IReadOnlyList<Simulation> _simulations;

      public GlobalPKAnalysis GlobalPKAnalysis { get; private set; }
      private GlobalPKAnalysisDTO _globalPKAnalysisDTO;
      private DefaultPresentationSettings _settings;
      private readonly IPresentationSettingsTask _presentationSettingsTask;

      public GlobalPKAnalysisPresenter(IGlobalPKAnalysisView view, IGlobalPKAnalysisTask globalPKAnalysisTask,
         IGlobalPKAnalysisToGlobalPKAnalysisDTOMapper globalPKAnalysisDTOMapper, IHeavyWorkManager heavyWorkManager, IRepresentationInfoRepository representationInfoRepository, IPresentationSettingsTask presentationSettingsTask) : base(view)
      {
         _globalPKAnalysisTask = globalPKAnalysisTask;
         _globalPKAnalysisDTOMapper = globalPKAnalysisDTOMapper;
         _heavyWorkManager = heavyWorkManager;
         _representationInfoRepository = representationInfoRepository;
         _presentationSettingsTask = presentationSettingsTask;
         _settings = new DefaultPresentationSettings();
      }

      public void CalculatePKAnalysis(IReadOnlyList<Simulation> simulations)
      {
         _simulations = simulations;
         showPKAnalysis();
      }

      private void showPKAnalysis()
      {
         GlobalPKAnalysis = _globalPKAnalysisTask.CalculateGlobalPKAnalysisFor(_simulations);
         updateView();
      }

      public bool ShouldCalculateBioAvailability(string compoundName, string parameterName)
      {
         return shouldCalculateGlobalPKParameter(compoundName, parameterName, CoreConstants.PKAnalysis.Bioavailability);
      }

      public bool ShouldCalculateDDIRatio(string compoundName, string parameterName)
      {
         return shouldCalculateGlobalPKParameter(compoundName, parameterName, CoreConstants.PKAnalysis.AUCRatio) ||
                shouldCalculateGlobalPKParameter(compoundName, parameterName, CoreConstants.PKAnalysis.C_maxRatio);
      }

      private bool shouldCalculateGlobalPKParameter(string compoundName, string parameterName,string globalPKParameterName)
      {
         if (!string.Equals(parameterName, globalPKParameterName))
            return false;

         return !GlobalPKAnalysis.HasParameter(compoundName, parameterName);
      }

      public void CalculateBioAvailability(string compoundName)
      {
         calculateGlobalPKAnalysis(x => x.CalculateBioavailabilityFor(firstSimulation, compoundName));
      }

      public void CalculateDDIRatioFor(string compoundName)
      {
         calculateGlobalPKAnalysis(x => x.CalculateDDIRatioFor(firstSimulation, compoundName));
      }

      private void calculateGlobalPKAnalysis(Action<IGlobalPKAnalysisTask> calculationAction)
      {
         _heavyWorkManager.Start(() => calculationAction(_globalPKAnalysisTask), PKSimConstants.UI.Calculating);
         showPKAnalysis();
      }

      private void updateView()
      {
         loadPreferredUnitsForPKAnalysis();
         _globalPKAnalysisDTO = _globalPKAnalysisDTOMapper.MapFrom(GlobalPKAnalysis);

         _view.BindTo(_globalPKAnalysisDTO);
      }

      private void loadPreferredUnitsForPKAnalysis()
      {
         if (_settings == null)
            return;
         GlobalPKAnalysis.AllPKParameterNames.Each(updateParameterDisplayUnits);
      }

      private void updateParameterDisplayUnits(string parameterName)
      {
         GlobalPKAnalysis.PKParameters(parameterName).Each(parameter =>
         {
            parameter.DisplayUnit = parameter.Dimension.Unit(_settings.GetSetting(parameterName, DisplayUnitFor(parameterName).Name));
         });
      }

      private Simulation firstSimulation => _simulations.FirstOrDefault();

      public void ChangeUnit(string parameterName, Unit newUnit)
      {
         GlobalPKAnalysis.PKParameters(parameterName).Each(p => p.DisplayUnit = newUnit);
         _settings.SetSetting(parameterName, newUnit.Name);
         updateView();
      }

      public Unit DisplayUnitFor(string parameterName)
      {
         return pkParameterNamed(parameterName).DisplayUnit;
      }

      public IEnumerable<Unit> AvailableUnitsFor(string parameterName)
      {
         return pkParameterNamed(parameterName).Dimension.Units;
      }

      private IParameter pkParameterNamed(string parameterName)
      {
         return GlobalPKAnalysis.PKParameters(parameterName).First();
      }

      public string DisplayNameFor(string parameterName)
      {
         var parameter = pkParameterNamed(parameterName);
         var displayName = _representationInfoRepository.DisplayNameFor(parameter);
         return Constants.NameWithUnitFor(displayName, parameter.DisplayUnit);
      }

      public bool HasParameters()
      {
         if (GlobalPKAnalysis != null)
            return GlobalPKAnalysis.AllPKParameters.Any();

         return false;
      }

      public void LoadSettingsForSubject(IWithId subject)
      {
         _settings = _presentationSettingsTask.PresentationSettingsFor<DefaultPresentationSettings>(this, subject);
      }

      public string PresentationKey => PresenterConstants.PresenterKeys.GlobalPKAnalysisPresenter;
   }
}