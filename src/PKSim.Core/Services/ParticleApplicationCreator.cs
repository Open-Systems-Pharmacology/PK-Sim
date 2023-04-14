using System;
using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Core.Maths.Statistics;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IParticleApplicationCreator
   {
      void CreateParticleIn(ApplicationBuilder applicationBuilder, IEnumerable<IParameter> formulationParameters, IFormulaCache formulaCache);
   }

   public class ParticleApplicationCreator : IParticleApplicationCreator
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IObjectPathFactory _objectPathFactory;
      private readonly IDimensionRepository _dimensionRepository;
      private IEnumerable<IParameter> _formulationParameters;

      public ParticleApplicationCreator(IObjectBaseFactory objectBaseFactory, IObjectPathFactory objectPathFactory, IDimensionRepository dimensionRepository)
      {
         _objectBaseFactory = objectBaseFactory;
         _objectPathFactory = objectPathFactory;
         _dimensionRepository = dimensionRepository;
      }

      public void CreateParticleIn(ApplicationBuilder applicationBuilder, IEnumerable<IParameter> formulationParameters, IFormulaCache formulaCache)
      {
         try
         {
            _formulationParameters = formulationParameters;
            addDrugStartFormulaForParticles(applicationBuilder, formulaCache);
         }
         finally
         {
            _formulationParameters = null;
         }
      }

      private void addDrugStartFormulaForParticles(ApplicationBuilder applicationBuilder, IFormulaCache formulaCache)
      {
         // get number of bins
         int numberOfBins;

         bool isPolyDisperse = formulationParameterValue(Constants.Parameters.PARTICLE_DISPERSE_SYSTEM) == CoreConstants.Parameters.POLYDISPERSE;

         if (isPolyDisperse)
            numberOfBins = (int) formulationParameterValue(Constants.Parameters.NUMBER_OF_BINS);
         else
            numberOfBins = 1;

         if (numberOfBins < 1 || numberOfBins > CoreConstants.Parameters.MAX_NUMBER_OF_BINS)
            throw new PKSimException(PKSimConstants.Error.InvalidNumberOfBins);

         // get (start)radius and factor for particles number for each bin
         var binRadius = new double[numberOfBins];
         var binNumberOfParticlesFactor = new double[numberOfBins];

         fillBinRadiusAndNumberOfParticlesFactor(numberOfBins, binRadius, binNumberOfParticlesFactor);

         // delete not required bin containers
         for (int i = numberOfBins + 1; i <= CoreConstants.Parameters.MAX_NUMBER_OF_BINS; i++)
         {
            var binName = binContainerName(i);
            applicationBuilder.RemoveChild(applicationBuilder.GetSingleChildByName(binName));
         }

         // setup particle bins (start-radius, number of particles, drugmass)
         for (int i = 1; i <= numberOfBins; i++)
         {
            setupParticleBin(applicationBuilder, i, binRadius[i - 1], binNumberOfParticlesFactor[i - 1], formulaCache);
         }

         //// add insoluble molecule for particle bin
         //var appMoleculeBuilder = _objectBaseFactory.Create<IApplicationMoleculeBuilder>().WithName(insolubleMoleculeName(0)); //TODO replace insolubleMoleculeName(0) with const string
         //appMoleculeBuilder.RelativeContainerPath = _objectPathFactory.CreateObjectPathFrom(CoreConstants.OrganName.InsolubleDrug);
         //appMoleculeBuilder.Formula = insolubleDrugStartFormula(formulaCache);
         //applicationBuilder.AddMolecule(appMoleculeBuilder);

         // add "ParticleApplication"-Tag required for Fraction Solid/Dissolved/Insoluble observer
         applicationBuilder.AddTag(CoreConstants.Tags.ParticlesApplicationWithNBins(numberOfBins));
      }

      private void fillBinRadiusAndNumberOfParticlesFactor(int numberOfBins, double[] binRadius, double[] binNumberOfParticlesFactor)
      {
         const double factor = 3.0 / 4.0 / Math.PI; // = 1 / (4/3*Pi)

         double meanRadius = formulationParameterValue(CoreConstants.Parameters.PARTICLE_RADIUS_MEAN);

         // monodisperse case (no real distribution)
         if (numberOfBins == 1)
         {
            binRadius[0] = meanRadius;
            binNumberOfParticlesFactor[0] = factor / Math.Pow(meanRadius, 3.0);
            return;
         }

         // polydisperse case
         var particlesDistributionType =
            (int) formulationParameterValue(Constants.Parameters.PARTICLE_SIZE_DISTRIBUTION);

         var minRadius = formulationParameterValue(CoreConstants.Parameters.PARTICLE_RADIUS_MIN);
         var maxRadius = formulationParameterValue(CoreConstants.Parameters.PARTICLE_RADIUS_MAX);

         // bin radii
         var binRadiusDistributionValue = new double[numberOfBins];

         IDistribution distribution;
         if (particlesDistributionType == CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION_NORMAL)
         {
            double mean = formulationParameterValue(CoreConstants.Parameters.PARTICLE_RADIUS_MEAN);
            double deviation = formulationParameterValue(CoreConstants.Parameters.PARTICLE_RADIUS_STD_DEVIATION);

            distribution = new NormalDistribution(mean, deviation);

            // bins radii - normal distribution
            for (int binIndex = 0; binIndex < numberOfBins; binIndex++)
            {
               binRadius[binIndex] = minRadius + (binIndex + 0.5) * (maxRadius - minRadius) / numberOfBins;
            }
         }
         else if (particlesDistributionType == CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION_LOG_NORMAL)
         {
            double mean = Math.Log(formulationParameterValue(CoreConstants.Parameters.PARTICLE_LOG_DISTRIBUTION_MEAN));
            double deviation = formulationParameterValue(CoreConstants.Parameters.PARTICLE_LOG_VARIATION_COEFF);

            distribution = new LogNormalDistribution(mean, deviation);

            // bins radii - lognormal distribution
            double r = Math.Pow(maxRadius / minRadius, 1.0 / numberOfBins);

            for (int binIndex = 0; binIndex < numberOfBins; binIndex++)
            {
               binRadius[binIndex] = minRadius / 2.0 * (1.0 + r) * Math.Pow(r, binIndex);
            }
         }
         else
            throw new PKSimException(PKSimConstants.Error.InvalidParticleSizeDistribution);

         // values of distribution function for the bin radii
         for (int binIndex = 0; binIndex < numberOfBins; binIndex++)
         {
            binRadiusDistributionValue[binIndex] = distribution.ProbabilityDensityFor(binRadius[binIndex]);
         }

         // finally, fill scale factor for the number of particles. The formula
         //     for NumberOfParticles is Drugmass/Drugdensity * ScaleFactor

         double factor2 = 0.0;
         for (int binIndex = 0; binIndex < numberOfBins; binIndex++)
         {
            factor2 += binRadiusDistributionValue[binIndex] * Math.Pow(binRadius[binIndex], 3);
         }

         if (factor2 > 0.0)
            factor2 = 1.0 / factor2;
         //factor2 = 1/Sum{f(r_i)*r_i^3}, where r_i is the radius of i-th particle bin
         // and f(r_i) is probability density for this radius

         for (int binIndex = 0; binIndex < numberOfBins; binIndex++)
         {
            binNumberOfParticlesFactor[binIndex] = factor * factor2 * binRadiusDistributionValue[binIndex];
            //NumberOfPrticlesFactor_i = f(r_i) / (4/3*Pi*Sum{f(r_i)*r_i^3})
         }
      }

      private double formulationParameterValue(string parameterName)
      {
         return _formulationParameters.FindByName(parameterName).Value;
      }

      private void setupParticleBin(ApplicationBuilder applicBuilder, int binIndex, double binRadius, double binNumberOfParticlesFactor, IFormulaCache formulaCache)
      {
         var binName = binContainerName(binIndex);
         var binContainer = applicBuilder.GetSingleChildByName<IContainer>(binName);

         var numberOfParticlesFactor = binContainer.Parameter(CoreConstants.Parameters.NUMBER_OF_PARTICLES_FACTOR);
         numberOfParticlesFactor.Value = binNumberOfParticlesFactor;

         var particleRadius = binContainer.Parameter(CoreConstants.Parameters.START_PARTICLE_RADIUS);
         particleRadius.Value = binRadius;

         // add application molecule in particle bin and set its formula
         var appMoleculeBuilder = _objectBaseFactory.Create<ApplicationMoleculeBuilder>().WithName(solubleMoleculeName(binIndex));
         appMoleculeBuilder.RelativeContainerPath = _objectPathFactory.CreateObjectPathFrom(binName);
         appMoleculeBuilder.Formula = particleDrugMassFormula(formulaCache);
         applicBuilder.AddMolecule(appMoleculeBuilder);

         // add insoluble molecule for particle bin
         appMoleculeBuilder = _objectBaseFactory.Create<ApplicationMoleculeBuilder>().WithName(insolubleMoleculeName(binIndex));
         appMoleculeBuilder.RelativeContainerPath = _objectPathFactory.CreateObjectPathFrom(binName, CoreConstants.ContainerName.InsolubleDrug);
         appMoleculeBuilder.Formula = insolubleDrugStartFormula(formulaCache);
         applicBuilder.AddMolecule(appMoleculeBuilder);
      }

      private IFormula insolubleDrugStartFormula(IFormulaCache formulaCache)
      {
         if (formulaCache.ExistsByName(CoreConstants.Formula.InsolubleDrugStartFormula))
            return formulaCache.FindByName(CoreConstants.Formula.InsolubleDrugStartFormula);

         var moleculeStartFormula = _objectBaseFactory.Create<ExplicitFormula>()
            .WithName(CoreConstants.Formula.InsolubleDrugStartFormula)
            .WithDimension(_dimensionRepository.Amount)
            .WithFormulaString("0");

         formulaCache.Add(moleculeStartFormula);

         return moleculeStartFormula;
      }

      private IFormula particleDrugMassFormula(IFormulaCache formulaCache)
      {
         if (formulaCache.ExistsByName(CoreConstants.Parameters.PARTICLE_BIN_DRUG_MASS))
            return formulaCache.FindByName(CoreConstants.Parameters.PARTICLE_BIN_DRUG_MASS);


         var pathToDrugMass = _objectPathFactory.CreateFormulaUsablePathFrom(ObjectPath.PARENT_CONTAINER, CoreConstants.Parameters.PARTICLE_BIN_DRUG_MASS)
            .WithAlias("ParticleBinDrugMass")
            .WithDimension(_dimensionRepository.Amount);

         var startFormula = _objectBaseFactory.Create<ExplicitFormula>()
            .WithFormulaString(pathToDrugMass.Alias)
            .WithDimension(_dimensionRepository.Amount)
            .WithName(CoreConstants.Parameters.PARTICLE_BIN_DRUG_MASS);


         startFormula.AddObjectPath(pathToDrugMass);
         formulaCache.Add(startFormula);
         return startFormula;
      }

      private string binContainerName(int binIndex)
      {
         return $"{CoreConstants.ContainerName.ParticleBinPrefix}{binIndex}";
      }

      private string solubleMoleculeName(int binIndex)
      {
         return $"{binContainerName(binIndex)}_{"soluble"}";
      }

      private string insolubleMoleculeName(int binIndex)
      {
         return $"{binContainerName(binIndex)}_{"insoluble"}";
      }
   }
}