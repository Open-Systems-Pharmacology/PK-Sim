using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using IEntitiesInContainerRetriever = PKSim.Core.Services.IEntitiesInContainerRetriever;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using ISimulationAnalysisCreator = PKSim.Core.Services.ISimulationAnalysisCreator;

namespace PKSim.Presentation.Services
{
   public class PopulationAnalysisTemplateTask : IPopulationAnalysisTemplateTask
   {
      private readonly ITemplateTask _templateTask;
      private readonly IDialogCreator _dialogCreator;
      private readonly IEntitiesInContainerRetriever _entitiesInContainerRetriever;
      private readonly IKeyPathMapper _keyPathMapper;
      private readonly IEntityTask _entityTask;
      private readonly ICloner _cloner;
      private readonly IAnalysableToSimulationAnalysisWorkflowMapper _simulationAnalysisWorkflowMapper;
      private readonly ISimulationAnalysisCreator _simulationAnalysisCreator;
      private readonly ILazyLoadTask _lazyLoadTask;

      public PopulationAnalysisTemplateTask(ITemplateTask templateTask, IDialogCreator dialogCreator,
         IEntitiesInContainerRetriever entitiesInContainerRetriever, IKeyPathMapper keyPathMapper,
         IEntityTask entityTask, ICloner cloner, IAnalysableToSimulationAnalysisWorkflowMapper simulationAnalysisWorkflowMapper,
         ISimulationAnalysisCreator simulationAnalysisCreator, ILazyLoadTask lazyLoadTask)
      {
         _templateTask = templateTask;
         _dialogCreator = dialogCreator;
         _entitiesInContainerRetriever = entitiesInContainerRetriever;
         _keyPathMapper = keyPathMapper;
         _entityTask = entityTask;
         _cloner = cloner;
         _simulationAnalysisWorkflowMapper = simulationAnalysisWorkflowMapper;
         _simulationAnalysisCreator = simulationAnalysisCreator;
         _lazyLoadTask = lazyLoadTask;
      }

      public void SaveDerivedField(PopulationAnalysisDerivedField derivedField)
      {
         _templateTask.SaveToTemplate(derivedField, TemplateType.PopulationAnalysisField);
      }

      public void SavePopulationAnalysis<TPopulationAnalysis>(TPopulationAnalysis populationAnalysis) where TPopulationAnalysis : PopulationAnalysis
      {
         _templateTask.SaveToTemplate(populationAnalysis, TemplateType.PopulationAnalysis, string.Empty);
      }

      public void LoadPopulationAnalysisWorkflowInto(IPopulationDataCollector populationDataCollector)
      {
         var populationAnalysisWorkflow = _templateTask.LoadFromTemplate<SimulationAnalysisWorkflow>(TemplateType.PopulationSimulationAnalysisWorkflow);
         if (populationAnalysisWorkflow == null)
            return;

         _lazyLoadTask.Load(populationDataCollector as ILazyLoadable);

         populationAnalysisWorkflow.AllAnalyses.Each(x => { _simulationAnalysisCreator.AddSimulationAnalysisTo(populationDataCollector, x); });

         var populationSimulation = populationDataCollector as PopulationSimulation;
         if (populationSimulation == null) return;

         populationSimulation.OutputSelections.UpdatePropertiesFrom(populationAnalysisWorkflow.OutputSelections, _cloner);
      }

      public void SavePopulationAnalysisWorkflowFrom(IPopulationDataCollector populationDataCollector)
      {
         _lazyLoadTask.Load(populationDataCollector as ILazyLoadable);
         var populationAnalysisWorkflow = _simulationAnalysisWorkflowMapper.MapFrom(populationDataCollector);
         _templateTask.SaveToTemplate(populationAnalysisWorkflow, TemplateType.PopulationSimulationAnalysisWorkflow);
      }

