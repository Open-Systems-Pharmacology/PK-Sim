using System.Collections.Generic;
using OSPSuite.Utility;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IIndividualSettingsView : IView<IIndividualSettingsPresenter>,  ILatchable, IBatchUpdatable
   {
      void BindToSettings(IndividualSettingsDTO individualSettingsDTO);
      void BindToParameters(IndividualSettingsDTO individualSettingsDTO);
      void BindToSubPopulation(IEnumerable<CategoryParameterValueVersionDTO> subPopulation);
      void RefreshAllIndividualList();
      bool AgeVisible { get; set; }
      bool HeightAndBMIVisible { get; set; }
      bool IsReadOnly { get; set; }
      bool SpeciesVisible { get; set; }
      bool GestationalAgeVisible { get; set; }
      void AddValueOriginView(IView view);
   }
}