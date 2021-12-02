using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatDiseaseStateToDiseaseStateMapper : IMapper<FlatDiseaseState, DiseaseState>
   {
   }

   public class FlatDiseaseStateToDiseaseStateMapper : IFlatDiseaseStateToDiseaseStateMapper
   {
      private readonly IParameterContainerTask _parameterContainerTask;

      public FlatDiseaseStateToDiseaseStateMapper(IParameterContainerTask parameterContainerTask)
      {
         _parameterContainerTask = parameterContainerTask;
      }

      public DiseaseState MapFrom(FlatDiseaseState flatDiseaseState)
      {
         var diseaseState = new DiseaseState
         {
            Id = flatDiseaseState.Id,
            Name = flatDiseaseState.Id,
            DisplayName = flatDiseaseState.DisplayName,
            Description = flatDiseaseState.Description,
            Implementation = flatDiseaseState.Implementation,
         };

         _parameterContainerTask.AddDiseaseStateParametersTo(diseaseState);

         return diseaseState;
      }
   }
}