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
      void AddAdvancedParameter(AdvancedParameter advancedParameter, bool generateRandomValues = true);

      IEnumerable<AdvancedParameter> AdvancedParameters { get; }

      void RemoveAdvancedParameter(AdvancedParameter advancedParameter);

      void RemoveAllAdvancedParameters();

      AdvancedParameter AdvancedParameterFor(IEntityPathResolver entityPathResolver, IParameter parameter);

      /// <summary>
      ///    Create a new set of random values for the advanced parameter
      /// </summary>
      void GenerateRandomValuesFor(AdvancedParameter advancedParameter);

      /// <summary>
      ///    Returns the parameters defined as advanced in the container
      /// </summary>
      IEnumerable<IParameter> AllAdvancedParameters(IEntityPathResolver entityPathResolver);

      /// <summary>
      ///    Returns the parameters defined as constant in the container
      /// </summary>
      IEnumerable<IParameter> AllConstantParameters(IEntityPathResolver entityPathResolver);

      void SetAdvancedParameters(AdvancedParameterCollection advancedParameterCollection);
   }
}