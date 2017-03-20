using System.Collections.Generic;
using PKSim.Presentation.Views.Diagrams;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Simulations
{
   public static class EditIndividualSimulationItems
   {
      public static EditIndividualSimulationItem<IIndividualSimulationParametersPresenter> Parameters = new EditIndividualSimulationItem<IIndividualSimulationParametersPresenter>();
      public static EditIndividualSimulationItem<IReactionDiagramPresenter> ReactionDiagram = new EditIndividualSimulationItem<IReactionDiagramPresenter>();
      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {Parameters, ReactionDiagram};
   }

   public class EditIndividualSimulationItem<TSimulationItemPresenter> : SubPresenterItem<TSimulationItemPresenter> where TSimulationItemPresenter : IEditIndividualSimulationItemPresenter
   {
   }
}