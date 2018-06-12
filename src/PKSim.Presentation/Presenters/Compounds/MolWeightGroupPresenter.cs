using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;

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
      private readonly IEditValueOriginPresenter _editValueOriginPresenter;
      private MolWeightDTO _molWeightDTO;
      private List<IParameter> _molWeightParameters;

      public MolWeightGroupPresenter(IMolWeightGroupView view,
         IRepresentationInfoRepository representationInfoRepository,
         ICompoundToMolWeightDTOMapper molWeightDTOMapper,
         IMolWeightHalogensPresenter molWeightHalogensPresenter,
         IParameterTask parameterTask,
         IEditValueOriginPresenter editValueOriginPresenter) :
         base(view, representationInfoRepository, CoreConstants.Groups.COMPOUND_MW)
      {
         _molWeightDTOMapper = molWeightDTOMapper;
         _molWeightHalogensPresenter = molWeightHalogensPresenter;
         _parameterTask = parameterTask;
         _editValueOriginPresenter = editValueOriginPresenter;
         AddSubPresenters(_editValueOriginPresenter, _molWeightHalogensPresenter);
         _view.SetHalogensView(_molWeightHalogensPresenter.View);
         _view.AddValueOriginView(_editValueOriginPresenter.View);
         _editValueOriginPresenter.ValueOriginUpdated = valueOriginUpdated;
      }

      private void valueOriginUpdated(ValueOrigin valueOrigin)
      {
         //This should update the value origin for all related parameters
         AddCommand(_parameterTask.SetParametersValueOrigin(_molWeightParameters, valueOrigin));
      }

      public override void EditCompound(Compound compound)
      {
         EditCompoundParameters(compound.AllParameters());
      }

      public void EditHalogens()
      {
         _molWeightHalogensPresenter.EditHalogens(_molWeightParameters);
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
         _molWeightParameters = compoundParameters.Where(x => string.Equals(x.GroupName, CoreConstants.Groups.COMPOUND_MW)).ToList();
         _molWeightDTO = _molWeightDTOMapper.MapFrom(_molWeightParameters);
         _view.BindTo(new[] {_molWeightDTO.MolWeightParameter, _molWeightDTO.HasHalogensParameter, _molWeightDTO.MolWeightEffParameter});
         _editValueOriginPresenter.Edit(_molWeightDTO.MolWeightParameter);
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

      public void SetParameterValueOrigin(IParameterDTO parameterDTO, ValueOrigin valueOrigin)
      {
         AddCommand(_parameterTask.SetParameterValueOrigin(parameterDTO.Parameter, valueOrigin));
      }
   }
}