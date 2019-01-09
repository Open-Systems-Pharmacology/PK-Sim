using System.Collections.Generic;
using OSPSuite.Core.Domain;
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
         get => (CompoundType) CompoundTypeValue;
         set => CompoundTypeValue = (double) value;
      }

      public double CompoundTypeValue
      {
         get => CompoundTypeParameter.Value;
         set => CompoundTypeParameter.Value = value;
      }

      public double PKa
      {
         get => PKaParameter.Value;
         set => PKaParameter.Value = value;
      }

      public virtual bool IsFavorite
      {
         get => PKaParameter.IsFavorite;
         set
         {
            PKaParameter.IsFavorite = value;
            OnPropertyChanged(() => IsFavorite);
         }
      }

      public IEnumerable<IParameter> Parameters
      {
         get
         {
            yield return PKaParameter?.Parameter;
            yield return CompoundTypeParameter?.Parameter;
         }

      }
   }
}