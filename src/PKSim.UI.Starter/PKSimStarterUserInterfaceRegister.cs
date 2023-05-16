using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Presentation.UICommands;
using PKSim.UI.UICommands;

namespace PKSim.UI.Starter
{
   public class PKSimStarterUserInterfaceRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<UserInterfaceRegister>();
            scan.WithConvention<PKSimRegistrationConvention>();
            scan.IncludeNamespaceContainingType<Views.ExpressionProfiles.CreateExpressionProfileView>();
            scan.IncludeNamespaceContainingType<Views.Individuals.CreateIndividualView>();
            scan.IncludeNamespaceContainingType<Views.DiseaseStates.DiseaseStateSelectionView>();
            scan.IncludeNamespaceContainingType<Views.Parameters.ParameterGroupsView>();
            scan.IncludeNamespaceContainingType<Views.ProteinExpression.ProteinExpressionsView>();
         });
         container.Register<OSPSuite.UI.Services.IToolTipCreator, IToolTipCreator, ToolTipCreator>(LifeStyle.Transient);
         container.Register<IExitCommand, ExitCommand>();
      }
   }
}