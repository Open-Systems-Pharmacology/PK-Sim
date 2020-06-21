using OSPSuite.Infrastructure.Import.Services;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class PopulationFile : ImportLogger
   {
      /// <summary>
      ///    Number of individual in imported file
      /// </summary>
      public virtual int NumberOfIndividuals { get; set; }

      public virtual PopulationFile Clone()
      {
         return MemberwiseClone() as PopulationFile;
      }

      public override string ToString()
      {
         return PKSimConstants.UI.PopulationFile;
      }
   }
}