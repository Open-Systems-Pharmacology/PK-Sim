using System;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Core.Services
{
   public interface IPKSimParameterStartValuesCreator
   {
      IParameterStartValuesBuildingBlock CreateFor(IBuildConfiguration buildConfiguration, Simulation simulation);
   }

   public class PKSimParameterStartValuesCreator : IPKSimParameterStartValuesCreator
   {
      private readonly IParameterStartValuesCreator _parameterStartValuesCreator;
      private readonly IObjectPathFactory _objectPathFactory;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IExpressionContainersRetriever _expressionContainersRetriever;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IEntityPathResolver _entityPathResolver;
      private ISpatialStructure _spatialStructure;
      private IParameterStartValuesBuildingBlock _defaultStartValues;
      private IFormulaCache _formulaCache;
      private readonly string _method;

      public PKSimParameterStartValuesCreator(IParameterStartValuesCreator parameterStartValuesCreator, IObjectPathFactory objectPathFactory,
         IObjectBaseFactory objectBaseFactory, IExpressionContainersRetriever expressionContainersRetriever, IFormulaFactory formulaFactory,
         IEntityPathResolver entityPathResolver)
      {
         _parameterStartValuesCreator = parameterStartValuesCreator;
         _objectPathFactory = objectPathFactory;
         _objectBaseFactory = objectBaseFactory;
         _expressionContainersRetriever = expressionContainersRetriever;
         _formulaFactory = formulaFactory;
         _entityPathResolver = entityPathResolver;
         _method = CoreConstants.CalculationMethod.ActiveProcess;
      }

      public IParameterStartValuesBuildingBlock CreateFor(IBuildConfiguration buildConfiguration, Simulation simulation)
      {
         try
         {
            //default default parameter start values matrix
            _spatialStructure = buildConfiguration.SpatialStructure;
            _defaultStartValues = _parameterStartValuesCreator.CreateFrom(_spatialStructure, buildConfiguration.Molecules);
            _formulaCache = _defaultStartValues.FormulaCache;
            var individual = simulation.Individual;

            //set the relative expression values for each protein defined in individual
            foreach (var protein in individual.AllMolecules<IndividualProtein>())
            {
               updateProteinParametersValues(protein);
            }

            foreach (var transporter in individual.AllMolecules<IndividualTransporter>())
            {
               updateTransporterParameterValues(transporter);
            }

            updateSimulationParameters(simulation);

            return _defaultStartValues.WithName(simulation.Name);
         }
         finally
         {
            _spatialStructure = null;
            _defaultStartValues = null;
            _formulaCache = null;
         }
      }

      private void updateSimulationParameters(Simulation simulation)
      {
         //this is only required if the simulation already has a model. That means that we should update the PSV with any
         //simulation parameters that might have been updated by the user
         if (simulation.Model == null) return;
         var allSimulationParameters = simulation.Model.Root.GetAllChildren<IParameter>(isChangedSimulationParameter);
         allSimulationParameters.Each(p =>
         {
            var psv = trySetValue(p);
            //Ensure that the formula will not become a constant after clone
            psv.OverrideFormulaWithValue = false;
         });
      }

      private bool isChangedSimulationParameter(IParameter parameter)
      {
         return parameter.BuildingBlockType.Is(PKSimBuildingBlockType.Simulation)
                && parameter.ValueDiffersFromDefault();
      }

      private void updateProteinParametersValues(IndividualProtein protein)
      {
         setGlobalParameterValue(protein, CoreConstants.Parameters.REL_EXP_BLOOD_CELL, CoreConstants.Compartment.BloodCells);
         setGlobalParameterValue(protein, CoreConstants.Parameters.REL_EXP_PLASMA, CoreConstants.Compartment.Plasma);
         setGlobalParameterValue(protein, CoreConstants.Parameters.REL_EXP_VASC_ENDO, CoreConstants.Compartment.VascularEndothelium);

         foreach (var expressionContainer in _expressionContainersRetriever.AllContainersFor(_spatialStructure, protein))
         {
            if (expressionContainer.IsBloodCell())
               setParameterValuesForBloodCells(protein, expressionContainer);
            else if (expressionContainer.IsPlasma())
               setParameterValuesForPlasma(protein, expressionContainer);
            else if (expressionContainer.IsInterstitial())
               setParameterValuesForInterstitial(protein, expressionContainer);
            else if (expressionContainer.IsEndosome())
               setParameterValuesForEndosome(protein, expressionContainer);
            else
               setParameterValuesForStandardContainer(protein, expressionContainer);
         }
      }

      private void setParameterValuesForInterstitial(IndividualProtein protein, IContainer expressionContainer)
      {
         var relExpNormPath = relExpNormPathFor(protein, expressionContainer);
         var relExpPath = relExpPathFor(protein, expressionContainer);
         var relExpOut = relExpOutPathFor(protein, expressionContainer);

         setParameterValuesForStandardContainer(protein, expressionContainer);

         //out path now depends on protein configuration
         switch (protein.TissueLocation)
         {
            case TissueLocation.ExtracellularMembrane:
               if (protein.MembraneLocation == MembraneLocation.Apical)
                  trySetFormula(relExpOut, CoreConstants.Rate.RelExpInterstialMembraneExtracellularApical);
               else
                  trySetFormula(relExpOut, CoreConstants.Rate.RelExpInterstialMembraneExtracellularBasolateral);
               break;
            case TissueLocation.Intracellular:
               if (protein.IntracellularVascularEndoLocation == IntracellularVascularEndoLocation.Interstitial)
               {
                  //reset value in interstitial that was set before 
                  trySetFormula(relExpNormPath, zeroFormula());
                  trySetFormula(relExpPath, zeroFormula());
                  trySetFormula(relExpOut, CoreConstants.Rate.RelExpInterstialIntraVascEndoIsInterstitial);
               }
               break;
            case TissueLocation.Interstitial:
               trySetFormula(relExpOut, CoreConstants.Rate.RelExpInterstialForInterstitial);
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      private void setParameterValuesForPlasma(IndividualProtein protein, IContainer expressionContainer)
      {
         trySetFormula(relExpNormPathFor(protein, expressionContainer), CoreConstants.Rate.RelExpPlasmaNormGlobal);
         trySetFormula(relExpPathFor(protein, expressionContainer), CoreConstants.Rate.RelExpPlasmaGlobal);
         var relExpOut = relExpOutPathFor(protein, expressionContainer);

         //out path now depends on protein configuration
         switch (protein.TissueLocation)
         {
            case TissueLocation.ExtracellularMembrane:
               if (protein.MembraneLocation == MembraneLocation.Apical)
                  if (expressionContainer.ParentContainer.IsBloodOrgan())
                     trySetFormula(relExpOut, CoreConstants.Rate.RelExpPlasmaMembraneExtracellularApicalBloodOrgan);
                  else
                     trySetFormula(relExpOut, CoreConstants.Rate.RelExpPlasmaMembraneExtracellularApicalTissueOrgan);

               else
                  trySetFormula(relExpOut, CoreConstants.Rate.RelExpPlasmaMembraneExtracellularBasolateral);

               break;
            case TissueLocation.Intracellular:
            case TissueLocation.Interstitial:
               trySetFormula(relExpOut, CoreConstants.Rate.RelExpOutFromNorm);
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      private void setParameterValuesForEndosome(IndividualProtein protein, IContainer expressionContainer)
      {
         trySetFormula(relExpNormPathFor(protein, expressionContainer), CoreConstants.Rate.RelExpVascEndoNormGlobal);
         trySetFormula(relExpPathFor(protein, expressionContainer), CoreConstants.Rate.RelExpVascEndoGlobal);
         trySetFormula(relExpOutPathFor(protein, expressionContainer), CoreConstants.Rate.RelExpEndosomal);
      }

      private void setParameterValuesForStandardContainer(IndividualMolecule molecule, IContainer expressionContainer)
      {
         var containerName = relativeExpressionContainerNameFrom(molecule, expressionContainer);
         var relExpNormPath = relExpNormPathFor(molecule, expressionContainer);
         var relExpPath = relExpPathFor(molecule, expressionContainer);
         trySetValue(relExpNormPath, molecule.GetRelativeExpressionNormParameterFor(containerName));
         trySetValue(relExpPath, molecule.GetRelativeExpressionParameterFor(containerName));
         var relExpOut = relExpOutPathFor(molecule, expressionContainer);
         trySetFormula(relExpOut, CoreConstants.Rate.RelExpOutFromNorm);
      }

      private void setParameterValuesForBloodCells(IndividualProtein protein, IContainer expressionContainer)
      {
         trySetFormula(relExpNormPathFor(protein, expressionContainer), CoreConstants.Rate.RelExpBloodCellsNormGlobal);
         trySetFormula(relExpPathFor(protein, expressionContainer), CoreConstants.Rate.RelExpBloodCellsGlobal);
         trySetFormula(relExpOutPathFor(protein, expressionContainer), CoreConstants.Rate.RelExpOutFromNorm);
      }

      private void updateTransporterParameterValues(IndividualTransporter transporter)
      {
         foreach (var relativeExpressionContainer in _expressionContainersRetriever.AllContainersFor(_spatialStructure, transporter))
         {
            setParameterValuesForStandardContainer(transporter, relativeExpressionContainer);
         }
      }

      private string relativeExpressionContainerNameFrom(IndividualMolecule individualMolecule, IContainer relativeExpressionContainer)
      {
         return _expressionContainersRetriever.RelativeExpressionContainerNameFrom(individualMolecule, relativeExpressionContainer);
      }

      private IParameterStartValue trySetValue(IParameter parameter)
      {
         var parameterPath = _entityPathResolver.ObjectPathFor(parameter);
         if (_defaultStartValues[parameterPath] != null)
            return trySetValue(parameterPath, parameter);

         var parameterStartValue = _parameterStartValuesCreator.CreateParameterStartValue(parameterPath, parameter);
         _defaultStartValues.Add(parameterStartValue);
         return parameterStartValue;
      }

      private IParameterStartValue trySetValue(IObjectPath parameterPath, IParameter parameter)
      {
         return trySetValue(parameterPath, parameter.Value);
      }

      private IParameterStartValue trySetValue(IObjectPath objectPath, double value)
      {
         var parameterStartValue = _defaultStartValues[objectPath];
         if (parameterStartValue == null)
            throw new Exception($"Parameter not found : {objectPath}");

         parameterStartValue.StartValue = value;
         return parameterStartValue;
      }

      private void trySetFormula(IObjectPath parameterPath, string rate)
      {
         var formula = _formulaFactory.RateFor(_method, rate, _formulaCache);
         trySetFormula(parameterPath, formula);
      }

      private void trySetFormula(IObjectPath objectPath, IFormula formula)
      {
         var parameterStartValue = _defaultStartValues[objectPath];
         if (parameterStartValue != null)
            parameterStartValue.Formula = formula;
         else
            throw new Exception(string.Format("Parameter not found : {0}", objectPath));
      }

      private void setGlobalParameterValue(IndividualMolecule molecule, string expressionName, string containerName)
      {
         if (!molecule.HasContainerNamed(containerName)) return;
         trySetValue(moleculeGlobalPathFor(molecule.Name).AndAdd(CoreConstants.Parameters.NormParameterFor(expressionName)), molecule.GetRelativeExpressionNormParameterFor(containerName));
         trySetValue(moleculeGlobalPathFor(molecule.Name).AndAdd(expressionName), molecule.GetRelativeExpressionParameterFor(containerName));
      }

      private IFormula zeroFormula()
      {
         return _objectBaseFactory.Create<ConstantFormula>().WithValue(0);
      }

      private IObjectPath moleculeGlobalPathFor(string moleculeName)
      {
         return _objectPathFactory.CreateObjectPathFrom(moleculeName);
      }

      private IObjectPath relExpNormPathFor(IndividualMolecule molecule, IContainer expressionContainer)
      {
         return parameterPathFor(molecule, expressionContainer, CoreConstants.Parameters.REL_EXP_NORM);
      }

      private IObjectPath relExpOutPathFor(IndividualMolecule molecule, IContainer expressionContainer)
      {
         return parameterPathFor(molecule, expressionContainer, CoreConstants.Parameters.REL_EXP_OUT);
      }

      private IObjectPath relExpPathFor(IndividualMolecule molecule, IContainer expressionContainer)
      {
         return parameterPathFor(molecule, expressionContainer, CoreConstants.Parameters.REL_EXP);
      }

      private IObjectPath parameterPathFor(IndividualMolecule molecule, IContainer expressionContainer, string parameterName)
      {
         return proteinPathFor(molecule, expressionContainer).AndAdd(parameterName);
      }

      private IObjectPath proteinPathFor(IndividualMolecule molecule, IContainer expressionContainer)
      {
         return _objectPathFactory.CreateAbsoluteObjectPath(expressionContainer).AndAdd(molecule.Name);
      }
   }
}