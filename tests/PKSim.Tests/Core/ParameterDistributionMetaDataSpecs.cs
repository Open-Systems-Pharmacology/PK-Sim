using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterDistributionMetaData : ContextSpecification<ParameterDistributionMetaData>
   {
      protected override void Context()
      {
         sut = new ParameterDistributionMetaData();
         sut.Age = 10;
         sut.BuildMode = ParameterBuildMode.Local;
         sut.BuildingBlockType = PKSimBuildingBlockType.Individual;
         sut.CanBeVaried = false;
         sut.ContainerId = 125;
         sut.ContainerName = "Liver";
         sut.ContainerType = "Compartment";
         sut.DefaultUnit = "ng";
         sut.Deviation = 15;
         sut.Dimension = "mass";
         sut.Distribution = DistributionTypes.LogNormal;
         sut.Gender = "male";
         sut.GestationalAge = 25;
         sut.GroupName = "para";
         sut.ReadOnly = false;
         sut.Mean = 25;
      }
   }

   
   public class When_creating_a_parameter_meta_data_from_another_one : concern_for_ParameterDistributionMetaData
   {
      private ParameterDistributionMetaData _copy;

      protected override void Because()
      {
         _copy = ParameterDistributionMetaData.From(sut);
      }

      [Observation]
      public void should_create_a_shallow_copy_of_the_source_distribution_meta_data()
      {
         _copy.Age.ShouldBeEqualTo(sut.Age);
         _copy.BuildMode.ShouldBeEqualTo(sut.BuildMode);
         _copy.CanBeVaried.ShouldBeEqualTo(sut.CanBeVaried);
         _copy.ContainerId.ShouldBeEqualTo(sut.ContainerId);
         _copy.ContainerName.ShouldBeEqualTo(sut.ContainerName);
         _copy.ContainerType.ShouldBeEqualTo(sut.ContainerType);
         _copy.DefaultUnit.ShouldBeEqualTo(sut.DefaultUnit);
         _copy.Deviation.ShouldBeEqualTo(sut.Deviation);
         _copy.Dimension.ShouldBeEqualTo(sut.Dimension);
         _copy.Distribution.ShouldBeEqualTo(sut.Distribution);
         _copy.DistributionType.ShouldBeEqualTo(sut.DistributionType);
         _copy.Gender.ShouldBeEqualTo(sut.Gender);
         _copy.GestationalAge.ShouldBeEqualTo(sut.GestationalAge);
         _copy.GroupName.ShouldBeEqualTo(sut.GroupName);
         _copy.ReadOnly.ShouldBeEqualTo(sut.ReadOnly);
         _copy.Mean.ShouldBeEqualTo(sut.Mean);
      }
   }
}