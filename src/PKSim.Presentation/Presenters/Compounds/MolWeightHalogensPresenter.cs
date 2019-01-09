using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IMolWeightHalogensPresenter : IPresenter<IMolWeightHalogensView>, ICommandCollectorPresenter
   {
      void EditHalogens(IEnumerable<IParameter> halogenParameters);
      void SaveHalogens();
   }

   public class MolWeightHalogensPresenter : AbstractCommandCollectorPresenter<IMolWeightHalogensView, IMolWeightHalogensPresenter>, IMolWeightHalogensPresenter
   {
      private readonly IMultiParameterEditPresenter _parameterEditPresenter;

      public MolWeightHalogensPresenter(IMolWeightHalogensView view, IMultiParameterEditPresenter parameterEditPresenter) : base(view)
      {
         _parameterEditPresenter = parameterEditPresenter;
         _parameterEditPresenter.IsSimpleEditor = true;
         _parameterEditPresenter.ValueOriginVisible = false;
         _parameterEditPresenter.HeaderVisible = false;
         AddSubPresenters(_parameterEditPresenter);
         view.FillWithParameterView(parameterEditPresenter.View);
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _parameterEditPresenter.InitializeWith(commandCollector);
      }

      public void EditHalogens(IEnumerable<IParameter> halogenParameters)
      {
         _parameterEditPresenter.Edit(halogenParameters);
      }

      public void SaveHalogens()
      {
         _parameterEditPresenter.SaveEditor();
      }
   }
}