using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.SimModel.Services;

namespace PKSim.Core.Services
{
   public class SimulationRunOptions : OSPSuite.Core.Domain.SimulationRunOptions
   {
      /// <summary>
      ///    Set to true, the <see cref="OutputSelections" /> will be effectively ignored and all default quantities will be
      ///    exproted
      /// </summary>
      public bool RunForAllOutputs { get; set; } = false;

      /// <summary>
      ///    Set to <c>true</c> application events will be raised indicating that the simulation run is executed.
      ///    Typically set to <c>false</c> in CLI mode
      /// </summary>
      public bool RaiseEvents { get; set; } = false;

      /// <summary>
      /// Set to <c>true</c> (default) to validate the simulation before running it. Typically set to <c>false</c> when running batch calculations
      /// </summary>
      public bool Validate { get; set; } = true;

      public SimulationRunOptions()
      {
         //Default export mode of sim model is optimized
         SimModelExportMode = SimModelExportMode.Optimized;
      }
   }
}