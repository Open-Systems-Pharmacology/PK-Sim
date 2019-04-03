namespace PKSim.Infrastructure.Reporting.Markdown.Elements
{
   public class TitleElement : TitleBaseElement
   {
      public TitleElement(string text) : base(text, 1)
      {
      }
   }

   public class SubTitleElement : TitleBaseElement
   {
      public SubTitleElement(string text) : base(text, 2)
      {
      }
   }
}