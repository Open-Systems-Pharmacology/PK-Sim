using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class Organism : Container
   {
      public Organism()
      {
         Name = Constants.ORGANISM;
         Mode = ContainerMode.Logical;
         ContainerType = ContainerType.Organism;
      }

      /// <summary>
      ///    Returns the organ with the given name if defined, or null otherwise
      /// </summary>
      public Organ Organ(string organName)
      {
         return this.GetSingleChildByName(organName) as Organ;
      }

      /// <summary>        
      ///    Returns collection of organs whose type match <c>organType</c>
      /// </summary>
      public IEnumerable<Organ> OrgansByType(OrganType organType)
      {
         return GetChildren<Organ>(organ => (organ.OrganType & organType) != 0);
      }

      /// <summary>
      ///    Returns collection of organs whose name is one of <c>names</c>
      /// </summary>
      public IEnumerable<Organ> OrgansByName(params string[] names)
      {
         return GetChildren<Organ>(organ => organ.NameIsOneOf(names));
      }

      /// <summary>
      ///    Returns all non-GI tissue containers defined in the organism
      /// </summary>
      public IEnumerable<IContainer> NonGITissueContainers
      {
         get
         {
            foreach (var organ in OrgansByType(OrganType.TissueOrgansNotInGiTract))
            {
               if (organ.IsNamed(CoreConstants.Organ.Liver))
               {
                  foreach (var liverZone in CoreConstants.Compartment.LiverZones)
                  {
                     yield return organ.Compartment(liverZone);
                  }
               }
               else
                  yield return organ;
            }
         }
      }

      /// <summary>
      ///    Returns all GI tissue containers defined in the organism
      /// </summary>
      public IEnumerable<IContainer> GITissueContainers => OrgansByType(OrganType.GiTractOrgans);

      /// <summary>
      ///    Returns all tissue containers (Non-GI and GI) defined in the organism
      /// </summary>
      public IEnumerable<IContainer> TissueContainers => NonGITissueContainers.Union(GITissueContainers);
   }
}