using PKSim.Core.Model;

namespace PKSim.Core.Events
{
   public interface IFormulationEvent
   {
      Formulation Formulation { get; }
   }
}