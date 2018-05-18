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

         foreach (var parameter in allParameters)
         {
            var row = parameterIdTable.NewRow();
            row[0] = _entityPathResolver.PathFor(parameter);
            row[1] = parameter.BuildingBlockType.ToString();
            row[2] = parameter.Origin.BuilingBlockId;
            row[3] = parameter.Origin.ParameterId;
            row[4] = parameter.Origin.SimulationId;
            parameterIdTable.Rows.Add(row);
         }

         _view.BindTo(parameterIdTable);
         _view.Display();
      }

      private bool parameterShouldBeDisplayed(IParameter parameter)
      {
         if (!parameter.BuildingBlockType.Is(PKSimBuildingBlockType.Simulation))
            return false;

         if (!parameter.Formula.IsConstant())
            return false;

         if (!parameter.Visible)
            return false;

         if (!parameter.Editable)
            return false;

         return true;
      }
   }
}