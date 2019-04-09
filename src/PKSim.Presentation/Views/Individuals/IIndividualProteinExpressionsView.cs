using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IIndividualMoleculeView
   {
      void Clear();
      void AddMoleculePropertiesView(IView view);
   }

   public interface IIndividualProteinExpressionsView : IView<IIndividualProteinExpressionsPresenter>, IIndividualMoleculeView
   {
      void BindTo(ProteinExpressionDTO proteinExpressionDTO);
      bool IntracellularVascularEndoLocationVisible { set; }
      bool LocationOnVascularEndoVisible { set; }
   }

   public interface IIndividualTransporterExpressionsView : IView<IIndividualTransporterExpressionsPresenter>, IIndividualMoleculeView
   {
      void BindTo(TransporterExpressionDTO transporterExpressionDTO);
      void ShowWarning(string warning);
      void HideWarning();
      void RefreshData();
   }
}