using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface IParametersByGroupPresenter : 
      IPresenter<IParametersByGroupView>,
      IBatchUpdatable, 
      IPresenterWithContextMenu<IParameterDTO>,
      IParameterSetPresenter

   {
      void EditParameters(IEnumerable<IParameter> parameters);
      void RefreshParameters();
      int OptimalHeight { get; }

      /// <summary>
      ///    Is the header panel visible?
      ///    Default is false
      /// </summary>
      bool HeaderVisible { set; }
   }

   public class ParametersByGroupPresenter : ParameterSetPresenter<IParametersByGroupView, IParametersByGroupPresenter>, IParametersByGroupPresenter
   {
      private readonly IParameterToParameterDTOMapper _mapper;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IParameterContextMenuFactory _contextMenuFactory;

      public ParametersByGroupPresenter(IParametersByGroupView view, IEditParameterPresenterTask editParameterPresenterTask, IParameterTask parameterTask,
         IParameterToParameterDTOMapper mapper, IRepresentationInfoRepository representationInfoRepository,
         IParameterContextMenuFactory contextMenuFactory)
         : base(view, editParameterPresenterTask, parameterTask)
      {
         _mapper = mapper;
         _representationInfoRepository = representationInfoRepository;
         _contextMenuFactory = contextMenuFactory;
         ShowFavorites = false;
      }

      public void EditParameters(IEnumerable<IParameter> parameters)
      {
         base.Edit(parameters);
         AllParametersDTO.Clear();
         foreach (var parameter in _visibleParameters)
         {
            var parameterDTO = _mapper.MapFrom(parameter).DowncastTo<ParameterDTO>();

            var groupInfo = _representationInfoRepository.InfoFor(RepresentationObjectType.GROUP, parameter.GroupName);
            parameterDTO.PathElements[0] = groupInfo.ToPathElement();
            AllParametersDTO.Add(parameterDTO);
         }

         var allGrouping = AllParametersDTO.Select(x => x.PathElements[0].DisplayName).Distinct();
         _view.GroupingVisible = (allGrouping.Count() > 1);
         _view.BindTo(AllParametersDTO);
      }

      public override bool ShowFavorites
      {
         set => _view.FavoritesVisible = value;
      }

      public override void AddCommand(ICommand command)
      {
         base.AddCommand(command);
         RefreshParameters();
      }

      public void RefreshParameters()
      {
         _view.RefreshData();
      }

      public int OptimalHeight => View.OptimalHeight;

      public bool HeaderVisible
      {
         set => _view.HeaderVisible = value;
      }

      public void BeginUpdate()
      {
         View.BeginUpdate();
      }

      public void EndUpdate()
      {
         View.EndUpdate();
      }

      public bool Updating => View.Updating;

      public void ShowContextMenu(IParameterDTO parameterDTO, Point popupLocation)
      {
         var contextMenu = _contextMenuFactory.CreateFor(parameterDTO, this);
         contextMenu.Show(_view, popupLocation);
      }

      protected override IEnumerable<IParameterDTO> AllVisibleParameterDTOs => AllParametersDTO;

      public override void Edit(IEnumerable<IParameter> parameters)
      {
         EditParameters(parameters);
      }
   }
}