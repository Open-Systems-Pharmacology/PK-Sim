using System.Collections.Generic;
using System.Linq;
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
      public virtual bool IsTable => string.Equals(FormulationType, CoreConstants.Formulation.TABLE);

      /// <summary>
      ///    returns true if the formulation is defining a particle dissolution formulation
      /// </summary>
      public virtual bool IsParticleDissolution => string.Equals(FormulationType, CoreConstants.Formulation.PARTICLES);

      /// <summary>
      ///    returns true if the formulation is defining a dissolved formulation
      /// </summary>
      public bool IsDissolved => string.Equals(FormulationType, CoreConstants.Formulation.DISSOLVED);
      
      /// <summary>
      /// Returns <c>true</c> if the formulation is particle formulation using monodisperse system otherwise <c>false</c>
      /// </summary>
      public bool IsMonodisperse => isParticleDisperseSystem(CoreConstants.Parameters.MONODISPERSE);

      /// <summary>
      /// Returns <c>true</c> if the formulation is particle formulation using polydisperse system otherwise <c>false</c>
      /// </summary>
      public bool IsPolydisperse => isParticleDisperseSystem(CoreConstants.Parameters.POLYDISPERSE);

      /// <summary>
      /// Returns <c>true</c> if the formulation is a poly disperse particle formulation using a normal distribution for particle size otherwise <c>false</c>
      /// </summary>
      public bool IsPolydisperseNormal => isPolydisperseWithDistribution(CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION_NORMAL);

      /// <summary>
      /// Returns <c>true</c> if the formulation is a poly disperse particle formulation using a lognormal distribution for particle size otherwise <c>false</c>
      /// </summary>
      public bool IsPolydisperseLogNormal => isPolydisperseWithDistribution(CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION_LOG_NORMAL);

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
      public virtual bool HasRoute(string route) => _allRoutes.Contains(route);

      /// <summary>
      /// Updates visibility and default state of parameters based on disperse system and particle size (only for Particle Dissolution formulation)
      /// </summary>
      public virtual void UpdateParticleParametersVisibility()
      {
         if (!IsParticleDissolution)
            return;

         //First ensure all parameters are visible. 
         this.AllParameters().Each(showParticleParameter);

         //Then hide parameters based on current selection
         particleParametersToHide.Select(this.Parameter).Each(hideParticleParameter);
      }


      private IEnumerable<string> particleParametersToHide
      {
         get
         {
            if (IsMonodisperse)
               return CoreConstants.Parameters.HiddenParameterForMonodisperse;

            if(IsPolydisperseNormal)
               return CoreConstants.Parameters.HiddenParameterForPolydisperseNormal;

            if (IsPolydisperseLogNormal)
               return CoreConstants.Parameters.HiddenParameterForPolydisperseLogNormal;

            return Enumerable.Empty<string>();
         }
      }
      private void showParticleParameter(IParameter parameter) => updateParticleParameterVisibility(parameter, visible: true);

      private void hideParticleParameter(IParameter parameter) => updateParticleParameterVisibility(parameter, visible: false);

      private void updateParticleParameterVisibility(IParameter parameter, bool visible)
      {
         if (parameter == null)
            return;

         parameter.Visible = visible;
         //Visible parameter should be considered input.
         parameter.IsDefault = !visible;
      }

      private bool isParticleDisperseSystem(int particleDisperseSystem) => IsParticleDissolution && this.Parameter(CoreConstants.Parameters.PARTICLE_DISPERSE_SYSTEM).Value == particleDisperseSystem;

      private bool isPolydisperseWithDistribution(int particleSizeDistribution) => IsPolydisperse && this.Parameter(CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION).Value == particleSizeDistribution;

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