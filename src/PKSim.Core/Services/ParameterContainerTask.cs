using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IParameterContainerTask
   {
      void AddIndividualParametersTo<TContainer>(TContainer parameterContainer, OriginData originData) where TContainer : IContainer;
      void AddIndividualParametersTo<TContainer>(TContainer parameterContainer, OriginData originData, string parameterName) where TContainer : IContainer;

      void AddActiveProcessParametersTo(CompoundProcess process);
      void AddProcessBuilderParametersTo(IContainer process);
      void AddFormulationParametersTo(Formulation formulation);
      void AddMoleculeParametersTo(IMoleculeBuilder molecule, IFormulaCache formulaCache);
      void AddApplicationParametersTo(IContainer container);
      void AddSchemaItemParametersTo(ISchemaItem schemaItem);
      void AddEventParametersTo(IContainer container);
      void AddCompoundParametersTo(Compound compound);

      void AddModelParametersTo<TContainer>(TContainer parameterContainer, OriginData originData, ModelProperties modelProperties, IFormulaCache formulaCache) where TContainer : IContainer;
      void AddApplicationTransportParametersTo(ITransportBuilder applicationTransportBuilder, string applicationName, string formulationName, IFormulaCache formulaCache);
   }

   public class ParameterContainerTask : IParameterContainerTask
   {
      private readonly IParameterFactory _parameterFactory;
      private readonly IParameterQuery _parameterQuery;

      public ParameterContainerTask(IParameterQuery parameterQuery, IParameterFactory parameterFactory)
      {
         _parameterQuery = parameterQuery;
         _parameterFactory = parameterFactory;
      }

      public void AddIndividualParametersTo<TContainer>(TContainer parameterContainer, OriginData originData) where TContainer : IContainer
      {
         addParametersTo(parameterContainer, originData, originData.AllCalculationMethods().Select(cm => cm.Name), param => param.BuildingBlockType == PKSimBuildingBlockType.Individual);
      }

      public void AddIndividualParametersTo<TContainer>(TContainer parameterContainer, OriginData originData, string parameterName) where TContainer : IContainer
      {
         addParametersTo(parameterContainer, originData, originData.AllCalculationMethods().Select(cm => cm.Name),
            param => param.BuildingBlockType == PKSimBuildingBlockType.Individual && string.Equals(param.ParameterName, parameterName));
      }

      public void AddModelParametersTo<TContainer>(TContainer parameterContainer, OriginData originData, ModelProperties modelProperties, IFormulaCache formulaCache) where TContainer : IContainer
      {
         addParametersTo(parameterContainer, originData, modelProperties.AllCalculationMethods().Select(cm => cm.Name), param => true, formulaCache);
      }

      public void AddActiveProcessParametersTo(CompoundProcess process)
      {
         addParametersTo(process, null, CoreConstants.CalculationMethod.ForProcesses);
      }

      public void AddApplicationTransportParametersTo(ITransportBuilder applicationTransportBuilder, string applicationName, string formulationName, IFormulaCache formulaCache)
      {
         //assuming that all application transports are located directly under
         //the application container
         var formulation = new Container().WithName(formulationName);
         var application = new Container().WithName(applicationName);
         var transport = new Container().WithName(applicationTransportBuilder.Name);

         formulation.Add(application);
         application.Add(transport);

         addParametersTo(transport, null, CoreConstants.CalculationMethod.ForApplications, x => true, formulaCache);

         updateProcessesParameters(transport, applicationTransportBuilder);
      }

      public void AddProcessBuilderParametersTo(IContainer processBuilder)
      {
         var container = new Container().WithName(processBuilder.Name);
         addParametersTo(container, null, CoreConstants.CalculationMethod.ForProcesses);
         updateProcessesParameters(container, processBuilder);
      }

      private static void updateProcessesParameters(IContainer container, IContainer processesBuilder)
      {
         foreach (var parameter in container.AllParameters())
         {
            var parameterName = parameter.Name;
            //parameter already available in process. nothing to do
            if (processesBuilder.ContainsName(parameterName))
               continue;

            processesBuilder.Add(parameter);
         }
      }

      public void AddFormulationParametersTo(Formulation formulation)
      {
         string oldName = formulation.Root.Name;
         formulation.Root.Name = formulation.Name;
         addParametersTo(formulation.Root, null, CoreConstants.CalculationMethod.ForFormulations);
         formulation.Root.Name = oldName;
      }

      public void AddMoleculeParametersTo(IMoleculeBuilder molecule, IFormulaCache formulaCache)
      {
         addParametersTo(molecule, null, CoreConstants.CalculationMethod.ForCompounds, x => true, formulaCache);
      }

      public void AddApplicationParametersTo(IContainer container)
      {
         addParametersTo(container, null, CoreConstants.CalculationMethod.ForApplications);
         //parameter input dose should not be added to the container 
         var inputDose = container.Parameter(CoreConstants.Parameters.INPUT_DOSE);
         if (inputDose != null)
            container.RemoveChild(inputDose);
      }

      public void AddSchemaItemParametersTo(ISchemaItem schemaItem)
      {
         addParametersTo(schemaItem, null, CoreConstants.CalculationMethod.ForSchemaItems);
      }

      public void AddEventParametersTo(IContainer container)
      {
         addParametersTo(container, null, CoreConstants.CalculationMethod.ForEvents);
      }

      public void AddCompoundParametersTo(Compound compound)
      {
         //reset compound name
         string oldName = compound.Root.Name;
         compound.Root.Name = CoreConstants.ContainerName.Drug;
         addParametersTo(compound.Root, null, CoreConstants.CalculationMethod.ForCompounds, x => x.BuildingBlockType == PKSimBuildingBlockType.Compound);
         compound.Root.Name = oldName;
      }

      private void addParametersTo<T>(T parameterContainer, OriginData originData, IEnumerable<string> calculationMethods) where T : IContainer
      {
         addParametersTo(parameterContainer, originData, calculationMethods, x => true);
      }

      private void addParametersTo<T>(T parameterContainer, OriginData originData, IEnumerable<string> calculationMethods, Func<ParameterMetaData, bool> predicate, IFormulaCache formulaCache = null)
         where T : IContainer
      {
         //RATE PARAMETERS
         foreach (ParameterRateMetaData parameterRateDefinition in _parameterQuery.ParameterRatesFor(parameterContainer, calculationMethods, predicate))
         {
            //parameter already available,
            var parameter = _parameterFactory.CreateFor(parameterRateDefinition, formulaCache);
            if (shouldAddRateParameterTo(parameterContainer, parameter))
               parameterContainer.Add(parameter);
         }

         //CONSTANT PARAMETERS
         foreach (ParameterValueMetaData parameterDefinition in _parameterQuery.ParameterValuesFor(parameterContainer, originData, predicate))
         {
            parameterContainer.Add(_parameterFactory.CreateFor(parameterDefinition));
         }

         //DISTRIBUTION PARAMETERS
         foreach (var distributionGroup in _parameterQuery.ParameterDistributionsFor(parameterContainer, originData, predicate).GroupBy(dist => dist.ParameterName))
         {
            var parameter = _parameterFactory.CreateFor(distributionGroup.ToList(), originData);

            //Parameter might be distributed only for a few species
            if (parameterContainer.ContainsName(parameter.Name))
               parameterContainer.RemoveChild(parameterContainer.Parameter(parameter.Name));

            parameterContainer.Add(parameter);
         }
      }

      private static bool shouldAddRateParameterTo(IContainer parameterContainer, IParameter parameterToAdd)
      {
         //parameter does not exist. Can be added to the container
         var parameter = parameterContainer.Parameter(parameterToAdd.Name);
         if (parameter == null)
            return true;

         //parameter already exist. If the parameter have the same formula, it should not be added
         //otherwise, it is an exception (configuration problem in PKSim)
         if (parameter.Formula.IsConstant())
            throw new PKSimException(PKSimConstants.Error.ConstantParameterAlreadyExistsInContainer(parameterContainer.Name, parameter.Name));

         var formula = parameter.Formula.DowncastTo<ExplicitFormula>();
         var formulaToAdd = parameterToAdd.Formula.DowncastTo<ExplicitFormula>();
         if (string.Equals(formula.FormulaString, formulaToAdd.FormulaString))
            return false;

         throw new PKSimException(PKSimConstants.Error.FormulaParameterAlreadyExistsInContainerWithAnotherFormula(parameterContainer.Name, parameter.Name, formula.FormulaString, formulaToAdd.FormulaString));
      }
   }
}