using System;

namespace Drifter
{
    public class DataMigrationStep<VersionType, InputType>
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
}
