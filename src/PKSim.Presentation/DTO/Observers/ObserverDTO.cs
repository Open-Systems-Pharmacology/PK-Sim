using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;

namespace PKSim.Presentation.DTO.Observers
{
   public class ObserverDTO : ValidatableDTO<ObserverBuilder>
   {
      public ObserverBuilder Observer { get; }

      public ObserverDTO(ObserverBuilder observer) : base(observer)
      {
         Observer = observer;
      }

      public IReadOnlyList<string> Details
      {
         get
         {
            var details = new List<string>();
            details.Add(AddNewValueIs(Captions.Name, Observer.Name));
            details.Add(AddNewValueIs(PKSimConstants.UI.ObserverType, Observer.IsAnImplementationOf<ContainerObserverBuilder>() ? ObjectTypes.ContainerObserverBuilder : ObjectTypes.AmountObserverBuilder));

            details.Add(AddNewValueIs(Captions.Dimension, Observer.Dimension.DisplayName));

            details.Add(AddNewValueIs(PKSimConstants.UI.ContainerCriteria, Observer.ContainerCriteria.ToString()));

            var hasExcludedMolecules = Observer.MoleculeList.MoleculeNamesToExclude.Any();
            addIf(Observer.ForAll && !hasExcludedMolecules,
               () => details.Add(AddNewValueIs(PKSimConstants.UI.ForAll, PKSimConstants.UI.Yes)));

            addIf(!Observer.ForAll && Observer.MoleculeList.MoleculeNames.Any(),
               () => details.Add(AddNewValueIs(PKSimConstants.UI.MoleculeIncludeList, Observer.MoleculeList.MoleculeNames.ToString(", "))));

            addIf(Observer.ForAll && hasExcludedMolecules,
               () => details.Add(AddNewValueIs(PKSimConstants.UI.MoleculeExcludeList, Observer.MoleculeList.MoleculeNamesToExclude.ToString(", "))));

            details.Add(AddNewValueIs(PKSimConstants.Information.Formula, Observer.Formula.ToString()));

            details.Add(InBold(PKSimConstants.Information.ObjectReferences));
            Observer.Formula.ObjectPaths.Each(x => { details.Add($"{x.Alias}:  {x.PathAsString}"); });

            return details;
         }
      }

      private void addIf(bool criteria, Action action)
      {
         if (!criteria) return;
         action();
      }

      public static string InBold(string stringToFormat)
      {
         return InHtml(stringToFormat, "b");
      }

      public static string InHtml(string stringToFormat, string marker)
      {
         return $"<{marker}>{stringToFormat}</{marker}>";
      }

      public static string AddNewValueIs(string caption, string value)
      {
         return AddNewLine($"{InBold(caption)}: {value}");
      }

      public static string AddNewLine(string stringToFormat)
      {
         return $"{stringToFormat}{NewLine}";
      }

      public static string StartWithNewLine(string stringToFormat)
      {
         return $"{NewLine}{stringToFormat}";
      }

      public static string NewLine = "<br>";
   }
}