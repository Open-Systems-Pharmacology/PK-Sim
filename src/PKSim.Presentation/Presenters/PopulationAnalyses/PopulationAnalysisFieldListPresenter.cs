using System;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisFieldListPresenter : IPresenter<IPopulationAnalysisFieldListView>, IPopulationAnalysisPresenter
   {
      PivotArea Area { get; set; }
      Type AllowedType { get; set; }
      string DragAndDropMessage { get; }
      int? MaximNumberOfAllowedFields { get; set; }
      void UpdateDescription(string description);
      void RefreshAnalysis();

      /// <summary>
      ///    Events is fired whenever fields are dropped onto the presenter
      /// </summary>
      event EventHandler<FieldsMovedEventArgs> FieldsMoved;
   }

   public class PopulationAnalysisFieldListPresenter : AbstractPresenter<IPopulationAnalysisFieldListView, IPopulationAnalysisFieldListPresenter>, IPopulationAnalysisFieldListPresenter
   {
      private readonly IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper _mapper;
      private NotifyList<PopulationAnalysisFieldDTO> _allFields;
      private PopulationPivotAnalysis _populationAnalysis;
      private readonly IPopulationAnalysisFieldsDragDropBinder _dragDropBinder;
      public event EventHandler<FieldsMovedEventArgs> FieldsMoved = delegate { };
      public Type AllowedType { get; set; }

      public PopulationAnalysisFieldListPresenter(IPopulationAnalysisFieldListView view, IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper mapper)
         : base(view)
      {
         _mapper = mapper;
         AllowedType = typeof (IStringValueField);
         _dragDropBinder = view.DragDropBinder;
         _dragDropBinder.FieldsDropped += fieldsMoved;
         Area = PivotArea.FilterArea;
      }

      private void fieldsMoved(object sender, FieldsMovedEventArgs e)
      {
         FieldsMoved(this, e);
      }

      public PivotArea Area
      {
         get => _dragDropBinder.Area;
         set => _dragDropBinder.Area = value;
      }

      public int? MaximNumberOfAllowedFields
      {
         get => _dragDropBinder.MaximNumberOfAllowedFields;
         set => _dragDropBinder.MaximNumberOfAllowedFields = value;
      }

      public void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         _populationAnalysis = populationAnalysis.DowncastTo<PopulationPivotAnalysis>();
      }

      public string DragAndDropMessage
      {
         get
         {
            if (Area.Is(PivotArea.FilterArea))
               return PKSimConstants.UI.FilterAreaDragFieldMessage();

            return PKSimConstants.UI.DragFieldMessage(View.Description);
         }
      }

      public void RefreshAnalysis()
      {
         _allFields = new NotifyList<PopulationAnalysisFieldDTO>(_populationAnalysis.AllFieldsOn(Area, AllowedType).MapAllUsing(_mapper));
         _view.BindTo(_allFields);
         updateFieldPositionIndex();
      }

      private void updateFieldPositionIndex()
      {
         if (_populationAnalysis == null) return;
         _allFields.Each((dto, i) => _populationAnalysis.SetPosition(dto.Field, Area, i));
      }

      public void UpdateDescription(string description)
      {
         View.Description = description;
      }
   }
}