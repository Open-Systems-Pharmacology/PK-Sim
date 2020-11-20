using System;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IIndividualMoleculeFactoryResolver
   {
      IIndividualMoleculeFactory FactoryFor<TIndividualMolecule>();
      IIndividualMoleculeFactory FactoryFor(Type individualMoleculeType);
      IIndividualMoleculeFactory FactoryFor(IndividualMolecule individualMolecule);
   }

   public class IndividualMoleculeFactoryResolver : IIndividualMoleculeFactoryResolver
   {
      private readonly IRepository<IIndividualMoleculeFactory> _enzymeExpressionFactoryRepository;

      public IndividualMoleculeFactoryResolver(IRepository<IIndividualMoleculeFactory> enzymeExpressionFactoryRepository)
      {
         _enzymeExpressionFactoryRepository = enzymeExpressionFactoryRepository;
      }

      public IIndividualMoleculeFactory FactoryFor(Type individualMoleculeType)
      {
         foreach (var expressionFactory in _enzymeExpressionFactoryRepository.All())
         {
            if (expressionFactory.IsSatisfiedBy(individualMoleculeType))
               return expressionFactory;
         }

         throw new IndividualProteinFactoryNotFoundException(individualMoleculeType);
      }

      public IIndividualMoleculeFactory FactoryFor<TIndividualMolecule>() => FactoryFor(typeof(TIndividualMolecule));

      public IIndividualMoleculeFactory FactoryFor(IndividualMolecule individualMolecule) => FactoryFor(individualMolecule.GetType());
   }
}