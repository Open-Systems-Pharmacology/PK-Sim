using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IHalogensPresenter : IMultiParameterEditPresenter
   {
      void Edit(IReadOnlyList<IParameter> halogens, IParameter effectiveMolWeight);
   }

   public class HalogensPresenter : MultiParameterEditPresenter, IHalogensPresenter
   {
      private IReadOnlyList<IParameter> _halogens;
      private IParameter _effectiveMolWeight;

      public HalogensPresenter(
         IMultiParameterEditView view,
         IScaleParametersPresenter scaleParametersPresenter,
         IEditParameterPresenterTask editParameterPresenterTask,
         IParameterTask parameterTask,
         IParameterToParameterDTOMapper parameterDTOMapper,
         IParameterContextMenuFactory contextMenuFactory) :
         base(view, scaleParametersPresenter, editParameterPresenterTask, parameterTask, parameterDTOMapper, contextMenuFactory)
      {
      }

      protected override IParameterDTO DTOFrom(IParameter x)
      {
         return _parameterDTOMapper.MapMolWeightDTOFrom(x, _effectiveMolWeight);
      }

      public void Edit(IReadOnlyList<IParameter> halogens, IParameter effectiveMolWeight)
      {
         _halogens = halogens;
         _effectiveMolWeight = effectiveMolWeight;
         base.Edit(_halogens);
      }
   }
}