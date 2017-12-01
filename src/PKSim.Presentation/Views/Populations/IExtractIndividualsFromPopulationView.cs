using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Presenters.Populations;

namespace PKSim.Presentation.Views.Populations
{
   public interface IExtractIndividualsFromPopulationView : IModalView<IExtractIndividualsFromPopulationPresenter>
   {
      void BindTo(ExtractIndividualsDTO extractIndividualDTO);
      string PopulationDescription { set; }
      void UpdateGeneratedOutputDescription(int numberOfIndividuals, IReadOnlyList<string> individualNames, string populationName);
   }
}