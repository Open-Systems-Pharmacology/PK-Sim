using OSPSuite.Core.Domain.Services;
using IOutputSchemaFactory = PKSim.Core.Model.IOutputSchemaFactory;
using ModelOutputSchema = OSPSuite.Core.Domain.OutputSchema;

namespace PKSim.Core.Snapshots.Mappers
{
   public class OutputSchemaMapper : OSPSuite.Core.Snapshots.Mappers.OutputSchemaMapper
   {
      private readonly IOutputSchemaFactory _outputSchemaFactory;

      public OutputSchemaMapper(OutputIntervalMapper outputIntervalMapper, IOutputSchemaFactory outputSchemaFactory, IContainerTask containerTask) : base(outputIntervalMapper, containerTask)
      {
         _outputSchemaFactory = outputSchemaFactory;
      }

      protected override ModelOutputSchema CreateEmpty()
      {
         return _outputSchemaFactory.CreateEmpty();
      }
   }
}