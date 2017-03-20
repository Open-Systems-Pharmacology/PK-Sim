using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using OSPSuite.Core.Chart;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.TeXReporting.TeX.Converter;
using OSPSuite.TeXReporting.TeX.PGFPlots;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.Reporting.TeX.Items;
using DistributionSettings = PKSim.Core.Chart.DistributionSettings;
using Plot = OSPSuite.TeXReporting.TeX.PGFPlots.Plot;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class HistogramTeXBuilder : OSPSuiteTeXBuilder<Histogram>
   {
      private readonly ITeXBuilderRepository _builderRepository;
      private readonly IDistributionDataCreator _distributionDataCreator;

      public HistogramTeXBuilder(ITeXBuilderRepository builderRepository, IDistributionDataCreator distributionDataCreator)
      {
         _builderRepository = builderRepository;
         _distributionDataCreator = distributionDataCreator;
      }

      public override void Build(Histogram histogram, OSPSuiteTracker buildTracker)
      {
         var distributionData = _distributionDataCreator.CreateFor(histogram.ParameterContainer, histogram.ParameterDistributionSettings);

         var colors = new List<Color>() {PKSimColors.Male, PKSimColors.Female};
         var settings = histogram.ParameterDistributionSettings.Settings;
         var axisOptions = getAxisOptions(settings, distributionData);
         var barPlotOptions = getBarPlotOptions(settings, distributionData);
         var plots = getPlots(distributionData);
         var caption = new Text("{0} of {1}", PKSimConstants.UI.Distribution, settings.PlotCaption ?? string.Empty);

         var histogramPlot = new BarPlot(colors, axisOptions, barPlotOptions, plots, caption) {Position = FigureWriter.FigurePositions.H};
         _builderRepository.Report(histogramPlot, buildTracker);
      }

      private bool isSingleValueDistribution(ContinuousDistributionData data)
      {
         var groups = getGroups(data);
         foreach (DataRow group in groups.Rows)
         {
            var value = group[data.GroupingName];
            var coordinates = getCoordinates(data, value);
            if (coordinates.Max(x => x.X) != coordinates.Min(x => x.X))
               return false;
         }
         return true;
      }

      private float getSingleValueDistributionValue(ContinuousDistributionData data)
      {
         var groups = getGroups(data);
         foreach (DataRow group in groups.Rows)
         {
            var value = group[data.GroupingName];
            var coordinates = getCoordinates(data, value);
            return coordinates.First().X;
         }
         return float.NaN;
      }

      private AxisOptions getAxisOptions(DistributionSettings settings, ContinuousDistributionData data)
      {
         var axisOptions = new AxisOptions(DefaultConverter.Instance)
                              {
                                 XLabel = settings.XAxisTitle,
                                 YLabel = settings.YAxisTitle,
                                 Title = settings.PlotCaption,
                                 EnlargeLimits = false,
                                 YMin = 0,
                                 YMajorGrid = true,
                                 LegendOptions = new LegendOptions {LegendPosition = LegendOptions.LegendPositions.NorthEast}
                              };

         if (isSingleValueDistribution(data))
         {
            var value = getSingleValueDistributionValue(data);
            if (!float.IsNaN(value))
            {
               axisOptions.XMin = value - (float)data.BarWidth;
               axisOptions.XMax = value + (float)data.BarWidth;
               axisOptions.XTicks = new [] {value};
            }
         }

         return axisOptions;
      }

      private BarPlotOptions getBarPlotOptions(DistributionSettings settings, ContinuousDistributionData data)
      {
         var width = data.BarWidth;
         var shift = Helper.Length(2, Helper.MeasurementUnits.pt);
         if (settings.BarType == BarType.SideBySide)
         {
            width /= (double)getGroups(data).Rows.Count;
         }
         else
         {
            shift = string.Empty;
         }

         return new BarPlotOptions
         {
            BarPlotType = getBarPlotType(settings.BarType),
            Width = width.ToString(CultureInfo.InvariantCulture),
            Shift = shift
         };
      }

      private static BarPlotOptions.BarPlotTypes getBarPlotType(BarType barType)
      {
         switch (barType)
         {
            case BarType.SideBySide:
               return BarPlotOptions.BarPlotTypes.SideBySide;
            case BarType.Stacked:
               return BarPlotOptions.BarPlotTypes.Stacked;
         }
         return BarPlotOptions.BarPlotTypes.SideBySide;
      }

      private DataTable getGroups(ContinuousDistributionData data)
      {
         return data.DataTable.DefaultView.ToTable(true, data.GroupingName);
      }

      private List<Plot> getPlots(ContinuousDistributionData data)
      {
         var plots = new List<Plot>();

         var groups = getGroups(data);
         foreach (DataRow group in groups.Rows)
         {
            var value = group[data.GroupingName];
            var coordinates = getCoordinates(data, value);
            var color = PKSimColors.Male;
            if (value.ToString().ToUpper() == "FEMALE")
            {
               color = PKSimColors.Female;
            }

            plots.Add(new Plot(coordinates, new PlotOptions
                                               {
                                                  PlotType = PlotOptions.PlotTypes.BarPlot,
                                                  Color = color.Name,
                                                  FillColor = string.Format("{0}!{1}", color.Name, 80),
                                                  LineStyle = PlotOptions.LineStyles.Solid
                                               }) {LegendEntry = value.ToString()});
         }

         return plots;
      }

      private static List<Coordinate> getCoordinates(ContinuousDistributionData data, object value)
      {
         var dv = data.DataTable.Copy().DefaultView;
         dv.RowFilter = string.Format("[{0}] = '{1}'", data.GroupingName, value);
         var coordinates = new List<Coordinate>();
         foreach (DataRowView point in dv)
         {
            float xValue = float.Parse(point[data.XAxisName].ToString());
            float yValue = float.Parse(point[data.YAxisName].ToString());
            coordinates.Add(new Coordinate(xValue, yValue));
         }
         return coordinates;
      }
   }
}