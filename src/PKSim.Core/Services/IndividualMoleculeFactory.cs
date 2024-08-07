using System;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using static PKSim.Core.CoreConstants.CalculationMethod;
using static PKSim.Core.CoreConstants.Parameters;
using FormulaCache = OSPSuite.Core.Domain.Formulas.FormulaCache;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IIndividualMoleculeFactory : ISpecification<Type>
   {
      /// <summary>
      ///    Returns an empty <see cref="IndividualMolecule" />  (only parameters are defined in the protein, no protein
      ///    container)
      /// </summary>
      IndividualMolecule CreateEmpty();

      IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName);

      /// <summary>
      ///    Add all predefined ontogeny parameters to the global molecule. This is only required for actual SimulationSubject
      /// </summary>
      /// <param name="individualMolecule"></param>
      void AddAgeDependentOntogenyParametersTo(IndividualMolecule individualMolecule);
   }

   public abstract class IndividualMoleculeFactory<TMolecule, TMoleculeExpressionContainer> : IIndividualMoleculeFactory
      where TMolecule : IndividualMolecule
      where TMoleculeExpressionContainer : MoleculeExpressionContainer
   {
      protected readonly IEntityPathResolver _entityPathResolver;
      private readonly IIdGenerator _idGenerator;
      protected readonly IParameterRateRepository _parameterRateRepository;
      private readonly string _containerPath;
      protected readonly IObjectBaseFactory _objectBaseFactory;
      protected readonly IObjectPathFactory _objectPathFactory;
      protected readonly IParameterFactory _parameterFactory;

      protected IndividualMoleculeFactory(
         IObjectBaseFactory objectBaseFactory,
         IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory,
         IEntityPathResolver entityPathResolver,
         IIdGenerator idGenerator,
         IParameterRateRepository parameterRateRepository,
         string containerPath)
      {
         _objectBaseFactory = objectBaseFactory;
         _parameterFactory = parameterFactory;
         _objectPathFactory = objectPathFactory;
         _entityPathResolver = entityPathResolver;
         _idGenerator = idGenerator;
         _parameterRateRepository = parameterRateRepository;
         _containerPath = containerPath;
      }

      public bool IsSatisfiedBy(Type item) => item.IsAnImplementationOf<TMolecule>();

      protected abstract ApplicationIcon Icon { get; }

      public virtual IndividualMolecule CreateEmpty() => CreateMolecule(string.Empty);

      public abstract IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName);

      protected bool HasAgeParameter(ISimulationSubject simulationSubject)
      {
         return simulationSubject?.Individual?.AgeParameter != null;
      }

      protected ParameterRateMetaData RelExpParam(string paramName)
      {
         return _parameterRateRepository.ParameterMetaDataFor(_containerPath, paramName);
      }

      protected ParameterRateMetaData FractionParam(string paramName, string rate, bool? visible = null)
      {
         var param = rateParam(paramName, rate);
         param.Visible = visible.GetValueOrDefault(param.Visible);
         return param;
      }

      protected ParameterRateMetaData InitialConcentrationParam(string rate) => rateParam(INITIAL_CONCENTRATION, rate);

      protected ParameterRateMetaData OntogenyFactorFromTable(string parameterName, string rate) => rateParam(parameterName, rate, ONTOGENY_FACTORS);

      private ParameterRateMetaData rateParam(string paramName, string rate, string calculationMethod = "")
      {
         var parameterMetaData = _parameterRateRepository.ParameterMetaDataFor(_containerPath, paramName, calculationMethod);
         var parameterRateMetaData = new ParameterRateMetaData();
         parameterRateMetaData.UpdatePropertiesFrom(parameterMetaData);
         parameterRateMetaData.Rate = rate;
         return parameterRateMetaData;
      }

      protected TMolecule CreateMolecule(string moleculeName, bool isAgeDependent = false)
      {
         var molecule = _objectBaseFactory.Create<TMolecule>().WithIcon(Icon.IconName).WithName(moleculeName);
         CreateMoleculeParameterIn(molecule, REFERENCE_CONCENTRATION, CoreConstants.DEFAULT_REFERENCE_CONCENTRATION_VALUE);
         CreateMoleculeParameterIn(molecule, HALF_LIFE_LIVER, CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_LIVER_VALUE_IN_MIN);
         CreateMoleculeParameterIn(molecule, HALF_LIFE_INTESTINE, CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_INTESTINE_VALUE_IN_MIN);
         CreateMoleculeParameterIn(molecule, DISEASE_FACTOR, CoreConstants.DEFAULT_DISEASE_FACTOR);

         //Default ontogeny parameter tables created for ALL molecules for age dependent species only
         if (isAgeDependent)
            AddAgeDependentOntogenyParametersTo(molecule);
         else
            AddConstantOntogenyParametersTo(molecule);

         return molecule;
      }

      public void AddAgeDependentOntogenyParametersTo(IndividualMolecule molecule)
      {
         OntogenyFactorTables.Each(x => CreateMoleculeParameterIn(molecule, x, CoreConstants.DEFAULT_ONTOGENY_FACTOR, ONTOGENY_FACTOR));

         AddGlobalExpression(molecule,
            OntogenyFactorFromTable(ONTOGENY_FACTOR, CoreConstants.Rate.ONTOGENY_FACTOR_FROM_TABLE),
            OntogenyFactorFromTable(ONTOGENY_FACTOR_GI, CoreConstants.Rate.ONTOGENY_FACTOR_GI_FROM_TABLE));
      }

      public void AddConstantOntogenyParametersTo(IndividualMolecule undefinedMolecule)
      {
         //Constant ontogeny parameters added for undefined enzymes
         OntogenyFactors.Each(x => CreateMoleculeParameterIn(undefinedMolecule, x, CoreConstants.DEFAULT_ONTOGENY_FACTOR));
      }

      protected IParameter CreateFormulaParameterIn(
         IContainer parameterContainer,
         ParameterRateMetaData parameterRateMetaData,
         string moleculeName,
         string groupName = null)
      {
         var parameter = _parameterFactory.CreateFor(parameterRateMetaData, new FormulaCache());
         parameterContainer.Add(parameter);

         if (!string.IsNullOrEmpty(groupName))
            parameter.GroupName = groupName;

         //Because we update the formula of the parameter, we need to make sure we also reset the formulaId so that it will appear as being unique
         parameter.Formula.ReplaceKeywordsInObjectPaths(new[] {ObjectPathKeywords.MOLECULE}, new[] {moleculeName});
         parameter.Formula.Id = _idGenerator.NewId();

         //All constant parameters do not have default values
         parameter.DefaultValue = null;
         return parameter;
      }

      protected void AddParameterIn(IContainer container, ParameterMetaData parameterMetaData, string moleculeName, string groupName = null)
      {
         switch (parameterMetaData)
         {
            case ParameterRateMetaData rateMetaData:
               CreateFormulaParameterIn(container, rateMetaData, moleculeName, groupName);
               break;
            case ParameterValueMetaData parameterValueMetaData:
               CreateConstantParameterIn(container, parameterValueMetaData, groupName);
               break;
         }
      }

      protected IParameter CreateConstantParameterIn(IContainer parameterContainer,
         ParameterValueMetaData parameterValueDefinition,
         string groupName = CoreConstants.Groups.RELATIVE_EXPRESSION)
      {
         var parameter = _parameterFactory.CreateFor(parameterValueDefinition);
         parameterContainer.Add(parameter);
         if (!string.IsNullOrEmpty(groupName))
            parameter.GroupName = groupName;

         //All constant parameters do not have default values
         parameter.DefaultValue = null;
         return parameter;
      }

      protected IParameter CreateMoleculeParameterIn(IContainer parameterContainer, string parameterName, double defaultValue, string calculationMethod = "")
      {
         var parameterRateMetaData = _parameterRateRepository.ParameterMetaDataFor(_containerPath, parameterName, calculationMethod);
         var parameterValue = new ParameterValueMetaData();
         parameterValue.UpdatePropertiesFrom(parameterRateMetaData);
         parameterValue.DefaultValue = defaultValue;
         return CreateConstantParameterIn(parameterContainer, parameterValue);
      }

      protected void AddGlobalExpression(IContainer moleculeContainer, params ParameterMetaData[] parameters)
      {
         parameters.Each(p => AddParameterIn(moleculeContainer, p, moleculeContainer.Name));
      }

      protected virtual TMoleculeExpressionContainer AddContainerExpression(IContainer parentContainer, string moleculeName,
         params ParameterMetaData[] parameters)
      {
         var expressionContainer = _objectBaseFactory.Create<TMoleculeExpressionContainer>()
            .WithName(moleculeName)
            .WithParentContainer(parentContainer);
         parameters.Each(p => AddParameterIn(expressionContainer, p, moleculeName));
         return expressionContainer;
      }
   }
}