using System.Linq;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Snapshots;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Model;
using ModelOverwriteParameterSet = PKSim.Core.Model.OverwriteParameterSet;
using ModelParameterValue = OSPSuite.Core.Domain.Builder.ParameterValue;
using OverwriteParameterSetMapper = PKSim.Core.Snapshots.Mappers.OverwriteParameterSetMapper;
using ParameterValueMapper = PKSim.Core.Snapshots.Mappers.ParameterValueMapper;
using SnapshotOverwriteParameterSet = PKSim.Core.Snapshots.OverwriteParameterSet;

namespace PKSim.Core
{
   public abstract class concern_for_OverwriteParameterSetMapper : ContextSpecificationAsync<OverwriteParameterSetMapper>
   {
      protected ModelOverwriteParameterSet _modelOverwriteParameterSet;
      protected SnapshotOverwriteParameterSet _snapshot;
      protected ParameterValueMapper _parameterValueMapper;
      protected ExtendedPropertyMapper _extendedPropertyMapper;

      protected override Task Context()
      {
         _parameterValueMapper = new ParameterValueMapper();
         _extendedPropertyMapper = new ExtendedPropertyMapper();
         sut = new OverwriteParameterSetMapper(_parameterValueMapper, _extendedPropertyMapper);

         _modelOverwriteParameterSet = new ModelOverwriteParameterSet
         {
            Name = "MyParameterSet",
            IsDefault = true
         };

         _modelOverwriteParameterSet.ExtendedProperties.Add(new ExtendedProperty<string> { Name = "Species", Value = "Human" });
         _modelOverwriteParameterSet.ExtendedProperties.Add(new ExtendedProperty<string> { Name = "DiseaseState", Value = "Healthy" });
         _modelOverwriteParameterSet.Add(new ModelParameterValue { Path = "Compound|Lipophilicity".ToObjectPath(), Value = 1.5 });
         _modelOverwriteParameterSet.Add(new ModelParameterValue { Path = "Compound|Solubility".ToObjectPath(), Value = 0.5 });

         return _completed;
      }
   }

   public class When_mapping_an_overwrite_parameter_set_to_snapshot : concern_for_OverwriteParameterSetMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_modelOverwriteParameterSet);
      }

      [Observation]
      public void should_map_the_name()
      {
         _snapshot.Name.ShouldBeEqualTo("MyParameterSet");
      }

      [Observation]
      public void should_map_the_is_default_flag()
      {
         _snapshot.IsDefault.ShouldBeEqualTo(true);
      }

      [Observation]
      public void should_map_the_extended_properties()
      {
         _snapshot.ExtendedProperties.Length.ShouldBeEqualTo(2);
         _snapshot.ExtendedProperties[0].Name.ShouldBeEqualTo("Species");
         _snapshot.ExtendedProperties[0].Value.ShouldBeEqualTo("Human");
         _snapshot.ExtendedProperties[1].Name.ShouldBeEqualTo("DiseaseState");
         _snapshot.ExtendedProperties[1].Value.ShouldBeEqualTo("Healthy");
      }

      [Observation]
      public void should_map_the_parameter_values()
      {
         _snapshot.ParameterValues.Length.ShouldBeEqualTo(2);
         _snapshot.ParameterValues[0].Path.ShouldBeEqualTo("Compound|Lipophilicity");
         _snapshot.ParameterValues[0].Value.ShouldBeEqualTo(1.5);
         _snapshot.ParameterValues[1].Path.ShouldBeEqualTo("Compound|Solubility");
         _snapshot.ParameterValues[1].Value.ShouldBeEqualTo(0.5);
      }
   }

   public class When_mapping_an_overwrite_parameter_set_with_default_values_to_snapshot : concern_for_OverwriteParameterSetMapper
   {
      protected override Task Context()
      {
         _parameterValueMapper = new ParameterValueMapper();
         _extendedPropertyMapper = new ExtendedPropertyMapper();
         sut = new OverwriteParameterSetMapper(_parameterValueMapper, _extendedPropertyMapper);

         _modelOverwriteParameterSet = new ModelOverwriteParameterSet
         {
            Name = "EmptySet",
            IsDefault = false
         };

         return _completed;
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_modelOverwriteParameterSet);
      }

      [Observation]
      public void should_not_set_is_default_when_false()
      {
         _snapshot.IsDefault.ShouldBeNull();
      }

      [Observation]
      public void should_not_set_extended_properties_when_empty()
      {
         _snapshot.ExtendedProperties.ShouldBeNull();
      }

      [Observation]
      public void should_not_set_parameter_values_when_empty()
      {
         _snapshot.ParameterValues.ShouldBeNull();
      }
   }

   public class When_mapping_a_snapshot_to_an_overwrite_parameter_set : concern_for_OverwriteParameterSetMapper
   {
      private ModelOverwriteParameterSet _result;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_modelOverwriteParameterSet);
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot, new SnapshotContext(new PKSimProject(), SnapshotVersions.Current));
      }

      [Observation]
      public void should_set_the_name()
      {
         _result.Name.ShouldBeEqualTo("MyParameterSet");
      }

      [Observation]
      public void should_set_the_is_default_flag()
      {
         _result.IsDefault.ShouldBeTrue();
      }

      [Observation]
      public void should_restore_the_extended_properties()
      {
         _result.ExtendedProperties.Count().ShouldBeEqualTo(2);
         _result.ExtendedProperties["Species"].ValueAsObject.ShouldBeEqualTo("Human");
         _result.ExtendedProperties["DiseaseState"].ValueAsObject.ShouldBeEqualTo("Healthy");
      }

      [Observation]
      public void should_restore_the_parameter_values()
      {
         _result.ParameterValues.Count.ShouldBeEqualTo(2);
         _result.ParameterValueByPath("Compound|Lipophilicity").Value.ShouldBeEqualTo(1.5);
         _result.ParameterValueByPath("Compound|Solubility").Value.ShouldBeEqualTo(0.5);
      }
   }

   public class When_mapping_a_snapshot_with_null_collections_to_an_overwrite_parameter_set : concern_for_OverwriteParameterSetMapper
   {
      private ModelOverwriteParameterSet _result;

      protected override async Task Because()
      {
         _snapshot = new SnapshotOverwriteParameterSet
         {
            Name = "EmptySet"
         };

         _result = await sut.MapToModel(_snapshot, new SnapshotContext(new PKSimProject(), SnapshotVersions.Current));
      }

      [Observation]
      public void should_create_a_model_with_empty_collections()
      {
         _result.Name.ShouldBeEqualTo("EmptySet");
         _result.IsDefault.ShouldBeFalse();
         _result.ExtendedProperties.Count().ShouldBeEqualTo(0);
         _result.ParameterValues.Count.ShouldBeEqualTo(0);
      }
   }
}