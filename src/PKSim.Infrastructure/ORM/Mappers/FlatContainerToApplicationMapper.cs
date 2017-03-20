using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatContainerToApplicationMapper : IMapper<FlatContainer, IApplicationBuilder>
   {
   }

   public class FlatContainerToApplicationMapper : FlatContainerIdToContainerMapperBase<IApplicationBuilder>, IFlatContainerToApplicationMapper
   {
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IApplicationTransportRepository _applicTransportRepo;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IFlatContainerRepository _flatContainerRepo;
      private readonly IFlatContainerIdToContainerMapper _flatContainerIdMapper;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IFlatContainerToEventBuilderMapper _eventMapper;

      public FlatContainerToApplicationMapper(IParameterContainerTask parameterContainerTask,
                                              IApplicationTransportRepository applicTransportRepo,
                                              IEntityPathResolver entityPathResolver,
                                              IFlatContainerRepository flatContainerRepo,
                                              IFlatContainerIdToContainerMapper flatContainerIdMapper,
                                              IObjectBaseFactory objectBaseFactory,
                                              IFlatContainerToEventBuilderMapper eventMapper)
      {
         _parameterContainerTask = parameterContainerTask;
         _applicTransportRepo = applicTransportRepo;
         _entityPathResolver = entityPathResolver;
         _flatContainerRepo = flatContainerRepo;
         _flatContainerIdMapper = flatContainerIdMapper;
         _objectBaseFactory = objectBaseFactory;
         _eventMapper = eventMapper;
      }

      public IApplicationBuilder MapFrom(FlatContainer flatApplicationContainer)
      {
         var applicBuilder = MapCommonPropertiesFrom(flatApplicationContainer);

         //Set molecule name to "DRUG".
         //Must be replaced during simulation building process
         applicBuilder.MoleculeName = CoreConstants.Molecule.Drug;

         //Add application processes (e.g. infusion)
         addApplicationProcessesTo(applicBuilder);

         //create parent formulation container of the application
         //(in order to resolve paths of application subobjects correctly)
         createParentFormulation(applicBuilder, flatApplicationContainer.ParentId);

         //Add substructure (subcontainers with parameters and events)
         addApplicationStructureTo(applicBuilder);

         //add application (=event group) source criteria
         //Each application will be settled under ApplicationSet-Container of
         //the simulation
         addApplicationSourceCriteria(applicBuilder);

         return applicBuilder;
      }

      private static void addApplicationSourceCriteria(IApplicationBuilder applicBuilder)
      {
         applicBuilder.SourceCriteria.Add(new MatchTagCondition(Constants.APPLICATIONS));
      }

      private void addEvents(IContainer container)
      {
         var flatEventSubContainers = flatSubContainersFor(container,
                                                           containerType =>
                                                           containerType == CoreConstants.ContainerType.Event);

         foreach (var flatEventContainer in flatEventSubContainers)
         {
            container.Add(_eventMapper.MapFrom(flatEventContainer));
         }
      }

      private void createParentFormulation(IApplicationBuilder applicBuilder, int? formulationContainerId)
      {
         var flatFormulation = _flatContainerRepo.ContainerFrom(formulationContainerId);
         var formulation = _objectBaseFactory.Create<IContainer>().WithName(flatFormulation.Name);
         formulation.Add(applicBuilder);
      }

      private void addApplicationStructureTo(IContainer container)
      {
         _parameterContainerTask.AddApplicationParametersTo(container);

         addEvents(container);

         foreach (var subContainer in subContainersFor(container))
         {
            container.Add(subContainer);
            addApplicationStructureTo(subContainer);
         }
      }

      private IEnumerable<IContainer> subContainersFor(IContainer container)
      {
         var flatSubContainers = flatSubContainersFor(container,
                                                      containerType =>
                                                      containerType != CoreConstants.ContainerType.Event &&
                                                      containerType != CoreConstants.ContainerType.Process);

         return flatSubContainers.Select(
            flatSpeciesContainer => _flatContainerIdMapper.MapFrom(flatSpeciesContainer));
      }

      private IEnumerable<FlatContainer> flatSubContainersFor(IContainer container, Func<string, bool> containerTypeCondition)
      {
         var pathToParentContainer = _entityPathResolver.PathFor(container);

         //find parent container in repo
         var flatParentContainer = _flatContainerRepo.ContainerFrom(pathToParentContainer);

         return from flatContainer in _flatContainerRepo.All()
                let parentContainer = _flatContainerRepo.ParentContainerFrom(flatContainer.Id)
                where parentContainer != null
                where parentContainer.Id == flatParentContainer.Id
                where containerTypeCondition(flatContainer.Type)
                select flatContainer;
      }

      /// <summary>
      ///   Add application processes to the application builder
      /// </summary>
      private void addApplicationProcessesTo(IApplicationBuilder applicBuilder)
      {
         var applicPocesses = _applicTransportRepo.TransportsFor(applicBuilder.Name);
         applicPocesses.Each(applicBuilder.AddTransport);
      }
   }
}