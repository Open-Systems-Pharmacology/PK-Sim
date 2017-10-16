using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICompoundParameterGroupPresenter : ISubPresenter

   {
      /// <summary>
      ///    Edit the alternative defined in the compound to edit for the group
      /// </summary>
      void EditCompound(Compound compound);

   }

   public abstract class CompoundParameterGroupPresenter<TView> : AbstractSubPresenter<TView, ICompoundParameterGroupPresenter>, ICompoundParameterGroupPresenter where TView : ICompoundParameterGroupView
   {
      protected CompoundParameterGroupPresenter(TView view, IRepresentationInfoRepository representationInfoRepository, string groupName, bool needsCaption = true) : base(view)
      {
         if (needsCaption)
            View.Caption = representationInfoRepository.DisplayNameFor(RepresentationObjectType.GROUP, groupName);
      }

      public abstract void EditCompound(Compound compound);
   }
}