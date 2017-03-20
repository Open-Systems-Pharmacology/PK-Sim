using PKSim.Core.Model;
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Compounds
{
   public class TypePKaDTO : DxValidatableDTO
   {
      public IParameterDTO PKaParameter { get; set; }
      public IParameterDTO CompoundTypeParameter { get; set; }

      public CompoundType CompoundType
      {
         get { return (CompoundType) CompoundTypeValue; }
         set { CompoundTypeValue = (double) value; }
      }

      public double CompoundTypeValue
      {
         get { return CompoundTypeParameter.Value; }
         set { CompoundTypeParameter.Value = value; }
      }

      public double PKa
      {
         get { return PKaParameter.Value; }
         set { PKaParameter.Value = value; }
      }

      public virtual bool IsFavorite
      {
         get { return PKaParameter.IsFavorite; }
         set
         {
            PKaParameter.IsFavorite = value;
            OnPropertyChanged(() => IsFavorite);
         }
      }
   }
}