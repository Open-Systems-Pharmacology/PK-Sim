using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IIndividualMoleculeView
   {
      void AddMoleculePropertiesView(IView view);
   }

   public interface IIndividualProteinExpressionsView : IView<IIndividualProteinExpressionsPresenter>, IIndividualMoleculeView
   {
      void BindTo(IEnumerable<ExpressionParameterDTO> parameters);
      void AddLocalizationView(IView view);
   }

   public interface IIndividualTransporterExpressionsView : IView<IIndividualTransporterExpressionsPresenter>, IIndividualMoleculeView
   {
      void Clear();
      void BindTo(TransporterExpressionDTO transporterExpressionDTO);
      void ShowWarning(string warning);
      void HideWarning();
      void RefreshData();
   }
}