using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using ICoreUserSettings = PKSim.Core.ICoreUserSettings;

namespace PKSim.IntegrationTests
{
   [NightlyOnly]
   public abstract class concern_for_TemplateTaskQuery : ContextForIntegration<ITemplateTaskQuery>
   {
      private string _tmpFile;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var userSettings = IoC.Resolve<ICoreUserSettings>();
         var templateDatabasePath = DomainHelperForSpecs.UserTemplateDatabasePath();
         _tmpFile = FileHelper.GenerateTemporaryFileName();
         FileHelper.Copy(templateDatabasePath, _tmpFile);
         userSettings.TemplateDatabasePath = _tmpFile;
         sut = IoC.Resolve<ITemplateTaskQuery>();
      }

      protected override void Context()
      {
         /*nothing to do here*/
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();

         //there seem to be some issue when deleting a file. It seems tha tthe connection is not always release as expected
         try
         {
            FileHelper.DeleteFile(_tmpFile);
         }
         catch (Exception)
         {
            try
            {
               //try one more time
               Thread.Sleep(5000);
               FileHelper.DeleteFile(_tmpFile);
            }
            catch
            {
               /*do nothing*/
            }
         }
      }
   }

   public class When_loading_building_blocks_from_system_templates : concern_for_TemplateTaskQuery
   {
      private IList<Individual> _individuals;
      private IList<Compound> _compounds;
      private IList<Formulation> _formulations;
      private IList<Protocol> _protocols;

      public override void GlobalContext()
      {
         base.GlobalContext();

         _individuals = loadSystemTemplates<Individual>(TemplateType.Individual);
         _compounds = loadSystemTemplates<Compound>(TemplateType.Compound);
         _formulations = loadSystemTemplates<Formulation>(TemplateType.Formulation);
         _protocols = loadSystemTemplates<Protocol>(TemplateType.Protocol);
      }

      private IList<T> loadSystemTemplates<T>(TemplateType templateType)
         where T: PKSimBuildingBlock
      {
         var templates = sut.AllTemplatesFor(TemplateDatabaseType.System, templateType);

         IList<T> buildingBlocks = new List<T>();
         foreach (var template in templates)
         {
            buildingBlocks.Add(sut.LoadTemplate<T>(template));
         }

         return buildingBlocks;
      }

      private void checkNewProteinModelStructure(Individual individual)
      {
         //just check that some of new parameters were added
         individual.Organism.Organ(CoreConstants.Organ.Bone)
            .AllParameters().Count(p => p.Name.EndsWith("flow proportionality factor")).ShouldBeEqualTo(2);
      }

      private void checkThatAllTemplatesAreCreatedAtLeastWithVersion(IEnumerable<PKSimBuildingBlock> buildingBlocks, ProjectVersion minVersion)
      {
         foreach (var buildingBlock in buildingBlocks)
         {
            var internalVersion = buildingBlock.Creation.InternalVersion;
            internalVersion.HasValue.ShouldBeTrue();
            internalVersion?.ShouldBeGreaterThanOrEqualTo(minVersion.Version);
         }
      }

      private void checkDefaultAlternative(Compound compound, string groupName, string defaultAlternativeName)
      {
         foreach (var alternative in compound.ParameterAlternativeGroup(groupName).AllAlternatives)
         {
            if (alternative.Name.Equals(defaultAlternativeName))
            {
               alternative.IsDefault.ShouldBeTrue($"{alternative} must be default");
               continue;
            }
            alternative.IsDefault.ShouldBeFalse($"{alternative} may not be default");
         }
      }

      private void checkParameter(IParameter parameter, double expectedValue, string expectedUnit = "")
      {
         parameter.Value.ShouldBeEqualTo(expectedValue);

         if (string.IsNullOrEmpty(expectedUnit))
            return;

         parameter.DisplayUnit.Name.ShouldBeEqualTo(expectedUnit);
      }

      private void checkAlternativeParameter(Compound compound, string groupName, string alternativeName,
         string parameterName, double expectedValue, string expectedUnit = "")
      {
         var alternative = compound.ParameterAlternativeGroup(groupName).AlternativeByName(alternativeName);
         checkParameter(alternative.Parameter(parameterName),expectedValue,expectedUnit);
      }

      private void checkCalculationMethod(Compound compound, string category, string expectedCalculationMethod)
      {
         compound.CalculationMethodCache.CalculationMethodFor(category)
            .Name.ShouldBeEqualTo(expectedCalculationMethod);
      }

      private void checkProcessParameter(Compound compound, string processName,
         string parameterName, double expectedValue, string expectedUnit = "")
      {
         checkParameter(compound.ProcessByName(processName).Parameter(parameterName), expectedValue, expectedUnit);
      }

      [Observation]
      public void loaded_individuals_should_have_updated_protein_model_structure()
      {
         _individuals.Count.ShouldBeGreaterThan(0);
         _individuals.Each(checkNewProteinModelStructure);
      }

      [Observation]
      public void all_individual_templates_should_be_created_with_at_least_version_7_3()
      {
         checkThatAllTemplatesAreCreatedAtLeastWithVersion(_individuals, ProjectVersions.V7_3_0);
      }

      [Observation]
      public void all_compound_templates_should_be_created_with_at_least_version_7_3()
      {
         checkThatAllTemplatesAreCreatedAtLeastWithVersion(_compounds, ProjectVersions.V7_3_0);
      }

      [Observation]
      public void all_formulation_templates_should_be_created_with_at_least_version_7_3()
      {
         checkThatAllTemplatesAreCreatedAtLeastWithVersion(_formulations, ProjectVersions.V7_3_0);
      }

      [Observation]
      public void all_protocol_templates_should_be_created_with_at_least_version_7_3()
      {
         checkThatAllTemplatesAreCreatedAtLeastWithVersion(_protocols, ProjectVersions.V7_3_0);
      }

      [Observation]
      public void system_database_should_contain_expected_number_of_templates()
      {
         _individuals.Count.ShouldBeEqualTo(1);
         _compounds.Count.ShouldBeEqualTo(14);
         _formulations.Count.ShouldBeEqualTo(1);
         _protocols.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void system_database_should_contain_new_templates()
      {
         _compounds.FindByName("Digoxin").ShouldNotBeNull();
      }

      [Observation]
      public void check_compound_template_fixes_for_7_4()
      {
         var permeabilityGroup = CoreConstants.Groups.COMPOUND_PERMEABILITY;
         var lipophilicityGroup = CoreConstants.Groups.COMPOUND_LIPOPHILICITY;
         var intestinalPermeabilityGroup = CoreConstants.Groups.COMPOUND_INTESTINAL_PERMEABILITY;

         //---- Midazolam
         var midazolam = _compounds.FindByName("Midazolam");
         checkCalculationMethod(midazolam, CoreConstants.Category.DistributionCellular, CoreConstants.CalculationMethod.RodgerAndRowland);
         checkDefaultAlternative(midazolam, intestinalPermeabilityGroup, "Optimization");

         //---- Digoxin
         var digoxin = _compounds.FindByName("Digoxin");

         checkAlternativeParameter(digoxin, lipophilicityGroup, "Alsenz 2007", CoreConstants.Parameters.LIPOPHILICITY, 1.623);
         checkProcessParameter(digoxin, "ATP1A2-Katz (2010)", CoreConstantsForSpecs.Parameter.KD, 25.6E-3, "nmol/l");

         checkDefaultAlternative(digoxin, permeabilityGroup, "fitted");
         checkAlternativeParameter(digoxin, permeabilityGroup, "fitted", CoreConstants.Parameters.PERMEABILITY, 1.01150E-5, "dm/min");

         //---- Itraconazole
         var itraconazole = _compounds.FindByName("Itraconazole");
         checkDefaultAlternative(itraconazole, intestinalPermeabilityGroup, "Optimization");
         
         //---- Rifampicin
         var rifampicin = _compounds.FindByName("Rifampicin");
         checkDefaultAlternative(rifampicin, intestinalPermeabilityGroup, "fitted");

         //---- S-Warfarin
         var swarfarin = _compounds.FindByName("S-Warfarin");
         checkDefaultAlternative(swarfarin, intestinalPermeabilityGroup, "fitted");

      }
   }

   public class When_saving_a_building_block_to_the_database : concern_for_TemplateTaskQuery
   {
      private Individual _individual;
      private Template _template;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual().WithName("TOTO'");

         _template = new Template {DatabaseType = TemplateDatabaseType.User, Name = _individual.Name, Object = _individual, TemplateType = TemplateType.Individual};
         sut.SaveToTemplate(_template);
      }

      [Observation]
      public void the_template_should_exist_by_name()
      {
         sut.Exists(TemplateDatabaseType.User, _individual.Name, TemplateType.Individual).ShouldBeTrue();
      }

      [Observation]
      public void should_be_able_to_load_the_template_by_name()
      {
         sut.LoadTemplate<Individual>(_template).ShouldNotBeNull();
      }
   }

   public class When_saving_a_building_block_with_references_to_the_template_database : concern_for_TemplateTaskQuery
   {
      private Compound _compound;
      private Template _template;
      private Compound _metabolite;
      private Template _reference;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound = DomainFactoryForSpecs.CreateStandardCompound().WithName("DRUG");
         _metabolite = DomainFactoryForSpecs.CreateStandardCompound().WithName("METABOLITE");

         _template = new Template {DatabaseType = TemplateDatabaseType.User, Name = _compound.Name, Object = _compound, TemplateType = TemplateType.Compound};
         _reference = new Template {DatabaseType = TemplateDatabaseType.User, Name = _metabolite.Name, Object = _metabolite, TemplateType = TemplateType.Compound};
         _template.References.Add(_reference);
         sut.SaveToTemplate(new[] {_template, _reference,});
      }

      [Observation]
      public void the_template_should_exist_by_name()
      {
         sut.Exists(TemplateDatabaseType.User, _compound.Name, TemplateType.Compound).ShouldBeTrue();
      }

      [Observation]
      public void the_reference_should_exist_by_name()
      {
         sut.Exists(TemplateDatabaseType.User, _metabolite.Name, TemplateType.Compound).ShouldBeTrue();
      }
   }

   public class When_deleting_the_reference_to_another_template : concern_for_TemplateTaskQuery
   {
      private Compound _compound;
      private Template _template;
      private Compound _metabolite;
      private Template _reference;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound = DomainFactoryForSpecs.CreateStandardCompound().WithName("DRUG");
         _metabolite = DomainFactoryForSpecs.CreateStandardCompound().WithName("METABOLITE");

         _template = new Template {DatabaseType = TemplateDatabaseType.User, Name = _compound.Name, Object = _compound, TemplateType = TemplateType.Compound};
         _reference = new Template {DatabaseType = TemplateDatabaseType.User, Name = _metabolite.Name, Object = _metabolite, TemplateType = TemplateType.Compound};
         _template.References.Add(_reference);
         sut.SaveToTemplate(new[] {_template, _reference,});
         sut.DeleteTemplate(_reference);
      }

      [Observation]
      public void should_be_able_to_load_the_template_and_delete_the_referece()
      {
         sut.LoadTemplate<Compound>(_template).ShouldNotBeNull();
         sut.Exists(TemplateDatabaseType.User, _metabolite.Name, TemplateType.Compound).ShouldBeFalse();
      }
   }
}