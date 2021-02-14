using System;
using System.IO;

namespace Drifter.Specializations
{
    public class ConfigMigrationStep : DataMigrationStep<int, FileInfo>
    {
        public ConfigMigrationStep(int from, int to, Func<FileInfo, bool> run, int cost = 1)
            : base(from, to, run, cost) { }
    }

    public class ConfigDataMigrator : DataMigrator<ConfigMigrationStep, int, FileInfo>
    {
        public bool SafeMode { get; set; } = true;

        public bool Reversible { get; set; } = true;

        public override bool RunMigration(FileInfo input)
        {
            return true;
        }
    }
}