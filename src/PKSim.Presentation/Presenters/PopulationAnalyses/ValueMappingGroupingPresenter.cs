using System;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IValueMappingGroupingPresenter : IPresenter<IValueMappingGroupingView>, IGroupingDefinitionPresenter
   {
   }

   public class ValueMappingGroupingPresenter : AbstractPresenter<IValueMappingGroupingView, IValueMappingGroupingPresenter>, IValueMappingGroupingPresenter
   {
      private readonly IColorGenerator _colorGenerator;
      private readonly ISymbolGenerator _symbolGenerator;
      private PopulationAnalysisDataField<string> _dataField;
      private readonly NotifyList<GroupingLabelDTO> _mapping;
      private ValueMappingGroupingDefinition _groupingDefinition;

      public ValueMappingGroupingPresenter(IValueMappingGroupingView view, IColorGenerator colorGenerator,ISymbolGenerator symbolGenerator) : base(view)
      {
         _colorGenerator = colorGenerator;
         _symbolGenerator = symbolGenerator;
         _mapping = new NotifyList<GroupingLabelDTO>();
      }

      public GroupingDefinition GroupingDefinition
      {
         get
         {
            UpdateGroupingDefinition();
            return _groupingDefinition;
         }
      }

      public void InitializeWith(IPopulationAnalysisField populationAnalysisField, IPopulationDataCollector populationDataCollector)
      {
         _dataField = populationAnalysisField as PopulationAnalysisDataField<string>;

         //this should never happen
         if (_dataField == null)
            throw new ArgumentException(PKSimConstants.Error.GroupingCannotBeUsedWithFieldOfType(populationAnalysisField.DataType, GroupingDefinitions.ValueMapping.DisplayName));

         _groupingDefinition = new ValueMappingGroupingDefinition(populationAnalysisField.Name);
         var values = _dataField.GetValues(populationDataCollector).Distinct().ToList();
         values.Each(v => _mapping.Add(mapFrom(v)));
      }

      private void bindToView()
      {
         _view.BindTo(_mapping);
      }

      public void Edit(GroupingDefinition groupingDefinition)
      {
         _groupingDefinition = groupingDefinition.DowncastTo<ValueMappingGroupingDefinition>();
         uint sequence = 1;
         foreach (var value in _groupingDefinition.Mapping.Keys)
         {
            var dto = _mapping.FirstOrDefault(x => string.Equals(x.Value, value));
            if (dto == null) continue;
            dto.UpdateFrom(_groupingDefinition.Mapping[value]);
            dto.Sequence = sequence++;
         }
         bindToView();
      }

      public void UpdateGroupingDefinition()
      {
         _groupingDefinition.Mapping.Clear();
         _mapping.OrderBy(x => x.Sequence).Each(m => _groupingDefinition.AddValueLabel(m.Value, m.ToGroupingItem()));
      }

      public void StartCreate()
      {
         bindToView();
      }

      private GroupingLabelDTO mapFrom(string fieldValue)
      {
         return new GroupingLabelDTO
         {
            Value = fieldValue, 
            //+1 to have a user friendly sequence
            Sequence = (_mapping.Count + 1).ConvertedTo<uint>(),
            Color = _colorGenerator.NextColor(),
            Symbol = _symbolGenerator.NextSymbol(),
         };
      }
   }
}