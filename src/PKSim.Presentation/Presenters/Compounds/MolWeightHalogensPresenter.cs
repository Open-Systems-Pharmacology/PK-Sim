using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IMolWeightHalogensPresenter : IPresenter<IMolWeightHalogensView>, ICommandCollectorPresenter
   {
      void EditHalogens(IReadOnlyList<IParameter> halogens, IParameter effectiveMolWeight);
      void SaveHalogens();
   }

   public class MolWeightHalogensPresenter : AbstractCommandCollectorPresenter<IMolWeightHalogensView, IMolWeightHalogensPresenter>, IMolWeightHalogensPresenter
   {
      private readonly IHalogensPresenter _halogensPresenter;

      public MolWeightHalogensPresenter(IMolWeightHalogensView view, IHalogensPresenter halogensPresenter) : base(view)
      {
         _halogensPresenter = halogensPresenter;
         _halogensPresenter.IsSimpleEditor = true;
         _halogensPresenter.ValueOriginVisible = false;
         _halogensPresenter.HeaderVisible = false;
         AddSubPresenters(_halogensPresenter);
         view.FillWithParameterView(halogensPresenter.View);
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _halogensPresenter.InitializeWith(commandCollector);
      }

      public void EditHalogens(IReadOnlyList<IParameter> halogens, IParameter effectiveMolWeight)
      {
         _halogensPresenter.Edit(halogens, effectiveMolWeight);
      }

      public void SaveHalogens()
      {
         _halogensPresenter.SaveEditor();
      }
   }
}