using System.Collections.Generic;
using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterChangeUpdater : ContextSpecification<IParameterChangeUpdater>
   {
      protected IBuildingBlockRepository _buildingBlockRepository;
      protected IBuildingBlockRetriever _buildingBockRetriever;
      protected IObservedDataRepository _observedDataRepository;
      protected List<Compound> _compounds;
      protected Compound _compound;
      protected List<DataRepository> _observedDataList;

      protected override void Context()
      {
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _buildingBockRetriever = A.Fake<IBuildingBlockRetriever>();
         _observedDataRepository = A.Fake<IObservedDataRepository>();
         sut = new ParameterChangeUpdater(_buildingBockRetriever, _observedDataRepository, _buildingBlockRepository);

         _compounds = new List<Compound>();
         A.CallTo(() => _buildingBlockRepository.All<Compound>()).Returns(_compounds);
         _observedDataList = new List<DataRepository>();
         A.CallTo(() => _observedDataRepository.All()).Returns(_observedDataList);

         _compound = A.Fake<Compound>().WithName("C");
         A.CallTo(() => _compound.MolWeight).Returns(200);
      }
   }

   public class When_updating_the_molweight_value_of_an_observed_data : concern_for_ParameterChangeUpdater
   {
      private DataRepository _observedData;

      protected override void Context()
      {
         base.Context();
         _observedData = DomainHelperForSpecs.ObservedData();
         _observedData.ExtendedProperties.Add(new ExtendedProperty<string> {Name = Constants.ObservedData.MOLECULE, Value = "C"});
         _observedData.AllButBaseGrid().Each(c => c.DataInfo.MolWeight = 100);
         _compounds.Add(_compound);
      }

      protected override void Because()
      {
         sut.UpdateMolWeightIn(_observedData);
      }

      [Observation]
      public void should_retrieve_the_corresponding_compound_building_block_and_update_the_mol_weight_of_all_columns()
      {
         _observedData.AllButBaseGrid().Each(c => c.DataInfo.MolWeight.ShouldBeEqualTo(200));
      }
   }

   public class When_updating_the_depedencies_on_a_mol_weight_parameter_defined_in_a_compound_building_block : concern_for_ParameterChangeUpdater
   {
      private IParameter _parameter;
      private DataRepository _observedDataForCompound;
      private DataRepository _observedDataForNotCompound;

      protected override void Context()
      {
         base.Context();
         _parameter = A.Fake<IParameter>().WithName(Constants.Parameters.MOL_WEIGHT);
         A.CallTo(() => _buildingBockRetriever.BuildingBlockContaining(_parameter)).Returns(_compound);

         _observedDataForCompound = DomainHelperForSpecs.ObservedData();
         _observedDataForCompound.ExtendedProperties.Add(new ExtendedProperty<string> {Name = Constants.ObservedData.MOLECULE, Value = "C"});
         _observedDataForCompound.AllButBaseGrid().Each(c => c.DataInfo.MolWeight = 100);

         _observedDataForNotCompound = DomainHelperForSpecs.ObservedData();
         _observedDataForNotCompound.ExtendedProperties.Add(new ExtendedProperty<string> {Name = Constants.ObservedData.MOLECULE, Value = "C2"});
         _observedDataForNotCompound.AllButBaseGrid().Each(c => c.DataInfo.MolWeight = 100);

         _observedDataList.AddRange(new[] {_observedDataForCompound, _observedDataForNotCompound});
      }

      protected override void Because()
      {
         sut.UpdateObjectsDependingOn(_parameter);
      }

      [Observation]
      public void should_update_the_mol_weight_values_of_all_observed_data_defined_for_this_compound()
      {
         _observedDataForCompound.AllButBaseGrid().Each(c => c.DataInfo.MolWeight.ShouldBeEqualTo(200));
      }

      [Observation]
      public void should_not_update_the_mol_weight_values_of_observed_data_defined_for_another_compound()
      {
         _observedDataForNotCompound.AllButBaseGrid().Each(c => c.DataInfo.MolWeight.ShouldBeEqualTo(100));
      }
   }

   public class When_updating_the_depedencies_on_a_mol_weight_parameter_defined_in_simulation : concern_for_ParameterChangeUpdater
   {
      private IParameter _parameter;
      private DataRepository _observedData;

      protected override void Context()
      {
         base.Context();
         _parameter = A.Fake<IParameter>().WithName(Constants.Parameters.MOL_WEIGHT);
         A.CallTo(() => _buildingBockRetriever.BuildingBlockContaining(_parameter)).Returns(null);

         _observedData = DomainHelperForSpecs.ObservedData();
         _observedData.ExtendedProperties.Add(new ExtendedProperty<string> {Name = Constants.ObservedData.MOLECULE, Value = "C"});
         _observedData.AllButBaseGrid().Each(c => c.DataInfo.MolWeight = 100);


         _observedDataList.AddRange(new[] {_observedData});
      }

      protected override void Because()
      {
         sut.UpdateObjectsDependingOn(_parameter);
      }

      [Observation]
      public void should_not_update_the_mol_weight_values_of_observed_data()
      {
         _observedData.AllButBaseGrid().Each(c => c.DataInfo.MolWeight.ShouldBeEqualTo(100));
      }
   }
}