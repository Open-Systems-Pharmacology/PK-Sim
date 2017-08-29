using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core.Services
{
   public interface IObservedDataTask : OSPSuite.Core.Domain.Services.IObservedDataTask
   {
      /// <summary>
      ///    Add the given observed data repository to the analysable. Curves will not be shown
      /// </summary>
      void AddObservedDataToAnalysable(IReadOnlyList<DataRepository> observedData, IAnalysable analysable);

      /// <summary>
      ///    Add the given observed data repository to the simulation. Curves be shown if the showData flat is set to true
      /// </summary>
      void AddObservedDataToAnalysable(IReadOnlyList<DataRepository> observedData, IAnalysable analysable, bool showData);

      void RemoveUsedObservedDataFromSimulation(IReadOnlyList<UsedObservedData> observedDataList);

      /// <summary>
      ///    Loads observed data from the template database
      /// </summary>
      void LoadObservedDataFromTemplate();

      /// <summary>
      ///    Saves the given <paramref name="observedData" /> to the template database
      /// </summary>
      void SaveToTemplate(DataRepository observedData);

      /// <summary>
      ///    Export observed data to pkml format
      /// </summary>
      void ExportToPkml(DataRepository observedData);

      Task LoadFromSnapshot();
   }
}