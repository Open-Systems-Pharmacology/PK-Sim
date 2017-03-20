using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_CompoundProcessParameterMappingRepository : ContextForIntegration<ICompoundProcessParameterMappingRepository>
   {
      protected override void Context()
      {
         sut = IoC.Resolve <ICompoundProcessParameterMappingRepository>();
      }
   }

   
   public class when_getting_parameter_mappings : concern_for_CompoundProcessParameterMappingRepository
   {
      private IEnumerable<ICompoundProcessParameterMapping> _result;

      protected override void Context()
      {
         base.Context();
         _result = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_element()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void should_return_valid_path_for_existing_process_parameter()
      {
         var mapping1 = _result.ElementAt(0);

         var path = sut.MappedParameterPathFor(mapping1.ProcessName, mapping1.ParameterName);

         string.IsNullOrEmpty(path.PathAsString).ShouldBeFalse();
      }

      [Observation]
      public void should_throw_exception_for_nonexisting_mapping()
      {
         The.Action(() => sut.MappedParameterPathFor("Trululu","Tralala")).ShouldThrowAn<Exception>();
      }
   }
}	