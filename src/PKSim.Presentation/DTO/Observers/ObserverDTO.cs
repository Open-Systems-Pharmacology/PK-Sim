﻿using System.Collections.Generic;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.DTO.Observers
{
   public class ObserverDTO : ValidatableDTO<IObserverBuilder>
   {
      public IObserverBuilder Observer { get; }

      public ObserverDTO(IObserverBuilder observer) : base(observer)
      {
         Observer = observer;
      }

      public IReadOnlyList<string> Details
      {
         get
         {
            var details = new List<string>();
            details.Add(InBold("Name"));
            details.Add(Observer.Name);

            details.Add(InBold("Formula"));
            details.Add(Observer.Formula.ToString());

            details.Add(InBold("References"));
            Observer.Formula.ObjectPaths.Each(x =>
            {
               details.Add($"{x.Alias}:  {x.PathAsString}");
            });

            details.Add(InBold("Container Criteria"));
            details.Add(Observer.ContainerCriteria.ToString());


            return details;
         }
      }

      public static string InBold(string stringToFormat)
      {
         return InHtml(stringToFormat, "b");
      }

      public static string InHtml(string stringToFormat, string marker)
      {
         return $"<{marker}>{stringToFormat}</{marker}>";
      }
   }
}