namespace PKSim.Infrastructure.Reporting.Markdown.Elements
{
   public interface IParameterElement
   {
      string Name { get; set; }
      string Value { get; set; }
      string ValueOrigin { get; set; }
   }

   public class ParameterElement : IParameterElement
   {
      public string Name { get; set; }
      public string Value { get; set; }
      public string ValueOrigin { get; set; }
   }
}