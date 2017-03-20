using System;
using System.IO;
using OSPSuite.Utility.Collections;
using PKSim.Infrastructure.Services;
using OSPSuite.Core.Domain.Data;

namespace PKSim.BatchTool.Services
{
   public interface ITrainingObservedDataRepository
   {
      DataRepository FindByName(string observedDataName);
   }

   public class TrainingObservedDataRepository : ITrainingObservedDataRepository
   {
      private readonly IObservedDataPersistor _dataRepositoryPersistor;
      private readonly Cache<string, DataRepository> _allObservedData;

      public TrainingObservedDataRepository(IObservedDataPersistor dataRepositoryPersistor)
      {
         _dataRepositoryPersistor = dataRepositoryPersistor;
         _allObservedData = new Cache<string, DataRepository>(x => x.Name);
      }

      public DataRepository FindByName(string observedDataName)
      {
         if (_allObservedData.Contains(observedDataName))
            return _allObservedData[observedDataName];

         var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", observedDataName + ".pkml");
         var observedData = _dataRepositoryPersistor.Load(fileName);
         observedData.Name = observedDataName;
         _allObservedData.Add(observedData);
         return observedData;
      }
   }
}