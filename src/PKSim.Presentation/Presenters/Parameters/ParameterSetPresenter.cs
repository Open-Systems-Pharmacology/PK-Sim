using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface IParameterSetPresenter : 
      IEditParameterPresenter,
      ICustomParametersPresenter,
      IListener<AddParameterToFavoritesEvent>,
      IListener<RemoveParameterFromFavoritesEvent>

   {
      /// <summary>
      ///    Resets all the visible parameters
      /// </summary>
      void ResetParameters();

      /// <summary>
      ///    Scales the visible parameters with the provided factor
      /// </summary>
      void ScaleParametersWith(double factor);

      /// <summary>
      ///    Reset the given parameter
      /// </summary>
      void ResetParameter(IParameterDTO parameterDTO);

      /// <summary>
      ///    Returns true if the parameter was set by the user
      /// </summary>
      bool IsSetByUser(IParameterDTO parameterDTO);

      /// <summary>
      ///    Returns true if the parameter is a formula parameter not fixed
      /// </summary>
      bool IsFormulaNotFixed(IParameterDTO parameterDTO);

      /// <summary>
      ///    Edit the table formula defined for the given parameter
      /// </summary>
      void EditTableFor(IParameterDTO tableParameter);

      /// <summary>
      ///    Specifies if the column giving access to the favorites in visible or not
      ///    Default is true
      /// </summary>
      bool ShowFavorites { set; }

      /// <summary>
      ///    Sets the favorite state of the given parameter
      /// </summary>
      /// <param name="parameterDTO">Parameter</param>
      /// <param name="isFavorite">Is the parameter a favorite parameter</param>
      void SetFavorite(IParameterDTO parameterDTO, bool isFavorite);

      /// <summary>
      ///    Returns true if the parameter can be edited otherwise false
      /// </summary>
      bool CanEditParameter(IParameterDTO parameterDTO);


      /// <summary>
      ///    Returns true if the value origin can be edited otherwise false
      /// </summary>
      bool CanEditValueOrigin(IParameterDTO parameterDTO);
   }

   public abstract class ParameterSetPresenter<TView, TPresenter> : EditParameterPresenter<TView, TPresenter>, IParameterSetPresenter
      where TView : IView<TPresenter>
      where TPresenter : IParameterSetPresenter
   {
      protected readonly IParameterTask _parameterTask;
      public virtual string Description { get; set; }
      protected PathCache<IParameter> _pathCache;
      protected List<ParameterDTO> AllParametersDTO { get; } = new List<ParameterDTO>();
      protected IReadOnlyList<IParameter> _visibleParameters;
      public virtual bool AlwaysRefresh { get; } = false;

      protected ParameterSetPresenter(TView view, IEditParameterPresenterTask editParameterTask, IParameterTask parameterTask)
         : base(view, editParameterTask)
      {
         _parameterTask = parameterTask;
      }

      public abstract bool ShowFavorites { set; }

      public bool CanEditParameter(IParameterDTO parameterDTO)
      {
         if (parameterDTO.Parameter.Editable)
            return true;

         //table parameter are always editable
         return parameterDTO.FormulaType == FormulaType.Table;
      }

      public virtual bool CanEditValueOrigin(IParameterDTO parameterDTO)
      {
         if (!CanEditParameter(parameterDTO))
            return false;

         return parameterIsSingleInstance(parameterDTO);
      }

      private bool parameterIsSingleInstance(IParameterDTO parameterDTO)
      {
         if (!parameterDTO.Parameter.BuildingBlockType.IsOneOf(PKSimBuildingBlockType.Compound, PKSimBuildingBlockType.Protocol))
            return true;

         return !parameterDTO.NameIsOneOf(CoreConstants.Parameters.AllParametersWithLockedValueOriginInSimulation);
      }

      protected abstract IEnumerable<IParameterDTO> AllVisibleParameterDTOs { get; }

      public virtual IEnumerable<IParameter> AllVisibleParameters => ParametersFrom(AllVisibleParameterDTOs);

      public void ResetParameters()
      {
         AddCommand(_parameterTask.ResetParameters(AllVisibleParameters));
      }

      public void ScaleParametersWith(double factor)
      {
         AddCommand(_parameterTask.ScaleParameters(AllVisibleParameters, factor));
      }

      public void ResetParameter(IParameterDTO parameterDTO)
      {
         AddCommand(_parameterTask.ResetParameter(ParameterFrom(parameterDTO)));
      }

      public bool IsSetByUser(IParameterDTO parameterDTO)
      {
         if (parameterDTO.Parameter == null)
            return false;

         return parameterDTO.Parameter.ValueDiffersFromDefault();
      }

      public bool IsFormulaNotFixed(IParameterDTO parameterDTO)
      {
         if (parameterDTO.Parameter == null) return false;
         if (parameterDTO.FormulaType != FormulaType.Rate) return false;
         return !parameterDTO.Parameter.IsFixedValue;
      }

      public virtual void Edit(IEnumerable<IParameter> parameters)
      {
         _visibleParameters = parameters.Where(shouldShowParameter).ToList();
         _pathCache = _parameterTask.PathCacheFor(_visibleParameters);
      }

      private bool shouldShowParameter(IParameter parameter)
      {
         return parameter.Visible ||
                parameter.IsFixedValue ||
                !parameter.IsDefault;
      }

      public virtual bool ForcesDisplay => false;

      public IEnumerable<IParameter> EditedParameters => _visibleParameters;

      public virtual void Handle(AddParameterToFavoritesEvent eventToHandle)
      {
         parameterFrom(eventToHandle.ParameterPath).IsFavorite = true;
      }

      public virtual void Handle(RemoveParameterFromFavoritesEvent eventToHandle)
      {
         parameterFrom(eventToHandle.ParameterPath).IsFavorite = false;
      }

      private IParameterDTO parameterFrom(string parameterPath)
      {
         if (_pathCache == null)
            return new NullParameterDTO();

         var parameter = _pathCache[parameterPath];
         if (parameter == null)
            return new NullParameterDTO();

         return AllParametersDTO.First(x => Equals(x.Parameter, parameter));
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         AllParametersDTO.Each(x => x.Release());
         AllParametersDTO.Clear();
      }
   }
}