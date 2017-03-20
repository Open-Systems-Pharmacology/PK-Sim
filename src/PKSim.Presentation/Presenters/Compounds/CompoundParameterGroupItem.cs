using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Compounds
{
   public static class CompoundParameterGroupItems
   {
      public static readonly CompoundParameterGroupItem<ICompoundMoleculeTypePresenter> MoleculeType = new CompoundParameterGroupItem<ICompoundMoleculeTypePresenter>();
      public static readonly CompoundParameterGroupItem<ILipophilicityGroupPresenter> Lipophilicity = new CompoundParameterGroupItem<ILipophilicityGroupPresenter>();
      public static readonly CompoundParameterGroupItem<IFractionUnboundGroupPresenter> FractionUnbound = new CompoundParameterGroupItem<IFractionUnboundGroupPresenter>();
      public static readonly CompoundParameterGroupItem<IMolWeightGroupPresenter> MolWeight = new CompoundParameterGroupItem<IMolWeightGroupPresenter>();
      public static readonly CompoundParameterGroupItem<ICompoundTypeGroupPresenter> CompoundType = new CompoundParameterGroupItem<ICompoundTypeGroupPresenter>();
      public static readonly CompoundParameterGroupItem<ISolubilityGroupPresenter> Solubility = new CompoundParameterGroupItem<ISolubilityGroupPresenter>();
      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> { MoleculeType, Lipophilicity, FractionUnbound, MolWeight, CompoundType, Solubility};

      public class CompoundParameterGroupItem<TParameterGroupPresenter> : SubPresenterItem<TParameterGroupPresenter>
         where TParameterGroupPresenter : ICompoundParameterGroupPresenter
      {
      }
   }
}