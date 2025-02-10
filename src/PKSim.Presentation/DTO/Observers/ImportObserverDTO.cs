using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Observers
{
   public class ImportObserverDTO : ValidatableDTO<ObserverBuilder>, IWithName
   {
      public ObserverBuilder Observer { get; }
      public string FilePath { get; set; }

      public ImportObserverDTO(ObserverBuilder observerBuilder) : base(observerBuilder)
      {
         Observer = observerBuilder;
      }

      public string Name
      {
         get => Observer.Name;
         set => Observer.Name = value;
      }
   }
}