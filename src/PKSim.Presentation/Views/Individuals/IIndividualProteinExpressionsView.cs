using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IIndividualMoleculeView
   {
      void AddMoleculePropertiesView(IView view);
      void AddExpressionParametersView(IView view);
      bool ReadOnly {  set; }
   }

   public interface IIndividualProteinExpressionsView : IView<IIndividualProteinExpressionsPresenter>, IIndividualMoleculeView
   {
      void AddLocalizationView(IView view);
   }

   public interface IIndividualTransporterExpressionsView : IView<IIndividualTransporterExpressionsPresenter>, IIndividualMoleculeView
   {
      void BindTo(IndividualTransporterDTO transporterExpressionDTO);
   }
}