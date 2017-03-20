using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICompoundItemPresenter : ISubPresenter
   {
      void EditCompound(PKSim.Core.Model.Compound compound);
   }
}