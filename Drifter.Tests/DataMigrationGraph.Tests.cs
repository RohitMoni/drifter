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

            Assert.Single(dummyMigrationGraph.graph);
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

            Assert.Equal(4, dummyMigrationGraph.graph.Count);
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

            var shortestPath = dummyMigrationGraph.GetShortestMigrationPath(1, 5);

            Assert.Equal(4, shortestPath.Count);
            Assert.Equal(1, shortestPath[0].From);
            Assert.Equal(2, shortestPath[0].To);
            Assert.Equal(2, shortestPath[1].From);
            Assert.Equal(3, shortestPath[1].To);
            Assert.Equal(3, shortestPath[2].From);
            Assert.Equal(4, shortestPath[2].To);
            Assert.Equal(4, shortestPath[3].From);
            Assert.Equal(5, shortestPath[3].To);
        }

        [Fact]
        public void TestShortestMigrationPath2()
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
                new ConfigMigrationStep(1, 4, null)
            );

            var shortestPath = dummyMigrationGraph.GetShortestMigrationPath(1, 4);

            Assert.Single(shortestPath);
            Assert.Equal(1, shortestPath[0].From);
            Assert.Equal(4, shortestPath[0].To);
        }

        [Fact]
        public void TestShortestMigrationPath3()
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
                new ConfigMigrationStep(2, 4, null)
            );

            var shortestPath = dummyMigrationGraph.GetShortestMigrationPath(1, 4);

            Assert.Equal(2, shortestPath.Count);
            Assert.Equal(1, shortestPath[0].From);
            Assert.Equal(2, shortestPath[0].To);
            Assert.Equal(2, shortestPath[1].From);
            Assert.Equal(4, shortestPath[1].To);
        }

        [Fact]
        public void TestShortestMigrationPathCycle()
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
                new ConfigMigrationStep(2, 1, null)
            );

            var shortestPath = dummyMigrationGraph.GetShortestMigrationPath(1, 4);

            Assert.Equal(3, shortestPath.Count);
            Assert.Equal(1, shortestPath[0].From);
            Assert.Equal(2, shortestPath[0].To);
            Assert.Equal(2, shortestPath[1].From);
            Assert.Equal(3, shortestPath[1].To);
            Assert.Equal(3, shortestPath[2].From);
            Assert.Equal(4, shortestPath[2].To);
        }

        [Fact]
        public void TestNoMigrationPath()
        {
            dummyMigrationGraph.RegisterMigrationStep(
                new ConfigMigrationStep(1, 2, null)
            );

            bool caughtNoMigrationPath = false;

            try 
            {
                var shortestPath = dummyMigrationGraph.GetShortestMigrationPath(1, 4);
            }
            catch (InvalidOperationException)
            {
                caughtNoMigrationPath = true;
            }
            finally
            {
                Assert.True(caughtNoMigrationPath);
            }
        }
    }
}
