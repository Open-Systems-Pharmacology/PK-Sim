using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IMolWeightGroupPresenter : ICompoundParameterGroupPresenter, IEditParameterPresenter
   {
      /// <summary>
      ///    Edit the halogens for the given mol weight group
      /// </summary>
      void EditHalogens();

      /// <summary>
      ///    Save the halogens being edited (happens if popup was closed and edit still has focus)
      /// </summary>
      void SaveHalogens();

      /// <summary>
      ///    Returns true if the parameter should have an advanced editor, for instance to display halogens otherwise false
      /// </summary>
      bool IsHalogens(IParameterDTO parameter);

      /// <summary>
      ///    Edit the parameters belonging to the MolWeight group
      /// </summary>
      void EditCompoundParameters(IEnumerable<IParameter> compoundParameters);

      /// <summary>
      ///    Returns true if the parameter is the molweight eff parameters otherwise false
      /// </summary>
      bool IsMolWeightEff(IParameterDTO parameter);
   }

   public class MolWeightGroupPresenter : CompoundParameterGroupPresenter<IMolWeightGroupView>, IMolWeightGroupPresenter
   {
      private readonly ICompoundToMolWeightDTOMapper _molWeightDTOMapper;
      private readonly IMolWeightHalogensPresenter _molWeightHalogensPresenter;
      private readonly IParameterTask _parameterTask;
      private MolWeightDTO _molWeightDTO;
      private IEnumerable<IParameter> _compoundParameters;

      public MolWeightGroupPresenter(IMolWeightGroupView view, IRepresentationInfoRepository representationInfoRepository, ICompoundToMolWeightDTOMapper molWeightDTOMapper,
         IMolWeightHalogensPresenter molWeightHalogensPresenter, IParameterTask parameterTask) :
            base(view, representationInfoRepository, CoreConstants.Groups.COMPOUND_MW)
      {
         _molWeightDTOMapper = molWeightDTOMapper;
         _molWeightHalogensPresenter = molWeightHalogensPresenter;
         _parameterTask = parameterTask;
         _view.SetHalogensView(_molWeightHalogensPresenter.View);
         _molWeightHalogensPresenter.StatusChanged += OnStatusChanged;
      }

      public override void EditCompound(Compound compound)
      {
         EditCompoundParameters(compound.AllParameters());
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _molWeightHalogensPresenter.InitializeWith(commandCollector);
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _molWeightHalogensPresenter.ReleaseFrom(eventPublisher);
         _molWeightHalogensPresenter.StatusChanged -= OnStatusChanged;
      }

      public void EditHalogens()
      {
         _molWeightHalogensPresenter.EditHalogens(_compoundParameters);
      }

      public void SaveHalogens()
      {
         _molWeightHalogensPresenter.SaveHalogens();
      }

      public bool IsHalogens(IParameterDTO parameter)
      {
         return Equals(parameter, _molWeightDTO.HasHalogensParameter);
      }

      public void EditCompoundParameters(IEnumerable<IParameter> compoundParameters)
      {
         _compoundParameters = compoundParameters.ToList();
         _molWeightDTO = _molWeightDTOMapper.MapFrom(_compoundParameters);
         _view.BindTo(new[] {_molWeightDTO.MolWeightParameter, _molWeightDTO.HasHalogensParameter, _molWeightDTO.MolWeightEffParameter});
      }

      public bool IsMolWeightEff(IParameterDTO parameter)
      {
         return parameter == molWeightEffParameter;
      }

      protected override void OnStatusChanged(object sender, EventArgs e)
      {
         base.OnStatusChanged(sender, e);
         _view.RefreshData();
      }

      private IParameterDTO molWeightEffParameter => _molWeightDTO.MolWeightEffParameter;

      public void SetParameterValue(IParameterDTO parameterDTO, double valueInGuiUnit)
      {
         if (parameterDTO == molWeightEffParameter) return;
         AddCommand(_parameterTask.SetParameterDisplayValue(parameterDTO.Parameter, valueInGuiUnit));
      }

      public void SetParameterUnit(IParameterDTO parameterDTO, Unit displayUnit)
      {
         AddCommand(_parameterTask.SetParameterUnit(parameterDTO.Parameter, displayUnit));
      }

      public void SetParameterPercentile(IParameterDTO parameterDTO, double percentileInPercent)
      {
         AddCommand(_parameterTask.SetParameterPercentile(parameterDTO.Parameter, percentileInPercent));
      }

      public void SetParameterValueDescription(IParameterDTO parameterDTO, string valueDescription)
      {
         AddCommand(_parameterTask.SetParameterValueDescription(parameterDTO.Parameter, valueDescription));
      }

      public void SetMolWeightValue(MolWeightDTO molWeightDTO, double newMolWeightValue)
      {
         AddCommand(_parameterTask.SetParameterDisplayValue(molWeightDTO.MolWeightParameter.Parameter, newMolWeightValue));
      }
   }
}