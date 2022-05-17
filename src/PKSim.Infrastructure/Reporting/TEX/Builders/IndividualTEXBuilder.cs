using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class IndividualTeXBuilder : BuildingBlockTeXBuilder<Individual>
   {
      private readonly IContainerTask _containerTask;
      private readonly IFullPathDisplayResolver _fullPathDisplayResolver;

      public IndividualTeXBuilder(IContainerTask containerTask, IFullPathDisplayResolver fullPathDisplayResolver,
                                  ITeXBuilderRepository texBuilderRepository, IReportGenerator reportGenerator, ILazyLoadTask lazyLoadTask) : base(texBuilderRepository, reportGenerator, lazyLoadTask)
      {
         _containerTask = containerTask;
         _fullPathDisplayResolver = fullPathDisplayResolver;
      }

      protected override IEnumerable<object> BuildingBlockReport(Individual individual, OSPSuiteTracker tracker)
      {
         var report = new List<object>
            {
               new SubSection(PKSimConstants.UI.Biometrics),
               biometricsFor(individual),
               new SubSection(PKSimConstants.UI.AnatomyAndPhysiology)
            };

         anatomyAndPhysiologyFor(individual).Each(report.Add);

         report.Add(new SubSection(PKSimConstants.UI.Expression));
         expressionsFor(individual).Each(report.Add);

         return report;
      }

      private object biometricsFor(Individual individual)
      {
         return individual.OriginData;
      }

      private IEnumerable<object> expressionsFor(Individual individual)
      {
         var report = new List<object>();
         report.AddRange(moleculeReportFor<IndividualEnzyme>(individual, PKSimConstants.UI.MetabolizingEnzymes));
         report.AddRange(moleculeReportFor<IndividualTransporter>(individual, PKSimConstants.UI.TransportProteins));
         report.AddRange(moleculeReportFor<IndividualOtherProtein>(individual, PKSimConstants.UI.ProteinBindingPartners));
         return report;
      }

      private IEnumerable<object> moleculeReportFor<TMolecule>(Individual individual, string sectionName) where TMolecule : IndividualMolecule
      {
         var report = new List<object> {new SubSubSection(sectionName)};
         var allMolecules = individual.AllMolecules<TMolecule>().ToList();
         if(allMolecules.Any())
            report.AddRange(allMolecules);
         else
            report.Add(PKSimConstants.UI.MoleculeNotDefined);

         return report;
      }

      private IEnumerable<object> anatomyAndPhysiologyFor(Individual individual)
      {
         var report = new List<object>();
         var allParameters = _containerTask.CacheAllChildrenSatisfying<IParameter>(individual, valueShouldBeExportedForAnatomy);
         if (!allParameters.Any())
         {
            report.Add(PKSimConstants.UI.Default);
            return report;
         }

         var dataTable = new DataTable {TableName = PKSimConstants.UI.AnatomyAndPhysiology};
         dataTable.AddColumn(PKSimConstants.UI.Parameter);
         dataTable.AddColumn(PKSimConstants.UI.Value);
         dataTable.AddColumn(PKSimConstants.UI.DefaultValue);

         foreach (var parameter in allParameters)
         {
            var row = dataTable.NewRow();
            row[PKSimConstants.UI.Parameter] = _fullPathDisplayResolver.FullPathFor(parameter);
            row[PKSimConstants.UI.Value] = ParameterMessages.DisplayValueFor(parameter);
            row[PKSimConstants.UI.DefaultValue] = ParameterMessages.DisplayValueFor(parameter, parameter.DefaultValue.GetValueOrDefault(parameter.Value));

            dataTable.Rows.Add(row);
         }

         var table = new Table(dataTable.DefaultView, new Text(dataTable.TableName));
         var reference = new Reference(table);
         var text = new Text(PKSimConstants.UI.AnatomyAndPhysiologyText, reference);

         report.Add(text);
         report.Add(table);
         return report;
      }

      private bool valueShouldBeExportedForAnatomy(IParameter parameter)
      {
         if (!parameter.Visible) return false;
         if (parameter.IsExpressionProfile()) return false;
         if (!parameter.ValueDiffersFromDefault()) return false;
         return true;
      }
   }
}