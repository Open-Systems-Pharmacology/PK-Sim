﻿using System.Collections.Generic;
using System.Drawing;
using PKSim.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;

namespace PKSim.Presentation
{
   public interface IUserSettings : ICoreUserSettings, IPresentationUserSettings, INotifier, IValidatable
   {
      /// <summary>
      ///    Number of decimal after the comma
      /// </summary>
      uint DecimalPlace { get; set; }

      /// <summary>
      ///    Scientific notation allows (1e-2 for instance)
      /// </summary>
      bool AllowsScientificNotation { get; set; }

      /// <summary>
      ///    String representing the serialization of the main view
      /// </summary>
      string MainViewLayout { get; set; }

      /// <summary>
      ///    String representing the serialization of the ribbon
      /// </summary>
      string RibbonLayout { get; set; }

      /// <summary>
      ///    Restore the layout in the dock manager
      /// </summary>
      void RestoreLayout();

      /// <summary>
      ///    Save the layout from the dock manager
      /// </summary>
      void SaveLayout();

      /// <summary>
      ///    Layout version. If the version is not equal to the current one, the layout settings should be ignored
      /// </summary>
      int LayoutVersion { get; set; }

      /// <summary>
      ///    Color used for cells containing parameter whose value was changed by the user
      /// </summary>
      Color ChangedColor { get; set; }

      /// <summary>
      ///    Color used for cells containing a formula that was not changed by the user
      /// </summary>
      Color FormulaColor { get; set; }

      /// <summary>
      ///    Should the opened view saved in project be restored when opening the project (default true)
      /// </summary>
      bool ShouldRestoreWorkspaceLayout { get; set; }

      /// <summary>
      ///    Default grouping mode for grouping presenter
      /// </summary>
      ParameterGroupingModeId DefaultParameterGroupingMode { get; set; }

      /// <summary>
      ///    Directory map for the current user
      /// </summary>
      DirectoryMapSettings DirectoryMapSettings { get; }

      /// <summary>
      ///    Key Value pair containing for a given key (ObservedData, Project etc..) the last selected folder by the user
      /// </summary>
      IEnumerable<DirectoryMap> UsedDirectories { get; }

      /// <summary>
      ///    Specifies if update notification should be shown to the user. Default is true
      /// </summary>
      bool ShowUpdateNotification { get; set; }

      /// <summary>
      ///    Full version x.y.z that was most recently ignored (empty if the user never ignored any version)
      /// </summary>
      string LastIgnoredVersion { get; set; }

      /// <summary>
      ///    Resets the current layout
      /// </summary>
      void ResetLayout();

      /// <summary>
      ///    Which layout should be used when creating complex views (not supported in all use cases so far)
      /// </summary>
      ViewLayout PreferredViewLayout { get; set; }

      /// <summary>
      /// 3 states flag: Load, do not load, ask
      /// </summary>
      LoadTemplateWithReference LoadTemplateWithReference { get; set; }

      void ResetToDefault();
   }
}