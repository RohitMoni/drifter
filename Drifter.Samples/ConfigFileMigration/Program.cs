﻿using System;
using System.IO;
using System.Collections.Generic;

/*  ConfigFileMigration

    This sample is to show off best practices and example usage when working with a config file.
    
    As with all migrations, we're going to go over examples for:
    1) Adding new data
    2) Removing data
    3) Modifying existing data values
    4) Renaming data keys

    Assumptions:
    1) We're working with a single config file
    2) The config file is json
    3) Our versioning uses a simple integer number
*/

namespace ConfigFileMigration
{
    public class DataMigrationStep<VersionType, InputType> 
        where VersionType : System.IComparable<VersionType>, System.IEquatable<VersionType>
    {
        public DataMigrationStep(VersionType from, VersionType to, Func<InputType, bool> run, int cost = 1)
        {
            From = from;
            To = to;
            Run = run;
            Cost = cost;
        }

        // What version does this step migrate from
        public VersionType From { get; init; }

        // What version does this step migrate to
        public VersionType To { get; init; }

        // Lambda / Function that actually does the migration
        public Func<InputType, bool> Run;

        // Optional: Cost of this migration step. Can be used to set up more efficient migration paths. 
        // Default is all migration steps have the same cost
        public int Cost { get; init; } = 1;
    }

    internal class MigrationGraph<MigrationStepType, VersionType, InputType>
        where MigrationStepType : DataMigrationStep<VersionType, InputType>
        where VersionType : System.IComparable<VersionType>, System.IEquatable<VersionType>
    {
        private Dictionary<VersionType, List<MigrationStepType>> graph;

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

    public abstract class DataMigrator<MigrationStepType, VersionType, InputType> 
        where MigrationStepType : DataMigrationStep<VersionType, InputType> 
        where VersionType : System.IComparable<VersionType>, System.IEquatable<VersionType>
    {
        private MigrationGraph<MigrationStepType, VersionType, InputType> migrationGraph;

        public void RegisterMigrationStep(MigrationStepType migrationStep)
        {
            migrationGraph.RegisterMigrationStep(migrationStep);
        }

        public abstract bool RunMigration(InputType input);
    }

    public class VersionNumber : IComparable<VersionNumber>, IEquatable<VersionNumber>
    {
        public VersionNumber(int version) => m_value = version;

        public int CompareTo(VersionNumber other)
        {
            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;

            // The temperature comparison depends on the comparison of
            // the underlying Double values.
            return m_value.CompareTo(other.m_value);
        }

        public bool Equals(VersionNumber other)
        {
            if (other == null)
                return false;

            if (this.m_value == other.m_value)
                return true;
            else
                return false;
        }

        protected int m_value = 0;

        public static implicit operator VersionNumber(int value) => new VersionNumber(value);
    }

    public class ConfigMigrationStep : DataMigrationStep<VersionNumber, FileInfo>
    {
        public ConfigMigrationStep(VersionNumber from, VersionNumber to, Func<FileInfo, bool> run, int cost = 1)
            : base(from, to, run, cost) {}
    }

    public class ConfigDataMigrator : DataMigrator<ConfigMigrationStep, VersionNumber, FileInfo> 
    {
        public bool SafeMode { get; set; } = true;

        public bool Reversible { get; set; } = true;

        public override bool RunMigration(FileInfo input)
        {
            return true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var configDataMigrator = new ConfigDataMigrator();
            configDataMigrator.RegisterMigrationStep(
                new ConfigMigrationStep(1, 2, 
                    fileInfo => {
                        return true;
                    }
                )
            );

            var configFileInfo = new FileInfo(@"Data/premigrationConfig.json");
            configDataMigrator.RunMigration(configFileInfo);
        }
    }
}
