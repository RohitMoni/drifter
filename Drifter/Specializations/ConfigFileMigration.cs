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
        // Modifies a separate file rather than the original before replacing it
        public bool SafeMode { get; set; } = true;

        // Ensures a backup of the original file is maintained so that the migration can be rolled back
        public bool Reversible { get; set; } = true;

        // Overrides the output file name of migrated config file
        public string OutputFileNameOverride { get; set; } = null;

        public override bool RunMigration(FileInfo input, int from, int to)
        {
            FileInfo configFile = input;

            var migrationPath = GetShortestMigrationPath(from, to);

            FileInfo outputFile = configFile;
            if (OutputFileNameOverride != null)
            {
                outputFile = new FileInfo(Path.Combine(configFile.DirectoryName, $"{OutputFileNameOverride}{configFile.Extension}"));
                File.Copy(configFile.FullName, outputFile.FullName);
            }

            if (Reversible)
            {
                File.Copy(configFile.FullName, $"{configFile.FullName}.rollbacktarget");
            }

            FileInfo operatingFile = outputFile;
            if (SafeMode)
            {
                File.Copy(operatingFile.FullName, $"{operatingFile.FullName}.migrating");
                operatingFile = new FileInfo($"{operatingFile.FullName}.migrating");
            }

            for (int i = 0; i < migrationPath.Count; ++i) 
            {
                var stepSuccess = migrationPath[i].Run(operatingFile);
                if (!stepSuccess)
                {
                    return false;
                }
            }

            if (SafeMode)
            {
                File.Move(operatingFile.FullName, outputFile.FullName, true);
                File.Delete(operatingFile.FullName);
            }

            if (OutputFileNameOverride != null)
            {
                File.Delete(configFile.FullName);
            }

            return true;
        }

        public void RollbackMigration(FileInfo input)
        {
            var rollbackTarget = new FileInfo($"{input.FullName}.rollbacktarget");
            if (File.Exists(rollbackTarget.FullName))
            {
                File.Move(rollbackTarget.FullName, input.FullName, true);
            }
            else
            {
                throw new InvalidOperationException("No rollback target for this file exists, cannot rollback");
            }
        }
    }
}