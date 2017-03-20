using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Utility;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatContainerToFormulationMapper : IMapper<FlatContainer, Formulation>
   {
   }

   public class FlatContainerToFormulationMapper : IFlatContainerToFormulationMapper
   {
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IFlatContainerIdToFormulationMapper _containerIdToFormulationMapper;

      public FlatContainerToFormulationMapper(IParameterContainerTask parameterContainerTask,
         IFlatContainerIdToFormulationMapper containerIdToFormulationMapper)
      {
         _parameterContainerTask = parameterContainerTask;
         _containerIdToFormulationMapper = containerIdToFormulationMapper;
      }

      public Formulation MapFrom(FlatContainer flatContainer)
      {
         var formulation = _containerIdToFormulationMapper.MapFrom(flatContainer);
         _parameterContainerTask.AddFormulationParametersTo(formulation);
         formulation.IsLoaded = true;
         return formulation;
      }
   }
}