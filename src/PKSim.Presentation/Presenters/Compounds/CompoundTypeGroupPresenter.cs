using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICompoundTypeGroupPresenter : ICompoundParameterGroupPresenter,
            IListener<RemoveParameterFromFavoritesEvent>

   {
      void SetPKa(TypePKaDTO typePkaDTO, double newValue);
      void SetCompoundType(TypePKaDTO typePkaDTO, CompoundType newValue);
      IEnumerable<CompoundType> AllCompoundTypes();

      /// <summary>
      ///    Edit the parameters belonging to the MolWeight group
      /// </summary>
      void EditCompoundParameters(IEnumerable<IParameter> compoundParameters);

      /// <summary>
      ///    Specifies if favorites column can be displayed or not. Default is <c>false</c>
      /// </summary>
      bool ShowFavorites { set; }

      void SetFavorite(TypePKaDTO typePkaDTO, bool isFavorite);
   }

   public class CompoundTypeGroupPresenter : CompoundParameterGroupPresenter<ICompoundTypeGroupView>, ICompoundTypeGroupPresenter
   {
      private readonly ICompoundToCompoundTypeDTOMapper _dtoMapper;
      private readonly IParameterTask _parameterTask;
      private readonly IEntityPathResolver _entityPathResolver;
      private CompoundTypeDTO _compoundTypeDTO;
      private PathCache<IParameter> _parameterCache;

      public CompoundTypeGroupPresenter(ICompoundTypeGroupView view, IRepresentationInfoRepository representationInfoRepository,
         ICompoundToCompoundTypeDTOMapper dtoMapper, IParameterTask parameterTask, IEntityPathResolver entityPathResolver)
         : base(view, representationInfoRepository, CoreConstants.Groups.COMPOUND_PKA)
      {
         _dtoMapper = dtoMapper;
         _parameterTask = parameterTask;
         _entityPathResolver = entityPathResolver;
         ShowFavorites = false;
      }

      public void SetPKa(TypePKaDTO typePkaDTO, double newValue)
      {
         AddCommand(_parameterTask.SetParameterDisplayValue(typePkaDTO.PKaParameter.Parameter, newValue));
      }

      public void SetCompoundType(TypePKaDTO typePkaDTO, CompoundType newValue)
      {
         AddCommand(_parameterTask.SetCompoundType(typePkaDTO.CompoundTypeParameter.Parameter, newValue));
      }

      public IEnumerable<CompoundType> AllCompoundTypes()
      {
         return EnumHelper.AllValuesFor<CompoundType>();
      }

      public void EditCompoundParameters(IEnumerable<IParameter> compoundParameters)
      {
         var parameters = compoundParameters.ToList();
         _compoundTypeDTO = _dtoMapper.MapFrom(parameters);
         _parameterCache = new PathCache<IParameter>(_entityPathResolver).For(parameters);
         _view.BindTo(_compoundTypeDTO.AllTypePKas);
      }

      public bool ShowFavorites
      {
         set { _view.ShowFavorites = value; }
      }

      public void SetFavorite(TypePKaDTO typePkaDTO, bool isFavorite)
      {
         typePkaDTO.IsFavorite = isFavorite;
         _parameterTask.SetParameterFavorite(typePkaDTO.PKaParameter.Parameter, isFavorite);
      }

      public override void EditCompound(Compound compound)
      {
         EditCompoundParameters(compound.AllParameters());
      }

      public void Handle(RemoveParameterFromFavoritesEvent eventToHandle)
      {
         var pKaParameter = _parameterCache[eventToHandle.ParameterPath];
         if (pKaParameter == null) return;

         var typePKA = _compoundTypeDTO.AllTypePKas.FirstOrDefault(x => Equals(x.PKaParameter.Parameter, pKaParameter));
         if (typePKA == null) return;
         typePKA.IsFavorite = false;
      }
   }
}