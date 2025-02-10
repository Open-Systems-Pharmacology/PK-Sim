using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Import.Core;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Presentation.Core;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Individuals;

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
      private readonly IDialogCreator _dialogCreator;

      public OntogenyTask(
         IExecutionContext executionContext,
         IApplicationController applicationController,
         IDataImporter dataImporter,
         IDimensionRepository dimensionRepository,
         IOntogenyRepository ontogenyRepository,
         IEntityTask entityTask,
         IDialogCreator dialogCreator)
      {
         _executionContext = executionContext;
         _applicationController = applicationController;
         _dataImporter = dataImporter;
         _dimensionRepository = dimensionRepository;
         _ontogenyRepository = ontogenyRepository;
         _entityTask = entityTask;
         _dialogCreator = dialogCreator;
      }

      public ICommand SetOntogenyForMolecule(IndividualMolecule molecule, Ontogeny ontogeny, ISimulationSubject simulationSubject)
      {
         return new SetOntogenyInMoleculeCommand(molecule, ontogeny, simulationSubject, _executionContext).Run(_executionContext);
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

         var fileName = _dialogCreator.AskForFileToOpen(Captions.Importer.OpenFile, Captions.Importer.ImportFileFilter, Constants.DirectoryKey.OBSERVED_DATA);
         if (string.IsNullOrEmpty(fileName))
            return null;

         var data = _dataImporter.ImportDataSets(new List<MetaDataCategory>(), getColumnInfos(), dataImporterSettings, fileName).DataRepositories.FirstOrDefault();
         if (data == null)
            return null;

         //when imported from excel, we are importing using the Geometric std format
         var ontogeny = new UserDefinedOntogeny {Table = _ontogenyRepository.DataRepositoryToDistributedTableFormula(data), Name = data.Name};

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


         ageColumn.SupportedDimensions.Add(ageColumn.DefaultDimension);
         columns.Add(ageColumn);

         var ontogenyFactor = new ColumnInfo
         {
            DefaultDimension = _dimensionRepository.NoDimension,
            Name = PKSimConstants.UI.OntogenyFactor,
            DisplayName = PKSimConstants.UI.OntogenyFactor,
            IsMandatory = true,
            BaseGridName = ageColumn.Name,
         };
         ontogenyFactor.SupportedDimensions.Add(ontogenyFactor.DefaultDimension);
         columns.Add(ontogenyFactor);

         var geoMean = new ColumnInfo
         {
            DefaultDimension = _dimensionRepository.NoDimension,
            Name = PKSimConstants.UI.GeometricStandardDeviation,
            DisplayName = PKSimConstants.UI.GeometricStandardDeviation,
            IsMandatory = false,
            BaseGridName = ageColumn.Name,
            RelatedColumnOf = ontogenyFactor.Name,
         };

         geoMean.SupportedDimensions.Add(geoMean.DefaultDimension);
         columns.Add(geoMean);

         return columns;
      }
   }
}