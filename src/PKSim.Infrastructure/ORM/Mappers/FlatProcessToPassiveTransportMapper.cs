using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatProcessToPassiveTransportMapper : IMapper<FlatProcess, PKSimTransport>
   {
   }

   public class FlatProcessToPassiveTransportMapper : IFlatProcessToPassiveTransportMapper
   {
      private readonly IObjectBaseFactory _entityBaseFactory;
      private readonly IFlatProcessDescriptorConditionRepository _processDescriptorRepo;
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IDimensionRepository _dimensionRepository;

      public FlatProcessToPassiveTransportMapper(IObjectBaseFactory entityBaseFactory,
                                                 IFlatProcessDescriptorConditionRepository processDescriptorRepo,
                                                 IParameterContainerTask parameterContainerTask,
                                                 IFormulaFactory formulaFactory,
                                                 IDimensionRepository dimensionRepository)
      {
         _entityBaseFactory = entityBaseFactory;
         _processDescriptorRepo = processDescriptorRepo;
         _parameterContainerTask = parameterContainerTask;
         _formulaFactory = formulaFactory;
         _dimensionRepository = dimensionRepository;
      }

      public PKSimTransport MapFrom(FlatProcess flatProcess)
      {
         var transport = _entityBaseFactory.Create<PKSimTransport>()
            .WithName(flatProcess.Name)
            .WithDimension(_dimensionRepository.AmountPerTime);

         _parameterContainerTask.AddProcessBuilderParametersTo(transport);
         addDescriptorConditions(transport.SourceCriteria, ProcessTagType.Source, transport.Name);
         addDescriptorConditions(transport.TargetCriteria, ProcessTagType.Target, transport.Name);
         transport.CalculationMethod = flatProcess.CalculationMethod;
         transport.Rate = flatProcess.Rate;
         transport.Formula = _formulaFactory.RateFor(transport, new FormulaCache());
         transport.CreateProcessRateParameter = flatProcess.CreateProcessRateParameter;

         return transport;
      }

      private void addDescriptorConditions(DescriptorCriteria descriptorCriteria,
                                           ProcessTagType tagType,
                                           string transportName)
      {
         var conditions = from pd in _processDescriptorRepo.All()
                          where pd.TagType == tagType
                          where pd.Process == transportName
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