using System.Collections.Generic;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Repositories
{
   public interface IMoleculeParameterRepository : IStartableRepository<MoleculeParameter>
   {
      /// <summary>
      ///    Returns all parameters defined in the database for the molecule named <paramref name="moleculeName" />
      /// </summary>
      IReadOnlyList<IDistributedParameter> AllParametersFor(string moleculeName);

      /// <summary>
      ///    Returns the parameter named <paramref name="parameterName" /> defined in the template database for the molecule
      ///    named <paramref name="moleculeName" /> or null if the molecule is not found or the parameter is not defined for the molecule.
      /// </summary>
      IDistributedParameter ParameterFor(string moleculeName, string parameterName);

      /// <summary>
      ///    Returns the value of the parameter named <paramref name="parameterName" /> defined in the template database for the
      ///    molecule named <paramref name="moleculeName" /> or <paramref name="defaultValue" /> if the parameter named
      ///    <paramref name="parameterName" /> does not exist for this molecule.
      ///    If a <paramref name="randomGenerator" /> is specified, a random value will be generated for the underlying
      ///    parameter. Otherwise the mean value will be returned.
      /// </summary>
      double ParameterValueFor(string moleculeName, string parameterName, double? defaultValue = double.NaN, RandomGenerator randomGenerator = null);
   }
}