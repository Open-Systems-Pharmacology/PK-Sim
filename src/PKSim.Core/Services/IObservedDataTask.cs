using System.Threading.Tasks;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core.Services
{
   public interface IObservedDataTask : OSPSuite.Core.Domain.Services.IObservedDataTask
   {
      /// <summary>
      ///    Loads observed data from the template database
      /// </summary>
      Task LoadObservedDataFromTemplateAsync();

      /// <summary>
      ///    Saves the given <paramref name="observedData" /> to the template database
      /// </summary>
      void SaveToTemplate(DataRepository observedData);

      /// <summary>
      ///    Export observed data to pkml format
      /// </summary>
      void ExportToPkml(DataRepository observedData);

      /// <summary>
      /// Loads observed data from snapshot
      /// </summary>
      void LoadFromSnapshot();
   }
}