using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Repositories;

namespace PKSim.Core.Model
{
   public interface IVectorialParametersContainer 
   {
      /// <summary>
      ///    Returns the values defined for the parameter in the container
      ///    The returned array contains one element for each item in the container and as thus
      ///    the dimension [0..NumberOfItems -1]
      /// </summary>
      /// <param name="parameterPath">Parameter path of parameter for which the value over all individuals should be retrieved</param>
      IReadOnlyList<double> AllValuesFor(string parameterPath);

      /// <summary>
      ///    Returns the percentile defined for the parameter in the container
      ///    The returned array contains one element for each item in the container and as thus
      ///    the dimension [0..NumberOfItems -1]
      /// </summary>
      /// <param name="parameterPath">Parameter path of parameter for which the percentiles over all individuals should be retrieved</param>
      IReadOnlyList<double> AllPercentilesFor(string parameterPath);

      /// <summary>
      ///    Vector dimension
      /// </summary>
      int NumberOfItems { get; } 

      /// <summary>
      ///    Returns all distinct vectorial parameters defined in the container
      /// </summary>
      /// <param name="entityPathResolver">Services used to resolve the path of the given parameter</param>
      IEnumerable<IParameter> AllVectorialParameters(IEntityPathResolver entityPathResolver);

      /// <summary>
      ///    Returns a cache containing all parameters defined in the containers (vectorial and constants) that can potentially be defined
      ///    as vectorial parameters
      /// </summary>
      PathCache<IParameter> AllParameters(IEntityPathResolver entityPathResolver);

      /// <summary>
      ///    Returns the vectorial parameter with the given path or null if not found
      /// </summary>
      IParameter ParameterByPath(string parameterPath, IEntityPathResolver entityPathResolver);

      /// <summary>
      ///    Returns all genders associated to the vectorial container
      /// </summary>
      IReadOnlyList<Gender> AllGenders(IGenderRepository genderRepository);

      /// <summary>
      /// Returns all covariates values defined for the covariate named <paramref name="covariateName"/>
      /// </summary>
      IReadOnlyList<string> AllCovariateValuesFor(string covariateName);

      /// <summary>
      /// Returns all defined covariate names
      /// </summary>
      IReadOnlyList<string> AllCovariateNames { get; }

      /// <summary>
      /// Returns whether the parameters should be displayed using the group structure or the container structure
      /// </summary>
      bool DisplayParameterUsingGroupStructure { get; }

      ParameterDistributionSettingsCache SelectedDistributions { get; }
   }
}