      public PopulationAnalysisDerivedField LoadDerivedFieldFor(PopulationAnalysis populationAnalysis, PopulationAnalysisDataField populationAnalysisDataField)
      {
         var field = _templateTask.LoadFromTemplate<PopulationAnalysisDerivedField>(TemplateType.PopulationAnalysisField);
         if (field == null)
            return null;

         //validate data type
         if (!field.CanBeUsedFor(populationAnalysisDataField.DataType))
            throw new PKSimException(PKSimConstants.Error.DerivedFieldCannotBeUsedForFieldOfType(field.Name, populationAnalysisDataField.Name, populationAnalysisDataField.DataType));

         //Rename referenced once loaded
         var groupingField = field as PopulationAnalysisGroupingField;
         if (groupingField != null)
         {
            if (!validateReferenceFieldInGrouping(populationAnalysisDataField, groupingField))
               return null;
         }

         //rename field if required
         if (populationAnalysis.Has(field.Name))
         {
            var name = renameFieldAfterImport(populationAnalysis, field);
            if (string.IsNullOrEmpty(name))
               return null;

            field.Name = name;
         }

         return field;
      }

      private bool validateReferenceFieldInGrouping(PopulationAnalysisDataField dataField, PopulationAnalysisGroupingField groupingField)
      {
         string referencedFieldName = groupingField.ReferencedFieldName;
         if (!string.Equals(referencedFieldName, dataField.Name, StringComparison.OrdinalIgnoreCase))
         {
            var shouldUseField = _dialogCreator.MessageBoxYesNo(PKSimConstants.Warning.DerivedFieldWasSavedForAnotherField(referencedFieldName, dataField.Name));
            if (shouldUseField == ViewResult.No)
               return false;
         }

         groupingField.RenameReferencedFieldTo(dataField.Name);
         return true;
      }

      private string renameFieldAfterImport(PopulationAnalysis populationAnalysis, PopulationAnalysisDerivedField field)
      {
         return _entityTask.NewNameFor(field, populationAnalysis.AllFields.Select(x => x.Name), PKSimConstants.UI.Grouping);
      }

      public TPopulationAnalysis LoadPopulationAnalysisFor<TPopulationAnalysis>(IPopulationDataCollector populationDataCollector) where TPopulationAnalysis : PopulationAnalysis, new()
      {
         //first load the template as basic population analysis.
         var populationAnalysis = _templateTask.LoadFromTemplate<PopulationAnalysis>(TemplateType.PopulationAnalysis);
         if (populationAnalysis == null)
            return null;

         var typedPopulationAnalysis = convertedAnalysis<TPopulationAnalysis>(populationAnalysis);
         if (synchronizeFieldsWithPopulation(typedPopulationAnalysis, populationDataCollector))
            return typedPopulationAnalysis;

         return null;
      }

      private bool synchronizeFieldsWithPopulation(PopulationAnalysis populationAnalysis, IPopulationDataCollector populationDataCollector)
      {
         var allErrors = new List<string>();

         synchronizeOutputs(populationAnalysis, populationDataCollector, allErrors);
         synchronizeCovariates(populationAnalysis, populationDataCollector, allErrors);
         synchronizeParameters(populationAnalysis, populationDataCollector, allErrors);
         synchronizePKParameters(populationAnalysis, populationDataCollector, allErrors);

         if (!allErrors.Any())
            return true;

         allErrors.Insert(0, PKSimConstants.Error.ErrorWhileImportingPopulationAnalyses);
         var result = _dialogCreator.MessageBoxYesNo(allErrors.ToString("\n"), PKSimConstants.UI.ImportAnyway, PKSimConstants.UI.CancelButton);
         return result == ViewResult.Yes;
      }

      private void synchronizeCovariates(PopulationAnalysis populationAnalysis, IPopulationDataCollector populationDataCollector, List<string> allErrors)
      {
         //ToList() because we are removing in the iteration 
         foreach (var covariateField in populationAnalysis.AllFields.OfType<PopulationAnalysisCovariateField>().ToList())
         {
            if (populationDataCollector.AllCovariateNames().Contains(covariateField.Covariate))
               continue;

            populationAnalysis.Remove(covariateField);
            allErrors.Add(PKSimConstants.Error.CovariateNotFoundWillBeRemovedFromAnalysis(covariateField.Covariate));
         }
      }

      private void synchronizeParameters(PopulationAnalysis populationAnalysis, IPopulationDataCollector populationDataCollector, List<string> allErrors)
      {
         var allParameters = _entitiesInContainerRetriever.ParametersFrom(populationDataCollector);

         //ToList() because we are removing in the iteration 
         foreach (var parameterField in populationAnalysis.AllFields.OfType<PopulationAnalysisParameterField>().ToList())
         {
            if (allParameters.Contains(parameterField.ParameterPath))
               continue;

            populationAnalysis.Remove(parameterField);
            allErrors.Add(PKSimConstants.Error.ParameterNotFoundWillBeRemovedFromAnalysis(parameterField.ParameterPath));
         }
      }

