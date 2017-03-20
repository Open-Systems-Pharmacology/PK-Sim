using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Nodes;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation
{
   public abstract class concern_for_SystemicProcessToRootNodeTypeMapper : ContextSpecification<SystemicProcessToRootNodeTypeMapper>
   {
      protected SystemicProcessType _processType;
      protected RootNodeType _result;

      protected override void Context()
      {
         sut = new SystemicProcessToRootNodeTypeMapper();
      }
   }

   public class When_process_type_is_GFR : concern_for_SystemicProcessToRootNodeTypeMapper
   {
      protected override void Because()
      {
         _result = sut.MapFrom(SystemicProcessTypes.GFR);
      }

      [Observation]
      public void root_node_type_should_be_renal()
      {
         _result.ShouldBeEqualTo(SystemicProcessNodeType.RenalClearance);
      }
   }

   public class When_process_type_is_Renal : concern_for_SystemicProcessToRootNodeTypeMapper
   {
      protected override void Because()
      {
         _result = sut.MapFrom(SystemicProcessTypes.Renal);
      }

      [Observation]
      public void root_node_type_should_be_renal()
      {
         _result.ShouldBeEqualTo(SystemicProcessNodeType.RenalClearance);
      }
   }

   public class When_process_type_is_Hepatic : concern_for_SystemicProcessToRootNodeTypeMapper
   {
      protected override void Because()
      {
         _result = sut.MapFrom(SystemicProcessTypes.Hepatic);
      }

      [Observation]
      public void root_node_type_should_be_renal()
      {
         _result.ShouldBeEqualTo(SystemicProcessNodeType.HepaticClearance);
      }
   }

   public class When_process_type_is_Biliary : concern_for_SystemicProcessToRootNodeTypeMapper
   {
      protected override void Because()
      {
         _result = sut.MapFrom(SystemicProcessTypes.Biliary);
      }

      [Observation]
      public void root_node_type_should_be_renal()
      {
         _result.ShouldBeEqualTo(SystemicProcessNodeType.BiliaryClearance);
      }
   }
}
