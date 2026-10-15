using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Import.Core;
using OSPSuite.Infrastructure.Import.Services;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using IObservedDataTask = PKSim.Core.Services.IObservedDataTask;
using IContainer = OSPSuite.Utility.Container.IContainer;
using ImporterConfiguration = OSPSuite.Core.Import.ImporterConfiguration;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ImportObservedDataTask : ContextSpecification<ImportObservedDataTask>
   {
      protected IDataImporter _dataImporter;
      protected IDialogCreator _dialogCreator;
      protected IObservedDataTask _observedDataTask;
      protected IParameterChangeUpdater _parameterChangeUpdater;

      protected override void Context()
      {
         _dataImporter = A.Fake<IDataImporter>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _observedDataTask = A.Fake<IObservedDataTask>();
         _parameterChangeUpdater = A.Fake<IParameterChangeUpdater>();

         sut = new ImportObservedDataTask(
            _dataImporter, A.Fake<IExecutionContext>(), A.Fake<IBuildingBlockRepository>(), A.Fake<ISpeciesRepository>(),
            A.Fake<IDefaultIndividualRetriever>(), A.Fake<IRepresentationInfoRepository>(), _observedDataTask,
            _parameterChangeUpdater, _dialogCreator, A.Fake<IContainer>(), A.Fake<IOSPSuiteXmlSerializerRepository>());
      }
   }

   public class When_importing_observed_data_from_a_configuration : concern_for_ImportObservedDataTask
   {
      private ImporterConfiguration _configuration;
      private IReadOnlyList<DataRepository> _importedObservedData;
      private IReadOnlyList<DataRepository> _result;
      private DataImporterSettings _dataImporterSettings;

      protected override void Context()
      {
         base.Context();
         _configuration = new ImporterConfiguration();
         _importedObservedData = new[] { new DataRepository() };

         A.CallTo(() => _dialogCreator.AskForFileToOpen(A<string>._, A<string>._, A<string>._, A<string>._, A<string>._)).Returns("data.xlsx");
         A.CallTo(() => _dataImporter.ImportFromConfiguration(_configuration, A<IReadOnlyList<MetaDataCategory>>._, A<IReadOnlyList<ColumnInfo>>._, A<DataImporterSettings>._, "data.xlsx"))
            .Invokes(x => _dataImporterSettings = x.GetArgument<DataImporterSettings>(3))
            .Returns(_importedObservedData);
      }

      protected override void Because()
      {
         _result = sut.ImportFrom(_configuration);
      }

      [Observation]
      public void should_return_the_observed_data_imported_from_the_selected_file()
      {
         _result.ShouldBeEqualTo(_importedObservedData);
      }

      [Observation]
      public void should_import_without_asking_the_user_to_confirm_the_mapping()
      {
         _dataImporterSettings.PromptForConfirmation.ShouldBeFalse();
      }
   }

   public class When_adding_imported_observed_data_to_the_project : concern_for_ImportObservedDataTask
   {
      private DataRepository _observedData;

      protected override void Context()
      {
         base.Context();
         _observedData = DomainHelperForSpecs.ObservedData("Obs");
         _observedData.Name = "Obs";
         _observedData.ExtendedProperties.Add(new ExtendedProperty<string> { Name = Constants.ObservedData.ORGAN, Value = "Liver" });
         _observedData.ExtendedProperties.Add(new ExtendedProperty<string> { Name = Constants.ObservedData.COMPARTMENT, Value = "Plasma" });
         _observedData.ExtendedProperties.Add(new ExtendedProperty<string> { Name = Constants.ObservedData.MOLECULE, Value = "Drug" });
      }

      protected override void Because()
      {
         sut.AddImportedObservedDataToProject(_observedData);
      }

      [Observation]
      public void should_update_the_molecular_weight_from_the_molecule()
      {
         A.CallTo(() => _parameterChangeUpdater.UpdateMolWeightIn(_observedData)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_observed_data_to_the_project()
      {
         A.CallTo(() => _observedDataTask.AddObservedDataToProject(_observedData)).MustHaveHappened();
      }

      [Observation]
      public void should_give_the_columns_the_path_of_the_matching_simulation_output()
      {
         var column = _observedData.AllButBaseGrid().Single();
         column.QuantityInfo.Path.ShouldOnlyContainInOrder("Obs", CoreConstants.ContainerName.ObservedData, "Liver", "Plasma", "Drug", column.Name);
      }
   }
}
