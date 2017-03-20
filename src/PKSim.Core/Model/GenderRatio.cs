using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class GenderRatio
   {
      public Gender Gender { get; set; }
      public int Ratio { get; set; }

      public GenderRatio Clone()
      {
         return new GenderRatio {Gender = Gender, Ratio = Ratio};
      }

      public override string ToString()
      {
         return Gender != null ? PKSimConstants.UI.GenderRationFor(Gender.DisplayName) : PKSimConstants.UI.GenderRatio;
      }
   }
}