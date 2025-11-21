using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.SimModel.Services;
using OSPSuite.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface ISimModelManagerFactory
   {
      ISimModelManager Create();
   }

   public class SimModelManagerFactory : ISimModelManagerFactory
   {
      private readonly ISimModelExporter _simModelExporter;
      private readonly ISimModelSimulationFactory _simModelSimulationFactory;
      private readonly IDisplayUnitRetriever _displayUnitRetriever;
      private readonly IDataRepositoryTask _dataRepositoryTask;
      private readonly IDimensionRepository _dimensionRepository;

      public SimModelManagerFactory(ISimModelExporter simModelExporter, ISimModelSimulationFactory simModelSimulationFactory, IExecutionContext context, IDisplayUnitRetriever displayUnitRetriever, IDataRepositoryTask dataRepositoryTask, IDimensionRepository dimensionRepository)
      {
         _simModelExporter = simModelExporter;
         _simModelSimulationFactory = simModelSimulationFactory;
         _displayUnitRetriever = displayUnitRetriever;
         _dataRepositoryTask = dataRepositoryTask;
         _dimensionRepository = dimensionRepository;
      }

      public ISimModelManager Create()
      {
         return new SimModelManager(_simModelExporter, _simModelSimulationFactory, createDataFactory());
      }

      private DataFactory createDataFactory()
      {
         return new DataFactory(_dimensionRepository.DimensionFactory, _displayUnitRetriever, _dataRepositoryTask);
      }
   }
}
