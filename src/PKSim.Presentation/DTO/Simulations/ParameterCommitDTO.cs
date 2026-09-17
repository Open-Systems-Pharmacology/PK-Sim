using OSPSuite.Presentation.DTO;
using PKSim.Assets;

namespace PKSim.Presentation.DTO.Simulations
{
   public class ParameterCommitDTO : DxValidatableDTO
   {
      public string Path { get; init; }
      public string DisplayPath { get; init; }
      public double Value { get; init; }
      public string Unit { get; init; }
      public string ValueOrigin { get; init; }
      public bool IsRemoval { get; init; }

      /// <summary>
      ///    The parameter is not changed in the simulation and is listed because it is carried over from the set applied
      ///    to the simulation into the new set.
      /// </summary>
      public bool IsUnchanged { get; init; }

      public bool Selected { get; set; } = true;

      public string Change => changeDescription();

      private string changeDescription()
      {
         if (IsRemoval)
            return PKSimConstants.UI.RemoveFromParameterSet;

         return IsUnchanged ? PKSimConstants.UI.UnchangedInParameterSet : PKSimConstants.UI.UpdateValueInParameterSet;
      }
   }
}
