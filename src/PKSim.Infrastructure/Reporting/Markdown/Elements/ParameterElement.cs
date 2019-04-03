using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Reporting.Markdown.Elements
{
   public interface IParameterElement
   {
      string Name { get; set; }
      string Value { get; set; }
      ValueOrigin ValueOrigin { get; set; }
   }

   public class ParameterElement : IParameterElement
   {
      public string Name { get; set; }
      public string Value { get; set; }
      public ValueOrigin ValueOrigin { get; set; }
   }
}