using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.UnitSystem;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_PKValuesToPKAnalysisMapper : ContextSpecification<IPKValuesToPKAnalysisMapper>
   {
      protected IParameterFactory _parameterFactory;
      private IDimensionRepository _dimensionRepository;
      protected IPKParameterRepository _pkParameterRepository;
      protected IDisplayUnitRetriever _displayUnitRetriever;

      protected override void Context()
      {
         _parameterFactory = A.Fake<IParameterFactory>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _pkParameterRepository = A.Fake<IPKParameterRepository>();
         _displayUnitRetriever= A.Fake<IDisplayUnitRetriever>();
         sut = new PKValuesToPKAnalysisMapper(_parameterFactory,_dimensionRepository,_pkParameterRepository, _displayUnitRetriever);
      }
   }

   public class When_mapping_pk_values_to_pk_analysis : concern_for_PKValuesToPKAnalysisMapper
   {
      private DataColumn _col;
      private PKValues _pkValues;
      private PKParameterMode _mode;   
      private string _drugName;
      private PKAnalysis _result;
      private PKParameter _pk1;
      private PKParameter _pk2;
      private PKParameter _pk3;

      protected override void Context()
      {
         base.Context();
         _pkValues = new PKValues();
         _pkValues.AddValue("P1",10f);
         _pkValues.AddValue("P2", 10f);
         _mode=PKParameterMode.Single;
         _drugName = "TOTO";
         _pk1 = new PKParameter {Mode = _mode, Name = "P1", Dimension = DomainHelperForSpecs.ConcentrationDimensionForSpecs() };
         _pk2 = new PKParameter { Mode = _mode, Name = "P2", Dimension = DomainHelperForSpecs.ConcentrationDimensionForSpecs() };
         _pk3 = new PKParameter { Mode = PKParameterMode.Multi, Name = "P3" };
         var baseGrid = new BaseGrid("Time", DomainHelperForSpecs.TimeDimensionForSpecs());
         _col = new DataColumn("COL", DomainHelperForSpecs.ConcentrationDimensionForSpecs(), baseGrid) {DataInfo = {MolWeight = 150}};
         A.CallTo(() => _pkParameterRepository.All()).Returns(new [] {_pk1,_pk2, _pk3});
         A.CallTo(() => _parameterFactory.CreateFor(A<string>._, A<double>._, A<string>._, PKSimBuildingBlockType.Simulation))
            .ReturnsLazily(x => DomainHelperForSpecs.ConstantParameterWithValue(x.GetArgument<double>(1)).WithName(x.GetArgument<string>(0)));

      }

      protected override void Because()
      {
         _result = sut.MapFrom(_col, _pkValues, _mode, _drugName);
      }

      [Observation]
      public void should_create_one_parameter_for_each_available_pk_value_available()
      {
         _result.PKParameters(_pk1.Name).Any().ShouldBeTrue();
         _result.PKParameters(_pk2.Name).Any().ShouldBeTrue();
         _result.PKParameters(_pk3.Name).Any().ShouldBeFalse();
      }

      [Observation]
      public void should_clear_all_the_default_rules_of_the_created_parameters_as_they_do_not_apply_for_pk_parameters()
      {
         _result.AllPKParameters.Each(p => p.Rules.IsEmpty.ShouldBeTrue());
      }

      [Observation]
      public void should_have_set_the_name_of_the_pk_analysis_to_the_name_of_the_molecule()
      {
         _result.Name.ShouldBeEqualTo(_drugName);
      }

      [Observation]
      public void should_have_set_the_molweight_of_the_pk_analysis_to_the_molweight_of_the_column_used_for_the_calculation()
      {
         _result.MolWeight.ShouldBeEqualTo(_col.DataInfo.MolWeight);
      }

      [Observation]
      public void should_have_used_the_display_unit_manager_to_retrieve_the_default_display_unit_for_all_pk_parameters()
      {
         _result.AllPKParameters.Each(p =>
         {
            A.CallTo(() => _displayUnitRetriever.PreferredUnitFor(p)).MustHaveHappened();
         });
      }
   }
}	