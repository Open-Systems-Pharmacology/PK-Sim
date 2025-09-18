using OSPSuite.Core.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Presentation.UICommands;
using PKSim.UI;
using PKSim.UI.UICommands;

namespace PKSim.Starter
{
   public class PKSimStarterUserInterfaceRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<UserInterfaceRegister>();
            scan.WithConvention<PKSimRegistrationConvention>();
            scan.IncludeNamespaceContainingType<UI.Views.ExpressionProfiles.CreateExpressionProfileView>();
            scan.IncludeNamespaceContainingType<UI.Views.Individuals.CreateIndividualView>();
            scan.IncludeNamespaceContainingType<UI.Views.DiseaseStates.DiseaseStateSelectionView>();
            scan.IncludeNamespaceContainingType<UI.Views.Parameters.ParameterGroupsView>();
            scan.IncludeNamespaceContainingType<UI.Views.ProteinExpression.ProteinExpressionsView>();
         });
         container.Register<OSPSuite.UI.Services.IToolTipCreator, IToolTipCreator, ToolTipCreator>(LifeStyle.Transient);
         container.Register<IExitCommand, ExitCommand>();
         container.Register<IProgressUpdater, NoneProgressUpdater>();
      }
   }
}