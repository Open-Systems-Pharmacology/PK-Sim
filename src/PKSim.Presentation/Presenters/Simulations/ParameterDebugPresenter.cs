using System.Data;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IParameterDebugPresenter : IPresenter<IParameterDebugView>, IDisposablePresenter
   {
      void ShowParametersFor(Simulation simulation);
   }

   public class ParameterDebugPresenter : AbstractDisposablePresenter<IParameterDebugView, IParameterDebugPresenter>, IParameterDebugPresenter
   {
      private readonly IEntityPathResolver _entityPathResolver;

      public ParameterDebugPresenter(IParameterDebugView view, IEntityPathResolver entityPathResolver) : base(view)
      {
         _entityPathResolver = entityPathResolver;
      }

      public void ShowParametersFor(Simulation simulation)
      {
         var allParameters = simulation.All<IParameter>().Where(parameterShouldBeDisplayed);
         _view.Caption = $"All Parameters for Simulation with Id {simulation.Id}";

         var parameterIdTable = new DataTable("Parameter Id");
         parameterIdTable.AddColumn("Parameter Path");
         parameterIdTable.AddColumn("Building block Type");
         parameterIdTable.AddColumn("Building block Id");
         parameterIdTable.AddColumn("Parameter Id");
         parameterIdTable.AddColumn("Simulation Id");
         parameterIdTable.AddColumn<bool>("Is Constant");
         parameterIdTable.AddColumn<bool>("Editable");
         parameterIdTable.AddColumn<bool>("Visible");


         foreach (var parameter in allParameters)
         {
            var row = parameterIdTable.NewRow();
            var i = 0;
            row[i++] = _entityPathResolver.PathFor(parameter);
            row[i++] = parameter.BuildingBlockType.ToString();
            row[i++] = parameter.Origin.BuilingBlockId;
            row[i++] = parameter.Origin.ParameterId;
            row[i++] = parameter.Origin.SimulationId;
            row[i++] = parameter.Formula.IsConstant();
            row[i++] = parameter.Editable;
            row[i++] = parameter.Visible;
            parameterIdTable.Rows.Add(row);
         }

         _view.BindTo(parameterIdTable);
         _view.Display();
      }

      private bool parameterShouldBeDisplayed(IParameter parameter)
      {
         return true;
      }
   }
}