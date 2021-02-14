using Drifter.Specializations;
using System;
using System.IO;
using Xunit;

namespace Drifter.Tests
{
    public class DataMigrationGraphTests
    {
        DataMigrationGraph<ConfigMigrationStep, int, FileInfo> dummyMigrationGraph;

        public DataMigrationGraphTests()
        {
            dummyMigrationGraph = new DataMigrationGraph<ConfigMigrationStep, int, FileInfo>();
        }

        [Fact]
        public void TestRegisterMigrationStep()
        {
            dummyMigrationGraph.RegisterMigrationStep(
                new ConfigMigrationStep(1, 2, null)
            );

            Assert.Equal(dummyMigrationGraph.graph.Count, 1);
        }

        [Fact]
        public void TestRegisterMigrationSteps()
        {
            dummyMigrationGraph.RegisterMigrationStep(
                new ConfigMigrationStep(1, 2, null)
            );
            dummyMigrationGraph.RegisterMigrationStep(
                new ConfigMigrationStep(2, 3, null)
            );
            dummyMigrationGraph.RegisterMigrationStep(
                new ConfigMigrationStep(3, 4, null)
            );
            dummyMigrationGraph.RegisterMigrationStep(
                new ConfigMigrationStep(4, 5, null)
            );

            Assert.Equal(dummyMigrationGraph.graph.Count, 4);
        }

        [Fact]
        public void TestCannotRegisterDuplicateMigrationSteps()
        {
            bool caughtInvalidStep = false;

            try 
            {
                dummyMigrationGraph.RegisterMigrationStep(
                    new ConfigMigrationStep(1, 2, null)
                );
                dummyMigrationGraph.RegisterMigrationStep(
                    new ConfigMigrationStep(1, 2, null)
                );
            }
            catch (ArgumentException)
            {
                caughtInvalidStep = true;
            }
            finally
            {
                Assert.True(caughtInvalidStep);
            }
        }

        [Fact]
        public void TestShortestMigrationPath()
        {

        }

        [Fact]
        public void TestShortestMigrationPath2()
        {

        }

        [Fact]
        public void TestShortestMigrationPath3()
        {

        }
    }
}
