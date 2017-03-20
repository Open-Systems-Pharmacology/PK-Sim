using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IMolWeightHalogensPresenter : IPresenter<IMolWeightHalogensView>, ICommandCollectorPresenter
   {
      void EditHalogens(IEnumerable<IParameter> compoundParameters);
      void SaveHalogens();
   }

   public class MolWeightHalogensPresenter : AbstractCommandCollectorPresenter<IMolWeightHalogensView, IMolWeightHalogensPresenter>, IMolWeightHalogensPresenter
   {
      private readonly IMultiParameterEditPresenter _paramEditPresenter;

      public MolWeightHalogensPresenter(IMolWeightHalogensView view, IMultiParameterEditPresenter paramEditPresenter) : base(view)
      {
         _paramEditPresenter = paramEditPresenter;
         _paramEditPresenter.IsSimpleEditor = true;
         _paramEditPresenter.StatusChanged += OnStatusChanged;
         view.FillWithParameterView(paramEditPresenter.View);
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _paramEditPresenter.InitializeWith(commandCollector);
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _paramEditPresenter.ReleaseFrom(eventPublisher);
         _paramEditPresenter.StatusChanged -= OnStatusChanged;
      }

      public void EditHalogens(IEnumerable<IParameter> compoundParameters)
      {
         _paramEditPresenter.Edit(compoundParameters.Where(isHalogens));
      }

      public void SaveHalogens()
      {
         _paramEditPresenter.SaveEditor();
      }

      private bool isHalogens(IParameter parameter)
      {
         if (parameter.GroupName != CoreConstants.Groups.COMPOUND_MW) return false;
         if (string.Equals(parameter.Name, Constants.Parameters.MOL_WEIGHT)) return false;
         if (string.Equals(parameter.Name, CoreConstants.Parameter.MolWeightEff)) return false;
         if (string.Equals(parameter.Name, CoreConstants.Parameter.HAS_HALOGENS)) return false;
         return true;
      }
   }
}