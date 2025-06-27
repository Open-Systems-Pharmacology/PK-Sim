using OSPSuite.Core.Domain;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Model;
using IOutputIntervalFactory = PKSim.Core.Model.IOutputIntervalFactory;

namespace PKSim.Core.Snapshots.Mappers
{
   public class OutputIntervalMapper : OSPSuite.Core.Snapshots.Mappers.OutputIntervalMapper
   {
      private readonly IOutputIntervalFactory _outputIntervalFactory;

      public OutputIntervalMapper(ParameterMapper parameterMapper, IOutputIntervalFactory outputIntervalFactory) : base(parameterMapper)
      {
         _outputIntervalFactory = outputIntervalFactory;
      }

      protected override bool ShouldExportToSnapshot(IParameter parameter)
      {
         
         return base.ShouldExportToSnapshot(parameter) || parameter.ShouldExportToSnapshot();
      }


      protected override OutputInterval CreateDefault()
      {
         return _outputIntervalFactory.CreateDefault();
      }
   }
}