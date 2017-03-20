using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Formulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Presentation.Core;
using OSPSuite.Assets;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Importer;

namespace PKSim.Presentation.Services
{
   public interface IFormulationTask : IBuildingBlockTask<Formulation>
   {
      Formulation CreateFormulationForRoute(string applicationRoute);
      Formulation LoadFormulationForRoute(string applicationRoute);

      TableFormula ImportTableFormula();
   }

   public class FormulationTask : BuildingBlockTask<Formulation>, IFormulationTask
   {
      public IDimensionRepository DimensionRepository { get; set; }
      private readonly IDataImporter _dataImporter;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IFormulaFactory _formulaFactory;

      public FormulationTask(IExecutionContext executionContext, IBuildingBlockTask buildingBlockTask, IApplicationController applicationController, IDataImporter dataImporter,
         IDimensionRepository dimensionRepository, IFormulaFactory formulaFactory)
         : base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.Formulation)
      {
         DimensionRepository = dimensionRepository;
         _dataImporter = dataImporter;
         _dimensionRepository = dimensionRepository;
         _formulaFactory = formulaFactory;
      }

      public override Formulation AddToProject()
      {
         //no need to specify route when adding a default formulation
         return AddToProject<ICreateFormulationPresenter>();
      }

      public Formulation CreateFormulationForRoute(string applicationRoute)
      {
         return AddToProject<ICreateFormulationPresenter>(x => x.CreateFormulation(applicationRoute));
      }

      public Formulation LoadFormulationForRoute(string applicationRoute)
      {
         var formulation = LoadSingleFromTemplate();
         if (formulation == null)
            return null;

         if (formulation.HasRoute(applicationRoute))
            return formulation;

         throw new FormulationCannotBeUsedForRouteException(formulation, applicationRoute);
      }

      public TableFormula ImportTableFormula()
      {
         var dataImporterSettings = new DataImporterSettings
         {
            Caption = string.Format("{0} - {1}", CoreConstants.ProductDisplayName, PKSimConstants.UI.ImportFormulation),
            Icon = ApplicationIcons.Formulation
         };

         var importedFormula = _dataImporter.ImportDataSet(new List<MetaDataCategory>(), getColumnInfos(), dataImporterSettings);
         return importedFormula == null ? null : formulaFrom(importedFormula);
      }

      private TableFormula formulaFrom(DataRepository dataRepository)
      {
         var baseGrid = dataRepository.BaseGrid;
         var valueColumn = dataRepository.AllButBaseGrid().Single();
         var formula = _formulaFactory.CreateTableFormula().WithName(dataRepository.Name);
         formula.InitializedWith(Constants.TIME, dataRepository.Name, baseGrid.Dimension, valueColumn.Dimension);
         formula.XDisplayUnit = baseGrid.Dimension.Unit(baseGrid.DataInfo.DisplayUnitName);
         formula.YDisplayUnit = valueColumn.Dimension.Unit(valueColumn.DataInfo.DisplayUnitName);

         foreach (var timeValue in baseGrid.Values)
         {
            formula.AddPoint(timeValue, valueColumn.GetValue(timeValue).ToDouble());
         }
         return formula;
      }

      private IReadOnlyList<ColumnInfo> getColumnInfos()
      {
         var columns = new List<ColumnInfo>();

         var timeColumn = new ColumnInfo
         {
            DefaultDimension = _dimensionRepository.Time,
            Name = PKSimConstants.UI.Time,
            Description = PKSimConstants.UI.Time,
            DisplayName = PKSimConstants.UI.Time,
            IsMandatory = true,
            NullValuesHandling = NullValuesHandlingType.DeleteRow,
         };


         timeColumn.DimensionInfos.Add(new DimensionInfo {Dimension = _dimensionRepository.Time, IsMainDimension = true});
         columns.Add(timeColumn);

         var fractionColumn = new ColumnInfo
         {
            DefaultDimension = _dimensionRepository.Fraction,
            Name = PKSimConstants.UI.Fraction,
            Description = PKSimConstants.UI.Fraction,
            DisplayName = PKSimConstants.UI.Fraction,
            IsMandatory = true,
            NullValuesHandling = NullValuesHandlingType.DeleteRow,
            BaseGridName = timeColumn.Name,
         };

         fractionColumn.DimensionInfos.Add(new DimensionInfo {Dimension = _dimensionRepository.Fraction, IsMainDimension = true});
         columns.Add(fractionColumn);

         return columns;
      }
   }
}