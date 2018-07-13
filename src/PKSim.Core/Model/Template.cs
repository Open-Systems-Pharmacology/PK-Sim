using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public enum TemplateDatabaseType
   {
      User,
      System
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
      PopulationSimulationAnalysisWorkflow = 1 << 10
   }

   public static class TemplateObjecTypeExtensions
   {
      public static bool Is(this TemplateType templateType, TemplateType typeToCompare)
      {
         return (templateType & typeToCompare) != 0;
      }
   }

   public class Template : ObjectBase
   {
      public TemplateDatabaseType DatabaseType { get; set; }
      public TemplateType TemplateType { get; set; }
      public object Object { get; set; }
      
      /// <summary>
      ///  List of <see cref="Template"/> referenced by current <see cref="Template"/>. 
      ///  A template should not reference itself!
      /// </summary>
      public List<Template> References { get; private set; }

      public bool HasReferences => References.Any();

      public Template() : base(ShortGuid.NewGuid())
      {
         References = new List<Template>();
      }
   }
}