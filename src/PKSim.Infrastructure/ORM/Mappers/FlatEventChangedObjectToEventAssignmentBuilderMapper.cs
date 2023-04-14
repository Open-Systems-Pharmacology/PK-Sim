using OSPSuite.Utility;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatEventChangedObjectToEventAssignmentBuilderMapper : IMapper<FlatEventChangedObject, EventAssignmentBuilder>
   {
   }

   public class FlatEventChangedObjectToEventAssignmentBuilderMapper : IFlatEventChangedObjectToEventAssignmentBuilderMapper
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IFlatObjectPathToObjectPathMapper _objectPathMapper;
      private readonly IFormulaCache _formulaCache;

      public FlatEventChangedObjectToEventAssignmentBuilderMapper(IObjectBaseFactory objectBaseFactory, IFormulaFactory formulaFactory,
         IFlatObjectPathToObjectPathMapper objectPathMapper)
      {
         _objectBaseFactory = objectBaseFactory;
         _formulaFactory = formulaFactory;
         _objectPathMapper = objectPathMapper;
         _formulaCache = new FormulaCache();
      }

      public EventAssignmentBuilder MapFrom(FlatEventChangedObject flatEventChangedObject)
      {
         var eventAssignmentBuilder = _objectBaseFactory.Create<EventAssignmentBuilder>();
         eventAssignmentBuilder.ObjectPath = _objectPathMapper.MapFrom(flatEventChangedObject);
         eventAssignmentBuilder.UseAsValue = flatEventChangedObject.UseAsValue;
         eventAssignmentBuilder.Formula = _formulaFactory.RateFor(flatEventChangedObject, _formulaCache);
         eventAssignmentBuilder.Dimension = eventAssignmentBuilder.Formula.Dimension;
         return eventAssignmentBuilder;
      }
   }
}