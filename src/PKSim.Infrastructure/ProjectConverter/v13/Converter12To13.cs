using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using CoreConverter121To130 = OSPSuite.Core.Converters.v13.Converter121To130;

namespace PKSim.Infrastructure.ProjectConverter.v13
{
   public class Converter12To13 : IObjectConverter,
      IVisitor<Simulation>,
      IVisitor<IUsingFormula>,
      IVisitor<EventAssignment>,
      IVisitor<IParameter>
   {
      private readonly CoreConverter121To130 _coreConverter;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private bool _converted;

      //name of the protocol container and name of the application container that was moved under a "No formulation" container
      private readonly HashSet<(string protocolName, string applicationName)> _movedApplications = new HashSet<(string, string)>();

      public Converter12To13(CoreConverter121To130 coreConverter, IObjectBaseFactory objectBaseFactory)
      {
         _coreConverter = coreConverter;
         _objectBaseFactory = objectBaseFactory;
      }

      // 13.0 is not compatible with 12.0, but you don't need an explicit conversion to move forward.
      // To satisfy the next converter, the object must pass through v13.0 conversion
      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V12;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         (_, bool coreConverted) = _coreConverter.Convert(objectToConvert);
         this.Visit(objectToConvert);
         return (ProjectVersions.V13, _converted || coreConverted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         (_, bool converted) = _coreConverter.ConvertXml(element);
         return (ProjectVersions.V13, converted);
      }

      public void Visit(Simulation simulation) => nestApplicationsUnderFormulationContainer(simulation);

      /// <summary>
      ///    Applications of a protocol that does not require a formulation (e.g. IV Bolus) used to be added directly under the
      ///    protocol container, while applications with a formulation were nested one level deeper. They are now always nested
      ///    under a formulation container named "No formulation" so that the protocol hierarchy has a consistent depth.
      /// </summary>
      private void nestApplicationsUnderFormulationContainer(Simulation simulation)
      {
         var root = simulation?.Model?.Root;
         if (root == null)
            return;

         _movedApplications.Clear();

         //an application that is already nested under a formulation container does not need to be converted
         root.GetAllChildren<IContainer>(x => x.ContainerType == ContainerType.Application)
            .Where(x => x.ParentContainer != null && x.ParentContainer.ContainerType != ContainerType.Formulation)
            .GroupBy(x => x.ParentContainer)
            .ToList()
            .Each(x => nestApplicationsOf(x.Key, x.ToList()));

         if (!_movedApplications.Any())
            return;

         //the applications are now one level deeper: all paths referencing them have to be updated accordingly
         root.AcceptVisitor(this);
         _converted = true;
      }

      private void nestApplicationsOf(IContainer protocolContainer, IReadOnlyList<IContainer> applications)
      {
         var noFormulationContainer = _objectBaseFactory.Create<IContainer>()
            .WithName(CoreConstants.ContainerName.NoFormulation);

         noFormulationContainer.ContainerType = ContainerType.Formulation;

         applications.Each(application =>
         {
            protocolContainer.RemoveChild(application);
            noFormulationContainer.Add(application);
            _movedApplications.Add((protocolContainer.Name, application.Name));
         });

         protocolContainer.Add(noFormulationContainer);
      }

      public void Visit(IUsingFormula entityUsingFormula) => insertNoFormulationInPathsOf(entityUsingFormula.Formula);

      public void Visit(IParameter parameter)
      {
         Visit((IUsingFormula) parameter);
         insertNoFormulationInPathsOf(parameter.RHSFormula);
      }

      public void Visit(EventAssignment eventAssignment)
      {
         Visit((IUsingFormula) eventAssignment);
         insertNoFormulationIn(eventAssignment.ObjectPath);
      }

      private void insertNoFormulationInPathsOf(IFormula formula)
      {
         if (formula == null || formula.IsConstant())
            return;

         formula.ObjectPaths.Each(insertNoFormulationIn);
      }

      /// <summary>
      ///    Inserts the "No formulation" container between the protocol and the application whenever the given path goes
      ///    through an application that was moved. Applying this twice on the same path is a no-op.
      /// </summary>
      private void insertNoFormulationIn(ObjectPath objectPath)
      {
         if (objectPath == null)
            return;

         var path = objectPath.ToList();
         var convertedPath = new List<string>();

         for (var i = 0; i < path.Count; i++)
         {
            convertedPath.Add(path[i]);
            if (i + 1 < path.Count && _movedApplications.Contains((path[i], path[i + 1])))
               convertedPath.Add(CoreConstants.ContainerName.NoFormulation);
         }

         if (convertedPath.Count != path.Count)
            objectPath.ReplaceWith(convertedPath);
      }
   }
}
