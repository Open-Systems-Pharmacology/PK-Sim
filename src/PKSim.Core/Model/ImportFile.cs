using PKSim.Assets;

namespace PKSim.Core.Model
{
   public abstract class ImportFile : ImportLogger
   {
      public virtual string FilePath { get; set; }
   }

   public class PopulationFile : ImportFile
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

   public class SimulationResultsImportFile : ImportFile
   {
      public virtual int NumberOfIndividuals { get; set; }
      public virtual int NumberOfQuantities { get; set; }
   }

   public class PKAnalysesImportFile : ImportFile
   {
   }
}