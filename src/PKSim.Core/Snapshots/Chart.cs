using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class Chart : IWithName, IWithDescription
   {
      public string Name { get; set; }
      public string Description { get; set; }
      public ChartFontAndSizeSettings FontAndSize { get; set; }
      public ChartSettings Settings { get; set; }
      public string Title { get; set; }
      public string OriginText { get; set; }
      public bool? IncludeOriginData { get; set; }
      public bool? PreviewSettings { get; set; }

   }
}