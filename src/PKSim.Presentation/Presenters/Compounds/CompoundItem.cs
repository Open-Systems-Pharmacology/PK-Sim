using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Compounds
{
   public class CompoundItems
   {
      public static CompoundItem<ICompoundParametersPresenter> Parameters = new CompoundItem<ICompoundParametersPresenter>();
      public static CompoundItem<ICompoundProcessesPresenter> Processes = new CompoundItem<ICompoundProcessesPresenter>();
      public static CompoundItem<ICompoundAdvancedParametersPresenter> AdvancedParameters = new CompoundItem<ICompoundAdvancedParametersPresenter>();

      public class CompoundItem<TCompoundItemPresenter> : SubPresenterItem<TCompoundItemPresenter> where TCompoundItemPresenter : ICompoundItemPresenter
      {
      }

      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {Parameters, Processes, AdvancedParameters};
   }
}