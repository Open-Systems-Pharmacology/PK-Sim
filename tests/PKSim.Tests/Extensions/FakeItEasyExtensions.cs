using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasy.Configuration;

namespace PKSim.Extensions
{
   public static class FakeItEasyExtensions
   {
      public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<Task<TResult>>> ReturnsAsync<TResult>(this IReturnValueConfiguration<Task<TResult>> valueConfiguration, TResult value)
      {
         return valueConfiguration.Returns(Task.FromResult(value));
      }
   }
}