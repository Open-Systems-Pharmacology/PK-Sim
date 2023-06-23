using System;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Infrastructure.ORM.Mappers
{

   public interface IFlatObserverToObserverBuilderMapper : IMapper<FlatObserver, IPKSimObserverBuilder>
   {
   }

   public class FlatObserverToObserverBuilderMapper : IFlatObserverToObserverBuilderMapper
   {
      private readonly IObjectBaseFactory _entityBaseFactory;
      private readonly IFlatObserverDescriptorConditionRepository _observerDescriptorRepo;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IDimensionRepository _dimensionRepository;

      public FlatObserverToObserverBuilderMapper(IObjectBaseFactory entityBaseFactory,
                                                 IFlatObserverDescriptorConditionRepository observerDescriptorRepo,
                                                 IFormulaFactory formulaFactory, IDimensionRepository dimensionRepository)
      {
         _entityBaseFactory = entityBaseFactory;
         _observerDescriptorRepo = observerDescriptorRepo;
         _formulaFactory = formulaFactory;
         _dimensionRepository = dimensionRepository;
      }

      public IPKSimObserverBuilder MapFrom(FlatObserver flatObserver)
      {
         var observerBuilder = createObserverBuilderByType(flatObserver);
         var pksimObserverBuilder = new PKSimObserverBuilder(observerBuilder);

         observerBuilder.Name = flatObserver.Name;
         observerBuilder.ForAll = flatObserver.ForAllMolecules;

         pksimObserverBuilder.CalculationMethod = flatObserver.CalculationMethod;
         pksimObserverBuilder.Rate = flatObserver.Rate;

         observerBuilder.Formula = _formulaFactory.RateFor(pksimObserverBuilder, new FormulaCache());

         observerBuilder.Dimension = _dimensionRepository.DimensionByName(flatObserver.Dimension);

         return pksimObserverBuilder;
      }

      private ObserverBuilder createObserverBuilderByType(FlatObserver flatObserver)
      {
         if (flatObserver.BuilderType == ObserverBuilderType.Amount)
         {
            var amountObserverBuilder = _entityBaseFactory.Create<AmountObserverBuilder>();
            
            addDescriptorConditions(amountObserverBuilder.ContainerCriteria, 
                                    ObserverTagType.PARENT, flatObserver.Name);

            return amountObserverBuilder;
         }

         if (flatObserver.BuilderType == ObserverBuilderType.Container)
         {
            var containerObserverBuilder = _entityBaseFactory.Create<ContainerObserverBuilder>();

            addDescriptorConditions(containerObserverBuilder.ContainerCriteria,
                                    ObserverTagType.PARENT, flatObserver.Name);

            return containerObserverBuilder;
         }

       

         throw new ArgumentException(PKSimConstants.Error.UnknownObserverBuilderType);
      }

      private void addDescriptorConditions(DescriptorCriteria descriptorCriteria,
                                           ObserverTagType tagType,
                                           string observerName)
      {
         var conditions = from pd in _observerDescriptorRepo.All()
                          where pd.TagType == tagType
                          where pd.Observer == observerName
                          select pd;

         foreach (var condition in conditions)
         {
            if (condition.ShouldHave)
               descriptorCriteria.Add(new MatchTagCondition(condition.Tag));
            else
               descriptorCriteria.Add(new NotMatchTagCondition(condition.Tag));
         }
      }
   }
}
