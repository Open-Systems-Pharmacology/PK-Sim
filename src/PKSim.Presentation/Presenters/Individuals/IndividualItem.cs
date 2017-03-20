using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Individuals
{
   public static class IndividualItems
   {
      public static readonly IndividualItem<IIndividualSettingsPresenter> Settings = new IndividualItem<IIndividualSettingsPresenter>();
      public static readonly IndividualItem<IIndividualParametersPresenter> Parameters = new IndividualItem<IIndividualParametersPresenter>();
      public static readonly IndividualItem<IIndividualMoleculesPresenter> Expression = new IndividualItem<IIndividualMoleculesPresenter>();

      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {Settings, Parameters, Expression};
   }

   public static class ScaleIndividualItems
   {
      public static readonly IndividualItem<IIndividualSettingsPresenter> Settings = new IndividualItem<IIndividualSettingsPresenter>();
      public static readonly IndividualItem<IIndividualScalingConfigurationPresenter> Scaling = new IndividualItem<IIndividualScalingConfigurationPresenter>();
      public static readonly IndividualItem<IIndividualParametersPresenter> Parameters = new IndividualItem<IIndividualParametersPresenter>();
      public static readonly IndividualItem<IIndividualMoleculesPresenter> Expressions = new IndividualItem<IIndividualMoleculesPresenter>();
      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> { Settings,Scaling, Parameters, Expressions };
   }
   public class IndividualItem<TIndividualItemPresenter> : SubPresenterItem<TIndividualItemPresenter> where TIndividualItemPresenter : IIndividualItemPresenter
   {
   }
}