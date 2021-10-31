using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;

namespace PKSim.Core.Model
{
   public enum TemplateDatabaseType
   {
      User,
      System,
      Remote
   }

   [Flags]
   public enum TemplateType
   {
      Simulation = 1 << 0,
      Compound = 1 << 1,
      Formulation = 1 << 2,
      Protocol = 1 << 3,
      Individual = 1 << 4,
      Population = 1 << 5,
      Event = 1 << 6,
      PopulationAnalysis = 1 << 7,
      PopulationAnalysisField = 1 << 8,
      ObservedData = 1 << 9,
      PopulationSimulationAnalysisWorkflow = 1 << 10,
      ObserverSet = 1 << 11,
   }

   public static class TemplateObjectTypeExtensions
   {
      public static bool Is(this TemplateType templateType, TemplateType typeToCompare)
      {
         return (templateType & typeToCompare) != 0;
      }

      public static bool SupportsReference(this TemplateType templateType) =>
         templateType.Is(TemplateType.Individual) ||
         templateType.Is(TemplateType.Population) ||
         templateType.Is(TemplateType.Compound);
   }

   public abstract class Template : ObjectBase
   {
      public TemplateDatabaseType DatabaseType { get; set; }
      public TemplateType Type { get; set; }
      public object Object { get; set; }


      /// <summary>
      /// This will be the default version of a template
      /// </summary>
      public string Version { get; set; }


      protected Template() : base(ShortGuid.NewGuid())
      {
      }
   }

   public class LocalTemplate : Template
   {
      /// <summary>
      ///    List of <see cref="Template" /> referenced by current <see cref="Template" />.
      ///    A template should not reference itself!
      /// </summary>
      public List<Template> References { get; }   = new List<Template>();

      public bool HasReferences => References.Any();
   }

   public class RemoteTemplate : Template
   {
   
      /// <summary>
      /// Url for a remote snapshot or null otherwise
      /// </summary>
      public string Url { get; set; }

   }


   public class RemoteTemplates
   {
      public string Version { get; set; }
      public RemoteTemplate[] Templates { get; set; }
   }
}