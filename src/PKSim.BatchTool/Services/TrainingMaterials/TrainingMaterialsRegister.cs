using OSPSuite.Utility.Container;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using OSPSuite.Core;
using OSPSuite.Core.Domain.Data;

namespace PKSim.BatchTool.Services.TrainingMaterials
{
   public class TrainingMaterialsRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.Register<TrainingMaterialTask, TrainingMaterialTask>();

         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<TrainingMaterialsRegister>();
            scan.IncludeNamespaceContainingType<TrainingMaterialsRegister>();
            scan.WithConvention<RegisterTypeConvention<ITrainingMaterialExercise>>();
         });
      }
   }
}