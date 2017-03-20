using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Compounds
{
   public class CompoundAdvancedParameterGroupItems
   {
      private static int _nextIndex;

      public static readonly CompoundAdvancedParameterGroupItem<IParticleDissolutionGroupPresenter> ParticleDissolution = createFor<IParticleDissolutionGroupPresenter>();
      public static readonly CompoundAdvancedParameterGroupItem<ITwoPoreGroupPresenter> TwoPore = createFor<ITwoPoreGroupPresenter>();

      private static CompoundAdvancedParameterGroupItem<TPresenter> createFor<TPresenter>() where TPresenter : ICompoundAdvancedParameterGroupPresenter
      {
         return new CompoundAdvancedParameterGroupItem<TPresenter> {Index = _nextIndex++};
      }

      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {ParticleDissolution, TwoPore};
   }

   public interface ICompoundAdvancedParameterGroupItem : ISubPresenterItem
   {
   }

   public class CompoundAdvancedParameterGroupItem<TParameterGroupPresenter> : SubPresenterItem<TParameterGroupPresenter>, ICompoundAdvancedParameterGroupItem
      where TParameterGroupPresenter : ICompoundAdvancedParameterGroupPresenter
   {
   }
}