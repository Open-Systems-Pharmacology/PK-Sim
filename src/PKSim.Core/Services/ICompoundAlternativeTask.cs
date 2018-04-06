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
      ///    Creates an alternative and adds it to the given <paramref name="compoundParameterGroup" />
      /// </summary>
      ICommand AddParameterGroupAlternativeTo(ParameterAlternativeGroup compoundParameterGroup);

      /// <summary>
      ///    Adds the given <paramref name="parameterAlternative" /> to the given <paramref name="compoundParameterGroup" />
      /// </summary>
      ICommand AddParameterGroupAlternativeTo(ParameterAlternativeGroup compoundParameterGroup, ParameterAlternative parameterAlternative);

      /// <summary>
      ///    Remove the  <paramref name="parameterAlternative" /> from the <paramref name="parameterGroup" />
      /// </summary>
      ICommand RemoveParameterGroupAlternative(ParameterAlternativeGroup parameterGroup, ParameterAlternative parameterAlternative);

      /// <summary>
      ///    Rename the given <paramref name="parameterAlternative" />
      /// </summary>
      ICommand RenameParameterAlternative(ParameterAlternative parameterAlternative);

      /// <summary>
      ///    Set the value of the parameter. If the alternative containing the parameter is not used in any simulation,
      ///    the command will not update the building block version of the compound
      /// </summary>
      ICommand SetAlternativeParameterValue(IParameter parameter, double valueInDisplayUnit);

      /// <summary>
      ///    Set the unit of the parameter. If the alternative containing the parameter is not used in any simulation,
      ///    the command will not update the building block version of the compound
      /// </summary>
      ICommand SetAlternativeParameterUnit(IParameter parameter, Unit newUnit);

      /// <summary>
      ///    Edit the value origin for the given <paramref name="parameterAlternative" />
      /// </summary>
      ICommand UpdateValueOrigin(ParameterAlternative parameterAlternative, ValueOrigin newValueOrigin);

      /// <summary>
      ///    Returns the possible parameters for the permeability depending on the different alternatives defined for
      ///    Lipophilicty
      /// </summary>
      IEnumerable<IParameter> PermeabilityValuesFor(Compound compound);

      /// <summary>
      ///    Returns the possible parameters for the intestinal permeability depending on the different alternatives defined for
      ///    Lipophilicty
      /// </summary>
      IEnumerable<IParameter> IntestinalPermeabilityValuesFor(Compound compound);

      /// <summary>
      ///    Set the given parameter alternative as default alternative in the parameter group
      /// </summary>
      /// <param name="parameterGroup">Parameter group containing the alternative</param>
      /// <param name="parameterAlternative">Parameter alternative that will be set as default</param>
      ICommand SetDefaultAlternativeFor(ParameterAlternativeGroup parameterGroup, ParameterAlternative parameterAlternative);

      /// <summary>
      ///    set the given <paramref name="species" /> as species describing the value entered for the given alternative
      /// </summary>
      /// <param name="parameterAlternative">Parameter alternative whose species should be set</param>
      /// <param name="species">Species to be set in the alternative</param>
      ICommand SetSpeciesForAlternative(ParameterAlternativeWithSpecies parameterAlternative, Species species);

      /// <summary>
      ///    Returns a table formula containg the solubiltiy as a function of ph for the given solubility alternative
      /// </summary>
      TableFormula SolubilityTableForPh(ParameterAlternative solubilityAlternative, Compound compound);
   }
}