using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Importer;
using OSPSuite.Presentation.Core;
using OSPSuite.Assets;

namespace PKSim.Presentation.Services
{
   public abstract class OntogenyTask<TSimulationSubject> : IOntogenyTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      protected readonly IExecutionContext _executionContext;
      private readonly IApplicationController _applicationController;
      private readonly IDataImporter _dataImporter;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly IEntityTask _entityTask;
      private readonly IFormulaFactory _formulaFactory;

      protected OntogenyTask(IExecutionContext executionContext, IApplicationController applicationController, IDataImporter dataImporter,
         IDimensionRepository dimensionRepository, IOntogenyRepository ontogenyRepository, IEntityTask entityTask, IFormulaFactory formulaFactory)
      {
         _executionContext = executionContext;
         _applicationController = applicationController;
         _dataImporter = dataImporter;
         _dimensionRepository = dimensionRepository;
         _ontogenyRepository = ontogenyRepository;
         _entityTask = entityTask;
         _formulaFactory = formulaFactory;
      }

      public abstract ICommand SetOntogenyForMolecule(IndividualMolecule molecule, Ontogeny ontogeny, TSimulationSubject simulationSubject);

      public void ShowOntogenyData(Ontogeny ontogeny)
      {
         if (ontogeny.IsUndefined()) return;

         using (var presenter = _applicationController.Start<IShowOntogenyDataPresenter>())
         {
            presenter.Show(ontogeny);
         }
      }

      public ICommand LoadOntogenyForMolecule(IndividualMolecule molecule, TSimulationSubject simulationSubject)
      {
         var dataImporterSettings = new DataImporterSettings
         {
            Caption = $"{CoreConstants.ProductDisplayName} - {PKSimConstants.UI.ImportOntogeny}",
            Icon = ApplicationIcons.Excel
         };

         var data = _dataImporter.ImportDataSet(new List<MetaDataCategory>(), getColumnInfos(), dataImporterSettings);
         if (data == null)
            return null;

         var ontogeny = new UserDefinedOntogeny {Table = formulaFrom(data), Name = data.Name};

         //only first formulation will be imported
         if (_ontogenyRepository.AllNames().Contains(ontogeny.Name))
         {
            var name = _entityTask.NewNameFor(ontogeny, _ontogenyRepository.AllNames());
            if (string.IsNullOrEmpty(name))
               return null;

            ontogeny.Name = name;
         }

         return SetOntogenyForMolecule(molecule, ontogeny, simulationSubject);
      }

      private DistributedTableFormula formulaFrom(DataRepository dataRepository)
      {
         var baseGrid = dataRepository.BaseGrid;
         var valueColumns = dataRepository.AllButBaseGrid().ToList();
         DataColumn meanColumn, deviationColumn;

         if (valueColumns.Count == 1)
         {
            meanColumn = valueColumns[0];
            //dummy deviation filled with 1 since this was not defined in the import action
            deviationColumn = new DataColumn(Constants.Distribution.DEVIATION, _dimensionRepository.NoDimension, baseGrid);
            deviationColumn.Values = new float[baseGrid.Count].InitializeWith(1f);
         }
         else
         {
            meanColumn = valueColumns.Single(x => x.RelatedColumns.Any());
            deviationColumn = valueColumns.Single(x => !x.RelatedColumns.Any());
         }

         var formula = _formulaFactory.CreateDistributedTableFormula().WithName(dataRepository.Name);
         formula.InitializedWith(CoreConstants.Parameter.PMA, dataRepository.Name, baseGrid.Dimension, meanColumn.Dimension);
         formula.XDisplayUnit = baseGrid.Dimension.Unit(baseGrid.DataInfo.DisplayUnitName);
         formula.YDisplayUnit = meanColumn.Dimension.Unit(meanColumn.DataInfo.DisplayUnitName);

         foreach (var ageValue in baseGrid.Values)
         {
            var mean = meanColumn.GetValue(ageValue).ToDouble();
            var pma = ageValue.ToDouble();
            var deviation = deviationColumn.GetValue(ageValue).ToDouble();
            var distribution = new DistributionMetaData {Mean = mean, Deviation = deviation, Distribution = DistributionTypes.LogNormal};
            formula.AddPoint(pma, mean, distribution);
         }
         return formula;
      }

      private IReadOnlyList<ColumnInfo> getColumnInfos()
      {
         var columns = new List<ColumnInfo>();

         var ageColumn = new ColumnInfo
         {
            DefaultDimension = _dimensionRepository.AgeInYears,
            Name = PKSimConstants.UI.PostMenstrualAge,
            Description = PKSimConstants.UI.PostMenstrualAge,
            DisplayName = PKSimConstants.UI.PostMenstrualAge,
            IsMandatory = true,
            NullValuesHandling = NullValuesHandlingType.DeleteRow,
         };


         ageColumn.DimensionInfos.Add(new DimensionInfo {Dimension = _dimensionRepository.AgeInYears, IsMainDimension = true});
         columns.Add(ageColumn);

         var ontogenyFactor = new ColumnInfo
         {
            DefaultDimension = _dimensionRepository.Fraction,
            Name = PKSimConstants.UI.OntogenyFactor,
            Description = PKSimConstants.UI.OntogenyFactor,
            DisplayName = PKSimConstants.UI.OntogenyFactor,
            IsMandatory = true,
            NullValuesHandling = NullValuesHandlingType.DeleteRow,
            BaseGridName = ageColumn.Name,
         };
         ontogenyFactor.DimensionInfos.Add(new DimensionInfo {Dimension = _dimensionRepository.Fraction, IsMainDimension = true});
         columns.Add(ontogenyFactor);

         var geoMean = new ColumnInfo
         {
            DefaultDimension = _dimensionRepository.NoDimension,
            Name = PKSimConstants.UI.StandardDeviation,
            Description = PKSimConstants.UI.StandardDeviation,
            DisplayName = PKSimConstants.UI.StandardDeviation,
            IsMandatory = false,
            NullValuesHandling = NullValuesHandlingType.Allowed,
            BaseGridName = ageColumn.Name,
            RelatedColumnOf = ontogenyFactor.Name
         };

         geoMean.DimensionInfos.Add(new DimensionInfo {Dimension = _dimensionRepository.NoDimension, IsMainDimension = true});
         columns.Add(geoMean);

         return columns;
      }
   }
}