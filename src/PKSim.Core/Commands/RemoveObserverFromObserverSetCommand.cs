﻿using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RemoveObserverFromObserverSetCommand : RemoveEntityFromContainerCommand<ObserverBuilder, ObserverSet, RemoveObserverFromObserverSetEvent>
   {
      public RemoveObserverFromObserverSetCommand(ObserverBuilder observer, ObserverSet observerSet, IExecutionContext context)
         : base(observer, observerSet, context)
      {
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new AddObserverToObserverSetCommand(_entityToRemove, _parentContainer, context).AsInverseFor(this);
      }
   }
}