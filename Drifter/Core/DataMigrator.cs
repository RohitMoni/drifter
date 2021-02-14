using System.Collections.Generic;

namespace Drifter
{
    public abstract class DataMigrator<MigrationStepType, VersionType, InputType>
        where MigrationStepType : DataMigrationStep<VersionType, InputType>
    {
        private DataMigrationGraph<MigrationStepType, VersionType, InputType> migrationGraph = new DataMigrationGraph<MigrationStepType, VersionType, InputType>();

        public void RegisterMigrationStep(MigrationStepType migrationStep)
        {
            migrationGraph.RegisterMigrationStep(migrationStep);
        }

        internal List<MigrationStepType> GetShortestMigrationPath(VersionType from, VersionType to)
        {
            return migrationGraph.GetShortestMigrationPath(from, to);
        }

        public abstract bool RunMigration(InputType input);
    }
}