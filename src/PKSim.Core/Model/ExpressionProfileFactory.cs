using System;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Exceptions;
using PKSim.Assets;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using static PKSim.Core.CoreConstants;

namespace PKSim.Core.Model
{
   public interface IExpressionProfileFactory
   {
      ExpressionProfile Create<TMolecule>() where TMolecule : IndividualMolecule;
      ExpressionProfile Create<TMolecule>(Species species, string moleculeName) where TMolecule : IndividualMolecule;
      ExpressionProfile Create<TMolecule>(string moleculeName) where TMolecule : IndividualMolecule;
      ExpressionProfile Create(Type moleculeType);
      ExpressionProfile Create(Type moleculeType, Species species, string moleculeName);
      ExpressionProfile Create(QuantityType moleculeType, string speciesName, string moleculeName);
      void UpdateSpecies(ExpressionProfile expressionProfile, Species species);
   }

   public class ExpressionProfileFactory : IExpressionProfileFactory
   {
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;
      private readonly IPKSimObjectBaseFactory _objectBaseFactory;
      private readonly IIndividualFactory _individualFactory;

      public ExpressionProfileFactory(
         ISpeciesRepository speciesRepository,
         IIndividualMoleculeFactoryResolver individualMoleculeFactoryResolver,
         IPKSimObjectBaseFactory objectBaseFactory,
         IIndividualFactory individualFactory)
      {
         _speciesRepository = speciesRepository;
         _individualMoleculeFactoryResolver = individualMoleculeFactoryResolver;
         _objectBaseFactory = objectBaseFactory;
         _individualFactory = individualFactory;
      }

      public ExpressionProfile Create<TMolecule>() where TMolecule : IndividualMolecule => Create(typeof(TMolecule));

      public ExpressionProfile Create<TMolecule>(Species species, string moleculeName) where TMolecule : IndividualMolecule => Create(typeof(TMolecule), species, moleculeName);

      public ExpressionProfile Create<TMolecule>(string moleculeName) where TMolecule : IndividualMolecule => Create<TMolecule>(_speciesRepository.DefaultSpecies, moleculeName);

      public ExpressionProfile Create(Type moleculeType) => Create(moleculeType, _speciesRepository.DefaultSpecies, DEFAULT_EXPRESSION_PROFILE_MOLECULE_NAME);

      public ExpressionProfile Create(Type moleculeType, Species species, string moleculeName)
      {
         var expressionProfile = _objectBaseFactory.Create<ExpressionProfile>();
         expressionProfile.IsLoaded = true;
         updateSpecies(expressionProfile, species, moleculeType, moleculeName);
         return expressionProfile;
      }

      public ExpressionProfile Create(QuantityType moleculeType, string speciesName, string moleculeName)
      {
         var species = _speciesRepository.FindByName(speciesName);
         switch (moleculeType)
         {
            case QuantityType.Enzyme:
               return Create<IndividualEnzyme>(species, moleculeName);
            case QuantityType.OtherProtein:
               return Create<IndividualOtherProtein>(species, moleculeName);
            case QuantityType.Transporter:
               return Create<IndividualTransporter>(species, moleculeName);
            default:
               throw new OSPSuiteException(PKSimConstants.Error.MoleculeTypeNotSupported(moleculeType.ToString()));
         }
      }

      public void UpdateSpecies(ExpressionProfile expressionProfile, Species species)
      {
         var currentMolecule = expressionProfile.Molecule;
         //should never happen as we are simply updating a
         if (currentMolecule == null)
            return;

         updateSpecies(expressionProfile, species, currentMolecule.GetType(), expressionProfile.MoleculeName);
      }

      private void updateSpecies(ExpressionProfile expressionProfile, Species species, Type individualMoleculeType, string moleculeName)
      {
         var moleculeFactory = _individualMoleculeFactoryResolver.FactoryFor(individualMoleculeType);
         expressionProfile.Individual = _individualFactory.CreateParameterLessIndividual(species);
         moleculeFactory.AddMoleculeTo(expressionProfile.Individual, moleculeName);
      }
   }
}