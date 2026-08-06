using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public class ProtocolProperties
   {
      private readonly List<FormulationMapping> _formulationMappings;
      private readonly List<EventPlaceholderMapping> _eventPlaceholderMappings;

      /// <summary>
      /// Reference to <see cref="Protocol"/> used in the simulation.
      /// </summary>
      public virtual Protocol Protocol { get; set; }

      public ProtocolProperties()
      {
         _formulationMappings = new List<FormulationMapping>();
         _eventPlaceholderMappings = new List<EventPlaceholderMapping>();
      }

      public virtual ProtocolProperties Clone()
      {
         var clone = new ProtocolProperties();
         FormulationMappings.Each(clone.AddFormulationMapping);
         EventPlaceholderMappings.Each(clone.AddEventPlaceholderMapping);

         //do not clone: simply update reference that should be changed if required
         clone.Protocol = Protocol;

         return clone;
      }

      public virtual void AddFormulationMapping(FormulationMapping formulationMapping) => _formulationMappings.Add(formulationMapping);

      public virtual void ClearFormulationMapping() => _formulationMappings.Clear();

      public virtual FormulationMapping MappingWith(string formulationKey) =>
         _formulationMappings.FirstOrDefault(x => string.Equals(x.FormulationKey, formulationKey));

      public virtual IReadOnlyList<FormulationMapping> FormulationMappings => _formulationMappings;

      public virtual void AddEventPlaceholderMapping(EventPlaceholderMapping eventPlaceholderMapping) => _eventPlaceholderMappings.Add(eventPlaceholderMapping);

      public virtual void ClearEventPlaceholderMappings() => _eventPlaceholderMappings.Clear();

      public virtual EventPlaceholderMapping EventMappingWith(string eventKey) =>
         _eventPlaceholderMappings.FirstOrDefault(x => string.Equals(x.EventKey, eventKey));

      public virtual IReadOnlyList<EventPlaceholderMapping> EventPlaceholderMappings => _eventPlaceholderMappings;

      public bool IsAdministered => Protocol != null;
   }
}