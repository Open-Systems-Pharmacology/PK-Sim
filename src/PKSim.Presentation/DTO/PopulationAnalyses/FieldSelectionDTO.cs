using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Presentation.DTO.PopulationAnalyses
{
   public class FieldSelectionDTO
   {
      public IPopulationAnalysisField PopulationAnalysisField { get; private set; }
      public bool Selected { get; set; }

      public FieldSelectionDTO(IPopulationAnalysisField populationAnalysisField)
      {
         PopulationAnalysisField = populationAnalysisField;
      }

      public string Name
      {
         get { return PopulationAnalysisField.Name; }
      }
   }
}