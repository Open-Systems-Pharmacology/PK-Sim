using System;
using System.Linq;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Formulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Formulations
{
   public interface ITableFormulationPresenter : IPresenter<ITableFormulationView>
   {
      void Save();
      void Edit(Formulation formulation);
      TableFormula EditedFormula { get; }
      event EventHandler TableFormulaChanged;
   }

   public class TableFormulationPresenter : AbstractCommandCollectorPresenter<ITableFormulationView, ITableFormulationPresenter>, ITableFormulationPresenter
   {
      private readonly ITableFormulationParameterPresenter _tableParameterPresenter;
      private readonly IMultiParameterEditPresenter _formulationParametersPresenter;
      public event EventHandler TableFormulaChanged = delegate { };

      public TableFormulationPresenter(ITableFormulationView view, ITableFormulationParameterPresenter tableParameterPresenter, IMultiParameterEditPresenter formulationParametersPresenter) :
         base(view)
      {
         _tableParameterPresenter = tableParameterPresenter;
         _formulationParametersPresenter = formulationParametersPresenter;
         _tableParameterPresenter.ConfigureCreatedTableAction = addZeroPointToTable;
         _formulationParametersPresenter.IsSimpleEditor = true;
         _formulationParametersPresenter.RowIndicatorVisible = false;
         _view.AddParametersView(_formulationParametersPresenter.BaseView);
         _view.AddTableView(_tableParameterPresenter.BaseView);

         AddSubPresenters(_tableParameterPresenter, _formulationParametersPresenter);
         _tableParameterPresenter.Description = PKSimConstants.UI.ImportFormulationDescription;
         _tableParameterPresenter.ImportToolTip = PKSimConstants.UI.ImportFormulation;
         _tableParameterPresenter.StatusChanged += (o, e) => TableFormulaChanged(this, e);
      }

      private void addZeroPointToTable(TableFormula table)
      {
         //always add 0 0 to Formulation Table
         table.AddPoint(0, 0);
      }

      public void Save()
      {
         _tableParameterPresenter.Save();
      }

      public void Edit(Formulation formulation)
      {
         var allParameters = formulation.AllParameters().ToList();
         var fractionOfDose = allParameters.FindByName(CoreConstants.Parameter.FRACTION_DOSE);
         allParameters.Remove(fractionOfDose);

         _formulationParametersPresenter.Edit(allParameters);
         _tableParameterPresenter.Edit(fractionOfDose);
      }

      public TableFormula EditedFormula
      {
         get { return _tableParameterPresenter.EditedFormula; }
      }
   }
}