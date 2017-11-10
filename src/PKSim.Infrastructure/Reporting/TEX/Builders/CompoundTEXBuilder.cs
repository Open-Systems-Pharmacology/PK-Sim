using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Data;
using OSPSuite.TeXReporting.Items;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class CompoundTeXBuilder : BuildingBlockTeXBuilder<Compound>
   {
      private readonly IRepresentationInfoRepository _representationRepository;
      private readonly ICompoundAlternativeTask _compoundAlternativeTask;
      private OSPSuiteTracker _tracker;

      public CompoundTeXBuilder(ITeXBuilderRepository builderRepository, IReportGenerator reportGenerator, ILazyLoadTask lazyLoadTask,
                                IRepresentationInfoRepository representationRepository, ICompoundAlternativeTask compoundAlternativeTask)
         : base(builderRepository, reportGenerator, lazyLoadTask)
      {
         _representationRepository = representationRepository;
         _compoundAlternativeTask = compoundAlternativeTask;
      }

      protected override IEnumerable<object> BuildingBlockReport(Compound compound, OSPSuiteTracker tracker)
      {
         try
         {
            var report = new List<object>();
            _tracker = tracker;
            report.Add(new SubSection(PKSimConstants.UI.BasicPharmacochemistry));
            report.AddRangeSafe(basicPharmacochemistry(compound));
            report.AddRangeSafe(dissociationConstants(compound));
            report.AddRangeSafe(lipophilicyAlternatives(compound));
            report.AddRangeSafe(fractionUnboundAlternatives(compound));
            report.AddRangeSafe(solubilityAlternatives(compound));



            report.Add(new SubSection(PKSimConstants.UI.ADME));

            createAbsorption(compound, report, tracker);

            createDistribution(compound, report, tracker);

            createMetabolism(compound, report, tracker);

            createTranspoortAndExcretion(compound, report, tracker);

            createInhibition(compound, report, tracker);

            createInduction(compound, report, tracker);

            return report;
         }
         finally
         {
            _tracker = null;
         }
      }

      private void createInhibition(Compound compound, List<object> report, BuildTracker tracker)
      {
         var thisSection = new List<object>();
         thisSection.AddRangeSafe(partialProcesses<InhibitionProcess>(compound));

         addToReport(report, tracker, thisSection, PKSimConstants.UI.Inhibition);
      }

      private void createInduction(Compound compound, List<object> report, BuildTracker tracker)
      {
         var thisSection = new List<object>();
         thisSection.AddRangeSafe(partialProcesses<InductionProcess>(compound));

         addToReport(report, tracker, thisSection, PKSimConstants.UI.Induction);
      }

      private void addToReport(List<object> report, BuildTracker tracker, List<object> thisSection, string newSectionName)
      {
         if (!thisSection.Any()) return;
         report.Add(tracker.CreateRelativeStructureElement(newSectionName, 2));
         report.AddRange(thisSection);
      }



      private void createTranspoortAndExcretion(Compound compound, List<object> report, BuildTracker tracker)
      {
         var thisSection = new List<object>();
         
         thisSection.AddRangeSafe(partialProcesses<TransportPartialProcess>(compound));
         thisSection.AddRangeSafe(systemicProcesses(compound, SystemicProcessTypes.Renal));
         thisSection.AddRangeSafe(systemicProcesses(compound, SystemicProcessTypes.Biliary));
         thisSection.AddRangeSafe(systemicProcesses(compound, SystemicProcessTypes.GFR));

         addToReport(report, tracker, thisSection, PKSimConstants.UI.TransportAndExcretionProcesses);
      }

      private void createMetabolism(Compound compound, List<object> report, BuildTracker tracker)
      {
         var thisSection = new List<object>();

         thisSection.AddRangeSafe(partialProcesses<EnzymaticProcess>(compound));
         thisSection.AddRangeSafe(systemicProcesses(compound, SystemicProcessTypes.Hepatic));

         addToReport(report, tracker, thisSection, PKSimConstants.UI.MetabolicProcesses);
      }

      private void createDistribution(Compound compound, List<object> report, BuildTracker tracker)
      {
         var thisSection = new List<object>();
         
         thisSection.AddRangeSafe(permeabilityAlternatives(compound));

         addCalculationMethodsAccordingToPredicate(compound, thisSection,
            cm => cm.Category.IsOneOf(CoreConstants.Category.DistributionCellular, CoreConstants.Category.DistributionInterstitial, CoreConstants.Category.DiffusionIntCell));

         thisSection.AddRangeSafe(partialProcesses<SpecificBindingPartialProcess>(compound));

         addToReport(report, tracker, thisSection, PKSimConstants.UI.Distribution);

      }

      private void createAbsorption(Compound compound, List<object> report, BuildTracker tracker)
      {
         var thisSection = new List<object>();

         thisSection.AddRangeSafe(intestinalPermeabilityAlternatives(compound));

         addCalculationMethodsAccordingToPredicate(compound, thisSection, cm =>
            cm.Category.IsOneOf(CoreConstants.Category.IntestinalPermeability));

         addToReport(report, tracker, thisSection, PKSimConstants.UI.Absorption);
      }

      private void addCalculationMethodsAccordingToPredicate(Compound compound, List<object> report, Func<CalculationMethod, bool> predicate)
      {
         var calculationMethodsToAdd = calculationMethods(compound.AllCalculationMethods().Where(predicate)).ToList();
         if (calculationMethodsToAdd.Any())
         {
            report.AddRangeSafe(calculationMethodsToAdd);
         }
      }

      private IEnumerable<object> calculationMethods(IEnumerable<CalculationMethod> calculationMethods)
      {
         var report = new List<object> {calculationMethods.ToList()};
         return report;
      }

      private IEnumerable<object> systemicProcesses(Compound compound, SystemicProcessType systemicProcessType)
      {
         var allProcessesToReport = compound.AllSystemicProcessesOfType(systemicProcessType).ToList();
         if (!allProcessesToReport.Any())
            return null;

         var report = new List<object>();
         report.AddRange(allProcessesToReport);
         return report;
      }

      private IEnumerable<object> partialProcesses<T>(Compound compound) where T : PartialProcess
      {
         var allProcessesToReport = compound.AllProcesses<T>().OrderBy(x => x.MoleculeName).ToList();
         if (!allProcessesToReport.Any())
            return null;

         var report = new List<object>();
         report.AddRange(allProcessesToReport);
         return report;
      }

      private IEnumerable<object> basicPharmacochemistry(Compound compound)
      {
         var molWeight = _representationRepository.DisplayNameFor(RepresentationObjectType.PARAMETER, Constants.Parameters.MOL_WEIGHT);
         return new List<object>
            {
               compound.Parameter(CoreConstants.Parameter.IS_SMALL_MOLECULE),
               new SubSubSection(molWeight),
               new ParameterList(molWeight,
                                 compound.Parameter(Constants.Parameters.MOL_WEIGHT),
                                 compound.Parameter(CoreConstants.Parameter.EFFECTIVE_MOLECULAR_WEIGHT),
                                 compound.Parameter(CoreConstants.Parameter.I),
                                 compound.Parameter(CoreConstants.Parameter.F),
                                 compound.Parameter(CoreConstants.Parameter.CL),
                                 compound.Parameter(CoreConstants.Parameter.BR))
            };
      }

      private IEnumerable<object> dissociationConstants(Compound compound)
      {
         var table = new DataTable(groupDisplayName(CoreConstants.Groups.COMPOUND_PKA));
         table.AddColumns<string>(CoreConstants.Parameter.PARAMETER_PKA_BASE, CoreConstants.Parameter.ParameterCompoundTypeBase);
         addCompoundTypePart(table, compound, CoreConstants.Parameter.PARAMETER_PKA1, CoreConstants.Parameter.COMPOUND_TYPE1);
         addCompoundTypePart(table, compound, CoreConstants.Parameter.PARAMETER_PKA2, CoreConstants.Parameter.COMPOUND_TYPE2);
         addCompoundTypePart(table, compound, CoreConstants.Parameter.PARAMETER_PKA3, CoreConstants.Parameter.COMPOUND_TYPE3);
         if (table.Rows.Count == 0)
            return null;

         return new object[] { new SubSubSection(PKSimConstants.UI.DissociationConstants), table };
      }

      private void addCompoundTypePart(DataTable table, Compound compound, string parameterPka1, string parameterCompoundType1)
      {
         var compoundType = compound.Parameter(parameterCompoundType1).Value;

         if (compoundType == CoreConstants.Compound.COMPOUND_TYPE_NEUTRAL) return;
         var pka = compound.Parameter(parameterPka1).Value;

         string compoundTypeDisplay;
         if (compoundType == CoreConstants.Compound.COMPOUND_TYPE_ACID)
            compoundTypeDisplay = PKSimConstants.UI.CompoundTypeAcid;
         else
            compoundTypeDisplay = PKSimConstants.UI.CompoundTypeBase;

         var row = table.NewRow();
         row[CoreConstants.Parameter.PARAMETER_PKA_BASE] = pka;
         row[CoreConstants.Parameter.ParameterCompoundTypeBase] = compoundTypeDisplay;
         table.Rows.Add(row);
      }

      private IEnumerable<object> solubilityAlternatives(Compound compound)
      {
         Action<DataTable> createColumns = t =>
            {
               t.AddColumn<double>(PKSimConstants.UI.RefpH);
               t.AddColumn<double>(PKSimConstants.UI.RefSolubility);
               t.AddColumn<double>(PKSimConstants.UI.SolubilityGainPerCharge);
            };

         Action<DataRow, ParameterAlternative> fillColumns = (row, alternative) =>
            {
               setParameterValue(alternative.Parameter(CoreConstants.Parameter.REFERENCE_PH), row, PKSimConstants.UI.RefpH);
               setParameterValue(alternative.Parameter(CoreConstants.Parameter.SOLUBILITY_AT_REFERENCE_PH), row, PKSimConstants.UI.RefSolubility);
               setParameterValue(alternative.Parameter(CoreConstants.Parameter.SolubilityGainPerCharge), row, PKSimConstants.UI.SolubilityGainPerCharge);
            };

         return createAlternatives(compound, CoreConstants.Groups.COMPOUND_SOLUBILITY, createColumns, fillColumns, PKSimConstants.Reporting.SolubilityDescription);
      }

      private IEnumerable<object> intestinalPermeabilityAlternatives(Compound compound)
      {
         return calculatedAlternatives(compound, CoreConstants.Groups.COMPOUND_INTESTINAL_PERMEABILITY, PKSimConstants.UI.Permeability,
                                       CoreConstants.Parameter.SpecificIntestinalPermeability, PKSimConstants.Reporting.IntestinalPermeabilityDescription, x => x.IntestinalPermeabilityValuesFor);
      }

      private IEnumerable<object> permeabilityAlternatives(Compound compound)
      {
         return calculatedAlternatives(compound, CoreConstants.Groups.COMPOUND_PERMEABILITY, PKSimConstants.UI.Permeability,
                                       CoreConstants.Parameter.Permeability, PKSimConstants.Reporting.PermeabilityDescription, x => x.PermeabilityValuesFor);
      }

      private IEnumerable<object> fractionUnboundAlternatives(Compound compound)
      {
         Action<DataTable> createColumns = t =>
            {
               t.AddColumn<double>(PKSimConstants.UI.FractionUnbound);
               t.AddColumn(PKSimConstants.UI.Species);
            };

         Action<DataRow, ParameterAlternativeWithSpecies> fillColumns = (row, alternative) =>
            {
               setParameterValue(alternative.Parameter(CoreConstants.Parameter.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE), row, PKSimConstants.UI.FractionUnbound);
               row[PKSimConstants.UI.Species] = alternative.Species.DisplayName;
            };

         return createAlternatives(compound, CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND, createColumns, fillColumns, PKSimConstants.Reporting.FractionUnboundDescription);
      }

      private IEnumerable<object> lipophilicyAlternatives(Compound compound)
      {
         return simpleAlternatives(compound, CoreConstants.Groups.COMPOUND_LIPOPHILICITY, CoreConstants.Parameter.LIPOPHILICITY, PKSimConstants.UI.Lipophilicity, PKSimConstants.Reporting.LipophilicityDescription);
      }

      private IEnumerable<object> calculatedAlternatives(Compound compound, string groupName, string columnName, string parameterName, string description, Func<ICompoundAlternativeTask, Func<Compound, IEnumerable<IParameter>>> calculatedParameters)
      {
         var parameterGroup = compound.ParameterAlternativeGroup(groupName);
         var table = new DataTable(groupDisplayName(groupName));
         var calcualtedAlternative = parameterGroup.AllAlternatives.First();
         table.AddColumns<string>(PKSimConstants.UI.Experiment, PKSimConstants.UI.Lipophilicity);
         table.AddColumn<double>(columnName);


         bool needsDefault = (parameterGroup.AllAlternatives.Count() > 1);
         if (needsDefault)
            table.AddColumn<bool>(PKSimConstants.UI.IsDefault);

         bool needsDescription = _tracker.Settings.Verbose && parameterGroup.AllAlternatives.Any(x => !string.IsNullOrEmpty(x.Description));
         if (needsDescription)
            table.AddColumn(PKSimConstants.UI.Description);

         Func<ParameterAlternative, DataRow> createDefaultRow = alternative =>
            {
               var row = table.NewRow();
               row[PKSimConstants.UI.Experiment] = alternative.Name;

               if (needsDefault)
                  row[PKSimConstants.UI.IsDefault] = alternative.IsDefault;

               if (needsDescription)
                  row[PKSimConstants.UI.Description] = alternative.Description;

               return row;
            };

         foreach (var alternative in parameterGroup.AllAlternatives)
         {
            if (calcualtedAlternative == alternative)
            {
               foreach (var parameter in calculatedParameters(_compoundAlternativeTask)(compound))
               {
                  var row = createDefaultRow(alternative);
                  row[PKSimConstants.UI.Lipophilicity] = parameter.Name;
                  setParameterValue(parameter, row, columnName);
                  table.Rows.Add(row);
               }
            }
            else
            {
               var row = createDefaultRow(alternative);
               setParameterValue(alternative.Parameter(parameterName), row, columnName);
               table.Rows.Add(row);
            }
         }
         return tableWithParagraph(table, description, compound.Name);
      }

      private IEnumerable<object> simpleAlternatives(Compound compound, string groupName, string parameterName, string columnName, string description)
      {
         Action<DataTable> createColumns = t => t.AddColumn<double>(columnName);

         Action<DataRow, ParameterAlternative> fillColumns = (row, alternative) => setParameterValue(alternative.Parameter(parameterName), row, columnName);

         return createAlternatives(compound, groupName, createColumns, fillColumns, description);
      }

      private void setParameterValue(IParameter parameter, DataRow row, string columnName)
      {
         row[columnName] = parameterValue(parameter, unitFor(columnName, row, parameter));
      }

      private static string unitFor(string columnName, DataRow row, IParameter parameter)
      {
         var unit = row.Table.Columns[columnName].GetUnit();
         if (unit == null)
         {
            row.Table.Columns[columnName].SetUnit(parameter.DisplayUnit.Name);
            unit = parameter.DisplayUnit.Name;
         }
         return unit;
      }

      private double? parameterValue(IParameter parameter, string unit)
      {
         try
         {
            return parameter.ConvertToUnit(unit);
         }
         catch (Exception)
         {
            return null;
         }
      }

      private IEnumerable<object> createAlternatives<TAlternatives>(Compound compound, string groupName, Action<DataTable> createColumns, Action<DataRow, TAlternatives> fillColumns, string description) where TAlternatives : ParameterAlternative
      {
         var table = createAlternativeTable(compound, groupName, createColumns, fillColumns);
         return tableWithSection(table, description, compound.Name);
      }

      private IEnumerable<object> tableWithStructureElement(StructureElement element, DataTable table, string description, string compoundName)
      {
         var longTable = new Table(table.DefaultView, table.TableName);
         return new object[] { element, longTable, new Text(description, new Reference(longTable), compoundName) };
      }

      private IEnumerable<object> tableWithParagraph(DataTable table, string description, string compoundName)
      {
         return tableWithStructureElement(new Paragraph(table.TableName), table, description, compoundName);
      }

      private IEnumerable<object> tableWithSection(DataTable table, string description, string compoundName)
      {
         return tableWithStructureElement(new SubSubSection(table.TableName), table, description, compoundName);
      }

      private DataTable createAlternativeTable<TAlternatives>(Compound compound, string groupName, Action<DataTable> createColumns, Action<DataRow, TAlternatives> fillColumns) where TAlternatives : ParameterAlternative
      {
         var parameterGroup = compound.ParameterAlternativeGroup(groupName);

         var table = new DataTable(groupDisplayName(groupName));

         table.AddColumn(PKSimConstants.UI.Experiment);
         createColumns(table);

         bool needsDefault = (parameterGroup.AllAlternatives.Count() > 1);
         if (needsDefault)
            table.AddColumn<bool>(PKSimConstants.UI.IsDefault);

         bool needsDescription = _tracker.Settings.Verbose && parameterGroup.AllAlternatives.Any(x => !string.IsNullOrEmpty(x.Description));
         if (needsDescription)
            table.AddColumn(PKSimConstants.UI.Description);

         foreach (var alternative in parameterGroup.AllAlternatives.Cast<TAlternatives>())
         {
            var row = table.NewRow();
            row[PKSimConstants.UI.Experiment] = alternative.Name;
            fillColumns(row, alternative);

            if (needsDefault)
               row[PKSimConstants.UI.IsDefault] = alternative.IsDefault;

            if (needsDescription)
               row[PKSimConstants.UI.Description] = alternative.Description;

            table.Rows.Add(row);
         }

         return table;
      }

      private string groupDisplayName(string groupName)
      {
         return _representationRepository.DisplayNameFor(RepresentationObjectType.GROUP, groupName);
      }
   }
}