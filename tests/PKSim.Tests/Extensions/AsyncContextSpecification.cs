using System;
using System.Threading.Tasks;
using NUnit.Framework;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.BDDHelper
{
   /// <summary>
   ///    Defines a static context specification used for static class or extensions methods
   /// </summary>
   [TestFixture]
   public abstract class ContextSpecificationAsync<T>
   {
      /// <summary>system under test</summary>
      protected T sut;

      protected Task _completed = Task.FromResult(true);

      /// <summary>
      ///    The Global context of the test (e.g. TestFixtureSetUp). This function will be called only once
      /// </summary>
      [OneTimeSetUp]
      public virtual Task GlobalContext()
      {
         return _completed;
      }

      /// <summary>calls the context and the because at once</summary>
      [SetUp]
      public async Task Setup()
      {
         await Context();
         await Because();
      }

      /// <summary>
      ///    The context of the test (e.g. SetUp). This function will be call for each observation
      /// </summary>
      protected virtual Task Context()
      {
         return _completed;
      }

      /// <summary>The actual action tested in the specification</summary>
      protected virtual Task Because()
      {
         return _completed;
      }

      /// <summary>
      ///    Cleanup function to release objects if necessary (e.g. TearDown). This function is called after each observation
      /// </summary>
      [TearDown]
      public virtual Task Cleanup()
      {
         return _completed;
      }

      /// <summary>
      ///    Global Cleanup function to release objects if necessary (e.g. TestFixtureTearDown). This function is called once
      ///    after all observations were tested
      /// </summary>
      [OneTimeTearDown]
      public virtual Task GlobalCleanup()
      {
         return _completed;
      }
   }

   /// <summary>Useful class to assert that an exception is raised</summary>
   public static class TheAsync
   {
      /// <summary>
      ///    Returns the action given as parameter to enable a syntax such as The.Action(xx).ShouldTrowAn...
      /// </summary>
      public static Func<Task> Action(Func<Task> action)
      {
         return action;
      }
   }

   public static class BDDExtensionsAsync
   {
      /// <summary>
      ///    Asserts that <paramref name="workToPerform" /> throws an exception when performed.
      /// </summary>
      public static void ShouldThrowAnAsync<TException>(this Func<Task> workToPerform) where TException : Exception
      {
         try
         {
            workToPerform().Wait();
            true.ShouldBeFalse("Exception not thrown as expected");
         }
         catch (TException)
         {
            
         }
         catch (AggregateException ex)
         {
            ex.InnerExceptions.Find(e => e.IsAnImplementationOf<TException>()).ShouldNotBeEqualTo(null, $"Exception of type {typeof(TException)} not found");
         }
      }
   }
}