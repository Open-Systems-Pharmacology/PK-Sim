using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using static PKSim.Core.Model.Localization;

namespace PKSim.Core.Commands
{
   internal class SetExpressionLocalizationInProteinCommand : BuildingBlockChangeCommand
   {
      private IndividualProtein _protein;
      private readonly string _proteinId;
      private readonly Localization _newLocalization;
      private Localization _oldLocalization;
      private ISimulationSubject _simulationSubject;

      public SetExpressionLocalizationInProteinCommand(
         IndividualProtein protein,
         Localization localization,
         ISimulationSubject simulationSubject,
         IExecutionContext context, bool setAsFlag = true)
      {
         _protein = protein;
         _simulationSubject = simulationSubject;
         BuildingBlockId = context.BuildingBlockIdContaining(protein);
         _newLocalization = setAsFlag ? _protein.Localization ^ localization : localization;
         ObjectType = PKSimConstants.ObjectTypes.Protein;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         _proteinId = _protein.Id;
         context.UpdateBuildingBlockPropertiesInCommand(this, context.BuildingBlockContaining(protein));
         //This command is not visible. Only required to ensure that we can undo
         Visible = false;
      }

      protected override void ClearReferences()
      {
         _protein = null;
         _simulationSubject = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _protein = context.Get<IndividualProtein>(_proteinId);
         _simulationSubject = context.Get<ISimulationSubject>(BuildingBlockId);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldLocalization = _protein.Localization;
         _protein.Localization = _newLocalization;

         Description = PKSimConstants.Command.SetProteinTissueLocationDescription(_oldLocalization.ToString(), _newLocalization.ToString());
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetExpressionLocalizationInProteinCommand(_protein, _oldLocalization, _simulationSubject, context, setAsFlag: false)
            .AsInverseFor(this);
      }
   }

   public class SetExpressionLocalizationCommand : PKSimMacroCommand
   {
      private IndividualProtein _protein;
      private ISimulationSubject _simulationSubject;
      private readonly Localization _localization;

      public SetExpressionLocalizationCommand(
         IndividualProtein protein,
         Localization localization,
         ISimulationSubject simulationSubject,
         IExecutionContext context)
      {
         _protein = protein;
         _simulationSubject = simulationSubject;
         _localization = localization;
         ObjectType = PKSimConstants.ObjectTypes.Protein;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         context.UpdateBuildingBlockPropertiesInCommand(this, context.BuildingBlockContaining(protein));
      }

      public override void Execute(IExecutionContext context)
      {
         Description = PKSimConstants.Command.SetProteinTissueLocationDescription(_protein.Localization.ToString(), _localization.ToString());
         var setLocationCommand = new SetExpressionLocalizationInProteinCommand(_protein, _localization, _simulationSubject, context).Run(context);
         Add(setLocationCommand);

         if (_localization.Is(InBloodCells))
            AddRange(updateBloodCellsExpressionParameters(context));

         if (_localization.Is(InVascularEndothelium))
            AddRange(updateVascularEndotheliumExpressionParameters(context));

         if (_localization.Is(InTissue))
            AddRange(updateTissueExpressionParameters(context));

         _protein = null;
         _simulationSubject = null;

         //update properties from first command
         this.UpdatePropertiesFrom(setLocationCommand);
      }

      private IEnumerable<IOSPSuiteCommand> updateTissueExpressionParameters(IExecutionContext context)
      {
         var command = new PKSimMacroCommand();
         //We need to iterate over all parameters defined in the protein and update the values as expected
         var allInterstitialFractionParameters = _simulationSubject.Individual.GetAllChildren<IParameter>(x =>
            x.IsNamed(CoreConstants.Parameters.FRACTION_EXPRESSED_INTERSTITIAL) && x.ParentContainer.IsNamed(_protein.Name));

         var allTissueRelExpParameters = _simulationSubject.Individual.GetAllChildren<IParameter>(x =>
            x.IsNamed(CoreConstants.Parameters.REL_EXP) && x.ParentContainer.IsNamed(_protein.Name));


         //not in tissue=> set expression to 0
         command.AddRange(setParametersForFlags(context, None, InTissue, allTissueRelExpParameters.Select(x => (x, 0.0)).ToArray()));

         //Only in Interstitial => set interstitial fraction to 1
         command.AddRange(setParametersForFlags(context, Interstitial, Intracellular, allInterstitialFractionParameters.Select(x => (x, 1.0)).ToArray()));

         //Only in Intracellular=> set interstitial fraction to 0
         command.AddRange(setParametersForFlags(context, Intracellular, Interstitial, allInterstitialFractionParameters.Select(x => (x, 0.0)).ToArray()));

         // no action is required when all localization settings of a group are active. {InTissue, None}

         return command.All();
      }

