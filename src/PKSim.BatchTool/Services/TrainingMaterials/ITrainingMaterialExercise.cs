using System.Threading.Tasks;

namespace PKSim.BatchTool.Services.TrainingMaterials
{
   public interface ITrainingMaterialExercise
   {
      Task Generate(string outputFolder);
   }
}