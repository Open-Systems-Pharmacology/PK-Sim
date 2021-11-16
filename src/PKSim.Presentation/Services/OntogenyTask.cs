using System;
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
using OSPSuite.Presentation.Core;
using OSPSuite.Assets;
using OSPSuite.Infrastructure.Import.Core;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Core.Services;
using PKSim.Core.Commands;

namespace PKSim.Presentation.Services
{
   public class OntogenyTask : IOntogenyTask
   {
      protected readonly IExecutionContext _executionContext;
      private readonly IApplicationController _applicationController;
      private readonly IDataImporter _dataImporter;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly IEntityTask _entityTask;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IDialogCreator _dialogCreator;

      public OntogenyTask(IExecutionContext executionContext, IApplicationController applicationController, IDataImporter dataImporter,
         IDimensionRepository dimensionRepository, IOntogenyRepository ontogenyRepository, IEntityTask entityTask, IFormulaFactory formulaFactory, IDialogCreator dialogCreator)
      {
         _executionContext = executionContext;
         _applicationController = applicationController;
         _dataImporter = dataImporter;
         _dimensionRepository = dimensionRepository;
         _ontogenyRepository = ontogenyRepository;
         _entityTask = entityTask;
         _formulaFactory = formulaFactory;
         _dialogCreator = dialogCreator;
   }

      public ICommand SetOntogenyForMolecule(IndividualMolecule molecule, Ontogeny ontogeny, ISimulationSubject simulationSubject)
      {
         return new SetOntogenyInMoleculeCommand(molecule, ontogeny, simulationSubject.Individual, _executionContext).Run(_executionContext);
      }

      public void ShowOntogenyData(Ontogeny ontogeny)
      {
         if (ontogeny.IsUndefined()) return;

         using (var presenter = _applicationController.Start<IShowOntogenyDataPresenter>())
         {
            presenter.Show(ontogeny);
         }
      }

      public ICommand LoadOntogenyForMolecule(IndividualMolecule molecule, ISimulationSubject simulationSubject)
      {
         var dataImporterSettings = new DataImporterSettings
         {
            Caption = $"{CoreConstants.ProductDisplayName} - {PKSimConstants.UI.ImportOntogeny}",
            IconName = ApplicationIcons.Excel.IconName
         };
         dataImporterSettings.AddNamingPatternMetaData(Constants.FILE);

         var data = _dataImporter.ImportDataSets(
            new List<MetaDataCategory>(), 
            getColumnInfos(), 
            dataImporterSettings,
            _dialogCreator.AskForFileToOpen(Captions.Importer.OpenFile, Captions.Importer.ImportFileFilter, Constants.DirectoryKey.OBSERVED_DATA)
         ).DataRepositories.FirstOrDefault();
         if (data == null)
            return null;

         var ontogeny = new UserDefinedOntogeny {Table = formulaFrom(data), Name = data.Name};

         //only first ontogeny will be imported
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
            deviationColumn = new DataColumn(Constants.Distribution.DEVIATION, _dimensionRepository.NoDimension, baseGrid)
            {
               Values = new float[baseGrid.Count].InitializeWith(1f)
            };
         }
         else
         {
            meanColumn = valueColumns.Single(x => x.RelatedColumns.Any());
            deviationColumn = valueColumns.Single(x => !x.RelatedColumns.Any());
         }

         var formula = _formulaFactory.CreateDistributedTableFormula().WithName(dataRepository.Name);
         formula.InitializedWith(CoreConstants.Parameters.PMA, dataRepository.Name, baseGrid.Dimension, meanColumn.Dimension);
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
            DisplayName = PKSimConstants.UI.PostMenstrualAge,
            IsMandatory = true,
         };


         ageColumn.SupportedDimensions.Add(_dimensionRepository.AgeInYears);
         columns.Add(ageColumn);

         var ontogenyFactor = new ColumnInfo
         {
            DefaultDimension = _dimensionRepository.Fraction,
            Name = PKSimConstants.UI.OntogenyFactor,
            DisplayName = PKSimConstants.UI.OntogenyFactor,
            IsMandatory = true,
            BaseGridName = ageColumn.Name,
         };
         ontogenyFactor.SupportedDimensions.Add(_dimensionRepository.Fraction);
         columns.Add(ontogenyFactor);

         var geoMean = new ColumnInfo
         {
            DefaultDimension = _dimensionRepository.NoDimension,
            Name = PKSimConstants.UI.StandardDeviation,
            DisplayName = PKSimConstants.UI.StandardDeviation,
            IsMandatory = false,
            BaseGridName = ageColumn.Name,
            RelatedColumnOf = ontogenyFactor.Name
         };

         geoMean.SupportedDimensions.Add(_dimensionRepository.NoDimension);
         columns.Add(geoMean);

         return columns;
      } 
   }
}