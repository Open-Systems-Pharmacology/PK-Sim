using System;
using System.Collections.Generic;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Presentation.Views.ProteinExpression;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.ProteinExpression
{
   public interface IExpressionDataPresenter : IExpressionItemPresenter
   {
      void EditMapping();
      event Action OnEditMapping;

      void SetData(string proteinName, IReadOnlyList<ContainerExpressionDataRecord> containerRecords);

      /// <summary>
      ///    Restores the layout of the view. Returns the criteria of the layout filter that could not be restored, or an empty
      ///    string
      /// </summary>
      string SetLayoutSetting(string layoutSettings);

      string GetLayoutSetting();
      void ActualizeData(IReadOnlyList<ContainerExpressionDataRecord> containerRecords);

      /// <summary>
      ///    Filter edited in the view
      /// </summary>
      ExpressionDataFilter Filter { get; set; }
   }

   public class ExpressionDataPresenter : AbstractSubPresenter<IExpressionDataView, IExpressionDataPresenter>, IExpressionDataPresenter
   {
      private readonly IExpressionDataTableMapper _expressionDataTableMapper;
      public event Action OnEditMapping = delegate { };

      public ExpressionDataPresenter(IExpressionDataView view, IExpressionDataTableMapper expressionDataTableMapper) : base(view)
      {
         _expressionDataTableMapper = expressionDataTableMapper;
      }

      public void EditMapping()
      {
         OnEditMapping();
      }

      public void SetData(string proteinName, IReadOnlyList<ContainerExpressionDataRecord> containerRecords) =>
         View.SetData(proteinName, _expressionDataTableMapper.DataTableFrom(containerRecords));

      public string SetLayoutSetting(string layoutSettings) => View.SetLayoutSettings(layoutSettings);

      public string GetLayoutSetting() => View.GetLayoutSettings();

      public void ActualizeData(IReadOnlyList<ContainerExpressionDataRecord> containerRecords) =>
         View.ActualizeData(_expressionDataTableMapper.DataTableFrom(containerRecords));

      public ExpressionDataFilter Filter
      {
         get => View.GetFilter();
         set => View.SetFilter(value);
      }
   }
}
