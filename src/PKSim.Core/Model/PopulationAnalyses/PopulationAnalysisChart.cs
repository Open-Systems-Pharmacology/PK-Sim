using System.Collections.Generic;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public abstract class PopulationAnalysisChart : ISimulationAnalysis, IUpdatable, IWithObservedData, IVisitable<IVisitor>, IChart
   {
      protected List<AxisSettings> _secondaryYAxisSettings;
      public string Name { get; set; }
      public string Id { get; set; }
      public string Description { get; set; }
      public ChartSettings ChartSettings { get; private set; }
      public IAnalysable Analysable { get; set; }
      public abstract PopulationAnalysisType AnalysisType { get; }
      public ObservedDataCollection ObservedDataCollection { get; private set; }
      public string Title { set; get; }
      public ChartFontAndSizeSettings FontAndSize { get; private set; }
      public bool IncludeOriginData { get; set; }
      public bool PreviewSettings { get; set; }
      public string OriginText { get; set; }
      public AxisSettings PrimaryXAxisSettings { set; get; }
      public AxisSettings PrimaryYAxisSettings { set; get; }

      public IReadOnlyList<AxisSettings> SecondaryYAxisSettings => _secondaryYAxisSettings;

      protected PopulationAnalysisChart()
      {
         Id = ShortGuid.NewGuid();
         ChartSettings = new ChartSettings();
         ObservedDataCollection = new ObservedDataCollection();
         Name = string.Empty;
         Title = string.Empty;
         FontAndSize = new ChartFontAndSizeSettings();
         _secondaryYAxisSettings = new List<AxisSettings>();
         PrimaryXAxisSettings = new AxisSettings();
         PrimaryYAxisSettings = new AxisSettings();
      }

      public virtual void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         var sourceAnalysis = source as PopulationAnalysisChart;
         if (sourceAnalysis == null) return;
         Name = sourceAnalysis.Name;
         Description = sourceAnalysis.Description;
         ChartSettings = cloneManager.Clone(sourceAnalysis.ChartSettings);
         ObservedDataCollection = cloneManager.Clone(sourceAnalysis.ObservedDataCollection);
         Title = sourceAnalysis.Title;
         FontAndSize.UpdatePropertiesFrom(sourceAnalysis.FontAndSize);
         IncludeOriginData = sourceAnalysis.IncludeOriginData;
         OriginText = sourceAnalysis.OriginText;

         PrimaryXAxisSettings.UpdatePropertiesFrom(sourceAnalysis.PrimaryXAxisSettings, cloneManager);
         PrimaryYAxisSettings.UpdatePropertiesFrom(sourceAnalysis.PrimaryYAxisSettings, cloneManager);

         sourceAnalysis.SecondaryYAxisSettings.Each((axisSettings, index) => { AxisSettingsForSecondaryYAxis(index).UpdatePropertiesFrom(axisSettings, cloneManager); });
      }

      public void AddObservedData(DataRepository dataRepository)
      {
         ObservedDataCollection.AddObservedData(dataRepository);
      }

      public IEnumerable<DataRepository> AllObservedData()
      {
         return ObservedDataCollection.AllObservedData();
      }

      public bool UsesObservedData(DataRepository dataRepository)
      {
         return ObservedDataCollection.UsesObservedData(dataRepository);
      }

      public void RemoveObservedData(DataRepository dataRepository)
      {
         ObservedDataCollection.RemoveObservedData(dataRepository);
      }

      public CurveOptions CurveOptionsFor(DataColumn dataColumn)
      {
         return ObservedDataCollection.ObservedDataCurveOptionsFor(dataColumn).CurveOptions;
      }

      public abstract void AcceptVisitor(IVisitor visitor);

      public AxisSettings AxisSettingsForSecondaryYAxis(int secondaryAxisIndex)
      {
         while (_secondaryYAxisSettings.Count <= secondaryAxisIndex)
            _secondaryYAxisSettings.Add(new AxisSettings());

         return _secondaryYAxisSettings[secondaryAxisIndex];
      }

      public void AddSecondaryAxis(AxisSettings secondaryAxisSettings)
      {
         _secondaryYAxisSettings.Add(secondaryAxisSettings);
      }

      public abstract PopulationAnalysis BasePopulationAnalysis { get; }
   }

   public abstract class PopulationAnalysisChart<TPopulationAnalysis> : PopulationAnalysisChart where TPopulationAnalysis : PopulationAnalysis
   {
      public TPopulationAnalysis PopulationAnalysis { get; set; }
      public override PopulationAnalysis BasePopulationAnalysis => PopulationAnalysis;

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var analysis = source as PopulationAnalysisChart<TPopulationAnalysis>;
         if (analysis == null) return;
         PopulationAnalysis = cloneManager.Clone(analysis.PopulationAnalysis);
      }

      public override void AcceptVisitor(IVisitor visitor)
      {
         visitor.Visit(this);
         PopulationAnalysis.AcceptVisitor(visitor);
      }
   }

   public class BoxWhiskerAnalysisChart : PopulationAnalysisChart<PopulationBoxWhiskerAnalysis>
   {
      public override PopulationAnalysisType AnalysisType => PopulationAnalysisType.BoxWhisker;
   }

   public class ScatterAnalysisChart : PopulationAnalysisChart<PopulationPivotAnalysis>
   {
      public override PopulationAnalysisType AnalysisType => PopulationAnalysisType.Scatter;
   }

   public class RangeAnalysisChart : PopulationAnalysisChart<PopulationPivotAnalysis>
   {
      public override PopulationAnalysisType AnalysisType => PopulationAnalysisType.Range;
   }

   public class TimeProfileAnalysisChart : PopulationAnalysisChart<PopulationStatisticalAnalysis>
   {
      public override PopulationAnalysisType AnalysisType => PopulationAnalysisType.TimeProfile;
   }
}