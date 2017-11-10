using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;

namespace PKSim.Core.Batch.Mapper
{
   public interface IBatchToCoreMapper
   {
      TCoreObject MapFrom<TCoreObject>(object batchBuilingBlock, params object[] parameters) where TCoreObject : class;
   }

   public class BatchToCoreMapper : IBatchToCoreMapper,
      IVisitor<Compound>,
      IVisitor<Individual>,
      IVisitor<ApplicationProtocol>,
      IVisitor<Formulation>,
      IVisitor<SimulationConfiguration>
   {
      private readonly IIndividualMapper _individualMapper;
      private readonly ICompoundMapper _compoundMapper;
      private readonly IApplicationProtocolMapper _protocolMapper;
      private readonly IFormulationMapper _formulationMapper;
      private readonly IModelPropertiesMapper _modelPropertiesMapper;
      private object _coreObject;
      private object[] _parameters;

      public BatchToCoreMapper(IIndividualMapper individualMapper, ICompoundMapper compoundMapper, IApplicationProtocolMapper protocolMapper, IFormulationMapper formulationMapper, IModelPropertiesMapper modelPropertiesMapper)
      {
         _individualMapper = individualMapper;
         _compoundMapper = compoundMapper;
         _protocolMapper = protocolMapper;
         _formulationMapper = formulationMapper;
         _modelPropertiesMapper = modelPropertiesMapper;
      }

      public TCoreObject MapFrom<TCoreObject>(object batchBuilingBlock, params object[] parameters) where TCoreObject : class
      {
         try
         {
            _parameters = parameters;
            this.Visit(batchBuilingBlock);
            return _coreObject.DowncastTo<TCoreObject>();
         }
         finally
         {
            _parameters = null;
            _coreObject = null;
         }
      }

      public void Visit(Compound compound)
      {
         _coreObject = _compoundMapper.MapFrom(compound);
      }

      public void Visit(Individual individual)
      {
         _coreObject = _individualMapper.MapFrom(individual);
      }

      public void Visit(ApplicationProtocol protocol)
      {
         _coreObject = _protocolMapper.MapFrom(protocol);
      }

      public void Visit(Formulation formulation)
      {
         _coreObject = _formulationMapper.MapFrom(formulation);
      }

      public void Visit(SimulationConfiguration simulationConfiguration)
      {
         _coreObject = _modelPropertiesMapper.MapFrom(simulationConfiguration, _parameters[0].DowncastTo<Model.Individual>());
      }
   }
}