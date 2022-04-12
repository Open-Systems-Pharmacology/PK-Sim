using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.SimModel.Services;

namespace PKSim.Core.Services
{
   public enum JacobianUse
   {
      /// <summary>
      ///    Do not change anything and respect the settings in simulation
      /// </summary>
      AsIs,

      /// <summary>
      ///    Turn off jacobian calculation performed by SimModel and let the solver deals with it irrespectively from simulation
      ///    settings
      /// </summary>
      TurnOff,

      /// <summary>
      ///    Turn on jacobian calculation performed by SimModel irrespectively from simulation settings
      /// </summary>
      TurnOn
   }

   public class SimulationRunOptions : OSPSuite.Core.Domain.SimulationRunOptions
   {
      /// <summary>
      ///    Set to <c>true</c>, the <see cref="OutputSelections" /> will be effectively ignored and all default quantities will
      ///    be
      ///    exported. Default is <c>false</c>
      /// </summary>
      public bool RunForAllOutputs { get; set; } = false;

      /// <summary>
      ///    Set to <c>true</c> application events will be raised indicating that the simulation run is executed.
      ///    Typically set to <c>false</c> in CLI mode
      /// </summary>
      public bool RaiseEvents { get; set; } = false;

      /// <summary>
      ///    Set to <c>true</c> (default) to validate the simulation before running it. Typically set to <c>false</c> when
      ///    running batch calculations
      /// </summary>
      public bool Validate { get; set; } = true;

      /// <summary>
      ///    Specifies how the Jacobian calculation should be performed
      /// </summary>
      public JacobianUse JacobianUse { get; set; } = JacobianUse.AsIs;

      public SimulationRunOptions()
      {
         //Default export mode of sim model is optimized
         SimModelExportMode = SimModelExportMode.Optimized;
      }
   }
}