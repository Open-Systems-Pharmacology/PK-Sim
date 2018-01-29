using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SchemaItemRepository : ContextForIntegration<ISchemaItemRepository>
   {
   }

   
   public class when_getting_schema_items_from_db:concern_for_SchemaItemRepository
   {
      private IEnumerable<ISchemaItem> _schemaItems;

      protected override void Because()
      {
         _schemaItems=sut.All();
         _schemaItems = sut.All();
      }

      [Observation]
      public void should_have_at_least_one_element()
      {
         _schemaItems.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void schema_item_should_contain_starttime_parameter()
      {
         _schemaItems.ShouldContainParameter(Constants.Parameters.START_TIME);
      }

      [Observation]
      public void schema_item_should_contain_dose_parameter()
      {
         _schemaItems.ShouldContainParameter(CoreConstants.Parameters.INPUT_DOSE);
      }

      [Observation]
      public void schema_item_should_not_contain_drugmass_parameter()
      {
         _schemaItems.ShouldNotContainParameter(Constants.Parameters.DRUG_MASS);
      }

      [Observation]
      public void schema_item_should_contain_only_parameter_as_children()
      {
         _schemaItems.Each(si => si.GetChildren<IParameter>().Count()
                                      .ShouldBeEqualTo(si.Children.Count()));
      }

      [Observation]
      public void application_type_should_be_contained_only_once()
      {
         _schemaItems.Each(
            si => _schemaItems.Count(si2 => si2.ApplicationType.Equals(si.ApplicationType)).ShouldBeEqualTo(1));
      }

      [Observation]
      public void parameters_should_have_the_accurate_building_block_type()
      {
         _schemaItems.Each(si=>
                              {
                                 si.Parameter(Constants.Parameters.START_TIME).BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Protocol);
                                 si.Parameter(CoreConstants.Parameters.INPUT_DOSE).BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Protocol);
                              }

            );
      }

      [Observation]
      public void protocol_schema_item_container_should_have_proper_name()
      {
         _schemaItems.Each(si => si.Name.ShouldBeEqualTo(CoreConstants.ContainerName.ProtocolSchemaItem));
      }
   }

   static class SchemaItemsExtensions
   {
      public static void ShouldContainParameter(this IEnumerable<ISchemaItem> schemaItems, string name)
      {
         schemaItems.Each(si => si.ContainsName(name).ShouldBeTrue());
      }

      public static void ShouldNotContainParameter(this IEnumerable<ISchemaItem> schemaItems, string name)
      {
         schemaItems.Each(si => si.ContainsName(name).ShouldBeFalse());
      }
   }

}	