using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core;

public abstract class concern_for_OverwriteParameterSetTask : ContextSpecification<OverwriteParameterSetTask>
{
   protected IExecutionContext _executionContext;
   protected Compound _compound;
   protected OverwriteParameterSet _overwriteParameterSet;
   protected const string _path = "Organism|Aspirin|Lipophilicity";

   protected override void Context()
   {
      _executionContext = A.Fake<IExecutionContext>();
      _compound = new Compound { Name = "Aspirin", Id = "CompId" };
      _overwriteParameterSet = new OverwriteParameterSet { Name = "MySet", Id = "SetId" };
      _overwriteParameterSet.Add(new ParameterValue { Path = _path.ToObjectPath(), Value = 1.0 });
      _compound.AddOverwriteParameterSet(_overwriteParameterSet);

      A.CallTo(() => _executionContext.BuildingBlockContaining(_compound)).Returns(_compound);
      A.CallTo(() => _executionContext.Get<Compound>(_compound.Id)).Returns(_compound);
      A.CallTo(() => _executionContext.Get<OverwriteParameterSet>(_overwriteParameterSet.Id)).Returns(_overwriteParameterSet);

      sut = new OverwriteParameterSetTask(_executionContext);
   }
}

public class When_updating_a_parameter_value_through_the_task : concern_for_OverwriteParameterSetTask
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.UpdateParameterValue(_overwriteParameterSet, _compound, _path, 9.0);
   }

   [Observation]
   public void should_update_the_value_in_the_set()
   {
      _overwriteParameterSet.ParameterValueByPath(_path).Value.ShouldBeEqualTo(9.0);
   }

   [Observation]
   public void should_return_a_non_empty_command()
   {
      _result.IsEmpty().ShouldBeFalse();
   }
}

public class When_updating_a_parameter_value_with_the_same_value : concern_for_OverwriteParameterSetTask
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.UpdateParameterValue(_overwriteParameterSet, _compound, _path, 1.0);
   }

   [Observation]
   public void should_return_an_empty_command()
   {
      _result.IsEmpty().ShouldBeTrue();
   }
}

public class When_updating_a_parameter_value_for_a_path_not_in_the_set : concern_for_OverwriteParameterSetTask
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.UpdateParameterValue(_overwriteParameterSet, _compound, "Organism|Aspirin|Permeability", 9.0);
   }

   [Observation]
   public void should_return_an_empty_command()
   {
      _result.IsEmpty().ShouldBeTrue();
   }
}

public class When_removing_a_parameter_value_through_the_task : concern_for_OverwriteParameterSetTask
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.RemoveParameterValue(_overwriteParameterSet, _compound, _path);
   }

   [Observation]
   public void should_remove_the_parameter_from_the_set()
   {
      _overwriteParameterSet.ParameterValueByPath(_path).ShouldBeNull();
   }

   [Observation]
   public void should_return_a_non_empty_command()
   {
      _result.IsEmpty().ShouldBeFalse();
   }
}

public class When_removing_a_parameter_value_for_a_path_not_in_the_set : concern_for_OverwriteParameterSetTask
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.RemoveParameterValue(_overwriteParameterSet, _compound, "Organism|Aspirin|Permeability");
   }

   [Observation]
   public void should_return_an_empty_command()
   {
      _result.IsEmpty().ShouldBeTrue();
   }
}
