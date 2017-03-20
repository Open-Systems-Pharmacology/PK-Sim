using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface IAdvancedParameterContainer : IPKSimBuildingBlock, IVectorialParametersContainer
   {
      /// <summary>
      ///    Add and advanced parameter to the container. When the parameter <paramref name="generateRandomValues" /> is set to
      ///    true (default),
      ///    random values are also generated for the advanced paraneters
      /// </summary>
      void AddAdvancedParameter(IAdvancedParameter advancedParameter, bool generateRandomValues = true);

      IEnumerable<IAdvancedParameter> AdvancedParameters { get; }

      void RemoveAdvancedParameter(IAdvancedParameter advancedParameter);

      IAdvancedParameter AdvancedParameterFor(IEntityPathResolver entityPathResolver, IParameter parameter);

      /// <summary>
      ///    Create a new set of random values for the advanced parameter
      /// </summary>
      void GenerateRandomValuesFor(IAdvancedParameter advancedParameter);

      /// <summary>
      ///    Returns the parameters defined as advanced in the container
      /// </summary>
      IEnumerable<IParameter> AllAdvancedParameters(IEntityPathResolver entityPathResolver);

      /// <summary>
      ///    Returns the parameters defined as constant in the container
      /// </summary>
      IEnumerable<IParameter> AllConstantParameters(IEntityPathResolver entityPathResolver);

      void SetAdvancedParameters(IAdvancedParameterCollection advancedParameterCollection);
   }
}