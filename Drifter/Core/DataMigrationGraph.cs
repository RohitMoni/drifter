using System;
using System.Collections.Generic;

namespace Drifter
{
    internal class DataMigrationGraph<MigrationStepType, VersionType, InputType>
        where MigrationStepType : DataMigrationStep<VersionType, InputType>
    {
        internal Dictionary<VersionType, List<MigrationStepType>> graph = new Dictionary<VersionType, List<MigrationStepType>>();

        // Throws an ArgumentException if there is a migration step that already exists for that specific directed edge of the graph
        internal void RegisterMigrationStep(MigrationStepType migrationStep)
        {
            var key = migrationStep.From;
            if (!graph.ContainsKey(key))
            {
                graph.Add(key, new List<MigrationStepType>() { migrationStep });
            }
            else
            {
                foreach (var step in graph[key])
                {
                    if (step.To.Equals(migrationStep.To))
                    {
                        throw new ArgumentException($"There is already a migration step registered to migrate from {migrationStep.From} to {migrationStep.To}");
                    }
                }

                graph[key].Add(migrationStep);
            }
        }

        private class PathNode
        {
            internal int totalCost = 0;
            internal MigrationStepType migrationStep = null;
        }

        // Throws an InvalidOperationException if there is no possible path between the versions specified
        internal List<MigrationStepType> GetShortestMigrationPath(VersionType from, VersionType to)
        {
            var visitedPathNodes = new Dictionary<VersionType, PathNode>();

            Action<VersionType, MigrationStepType, int> traverse = null;

            traverse = (v, migrationStep, cost) =>
            {            
                if (!visitedPathNodes.ContainsKey(v))
                {
                    visitedPathNodes.Add(v, new PathNode { totalCost = cost, migrationStep = migrationStep });
                    if (v.Equals(to)) // Our current version is the desired version
                        return;

                    if (graph.ContainsKey(v)) {
                        foreach (var migration in graph[v])
                        {
                            traverse(migration.To, migration, cost + migration.Cost);
                        }
                    }
                }
                else if (visitedPathNodes[v].totalCost > cost)
                {
                    visitedPathNodes[v].migrationStep = migrationStep;
                    visitedPathNodes[v].totalCost = cost;
                }
            };

            traverse(from, null, 0);

            // Graph traversal done

            // If we never visited the 'to' version, means that no path to it exists
            if (!visitedPathNodes.ContainsKey(to))
                throw new InvalidOperationException($"No migration path defined from version {from} to version {to}");

            // Trace visited nodes backwards to find our shortest path
            var shortestMigrationPath = new List<MigrationStepType>();

            var step = visitedPathNodes[to].migrationStep;
            while (step != null)
            {
                shortestMigrationPath.Add(step);
                step = visitedPathNodes[step.From].migrationStep;
            }

            shortestMigrationPath.Reverse();
            return shortestMigrationPath;
        }
    }
}