using System.Linq;
using OSPSuite.Utility;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatContainerToEventBuilderMapper : IMapper<FlatContainer, EventBuilder>
   {
      EventBuilder MapFrom(FlatContainer flatEventContainer, IFormulaCache formulaCache);
   }

   public class FlatContainerToEventBuilderMapper : IFlatContainerToEventBuilderMapper
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IFlatEventConditionRepository _flatEventConditionRepo;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IFlatEventChangedObjectRepository _flatEventChangedObjectRepo;
      private readonly IFlatEventChangedObjectToEventAssignmentBuilderMapper _eventAssignmentBuilderMapper;
      private readonly IObjectPathFactory _objectPathFactory;
      private readonly IDimensionRepository _dimensionRepository;
      private IFormulaCache _formulaCache;

      public FlatContainerToEventBuilderMapper(IObjectBaseFactory objectBaseFactory,
         IFlatEventConditionRepository flatEventConditionRepo,
         IFormulaFactory formulaFactory,
         IFlatEventChangedObjectRepository flatEventChangedObjectRepo,
         IFlatEventChangedObjectToEventAssignmentBuilderMapper eventAssignmentBuilderMapper,
         IObjectPathFactory objectPathFactory,
         IDimensionRepository dimensionRepository)
      {
         _objectBaseFactory = objectBaseFactory;
         _flatEventConditionRepo = flatEventConditionRepo;
         _formulaFactory = formulaFactory;
         _flatEventChangedObjectRepo = flatEventChangedObjectRepo;
         _eventAssignmentBuilderMapper = eventAssignmentBuilderMapper;
         _objectPathFactory = objectPathFactory;
         _dimensionRepository = dimensionRepository;
         _formulaCache = new FormulaCache();
      }

      public EventBuilder MapFrom(FlatContainer flatEventContainer)
      {
         var eventBuilder = _objectBaseFactory.Create<EventBuilder>();
         eventBuilder.Name = flatEventContainer.Name;

         var flatEventCondition = _flatEventConditionRepo.EventConditionFor(flatEventContainer.Id);

         eventBuilder.OneTime = flatEventCondition.IsOneTime;
         eventBuilder.Formula =
            _formulaFactory.RateFor(flatEventCondition, _formulaCache);

         //add time reference to event condition formula
         if (!eventBuilder.Formula.ContainsTimePath())
            eventBuilder.Formula.AddObjectPath(_objectPathFactory.CreateTimePath(_dimensionRepository.Time));

         // fill event assignments
         foreach (var flatEventAssignment in _flatEventChangedObjectRepo.ChangedObjectsFor(flatEventContainer.Id))
         {
            var eventAssignment = _eventAssignmentBuilderMapper.MapFrom(flatEventAssignment);
            eventAssignment.Name = $"Assignment_{eventBuilder.Assignments.Count() + 1}";

            eventBuilder.AddAssignment(eventAssignment);
         }

         return eventBuilder;
      }

      public EventBuilder MapFrom(FlatContainer flatEventContainer, IFormulaCache formulaCache)
      {
         _formulaCache = formulaCache;

         return MapFrom(flatEventContainer);
      }
   }
}