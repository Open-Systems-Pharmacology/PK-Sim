using System;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core.Model
{
   public interface IExpressionProfileFactory
   {
      ExpressionProfile Create<TMolecule>() where TMolecule : IndividualMolecule;
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

      public ExpressionProfile Create<TMolecule>() where TMolecule : IndividualMolecule
      {
         var expressionProfile = _objectBaseFactory.Create<ExpressionProfile>();
         expressionProfile.IsLoaded = true;
         updateSpecies(expressionProfile, _speciesRepository.DefaultSpecies, typeof(TMolecule));
         return expressionProfile;
      }

      public void UpdateSpecies(ExpressionProfile expressionProfile, Species species)
      {
         var currentMolecule = expressionProfile.Molecule;
         //should never happen as we are simply updating a
         if (currentMolecule == null)
            return;

         updateSpecies(expressionProfile, species, currentMolecule.GetType());
      }

      private void updateSpecies(ExpressionProfile expressionProfile, Species species, Type individualMoleculeType)
      {
         var moleculeFactory = _individualMoleculeFactoryResolver.FactoryFor(individualMoleculeType);
         expressionProfile.Individual = _individualFactory.CreateParameterLessIndividual(species);
         moleculeFactory.AddMoleculeTo(expressionProfile.Individual, ExpressionProfile.MOLECULE_NAME);
      }
   }
}