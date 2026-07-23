using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v13;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v13
{
   /// <summary>
   ///    No saved project fixture in the repository carries a meal event, so the event branch of the converter is
   ///    exercised here against the real database template instead. A <see cref="PKSimEvent" /> is rebuilt from a clone of
   ///    that template and then degraded to look like a project saved before v13, so the conversion has real work to do.
   /// </summary>
   public abstract class concern_for_Converter12To13_events : ContextForIntegration<Converter12To13>
   {
      protected EventGroupBuilder _mealTemplate;
      protected PKSimEvent _oldEvent;
      protected string _resetEventName;
      protected ICloner _cloner;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _cloner = OSPSuite.Utility.Container.IoC.Resolve<ICloner>();
         var eventGroupRepository = OSPSuite.Utility.Container.IoC.Resolve<IEventGroupRepository>();

         //Any meal that gained the new reset events works. Picking it from the repository keeps the test independent of a
         //specific meal name
         _mealTemplate = eventGroupRepository.All()
            .First(x => x.GetAllChildren<IContainer>(isResetEvent).Any());

         _resetEventName = _mealTemplate.GetAllChildren<IContainer>(isResetEvent).First().Name;

         _oldEvent = oldEventFrom(_mealTemplate);
      }

      protected override void Because()
      {
         sut.Convert(_oldEvent, ProjectVersions.V12);
      }

      /// <summary>
      ///    Rebuilds a <see cref="PKSimEvent" /> from a clone of the template, then strips one reset event and adds back the
      ///    obsolete stop event so that the state matches a meal saved before the new oral absorption model.
      /// </summary>
      private PKSimEvent oldEventFrom(EventGroupBuilder template)
      {
         var oldEvent = new PKSimEvent {TemplateName = template.Name}.WithName(template.Name);
         template.Children.Each(x => oldEvent.Add(_cloner.Clone(x)));

         var resetEventToDrop = oldEvent.GetAllChildren<IContainer>(isResetEvent).First(x => x.IsNamed(_resetEventName));
         resetEventToDrop.ParentContainer.RemoveChild(resetEventToDrop);

         var subContainer = oldEvent.GetAllChildren<IContainer>()
            .First(x => x.IsNamed(CoreConstants.ContainerName.EventGroupMainSubContainer));
         subContainer.Add(new Container().WithName(ConverterConstants.Containers.MEAL_STOP_EVENT));

         return oldEvent;
      }

      private static bool isResetEvent(IContainer container) => container.Name.StartsWith("Reset ");
   }

   public class When_converting_a_meal_event_saved_before_the_new_oral_absorption_model : concern_for_Converter12To13_events
   {
      [Observation]
      public void should_have_added_back_the_reset_event_that_was_missing()
      {
         _oldEvent.GetAllChildren<IContainer>(x => x.IsNamed(_resetEventName)).Any().ShouldBeTrue();
      }

      [Observation]
      public void should_have_removed_the_obsolete_meal_stop_event()
      {
         _oldEvent.GetAllChildren<IContainer>(x => x.IsNamed(ConverterConstants.Containers.MEAL_STOP_EVENT)).ShouldBeEmpty();
      }
   }
}