      private void synchronizeOutputs(PopulationAnalysis populationAnalysis, IPopulationDataCollector populationDataCollector, List<string> allErrors)
      {
         var statiscalAnalsysis = populationAnalysis as PopulationStatisticalAnalysis;
         var keyPathCache = outputKeyPathCacheFor(populationDataCollector);

         foreach (var outputField in populationAnalysis.AllFields.OfType<PopulationAnalysisOutputField>().ToList())
         {
            if (statiscalAnalsysis == null)
            {
               populationAnalysis.Remove(outputField);
               allErrors.Add(PKSimConstants.Error.OutputFieldCannotBeUsedInAnalysis(outputField.Name));
               continue;
            }

            if (fieldForOutputWasRemoved(outputField, keyPathCache, populationAnalysis, allErrors))
               continue;
         }
      }

      private bool fieldForOutputWasRemoved<TField>(TField field, Cache<string, string> keyPathCache, PopulationAnalysis populationAnalysis, List<string> allErrors) where TField : IQuantityField
      {
         //check that the quantity is also available 
         var path = pathFrom(field.QuantityPath, field.QuantityType);
         if (keyPathCache.Contains(path))
            return false;

         populationAnalysis.Remove(field);
         allErrors.Add(PKSimConstants.Error.QuantityNotFoundWillBeRemovedFromAnalysis(field.QuantityPath));
         return true;
      }

      private void synchronizePKParameters(PopulationAnalysis populationAnalysis, IPopulationDataCollector populationDataCollector, List<string> allErrors)
      {
         //path where compound name has been removed
         var keyPathCache = outputKeyPathCacheFor(populationDataCollector);

         foreach (var pkParameterField in populationAnalysis.AllFields.OfType<PopulationAnalysisPKParameterField>().ToList())
         {
            if (fieldForOutputWasRemoved(pkParameterField, keyPathCache, populationAnalysis, allErrors))
               continue;

            var path = pathFrom(pkParameterField.QuantityPath, pkParameterField.QuantityType);

            //quantity was found. Yet it's not over. We need to verify that the pk parameter is also available
            pkParameterField.QuantityPath = keyPathCache[path];
            if (!populationDataCollector.HasPKParameterFor(pkParameterField.QuantityPath, pkParameterField.PKParameter))
            {
               populationAnalysis.Remove(pkParameterField);
               allErrors.Add(PKSimConstants.Error.PKParameterWasNotCalculatedForQuantity(pkParameterField.PKParameter, pkParameterField.QuantityPath));
            }
         }
      }

      private string pathFrom(string quantityPath, QuantityType quantityType)
      {
         return _keyPathMapper.MapFrom(quantityPath, quantityType, removeFirstEntry: true).Path;
      }

      private Cache<string, string> outputKeyPathCacheFor(IPopulationDataCollector populationDataCollector)
      {
         var allOutputs = _entitiesInContainerRetriever.OutputsFrom(populationDataCollector);
         var keyPathCache = new Cache<string, string>();
         foreach (var quantityKeyValues in allOutputs.KeyValues)
         {
            var quantity = quantityKeyValues.Value;
            keyPathCache[pathFrom(quantityKeyValues.Key, quantity.QuantityType)] = quantityKeyValues.Key;
         }
         return keyPathCache;
      }

      private TPopulationAnalysis convertedAnalysis<TPopulationAnalysis>(PopulationAnalysis templatePopulationAnalysis) where TPopulationAnalysis : PopulationAnalysis, new()
      {
         //same type? No need to convert the template. It can be use as is
         if (templatePopulationAnalysis.GetType() == typeof (TPopulationAnalysis))
            return templatePopulationAnalysis.DowncastTo<TPopulationAnalysis>();

         var typedAnalysis = new TPopulationAnalysis();
         typedAnalysis.UpdatePropertiesFrom(templatePopulationAnalysis, _cloner);
         return typedAnalysis;
      }
   }
}