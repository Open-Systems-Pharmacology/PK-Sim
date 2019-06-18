using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class ObserverSetMapping
   {
      /// <summary>
      ///    Id of template observer set used in mapping
      /// </summary>
      public string TemplateObserverSetId { get; set; }

      public ObserverSetMapping Clone(ICloneManager cloneManager)
      {
         return new ObserverSetMapping {TemplateObserverSetId = TemplateObserverSetId};
      }
   }
}