using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class ImportPopulation : Population
   {
      /// <summary>
      ///    Population settings for the population
      /// </summary>
      public virtual ImportPopulationSettings Settings { get; private set; }

      public ImportPopulation()
      {
         Settings = new ImportPopulationSettings();
      }

      public override Individual FirstIndividual => Settings?.BaseIndividual;

      public virtual bool ImportSuccessful
      {
         get
         {
            if (!Settings.AllFiles.Any())
               return false;

            return !Settings.AllFiles.Any(x => x.Status.Is(NotificationType.Error));
         }
      }

      public virtual bool ImportHasWarningOrError
      {
         get { return Settings.AllFiles.Any(x => x.Status.Is(NotificationType.Warning | NotificationType.Error)); }
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         var sourcePopulation = sourceObject as ImportPopulation;
         if (sourcePopulation == null) return;
         base.UpdatePropertiesFrom(sourcePopulation, cloneManager);
         Settings = sourcePopulation.Settings.Clone(cloneManager);
      }
   }
}