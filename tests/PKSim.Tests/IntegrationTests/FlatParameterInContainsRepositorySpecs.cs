using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Conventions.AcceptanceCriteria;
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

      [Observation]
      public void should_set_is_input_flag_according_to_specification()
      {
         var parametersWithWrongIsInputFlag = new List<ParameterMetaData>();

         foreach (var parameter in _allParameters)
         {
            if (parameter.ReadOnly)
            {
               checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, 
                  isInputShouldBe: isOneOfReadOnlyAndIsInputParameters(parameter.ParameterName));
               continue;
            }

            if (parameter.ContainerType.Equals(CoreConstants.ContainerType.Formulation))
            {
               //all editable formulation parameters with exception of "Thickness (unstirred water layer)" must be input
               if (!parameter.ReadOnly && !parameter.ParameterName.Equals(CoreConstantsForSpecs.Parameter.THICKNESS_WATER_LAYER))
                  checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: true);
               else
                  checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: false);
               continue;
            }

            if (parameter.ContainerType.Equals(CoreConstants.ContainerType.Compound))
            {
               if (parameter.ParameterName.IsOneOf(
                  CoreConstants.Parameters.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE,
                  CoreConstants.Parameters.IS_SMALL_MOLECULE,
                  CoreConstants.Parameters.LIPOPHILICITY,
                  CoreConstants.Parameters.MOLECULAR_WEIGHT,
                  CoreConstants.Parameters.PLASMA_PROTEIN_BINDING_PARTNER,
                  CoreConstants.Parameters.REFERENCE_PH,
                  CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH))
                  checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: true);
               else
                  checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: false);
               continue;
            }

            if (parameter.ContainerType.Equals(CoreConstants.ContainerType.General))
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
               continue;
            }

            if (parameter.ContainerType.Equals(CoreConstants.ContainerType.Process))
            {
               if (parameter.GroupName.Equals(CoreConstants.Groups.COMPOUNDPROCESS_SIMULATION_PARAMETERS))
               {
                  if (parameter.ParameterName.Equals(CoreConstants.Parameters.KI))
                  {
                     //Ki is defined as formula for irreversible inhibition => not an input;
                     //for all other processes: input
                     bool kiShouldBeInput = !parameter.ContainerName.Equals(CoreConstantsForSpecs.ContainerName.IRREVERSIBLE_INHIBITION);
                     checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: kiShouldBeInput);
                     continue;
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
                  continue;
               }

               if (parameter.GroupName.Equals(CoreConstants.Groups.COMPOUNDPROCESS_CALCULATION_PARAMETERS))
               {
                  if (parameter.ParameterName.IsOneOf(CoreConstantsForSpecs.Parameter.ENZYME_CONCENTRATION, 
                     CoreConstants.Parameters.FRACTION_UNBOUND_EXPERIMENT,
                     CoreConstantsForSpecs.Parameter.IN_VITRO_CL_FOR_LIVER_MICROSOMES,
                     CoreConstantsForSpecs.Parameter.IN_VITRO_CL_FOR_RECOMBINANT_ENZYMES,
                     CoreConstantsForSpecs.Parameter.IN_VITRO_VMAX_FOR_LIVER_MICROSOMES,
                     CoreConstantsForSpecs.Parameter.IN_VITRO_VMAX_FOR_RECOMBINANT_ENZYMES,
                     CoreConstantsForSpecs.Parameter.IN_VITRO_VMAX_FOR_TRANSPORTER,
                     CoreConstantsForSpecs.Parameter.INTRINSIC_CLEARANCE, 
                     CoreConstants.Parameters.LIPOPHILICITY_EXPERIMENT, CoreConstantsForSpecs.Parameter.MEASURING_TIME,
                     CoreConstantsForSpecs.Parameter.PLASMA_CLEARANCE, CoreConstants.Parameters.SPECIFIC_CLEARANCE, 
                     CoreConstantsForSpecs.Parameter.HALF_LIFE_HEPATOCYTE_ASSAY, CoreConstantsForSpecs.Parameter.HALF_LIFE_MICROSOMAL_ASSAY,
                     CoreConstantsForSpecs.Parameter.TRANSPORTER_CONCENTRATION,
                     CoreConstantsForSpecs.Parameter.TS_MAX, CoreConstantsForSpecs.Parameter.TUBULAR_SECRETION,
                     CoreConstantsForSpecs.Parameter.VMAX, CoreConstantsForSpecs.Parameter.VMAX_LIVER_TISSUE))
                     checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: true);
                  else
                     checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: false);
                  continue;
               }
            }

            //all other parameters should be not an input
            checkIsInputFlag(parametersWithWrongIsInputFlag, parameter, isInputShouldBe: false);
         }

         parametersWithWrongIsInputFlag.Any().ShouldBeFalse(ErrorMessageFor(parametersWithWrongIsInputFlag));
      }
   }
}