      private IEnumerable<IOSPSuiteCommand> updateVascularEndotheliumExpressionParameters(IExecutionContext context)
      {
         var command = new PKSimMacroCommand();
         var f_exp_apical = _protein.Parameter(CoreConstants.Parameters.FRACTION_EXPRESSED_VASC_ENDO_APICAL);
         var f_exp_endosome = _protein.Parameter(CoreConstants.Parameters.FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME);
         var rel_exp_vasc = _protein.Parameter(CoreConstants.Parameters.REL_EXP_VASC_ENDO);

         //not in vasc endo => set expression to 0
         command.AddRange(setParametersForFlags(context, None, InVascularEndothelium, (rel_exp_vasc, 0)));

         //In apical and endosome but not basolateral => split equally between apical and endosome
         command.AddRange(setParametersForFlags(context, VascMembraneApical | VascEndosome, VascMembraneBasolateral, (f_exp_apical, 0.5), (f_exp_endosome, 0.5)));

         //In apical and basolateral but not endosome => split equally between apical and basolateral (basolateral = 1 - apical - endosome)
         command.AddRange(setParametersForFlags(context, VascMembraneApical | VascMembraneBasolateral, VascEndosome, (f_exp_apical, 0.5), (f_exp_endosome, 0)));

         //In endosome and basolateral but not apical => split equally between endosome and basolateral (basolateral = 1 - apical - endosome)
         command.AddRange(setParametersForFlags(context, VascEndosome | VascMembraneBasolateral, VascMembraneApical, (f_exp_apical, 0), (f_exp_endosome, 0.5)));

         //In apical but not endosome or basolateral => set all to apical
         command.AddRange(setParametersForFlags(context, VascMembraneApical, VascEndosome | VascMembraneBasolateral, (f_exp_apical, 1), (f_exp_endosome, 0)));

         //In endosome but not apical or basolateral => set all to endosome
         command.AddRange(setParametersForFlags(context, VascEndosome, VascMembraneApical | VascMembraneBasolateral, (f_exp_apical, 0), (f_exp_endosome, 1)));

         //In basolateral but not apical or endosome => set all to basolateral
         command.AddRange(setParametersForFlags(context, VascMembraneBasolateral, VascMembraneApical | VascEndosome, (f_exp_apical, 0), (f_exp_endosome, 0)));


         // no action is required when all localization settings of a group are active. {InVascularEndothelium, None}
         return command.All();
      }

      private IEnumerable<IOSPSuiteCommand> updateBloodCellsExpressionParameters(IExecutionContext context)
      {
         var command = new PKSimMacroCommand();
         var f_exp_bc_cell = _protein.Parameter(CoreConstants.Parameters.FRACTION_EXPRESSED_BLOOD_CELLS);
         var rel_exp_bc = _protein.Parameter(CoreConstants.Parameters.REL_EXP_BLOOD_CELLS);

         //not in blood cells => set expression to 0
         command.AddRange(setParametersForFlags(context, None, InBloodCells, (rel_exp_bc, 0)));

         // Only in BC interstitial
         command.AddRange(setParametersForFlags(context, BloodCellsIntracellular, BloodCellsMembrane, (f_exp_bc_cell, 1)));

         // Only in BC membrane
         command.AddRange(setParametersForFlags(context, BloodCellsMembrane, BloodCellsIntracellular, (f_exp_bc_cell, 0)));

         // no action is required when all localization settings of a group are active. {InBloodCells, None}
         return command.All();
      }

      private IEnumerable<ICommand> setParametersForFlags(IExecutionContext context, 
         Localization enabledLocalization,
         Localization disabledLocalization,
         params (IParameter param, double value)[] parametersToSet)
      {
         if (enabledLocalization != None && !_protein.Localization.Is(enabledLocalization))
            return Enumerable.Empty<IOSPSuiteCommand>();

         if (disabledLocalization != None && _protein.Localization.Is(disabledLocalization))
            return Enumerable.Empty<IOSPSuiteCommand>();

         return parametersToSet.Select(x =>
         {
            var (parameter, value) = x;
            var macroCommand = new PKSimMacroCommand();
            var setParameterCommand = new SetParameterValueCommand(parameter, value);
            macroCommand.Add(setParameterCommand);
            macroCommand.Add(new SetParameterDefaultStateCommand(parameter, isDefault:true) { ShouldChangeVersion = false, Visible = false});
            macroCommand.Add(new UpdateParameterValueOriginCommand(parameter, ValueOrigin.Undefined) { ShouldChangeVersion = false, Visible = false });
            macroCommand.Execute(context);
            macroCommand.WithHistoryEntriesFrom(macroCommand);
            return macroCommand;
         });
      }
   }
}