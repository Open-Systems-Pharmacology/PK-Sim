using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
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
      ExpressionProfile = 1 << 12,
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

      protected Template() : base(ShortGuid.NewGuid())
      {
      }

      public abstract bool IsSupportedByCurrentVersion(string currentVersion);
   }

   public class LocalTemplate : Template
   {
      /// <summary>
      ///    List of <see cref="Template" /> referenced by current <see cref="Template" />.
      ///    A template should not reference itself!
      /// </summary>
      public List<Template> References { get; } = new List<Template>();

      public bool HasReferences => References.Any();

      //Local templates are always supported by local version
      public override bool IsSupportedByCurrentVersion(string currentVersion) => true;
   }

   public class RemoteTemplate : Template
   {
      /// <summary>
      ///    Version of the template (not the software version)
      /// </summary>
      public string Version { get; set; }

      /// <summary>
      ///    Url for a remote snapshot (raw json file)
      /// </summary>
      public string Url { get; set; }

      /// <summary>
      ///    Actual repository URL inferred from the raw Url
      /// </summary>
      public string RepositoryUrl { get; set; }

      /// <summary>
      ///    Optional minimum version of the software required to use this template. For instance, if the template requires at
      ///    least v11 and the software is v10, this template won't be loaded
      /// </summary>
      public string MinVersion { get; set; }

      /// <summary>
      ///    Optional maximal version of the software required to use this template. For instance, if the template requires
      ///    at most v10 and the software is v11, this template won't be loaded
      /// </summary>
      public string MaxVersion { get; set; }

      public override bool IsSupportedByCurrentVersion(string currentVersion)
      {
         var curVersion = new Version(currentVersion);
         var supported = true;

         if (MinVersion.StringIsNotEmpty())
            supported = new Version(MinVersion).CompareTo(curVersion) <= 0;

         if (MaxVersion.StringIsNotEmpty())
            supported = supported && curVersion.CompareTo(new Version(MaxVersion)) <= 0;

         return supported;
      }

      public RemoteTemplate()
      {
         DatabaseType = TemplateDatabaseType.Remote;
      }
   }

   public class RemoteTemplates
   {
      public string Version { get; set; }
      public RemoteTemplate[] Templates { get; set; }
   }
}