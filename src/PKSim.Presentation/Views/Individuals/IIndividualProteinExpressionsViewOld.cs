using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IIndividualMoleculeView
   {
      void Clear();
      void AddMoleculePropertiesView(IView view);
   }

   public interface IIndividualProteinExpressionsViewOld : IView<IIndividualProteinExpressionsPresenterOld>, IIndividualMoleculeView
   {
      void BindTo(ProteinExpressionDTO proteinExpressionDTO);
      bool IntracellularVascularEndoLocationVisible { set; }
      bool LocationOnVascularEndoVisible { set; }
   }

   public interface IIndividualProteinExpressionsView : IView<IIndividualProteinExpressionsPresenter>, IIndividualMoleculeView
   {
      void BindTo(IEnumerable<ExpressionParameterDTO> parameters);
      void AddLocalizationView(IView view);
   }

   public interface IIndividualTransporterExpressionsView : IView<IIndividualTransporterExpressionsPresenter>, IIndividualMoleculeView
   {
      void BindTo(TransporterExpressionDTO transporterExpressionDTO);
      void ShowWarning(string warning);
      void HideWarning();
      void RefreshData();
   }
}