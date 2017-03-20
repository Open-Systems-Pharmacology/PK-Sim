using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Creates a new instance of application builder by cloning application template (given by {ApplicationType, FormulationType} combination) Formulation parameters collection is only required for particles dissolution
   /// </summary>
   public interface IApplicationFactory
   {
      IApplicationBuilder CreateFor(ISchemaItem schemaItem, string formulationType, string applicationName,
                                    string compoundName, IEnumerable<IParameter> formulationParameters, IFormulaCache formulaCache);
   }

   public class ApplicationFactory : IApplicationFactory
   {
      private readonly IApplicationRepository _applicationRepository;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IObjectPathFactory _objectPathFactory;
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IParticleApplicationCreator _particleApplicationCreator;
      private readonly ICloneManagerForBuildingBlock _cloneManagerForBuildingBlock;

      public ApplicationFactory(IApplicationRepository applicationRepository, IObjectBaseFactory objectBaseFactory,
                                IObjectPathFactory objectPathFactory, IParameterSetUpdater parameterSetUpdater,
                                IDimensionRepository dimensionRepository, IParameterContainerTask parameterContainerTask,
                                IParticleApplicationCreator particleApplicationCreator,ICloneManagerForBuildingBlock cloneManagerForBuildingBlock)
      {
         _applicationRepository = applicationRepository;
         _objectBaseFactory = objectBaseFactory;
         _objectPathFactory = objectPathFactory;
         _parameterSetUpdater = parameterSetUpdater;
         _dimensionRepository = dimensionRepository;
         _parameterContainerTask = parameterContainerTask;
         _particleApplicationCreator = particleApplicationCreator;
         _cloneManagerForBuildingBlock = cloneManagerForBuildingBlock;
      }

      public IApplicationBuilder CreateFor(ISchemaItem schemaItem, string formulationType, string applicationName,
                                           string compoundName, IEnumerable<IParameter> formulationParameters, IFormulaCache formulaCache)
      {
         // clone new application from template
         _cloneManagerForBuildingBlock.FormulaCache =formulaCache;
         var application = _cloneManagerForBuildingBlock.Clone(_applicationRepository.ApplicationFrom(schemaItem.ApplicationType.Name, formulationType));

         var applicDbName = application.Name; //required for adding application transport parameters
         application.Name = applicationName;
         application.EventGroupType = schemaItem.ApplicationType.Name;

         // set protocol schema item values
         copyParameterValues(schemaItem, application.ProtocolSchemaItemContainer());

         // set name of compound (= molecule which will be transported by this application)
         application.MoleculeName = compoundName;
         application.Icon = schemaItem.ApplicationType.IconName;

         //add 'MOLECULE' Tag to the Drug mass parameter of the application
         var drugMassParameter = application.ProtocolSchemaItemContainer().Parameter(Constants.Parameters.DRUG_MASS);
         drugMassParameter?.AddTag(ObjectPathKeywords.MOLECULE);

         // add start formula(s) for the drug molecule
         if (string.Equals(formulationType, CoreConstants.Formulation.Particles))
            _particleApplicationCreator.CreateParticleIn(application, formulationParameters, formulaCache);
         else
            addDrugStartFormula(application, formulaCache);

         // add "Application" and <ApplicationName> as new Tags 
         //to application and all its subcontainers
         addApplicationTags(application, CoreConstants.Tags.APPLICATION);
         addApplicationTags(application, application.Name);

         //Add "ApplicationRoot"-Tag to application root container
         addTagToContainer(application, CoreConstants.Tags.APPLICATION_ROOT);

         // add <ApplicationName>-Descriptor-Criteria to all
         //application processes source and or target, whose descriptor criteria
         //also contains "Application"-keyword
         addApplicationProcessDescriptorCondition(schemaItem, application.Transports, application.Name);

         // add application transport parameters
         addApplicationTransportParameters(application.Transports, applicDbName, formulationType, formulaCache);

         return application;
      }

      private void addApplicationTransportParameters(IEnumerable<ITransportBuilder> transports, string applicationTypeName, string formulationType, IFormulaCache formulaCache)
      {
         foreach (var transportBuilder in transports)
         {
            _parameterContainerTask.AddApplicationTransportParametersTo(transportBuilder, applicationTypeName, formulationType, formulaCache);
         }
      }

      /// <summary>
      ///    Add "condition" as descriptor criteria to every source/target descriptor conditions of given transports, which also contain "Application" condition
      /// </summary>
      private void addApplicationProcessDescriptorCondition(ISchemaItem schemaItem, IEnumerable<ITransportBuilder> transports, string condition)
      {
         foreach (var applicationTransport in transports)
         {
            addApplicationDescriptorConditionTo(applicationTransport.SourceCriteria, condition);
            addApplicationDescriptorConditionTo(applicationTransport.TargetCriteria, condition);

            if (schemaItem.ApplicationType == ApplicationTypes.UserDefined)
            {
               applicationTransport.TargetCriteria = Create.Criteria(x => x.With(schemaItem.TargetOrgan).And.With(schemaItem.TargetCompartment));
            }
         }
      }

      private void addApplicationDescriptorConditionTo(DescriptorCriteria descriptorCriteria, string condition)
      {
         if (descriptorCriteria.Contains(new MatchTagCondition(CoreConstants.Tags.APPLICATION)))
            descriptorCriteria.Add(new MatchTagCondition(condition));
      }

      private void addApplicationTags(IApplicationBuilder applicationBuilder, string tagValue)
      {
         addTagToContainer(applicationBuilder, tagValue);

         foreach (var subContainer in applicationBuilder.GetAllChildren<IContainer>())
         {
            addTagToContainer(subContainer, tagValue);
         }
      }

      private void addTagToContainer(IContainer container, string tagValue)
      {
         if (!container.Tags.Contains(tagValue))
            container.AddTag(tagValue);
      }

      /// <summary>
      ///    Add start formula for the drug molecule. Will be created in the root application container
      /// </summary>
      private void addDrugStartFormula(IApplicationBuilder applicationBuilder, IFormulaCache formulaCache)
      {
         var appMoleculeBuilder = _objectBaseFactory.Create<IApplicationMoleculeBuilder>().WithName(applicationBuilder.Name);
         appMoleculeBuilder.RelativeContainerPath = _objectPathFactory.CreateObjectPathFrom(ObjectPath.PARENT_CONTAINER, applicationBuilder.Name);
         appMoleculeBuilder.Formula = drugMassFormula(formulaCache);
         applicationBuilder.AddMolecule(appMoleculeBuilder);
      }

      private IFormula drugMassFormula(IFormulaCache formulaCache)
      {
         if (formulaCache.ExistsByName(Constants.Parameters.DRUG_MASS))
            return formulaCache.FindByName(Constants.Parameters.DRUG_MASS);

         var startFormula = _objectBaseFactory.Create<ExplicitFormula>()
            .WithFormulaString(Constants.Parameters.DRUG_MASS)
            .WithDimension(_dimensionRepository.Amount)
            .WithName(Constants.Parameters.DRUG_MASS);

         var pathToDrugMass = _objectPathFactory.CreateFormulaUsablePathFrom(ObjectPath.PARENT_CONTAINER, CoreConstants.ContainerName.ProtocolSchemaItem, Constants.Parameters.DRUG_MASS)
            .WithAlias(Constants.Parameters.DRUG_MASS)
            .WithDimension(_dimensionRepository.Amount);

         startFormula.AddObjectPath(pathToDrugMass);
         formulaCache.Add(startFormula);
         return startFormula;
      }

      private void copyParameterValues(ISchemaItem schemaItem, IContainer targetContainer)
      {
         _parameterSetUpdater.UpdateValuesByName(schemaItem, targetContainer);

         //now that parameter have been set, we need to update the dose by hand according to the selected unit 
         var inputDose = schemaItem.Dose;
         var doseDimension = _dimensionRepository.Mass;
         var dose = targetContainer.Parameter(CoreConstants.Parameter.DOSE);
         var dosePerBodyWeight = targetContainer.Parameter(CoreConstants.Parameter.DOSE_PER_BODY_WEIGHT);
         //dose exists in the dose dimension=> we need to set the parameter dose in the target container

         //We have a dose in mg
         if (doseDimension.GetUnitNames().Contains(inputDose.DisplayUnit.Name))
         {
            //we have to create a constant value for the dose parameter
            dose.Formula = _objectBaseFactory.Create<ConstantFormula>().WithValue(inputDose.Value).WithDimension(_dimensionRepository.Mass);
            _parameterSetUpdater.UpdateValue(inputDose, dose);
            dose.Visible = true;
            dosePerBodyWeight.Visible = false;
            dosePerBodyWeight.BuildingBlockType = PKSimBuildingBlockType.Simulation;
         }
         else
         {
            //in that case. We have a dose in mg/kg
            _parameterSetUpdater.UpdateValue(inputDose, dosePerBodyWeight);
            dosePerBodyWeight.Visible = true;
            dose.Visible = false;
            dose.BuildingBlockType = PKSimBuildingBlockType.Simulation;
         }
      }
   }
}