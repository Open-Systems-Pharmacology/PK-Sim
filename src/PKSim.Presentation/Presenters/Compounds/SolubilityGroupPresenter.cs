using System.Collections.Generic;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ISolubilityGroupPresenter : ICompoundParameterGroupPresenter
   {
      void SetSolubilityValue(SolubilityAlternativeDTO solubilityAlternativeDTO, double newValue);
      void SetSolubilityUnit(IParameterDTO solubilityParameter, Unit newUnit);
      void SetRefpHValue(SolubilityAlternativeDTO solubilityAlternativeDTO, double newValue);
      void SetGainPerChargeValue(SolubilityAlternativeDTO solubilityAlternativeDTO, double newValue);
      void UpdateSolubilityChart(SolubilityAlternativeDTO solubilityAlternativeDTO);
      void EditSolubilityTable(SolubilityAlternativeDTO solubilityAlternativeDTO);
   }

   public class SolubilityGroupPresenter : CompoundParameterGroupWithAlternativePresenter<ISolubilityGroupView>, ISolubilityGroupPresenter
   {
      private readonly IParameterGroupAlternativeToSolubilityAlternativeDTOMapper _solubilityAlternativeDTOMapper;
      private readonly ISimpleChartPresenter _simpleChartPresenter;
      private IReadOnlyList<SolubilityAlternativeDTO> _solubilityDTOs;

      public SolubilityGroupPresenter(ISolubilityGroupView view,
         IRepresentationInfoRepository representationRepository,
         ICompoundAlternativeTask compoundAlternativeTask,
         ICompoundAlternativePresentationTask compoundAlternativePresentationTask,
         IParameterGroupAlternativeToSolubilityAlternativeDTOMapper solubilityAlternativeDTOMapper,
         IDialogCreator dialogCreator,
         ISimpleChartPresenter simpleChartPresenter)
         : base(view, representationRepository, compoundAlternativeTask, compoundAlternativePresentationTask, dialogCreator, CoreConstants.Groups.COMPOUND_SOLUBILITY)
      {
         _solubilityAlternativeDTOMapper = solubilityAlternativeDTOMapper;
         _simpleChartPresenter = simpleChartPresenter;
         _view.SetChartView(simpleChartPresenter.View);
         AddSubPresenters(_simpleChartPresenter);
      }

      public void SetSolubilityValue(SolubilityAlternativeDTO solubilityAlternativeDTO, double newValue)
      {
         AddCommand(_compoundAlternativeTask.SetAlternativeParameterValue(solubilityAlternativeDTO.SolubilityParameter.Parameter, newValue));
      }

      public void SetSolubilityUnit(IParameterDTO solubilityParameter, Unit newUnit)
      {
         AddCommand(_compoundAlternativeTask.SetAlternativeParameterUnit(solubilityParameter.Parameter, newUnit));
      }

      public void SetRefpHValue(SolubilityAlternativeDTO solubilityAlternativeDTO, double newValue)
      {
         AddCommand(_compoundAlternativeTask.SetAlternativeParameterValue(solubilityAlternativeDTO.RefpHParameter.Parameter, newValue));
      }

      public void SetGainPerChargeValue(SolubilityAlternativeDTO solubilityAlternativeDTO, double newValue)
      {
         AddCommand(_compoundAlternativeTask.SetAlternativeParameterValue(solubilityAlternativeDTO.GainPerChargeParameter.Parameter, newValue));
      }

      public void UpdateSolubilityChart(SolubilityAlternativeDTO solubilityAlternativeDTO)
      {
         var chart = _simpleChartPresenter.Plot(_compoundAlternativeTask.SolubilityTableForPh(solubilityAlternativeDTO.ParameterAlternative, _compound));
         //log scaling for solubility chart is more appropriate
         chart.AxisBy(AxisTypes.Y).Scaling = Scalings.Log;
      }

      public void EditSolubilityTable(SolubilityAlternativeDTO solubilityAlternativeDTO)
      {
         AddCommand(_compoundAlternativePresentationTask.EditSolubilityTableFor(solubilityAlternativeDTO.SolubilityParameter.Parameter));
      }

      protected override IEnumerable<ParameterAlternativeDTO> FillUpParameterGroupAlternatives()
      {
         _solubilityDTOs = _parameterGroup.AllAlternatives.MapAllUsing(_solubilityAlternativeDTOMapper);
         _view.BindTo(_solubilityDTOs);
         return _solubilityDTOs;
      }
   }
}