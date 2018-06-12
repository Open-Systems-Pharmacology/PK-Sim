using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IMolWeightHalogensPresenter : IPresenter<IMolWeightHalogensView>, ICommandCollectorPresenter
   {
      void EditHalogens(IEnumerable<IParameter> compoundParameters);
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
         _parameterEditPresenter.StatusChanged += OnStatusChanged;
         view.FillWithParameterView(parameterEditPresenter.View);
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _parameterEditPresenter.InitializeWith(commandCollector);
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _parameterEditPresenter.ReleaseFrom(eventPublisher);
         _parameterEditPresenter.StatusChanged -= OnStatusChanged;
      }

      public void EditHalogens(IEnumerable<IParameter> compoundParameters)
      {
         _parameterEditPresenter.Edit(compoundParameters.Where(isHalogens));
      }

      public void SaveHalogens()
      {
         _parameterEditPresenter.SaveEditor();
      }

      private bool isHalogens(IParameter parameter)
      {
         if (parameter.GroupName != CoreConstants.Groups.COMPOUND_MW) return false;
         if (parameter.NameIsOneOf(
            Constants.Parameters.MOL_WEIGHT, 
            CoreConstants.Parameters.EFFECTIVE_MOLECULAR_WEIGHT, 
            CoreConstants.Parameters.HAS_HALOGENS))
            return false;

         return true;
      }
   }
}