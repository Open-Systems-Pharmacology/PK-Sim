namespace PKSim.Presentation.DTO.Formulations
{
   public class FormulationTypeDTO
   {
      /// <summary>
      /// Formulation id coming from PKSim database
      /// </summary>
      public string Id { get; set; }

      /// <summary>
      /// Display Name for that formulation type
      /// </summary>
      public string DisplayName { get; set; }

      public override bool Equals(object obj)
      {
         if (ReferenceEquals(null, obj)) return false;
         if (ReferenceEquals(this, obj)) return true;
         if (obj.GetType() != typeof (FormulationTypeDTO)) return false;
         return Equals((FormulationTypeDTO) obj);
      }

      public bool Equals(FormulationTypeDTO other)
      {
         if (ReferenceEquals(null, other)) return false;
         if (ReferenceEquals(this, other)) return true;
         return Equals(other.Id, Id);
      }

      public override int GetHashCode()
      {
         return (Id != null ? Id.GetHashCode() : 0);
      }
   }
}