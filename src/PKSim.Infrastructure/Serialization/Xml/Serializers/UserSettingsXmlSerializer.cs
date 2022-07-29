using System.Xml.Linq;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Serializer;
using PKSim.Core;
using PKSim.Presentation;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class UserSettingsXmlSerializer : BaseXmlSerializer<IUserSettings>
   {
      public UserSettingsXmlSerializer()
      {
         ElementName = CoreConstants.Serialization.UserSettings;
      }

      public override void PerformMapping()
      {
         Map(x => x.DecimalPlace);
         Map(x => x.AllowsScientificNotation);
         Map(x => x.ActiveSkin);
         Map(x => x.IconSizeTab);
         Map(x => x.IconSizeTreeView);
         Map(x => x.IconSizeContextMenu);
         Map(x => x.DefaultSpecies);
         Map(x => x.DefaultPopulation);
         Map(x => x.AbsTol);
         Map(x => x.RelTol);
         Map(x => x.TemplateDatabasePath);
         Map(x => x.MRUListItemCount);
         Map(x => x.LayoutVersion);
         Map(x => x.ChartBackColor);
         Map(x => x.FormulaColor);
         Map(x => x.ChartDiagramBackColor);
         Map(x => x.DisabledColor);
         Map(x => x.ChangedColor);
         Map(x => x.ShouldRestoreWorkspaceLayout);
         Map(x => x.DefaultParameterGroupingMode);
         Map(x => x.DefaultLipophilicityName);
         Map(x => x.DefaultFractionUnboundName);
         Map(x => x.DefaultSolubilityName);
         Map(x => x.DefaultChartEditorLayout);
         Map(x => x.MainViewLayout);
         Map(x => x.RibbonLayout);
         Map(x => x.ShowUpdateNotification);
         Map(x => x.LastIgnoredVersion);
         Map(x => x.MaximumNumberOfCoresToUse);
         Map(x => x.DefaultPopulationAnalysis);
         Map(x => x.OutputSelections);
         Map(x => x.ColorGroupObservedDataFromSameFolder);
         Map(x => x.PreferredViewLayout);
         Map(x => x.DefaultChartYScaling);
         Map(x => x.DisplayUnits);
         Map(x => x.ComparerSettings);
         Map(x => x.NumberOfBins);
         Map(x => x.NumberOfIndividualsPerBin);
         Map(x => x.ChartEditorLayout);
         Map(x => x.JournalPageEditorSettings);
         Map(x => x.ParameterIdentificationFeedbackEditorSettings);
         Map(x => x.LoadTemplateWithReference);
         MapEnumerable(x => x.UsedDirectories, x => x.DirectoryMapSettings.AddUsedDirectory);
         MapEnumerable(x => x.ProjectFiles, x => x.ProjectFiles.Add).WithChildMappingName(CoreConstants.Serialization.ProjectFile);
      }

      public override IUserSettings CreateObject(XElement element, SerializationContext context)
      {
         return context.Resolve<IUserSettings>();
      }
   }
}