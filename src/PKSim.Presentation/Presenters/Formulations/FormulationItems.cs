using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Formulations
{
   public class FormulationItems
   {
      public static readonly FormulationItem<IFormulationSettingsPresenter> Settings = new FormulationItem<IFormulationSettingsPresenter>();
      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {Settings};
   }

   public class FormulationItem<TFormulationItemPresenter> : SubPresenterItem<TFormulationItemPresenter> where TFormulationItemPresenter : IFormulationItemPresenter
   {
   }
}