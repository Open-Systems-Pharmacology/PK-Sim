using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ICompoundAlternativeTask
   {
      /// <summary>
      ///    Adds the given <paramref name="parameterAlternative" /> to the given <paramref name="compoundParameterGroup" />
      /// </summary>
      ICommand AddParameterGroupAlternativeTo(ParameterAlternativeGroup compoundParameterGroup, ParameterAlternative parameterAlternative);

      /// <summary>
      ///    Removes the  <paramref name="parameterAlternative" /> from the <paramref name="parameterGroup" />
      /// </summary>
      ICommand RemoveParameterGroupAlternative(ParameterAlternativeGroup parameterGroup, ParameterAlternative parameterAlternative);

      /// <summary>
      ///    Sets the value of the parameter. If the alternative containing the parameter is not used in any simulation,
      ///    the command will not update the building block version of the compound
      /// </summary>
      ICommand SetAlternativeParameterValue(IParameter parameter, double valueInDisplayUnit);

      /// <summary>
      ///    Sets the unit of the parameter. If the alternative containing the parameter is not used in any simulation,
      ///    the command will not update the building block version of the compound
      /// </summary>
      ICommand SetAlternativeParameterUnit(IParameter parameter, Unit newUnit);

      /// <summary>
      ///    Updates the table formula in <paramref name="parameter" /> with the <paramref name="formula" />.  If the alternative
      ///    containing the parameter is not used in any simulation,the command will not update the building block version of the
      ///    compound
      /// </summary>
      ICommand SetAlternativeParameterTable(IParameter parameter, TableFormula formula);

      /// <summary>
      ///    Edits the value origin for the given <paramref name="parameterAlternative" />
      /// </summary>
      ICommand UpdateValueOrigin(ParameterAlternative parameterAlternative, ValueOrigin newValueOrigin);

      /// <summary>
      ///    Returns the possible parameters for the permeability depending on the different alternatives defined for
      ///    Lipophilicity
      /// </summary>
      IEnumerable<IParameter> PermeabilityValuesFor(Compound compound);

      /// <summary>
      ///    Returns the possible parameters for the intestinal permeability depending on the different alternatives defined for
      ///    Lipophilicity
      /// </summary>
      IEnumerable<IParameter> IntestinalPermeabilityValuesFor(Compound compound);

      /// <summary>
      ///    Sets the given parameter alternative as default alternative in the parameter group
      /// </summary>
      /// <param name="parameterGroup">Parameter group containing the alternative</param>
      /// <param name="parameterAlternative">Parameter alternative that will be set as default</param>
      ICommand SetDefaultAlternativeFor(ParameterAlternativeGroup parameterGroup, ParameterAlternative parameterAlternative);

      /// <summary>
      ///    Sets the given <paramref name="species" /> as species describing the value entered for the given alternative
      /// </summary>
      /// <param name="parameterAlternative">Parameter alternative whose species should be set</param>
      /// <param name="species">Species to be set in the alternative</param>
      ICommand SetSpeciesForAlternative(ParameterAlternativeWithSpecies parameterAlternative, Species species);

      /// <summary>
      ///    Returns a table formula containing the solubility as a function of ph for the given solubility alternative
      /// </summary>
      TableFormula SolubilityTableForPh(ParameterAlternative solubilityAlternative, Compound compound);

      /// <summary>
      ///    Imports a solubility table from file and returns the imported table
      /// </summary>
      TableFormula ImportSolubilityTableFormula();

      /// <summary>
      ///    Performs initialization steps for a brand new solubility alternative that should behave as a Table alternative
      /// </summary>
      /// <param name="solubilityAlternative"></param>
      void PrepareSolubilityAlternativeForTableSolubility(ParameterAlternative solubilityAlternative);

      ParameterAlternative CreateSolubilityTableAlternativeFor(ParameterAlternativeGroup solubilityAlternativeGroup, string name);

      ParameterAlternative CreateAlternative(ParameterAlternativeGroup compoundParameterGroup, string name);
   }
}