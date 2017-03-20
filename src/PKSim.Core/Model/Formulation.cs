using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class Formulation : PKSimBuildingBlock
   {
      private readonly IList<string> _allRoutes;

      /// <summary>
      ///    Type of formulation (identifier)
      /// </summary>
      public virtual string FormulationType { get; set; }

      public Formulation() : base(PKSimBuildingBlockType.Formulation)
      {
         _allRoutes = new List<string>();
         ContainerType = ContainerType.Formulation;
      }

      /// <summary>
      ///    Application route (iv, oral, dermal, ...)
      ///    formulation can be used for
      /// </summary>
      public virtual IEnumerable<string> Routes => _allRoutes;

      /// <summary>
      ///    returns true if the formulation is defining a table formulation
      /// </summary>
      public virtual bool IsTable => string.Equals(FormulationType, CoreConstants.Formulation.Table);

      /// <summary>
      ///    returns true if the formulation is defining a particle dissolution formulation
      /// </summary>
      public virtual bool IsParticleDissolution => string.Equals(FormulationType, CoreConstants.Formulation.Particles);

      /// <summary>
      ///    returns true if the formulation is defining a dissolved formulation
      /// </summary>
      public bool IsDissolved => string.Equals(FormulationType, CoreConstants.Formulation.Dissolved);

      /// <summary>
      ///    add a route to the list of routes where the formula can be used
      /// </summary>
      /// <param name="route"></param>
      public virtual void AddRoute(string route)
      {
         if (HasRoute(route)) return;
         _allRoutes.Add(route);
      }

      /// <summary>
      ///    returns true if the formulation can be used by the given route otherwise false
      /// </summary>
      public virtual bool HasRoute(string route)
      {
         return _allRoutes.Contains(route);
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var formulation = sourceObject as Formulation;
         if (formulation == null) return;
         FormulationType = formulation.FormulationType;
         formulation.Routes.Each(AddRoute);
      }
   }
}