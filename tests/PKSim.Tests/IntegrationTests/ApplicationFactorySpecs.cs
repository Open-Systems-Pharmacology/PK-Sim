using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using IContainer = OSPSuite.Core.Domain.IContainer;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ApplicationFactory : ContextForIntegration<IApplicationFactory>
   {
      protected const string COMPOUND_NAME = "Aspirin";
      protected IFormulaCache _formulaCache;
      protected ISchemaItem _schemaItemIV, _schemaItemOral;
      protected string _applicationName;
      protected IApplicationBuilder _applicationBuilder;
      protected IContainer ApplicationParameterContainer => _applicationBuilder.ProtocolSchemaItemContainer();

      public override void GlobalContext()
      {
         base.GlobalContext();
         var schemaItemFactory = IoC.Resolve<ISchemaItemFactory>();
         _schemaItemIV = schemaItemFactory.Create(ApplicationTypes.Intravenous, new Container());
         _schemaItemOral = schemaItemFactory.Create(ApplicationTypes.Oral, new Container());
         _schemaItemIV.Dose.Value = 5;
         _schemaItemIV.StartTime.Value = 20;
         _schemaItemIV.Dose.DisplayUnit = _schemaItemIV.Dose.Dimension.Unit(CoreConstants.Units.mg);
         sut = IoC.Resolve<IApplicationFactory>();
         _applicationName = "AAA";
         _formulaCache = new FormulaCache();
      }

      protected override void Context()
      {
      }

      protected void DoseParameterShouldBeHidden(IParameter doseParameter)
      {
         doseParameter.Visible.ShouldBeFalse();
         doseParameter.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Simulation);
      }
   }

   public class When_creating_an_application_for_a_given_application_type_and_formulation_type_in_dose_per_mg : concern_for_ApplicationFactory
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _applicationBuilder = sut.CreateFor(_schemaItemIV, CoreConstants.Formulation.EmptyFormulation, _applicationName, COMPOUND_NAME, new List<IParameter>(), _formulaCache);
      }

      protected IApplicationMoleculeBuilder Molecule => _applicationBuilder.Molecules.First();

      protected IEnumerable<IContainer> AllApplicationContainers => _applicationBuilder.GetAllChildren<IContainer>().Union(new IContainer[] {_applicationBuilder});

      [Observation]
      public void schema_item_parameters_should_be_copied()
      {
         var parameterContainer = _applicationBuilder.ProtocolSchemaItemContainer();
         foreach (var srcParam in _schemaItemIV.AllParameters())
         {
            var appliBuilderParam = parameterContainer.Parameter(srcParam.Name);
            appliBuilderParam?.Value.ShouldBeEqualTo(srcParam.Value);
         }
      }

      [Observation]
      public void drugmass_parameter_should_have_molecule_tag()
      {
         var parameterContainer = _applicationBuilder.ProtocolSchemaItemContainer();
         var drugMassParameter = parameterContainer.Parameter(Constants.Parameters.DRUG_MASS);
         drugMassParameter?.Tags.Contains(ObjectPathKeywords.MOLECULE).ShouldBeTrue();
      }

      [Observation]
      public void molecule_name_should_be_set()
      {
         _applicationBuilder.MoleculeName.ShouldBeEqualTo(COMPOUND_NAME);
      }

      [Observation]
      public void single_molecule_amount_should_be_created()
      {
         _applicationBuilder.Molecules.Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void every_container_should_have_application_tag()
      {
         AllApplicationContainers.Each(c => c.Tags.Contains(CoreConstants.Tags.APPLICATION).ShouldBeTrue());
      }

      [Observation]
      public void only_root_application_container_should_have_application_root_tag()
      {
         _applicationBuilder.Tags.Contains(CoreConstants.Tags.APPLICATION_ROOT).ShouldBeTrue();
         _applicationBuilder.GetAllChildren<IContainer>().Each(
            c => c.Tags.Contains(CoreConstants.Tags.APPLICATION_ROOT).ShouldBeFalse());
      }

      [Observation]
      public void should_define_the_dose_parameter_as_a_constant_formula()
      {
         var dose = ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE);
         dose.Formula.IsConstant().ShouldBeTrue();
      }

      [Observation]
      public void should_hide_the_dose_per_body_surface_area_parameter()
      {
         DoseParameterShouldBeHidden(ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE_PER_BODY_SURFACE_AREA));
      }

      [Observation]
      public void should_hide_the_dose_per_body_weight_parameter()
      {
         DoseParameterShouldBeHidden(ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE_PER_BODY_WEIGHT));
      }

      [Observation]
      public void should_update_the_dose_parameter_value()
      {
         ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE).Value.ShouldBeEqualTo(_schemaItemIV.Dose.Value);
      }

      [Observation]
      public void the_value_of_dose_per_body_weight_should_be_zero()
      {
         ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE_PER_BODY_WEIGHT).Value.ShouldBeEqualTo(0);
      }

      [Observation]
      public void the_value_of_dose_per_body_surface_aera__should_be_zero()
      {
         ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE_PER_BODY_SURFACE_AREA).Value.ShouldBeEqualTo(0);
      }
   }

   public class When_creating_an_application_based_on_a_protocol_defined_in_dose_per_body_surface_area : concern_for_ApplicationFactory
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _schemaItemIV.Dose.DisplayUnit = _schemaItemIV.Dose.Dimension.Unit(CoreConstants.Units.MgPerM2);
         _applicationBuilder = sut.CreateFor(_schemaItemIV, CoreConstants.Formulation.EmptyFormulation, _applicationName, COMPOUND_NAME, new List<IParameter>(), _formulaCache);
      }

      [Observation]
      public void should_define_the_dose_parameter_as_a_function_of_body_surface_area_and_dose_per_body_surface_area()
      {
         var dose = ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE);
         dose.Formula.IsExplicit().ShouldBeTrue();
         dose.Formula.ObjectPaths.Select(x=>x.Alias).ShouldContain(CoreConstants.Parameters.BSA, CoreConstants.Parameters.DOSE_PER_BODY_SURFACE_AREA);
      }

      [Observation]
      public void should_hide_the_dose_parameter()
      {
         DoseParameterShouldBeHidden(ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE));
      }

      [Observation]
      public void should_hide_the_dose_per_body_weight_parameter()
      {
         DoseParameterShouldBeHidden(ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE_PER_BODY_WEIGHT));
      }

      [Observation]
      public void should_update_the_dose_per_body_surface_parameter_value()
      {
         ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE_PER_BODY_SURFACE_AREA).Value.ShouldBeEqualTo(_schemaItemIV.Dose.Value);
      }

      [Observation]
      public void the_value_of_dose_per_body_weight_should_be_zero()
      {
         ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE_PER_BODY_WEIGHT).Value.ShouldBeEqualTo(0);
      }
   }

   public class When_creating_an_application_based_on_a_protocol_defined_in_dose_per_body_weight : concern_for_ApplicationFactory
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _schemaItemIV.Dose.DisplayUnit = _schemaItemIV.Dose.Dimension.Unit(CoreConstants.Units.MgPerKg);
         _applicationBuilder = sut.CreateFor(_schemaItemIV, CoreConstants.Formulation.EmptyFormulation, _applicationName, COMPOUND_NAME, new List<IParameter>(), _formulaCache);
      }

      [Observation]
      public void should_define_the_dose_parameter_as_a_function_of_body_weight_and_dose_per_body_weight()
      {
         var dose = ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE);
         dose.Formula.IsExplicit().ShouldBeTrue();
         dose.Formula.ObjectPaths.Select(x => x.Alias).ShouldContain("BW", CoreConstants.Parameters.DOSE_PER_BODY_WEIGHT);
      }

      [Observation]
      public void should_hide_the_dose_parameter()
      {
         DoseParameterShouldBeHidden(ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE));
      }

      [Observation]
      public void should_hide_the_dose_per_body_surface_area_parameter()
      {
         DoseParameterShouldBeHidden(ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE_PER_BODY_SURFACE_AREA));
      }

      [Observation]
      public void should_update_the_dose_per_body_weight_parameter_value()
      {
         ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE_PER_BODY_WEIGHT).Value.ShouldBeEqualTo(_schemaItemIV.Dose.Value);
      }

      [Observation]
      public void the_value_of_dose_per_body_surface_area_should_be_zero()
      {
         ApplicationParameterContainer.Parameter(CoreConstants.Parameters.DOSE_PER_BODY_SURFACE_AREA).Value.ShouldBeEqualTo(0);
      }

   }
}