using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers
{
   public class IdentificationParameterMapper : OSPSuite.Core.Snapshots.Mappers.IdentificationParameterMapper
   {
      public IdentificationParameterMapper(
         ParameterMapper parameterMapper,
         IIdentificationParameterFactory identificationParameterFactory,
         IIdentificationParameterTask identificationParameterTask,
         IOSPSuiteLogger logger
      ) : base(parameterMapper, identificationParameterFactory, identificationParameterTask, logger)
      {
      }

      protected override IModelCoreSimulation SimulationByName(ParameterIdentificationContext parameterIdentificationContext, string simulationName) => 
         parameterIdentificationContext.PKSimProject().All<Model.Simulation>().FindByName(simulationName);

      protected override bool ShouldExportToSnapshot(IParameter parameter) => parameter.ShouldExportToSnapshot();
   }
}