using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Observers
{
   public class ImportObserverDTO : ValidatableDTO<IObserverBuilder>, IWithName
   {
      public IObserverBuilder Observer { get; }
      public string FilePath { get; set; }

      public ImportObserverDTO(IObserverBuilder observerBuilder) : base(observerBuilder)
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