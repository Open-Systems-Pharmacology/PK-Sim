using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_CoreConstants : StaticContextSpecification
   {
   }

   public class The_list_of_parameters_that_behaves_as_boolean : concern_for_CoreConstants
   {
      [Observation]
      public void should_contain_the_parameter_if_liver_zonated()
      {
         Constants.Parameters.AllBooleanParameters.ShouldContain(Constants.Parameters.IS_LIVER_ZONATED);
      }
   }
}