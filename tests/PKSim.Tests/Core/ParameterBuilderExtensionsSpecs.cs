using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterBuilderExtensions : StaticContextSpecification
   {
      protected IParameter _para1;
      protected IParameter _para2;
      protected IEnumerable<IParameter> _parameterBuilders;

      protected override void Context()
      {
         _para1 = new PKSimParameter();
         _para2 = new PKSimParameter();
         _parameterBuilders = new List<IParameter>
                                 {
                                    _para1,
                                    _para2
                                 };
      }
   }

   
   public class When_retrieving_the_pksim_parameters_defined_in_a_parameter_builder : concern_for_ParameterBuilderExtensions
   {
      [Observation]
      public void should_return_the_underlying_parameters_template()
      {
         _parameterBuilders.ShouldOnlyContainInOrder(_para1, _para2);
      }
   }
}