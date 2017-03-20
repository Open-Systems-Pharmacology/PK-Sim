using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ClassificationXmlSerializer : ContextForIntegration<ClassificationXmlSerializer>
   {
      protected ISerializationManager _serializationManager;

      protected override void Context()
      {
         _serializationManager = IoC.Resolve<ISerializationManager>();
      }
   }

   public class When_serializing_a_classification_structure : concern_for_ClassificationXmlSerializer
   {
      private Classification _parent;
      private Classification _child;
      private PKSimProject _project;
      private IReadOnlyCollection<IClassification> _allDeserializedClassifications;

      protected override void Context()
      {
         base.Context();
         _parent = new Classification {Name = "Parent"};
         _child = new Classification {Name = "Child", Parent = _parent};
         _project = new PKSimProject();
         _project.AddClassification(_parent);
         _project.AddClassification(_child);
      }

      protected override void Because()
      {
         //needs to serialize a project in order to test the parent relationship
         var stream = _serializationManager.Serialize(_project);
         var deserialized = _serializationManager.Deserialize<PKSimProject>(stream);
         _allDeserializedClassifications = deserialized.AllClassifications;
      }

      [Observation]
      public void should_be_able_to_deserialize_the_same_structure()
      {
         _allDeserializedClassifications.Count.ShouldBeEqualTo(2);
         var parent = _allDeserializedClassifications.First((x => x.Name == "Parent"));
         var child = _allDeserializedClassifications.First(x => x.Name == "Child");
         child.Parent.ShouldBeEqualTo(parent);
         parent.Parent.ShouldBeNull();
      }
   }
}