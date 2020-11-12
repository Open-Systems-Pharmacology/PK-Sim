using System;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IIndividualMoleculeFactoryResolver
   {
      IIndividualMoleculeTask FactoryFor<TIndividualMolecule>();
      IIndividualMoleculeTask FactoryFor(Type individualMoleculeType);
      IIndividualMoleculeTask FactoryFor(IndividualMolecule individualMolecule);
   }

   public class IndividualMoleculeFactoryResolver : IIndividualMoleculeFactoryResolver
   {
      private readonly IRepository<IIndividualMoleculeTask> _enzymeExpressionFactoryRepository;

      public IndividualMoleculeFactoryResolver(IRepository<IIndividualMoleculeTask> enzymeExpressionFactoryRepository)
      {
         _enzymeExpressionFactoryRepository = enzymeExpressionFactoryRepository;
      }

      public IIndividualMoleculeTask FactoryFor(Type individualMoleculeType)
      {
         foreach (var expressionFactory in _enzymeExpressionFactoryRepository.All())
         {
            if (expressionFactory.IsSatisfiedBy(individualMoleculeType))
               return expressionFactory;
         }

         throw new IndividualProteinFactoryNotFoundException(individualMoleculeType);
      }

      public IIndividualMoleculeTask FactoryFor<TIndividualMolecule>() => FactoryFor(typeof(TIndividualMolecule));

      public IIndividualMoleculeTask FactoryFor(IndividualMolecule individualMolecule) => FactoryFor(individualMolecule.GetType());
   }
}