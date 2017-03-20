using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using IContainer = OSPSuite.Core.Domain.IContainer;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ApplicationFactory : ContextForIntegration<IApplicationFactory>
   {
      protected const string COMPOUND_NAME = "Aspirin";
      protected IFormulaCache _formulaCache;
      protected ISchemaItem _schemaItemIV, _schemaItemOral;

      protected override void Context()
      {
         var schemaItemFactory = IoC.Resolve<ISchemaItemFactory>();
         _schemaItemIV = schemaItemFactory.Create(ApplicationTypes.Intravenous, new Container());
         _schemaItemOral = schemaItemFactory.Create(ApplicationTypes.Oral, new Container());
         _schemaItemIV.Dose.Value = 5;
         _schemaItemIV.StartTime.Value = 20;
         sut = IoC.Resolve<IApplicationFactory>();
      }
   }

   public class When_creating_an_application_for_a_given_application_type_and_formulation_type : concern_for_ApplicationFactory
   {
      private IApplicationBuilder _applicBuilder;
      private string _applicationName;

      protected override void Context()
      {
         base.Context();
         _applicationName = "AAA";
      }

      protected override void Because()
      {
         _applicBuilder = sut.CreateFor(_schemaItemIV, CoreConstants.Formulation.EmptyFormulation, _applicationName, COMPOUND_NAME, new List<IParameter>(), new FormulaCache());
      }

      protected IApplicationMoleculeBuilder Molecule
      {
         get { return _applicBuilder.Molecules.First(); }
      }

      protected IEnumerable<IContainer> AllApplicationContainers
      {
         get { return _applicBuilder.GetAllChildren<IContainer>().Union(new IContainer[] {_applicBuilder}); }
      }

      [Observation]
      public void schema_item_parameters_should_be_copied()
      {
         var parameterContainer = _applicBuilder.ProtocolSchemaItemContainer();
         foreach (var srcParam in _schemaItemIV.AllParameters())
         {
            var appliBuilderParam = parameterContainer.Parameter(srcParam.Name);
            if (appliBuilderParam != null)
               appliBuilderParam.Value.ShouldBeEqualTo(srcParam.Value);
         }
      }

      [Observation]
      public void drugmass_parameter_should_have_molecule_tag()
      {
         var parameterContainer = _applicBuilder.ProtocolSchemaItemContainer();
         var drugMassParameter = parameterContainer.Parameter(Constants.Parameters.DRUG_MASS);
         drugMassParameter?.Tags.Contains(ObjectPathKeywords.MOLECULE).ShouldBeTrue();
      }

      [Observation]
      public void molecule_name_should_be_set()
      {
         _applicBuilder.MoleculeName.ShouldBeEqualTo(COMPOUND_NAME);
      }

      [Observation]
      public void single_molecule_amount_should_be_created()
      {
         _applicBuilder.Molecules.Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void every_container_should_have_application_tag()
      {
         AllApplicationContainers.Each(c => c.Tags.Contains(CoreConstants.Tags.APPLICATION).ShouldBeTrue());
      }

      [Observation]
      public void only_root_application_container_should_have_application_root_tag()
      {
         _applicBuilder.Tags.Contains(CoreConstants.Tags.APPLICATION_ROOT).ShouldBeTrue();
         _applicBuilder.GetAllChildren<IContainer>().Each(
            c => c.Tags.Contains(CoreConstants.Tags.APPLICATION_ROOT).ShouldBeFalse());
      }
   }
}