using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Comparison;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;
using PKSim.Presentation.Mappers;

namespace PKSim.Presentation;

public abstract class concern_for_PKSimDiffItemToDiffItemDTOMapper : ContextSpecification<PKSimDiffItemToDiffItemDTOMapper>
{
   protected IPathAndValueEntityToPathElementsMapper _pathAndValueEntityToPathElementsMapper;
   protected IDisplayNameProvider _displayNameProvider;
   protected OverwriteParameterSet _overwriteParameterSet;
   protected ParameterValue _parameterValue;
   protected PathElements _parameterValuePathElements;
   protected DiffItem _diffItem;
   protected DiffItemDTO _dto;

   protected override void Context()
   {
      _pathAndValueEntityToPathElementsMapper = A.Fake<IPathAndValueEntityToPathElementsMapper>();
      _displayNameProvider = A.Fake<IDisplayNameProvider>();
      _overwriteParameterSet = new OverwriteParameterSet { Name = "Set A" };
      _parameterValue = new ParameterValue { Path = "Neighborhoods|Organ|C1|P".ToObjectPath() };
      _parameterValuePathElements = new PathElements();
      A.CallTo(() => _pathAndValueEntityToPathElementsMapper.MapFrom(_parameterValue)).Returns(_parameterValuePathElements);
      sut = new PKSimDiffItemToDiffItemDTOMapper(A.Fake<IPathToPathElementsMapper>(), _displayNameProvider, _pathAndValueEntityToPathElementsMapper);
   }

   protected override void Because()
   {
      _dto = sut.MapFrom(_diffItem);
   }
}

public class When_mapping_a_difference_of_a_parameter_value_defined_in_an_overwrite_parameter_set : concern_for_PKSimDiffItemToDiffItemDTOMapper
{
   protected override void Context()
   {
      base.Context();
      _diffItem = new PropertyValueDiffItem
      {
         Object1 = _parameterValue,
         Object2 = new ParameterValue { Path = _parameterValue.Path },
         CommonAncestor = _overwriteParameterSet
      };
   }

   [Observation]
   public void should_prefix_the_object_name_with_the_name_of_the_set()
   {
      _dto.ObjectName.ShouldBeEqualTo("Set A: P");
   }
}

public class When_mapping_a_parameter_value_missing_from_an_overwrite_parameter_set : concern_for_PKSimDiffItemToDiffItemDTOMapper
{
   protected override void Context()
   {
      base.Context();
      _diffItem = new MissingDiffItem
      {
         Object1 = _overwriteParameterSet,
         Object2 = new OverwriteParameterSet { Name = "Set A" },
         MissingObject1 = _parameterValue,
         MissingObjectName = _parameterValue.Path.ToString(),
         CommonAncestor = _overwriteParameterSet
      };
   }

   [Observation]
   public void should_show_the_name_of_the_parameter_value_prefixed_with_the_name_of_the_set()
   {
      _dto.ObjectName.ShouldBeEqualTo("Set A: P");
   }

   [Observation]
   public void should_show_the_path_of_the_parameter_value()
   {
      _dto.PathElements.ShouldBeEqualTo(_parameterValuePathElements);
   }
}

public class When_mapping_a_difference_of_an_extended_property_defined_in_an_overwrite_parameter_set : concern_for_PKSimDiffItemToDiffItemDTOMapper
{
   protected override void Context()
   {
      base.Context();
      _diffItem = new PropertyValueDiffItem
      {
         Object1 = new ExtendedProperty<string> { Name = "Author", Value = "A" },
         Object2 = new ExtendedProperty<string> { Name = "Author", Value = "B" },
         CommonAncestor = _overwriteParameterSet
      };
   }

   [Observation]
   public void should_prefix_the_object_name_with_the_name_of_the_set()
   {
      _dto.ObjectName.ShouldBeEqualTo("Set A: Author");
   }
}

public class When_mapping_a_type_mismatch_of_an_extended_property_defined_in_an_overwrite_parameter_set : concern_for_PKSimDiffItemToDiffItemDTOMapper
{
   protected override void Context()
   {
      base.Context();
      A.CallTo(() => _displayNameProvider.DisplayNameFor(_overwriteParameterSet)).Returns("Set A");
      _diffItem = new MismatchDiffItem
      {
         Object1 = new ExtendedProperty<string> { Name = "Author" },
         Object2 = new ExtendedProperty<double> { Name = "Author" },
         CommonAncestor = _overwriteParameterSet
      };
   }

   [Observation]
   public void should_show_the_name_of_the_set_only_once()
   {
      _dto.ObjectName.ShouldBeEqualTo("Set A");
   }
}

public class When_mapping_a_difference_that_is_not_defined_in_an_overwrite_parameter_set : concern_for_PKSimDiffItemToDiffItemDTOMapper
{
   protected override void Context()
   {
      base.Context();
      var compound = new Compound().WithName("C1");
      A.CallTo(() => _displayNameProvider.DisplayNameFor(_overwriteParameterSet)).Returns("Set A");
      _diffItem = new PropertyValueDiffItem
      {
         Object1 = _overwriteParameterSet,
         Object2 = new OverwriteParameterSet { Name = "Set A" },
         CommonAncestor = compound
      };
   }

   [Observation]
   public void should_leave_the_object_name_unchanged()
   {
      _dto.ObjectName.ShouldBeEqualTo("Set A");
   }
}
