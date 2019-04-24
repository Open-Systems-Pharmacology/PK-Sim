using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   //This tests ensure that the flags defined in the database for parameters are consistent
   public abstract class concern_for_FlatParameterInContainsRepository : ContextForIntegration<IFlatParameterInContainsRepository>
   {
      protected List<ParameterMetaData> _allParameters;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = IoC.Resolve<IFlatParameterInContainsRepository>();
         _allParameters = sut.All().ToList();
      }

      protected string ErrorMessageFor(IEnumerable<ParameterMetaData> parameters)
      {
         return parameters.Select(p => p.ToString()).ToString("\n");
      }
   }

   public class When_checking_the_database_consistency_regarding_parameter_flags : concern_for_FlatParameterInContainsRepository
   {
      private bool isOneOfReadOnlyAndIsInputParameters(string parameterName)
      {
         return parameterName.IsOneOf(CoreConstants.Parameters.IS_SMALL_MOLECULE, CoreConstants.Parameters.INPUT_DOSE);
      }

      [Observation]
      public void all_hidden_parameters_should_be_read_only()
      {
         var parameters = _allParameters.Where(x => !x.Visible)
            .Where(x => !x.ReadOnly)
            .Where(x => x.ParameterName != CoreConstants.Parameters.SOLUBILITY_TABLE)
            .ToList();

         parameters.Any().ShouldBeFalse(ErrorMessageFor(parameters));
      }

      [Observation]
      public void all_read_only_parameters_should_not_be_variable_in_a_population()
      {
         var parameters = _allParameters.Where(x => x.ReadOnly).Where(x => x.CanBeVariedInPopulation).ToList();
         parameters.Any().ShouldBeFalse(ErrorMessageFor(parameters));
      }

      [Observation]
      public void all_parameters_variable_in_a_population_should_be_visible()
      {
         var parameters = _allParameters.Where(x => x.CanBeVariedInPopulation).Where(x => !x.Visible).ToList();
         parameters.Any().ShouldBeFalse(ErrorMessageFor(parameters));
      }

      [Observation]
      public void all_visible_and_editable_parameters_should_be_can_be_varied_except_mol_weight()
      {
         var parameters = _allParameters.Where(x => x.Visible)
            .Where(x => !x.ReadOnly)
            .Where(x => !x.CanBeVaried)
            .Where(x => x.ParameterName != Constants.Parameters.MOL_WEIGHT).ToList();

         parameters.Any().ShouldBeFalse(ErrorMessageFor(parameters));
      }

      [Observation]
      public void all_readonly_parameter_should_be_marked_as_non_input_with_some_known_exceptions()
      {
         var parameters = _allParameters.Where(x => x.ReadOnly)
            .Where(x => !isOneOfReadOnlyAndIsInputParameters(x.ParameterName))
            .Where(x => x.IsInput).ToList();

         parameters.Any().ShouldBeFalse(ErrorMessageFor(parameters));
      }

      [Observation]
      public void all_hidden_parameter_should_be_marked_as_non_input_with_some_known_exceptions()
      {
         var parameters = _allParameters.Where(x => !x.Visible)
            .Where(x => !isOneOfReadOnlyAndIsInputParameters(x.ParameterName))
            .Where(x => x.IsInput).ToList();

         parameters.Any().ShouldBeFalse(ErrorMessageFor(parameters));
      }

      private void checkIsInputFlag(List<ParameterMetaData> parametersWithWrongIsInputFlag,
                                    ParameterMetaData parameter, bool isInputShouldBe)
      {
         if(parameter.IsInput != isInputShouldBe)
            parametersWithWrongIsInputFlag.Add(parameter);
      }

      private void checkIsInput(Action<List<ParameterMetaData>> checkIsInputAction)
      {
         var parametersWithWrongIsInputFlag = new List<ParameterMetaData>();
         checkIsInputAction(parametersWithWrongIsInputFlag);
         parametersWithWrongIsInputFlag.Any().ShouldBeFalse(ErrorMessageFor(parametersWithWrongIsInputFlag));
      }

      private void checkIsInputForSomeReadOnlyParameters(List<ParameterMetaData> parametersWithWrongIsInputFlag)
      {
         foreach (var parameter in _allParameters.Where(p => isOneOfReadOnlyAndIsInputParameters(p.ParameterName)))
         {
            checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: true);
         }
      }

      private void checkIsInputForEditableFormulationParameters(List<ParameterMetaData> parametersWithWrongIsInputFlag)
      {
         foreach (var parameter in _allParameters
            .Where(p=>p.ContainerType == CoreConstants.ContainerType.Formulation)
            .Where(p=>!p.ReadOnly))
         {
            //all editable formulation parameters with exception of "Thickness (unstirred water layer)" must be input
            if (!parameter.ReadOnly && parameter.ParameterName != CoreConstantsForSpecs.Parameter.THICKNESS_WATER_LAYER)
               checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: true);
            else
               checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: false);
         }
      }

      private void checkIsInputForCompoundParameters(List<ParameterMetaData> parametersWithWrongIsInputFlag)
      {
         foreach (var parameter in _allParameters.Where(p => p.ContainerType == CoreConstants.ContainerType.Compound))
         {
            if (parameter.ParameterName.IsOneOf(
               CoreConstants.Parameters.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE,
               CoreConstants.Parameters.IS_SMALL_MOLECULE,
               CoreConstants.Parameters.ENABLE_SUPERSATURATION,
               CoreConstants.Parameters.LIPOPHILICITY,
               CoreConstants.Parameters.MOLECULAR_WEIGHT,
               CoreConstants.Parameters.PLASMA_PROTEIN_BINDING_PARTNER,
               CoreConstants.Parameters.REFERENCE_PH,
               CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH))
               checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: true);
            else
               checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: false);
         }
      }

      private void checkIsInputForGeneralParameters(List<ParameterMetaData> parametersWithWrongIsInputFlag)
      {
         foreach (var parameter in _allParameters.Where(p=>p.ContainerType == CoreConstants.ContainerType.General))
         {
            //few application parameters which are input
            if (parameter.ParameterName.IsOneOf(
               CoreConstants.Parameters.INPUT_DOSE,
               CoreConstantsForSpecs.Parameter.INFUSION_TIME,
               CoreConstantsForSpecs.Parameter.START_TIME,
               CoreConstantsForSpecs.Parameter.VOLUME_OF_WATER_PER_BODYWEIGHT))
               checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: true);
            else
               checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: false);
         }
      }

      private void checkIsInputForEditableProcessParameters(List<ParameterMetaData> parametersWithWrongIsInputFlag)
      {
         foreach (var parameter in _allParameters
            .Where(p => p.ContainerType == CoreConstants.ContainerType.Process)
            .Where(p=> !p.ReadOnly))
         {
            checkIsInputForProcessParameter(parameter, parametersWithWrongIsInputFlag);
         }
      }

      private void checkIsInputForProcessParameter(ParameterMetaData parameter, List<ParameterMetaData> parametersWithWrongIsInputFlag)
      {
         if (parameter.GroupName == CoreConstants.Groups.COMPOUNDPROCESS_SIMULATION_PARAMETERS)
         {
            if (parameter.ParameterName == CoreConstants.Parameters.KI)
            {
               //Ki is defined as formula for irreversible inhibition => not an input;
               //for all other processes: input
               bool kiShouldBeInput = parameter.ContainerName != CoreConstantsForSpecs.ContainerName.IRREVERSIBLE_INHIBITION;
               checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: kiShouldBeInput);
               return;
            }

            if (parameter.ParameterName.IsOneOf(CoreConstants.Parameters.EC50, CoreConstants.Parameters.EMAX,
               CoreConstants.Parameters.GFR_FRACTION, CoreConstantsForSpecs.Parameter.HILL_COEFFICIENT,
               CoreConstants.Parameters.K_KINACT_HALF, CoreConstantsForSpecs.Parameter.KD,
               CoreConstants.Parameters.KI_C, CoreConstants.Parameters.KI_U,
               CoreConstants.Parameters.KINACT, CoreConstantsForSpecs.Parameter.KM,
               CoreConstantsForSpecs.Parameter.KOFF))
               checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: true);
            else
               checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: false);
            return;
         }

         if (parameter.GroupName == CoreConstants.Groups.COMPOUNDPROCESS_CALCULATION_PARAMETERS)
         {
            if (parameter.ParameterName.IsOneOf(CoreConstantsForSpecs.Parameter.ENZYME_CONCENTRATION,
               CoreConstants.Parameters.FRACTION_UNBOUND_EXPERIMENT,
               CoreConstantsForSpecs.Parameter.IN_VITRO_CL_FOR_LIVER_MICROSOMES,
               CoreConstantsForSpecs.Parameter.IN_VITRO_CL_FOR_RECOMBINANT_ENZYMES,
               CoreConstantsForSpecs.Parameter.IN_VITRO_VMAX_FOR_LIVER_MICROSOMES,
               CoreConstantsForSpecs.Parameter.IN_VITRO_VMAX_FOR_RECOMBINANT_ENZYMES,
               CoreConstantsForSpecs.Parameter.IN_VITRO_VMAX_FOR_TRANSPORTER,
               CoreConstantsForSpecs.Parameter.INTRINSIC_CLEARANCE,
               CoreConstants.Parameters.LIPOPHILICITY_EXPERIMENT, 
               CoreConstantsForSpecs.Parameter.PLASMA_CLEARANCE, CoreConstants.Parameters.SPECIFIC_CLEARANCE,
               CoreConstantsForSpecs.Parameter.TRANSPORTER_CONCENTRATION,
               CoreConstantsForSpecs.Parameter.TS_MAX, CoreConstantsForSpecs.Parameter.TUBULAR_SECRETION,
               CoreConstantsForSpecs.Parameter.VMAX, CoreConstantsForSpecs.Parameter.VMAX_LIVER_TISSUE))
               checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: true);
            else
               checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: false);
         }
      }

      private void checkIsInputForOtherContainerTypes(List<ParameterMetaData> parametersWithWrongIsInputFlag)
      {
         foreach (var parameter in _allParameters.Where(p => !p.ContainerType.IsOneOf(CoreConstants.ContainerType.Formulation,
            CoreConstants.ContainerType.Compound, CoreConstants.ContainerType.General, CoreConstants.ContainerType.Process)))
         {
            //all other parameters should be not an input
            checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: false);
         }
      }

      [Observation]
      public void should_set_is_input_flag_for_some_readonly_parameters()
      {
         checkIsInput(checkIsInputForSomeReadOnlyParameters);
      }

      [Observation]
      public void should_set_is_input_flag_for_editable_formulation_parameters_according_to_specification()
      {
         checkIsInput(checkIsInputForEditableFormulationParameters);
      }

      [Observation]
      public void should_set_is_input_flag_for_compound_parameters_according_to_specification()
      {
         checkIsInput(checkIsInputForCompoundParameters);
      }

      [Observation]
      public void should_set_is_input_flag_for_general_parameters_according_to_specification()
      {
         checkIsInput(checkIsInputForGeneralParameters);
      }

      [Observation]
      public void should_set_is_input_flag_for_editable_process_parameters_according_to_specification()
      {
         checkIsInput(checkIsInputForEditableProcessParameters);
      }

      [Observation]
      public void should_set_is_input_flag_for_other_parameters_according_to_specification()
      {
         checkIsInput(checkIsInputForOtherContainerTypes);
      }

   }
}