using System;
using System.Text.Json;

/*  ConfigFileMigration

    This sample is to show off best practices and example usage when working with a config file.
    In this example, we'll be using json for our file format, but it can be any format you choose (yaml, toml, etc)
    For our Json parser, we'll be using .Net 5's System.Text.Json library

    As with all migrations, we're going to go over examples for:
    1) Adding new data
    2) Removing data
    3) Modifying existing data values
    4) Renaming data keys
*/

namespace ConfigFileMigration
{
    interface DataMigrator
    {

    }

    public abstract class MigrationStep<VersionType, DataType> where VersionType : System.IComparable<VersionType>
    {
        public abstract VersionType from { get; }
        public abstract VersionType to { get; }
        public abstract bool Run(DataType input);
    }

    public class MyVersion : IComparable<MyVersion>
    {
        public MyVersion(int version) => m_value = version;

        public int CompareTo(MyVersion other)
        {
            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;

            // The temperature comparison depends on the comparison of
            // the underlying Double values.
            return m_value.CompareTo(other.m_value);
        }

        protected int m_value = 0;

        public static implicit operator MyVersion(int value) => new MyVersion(value);
    }

    public abstract class ConfigMigrationStep : MigrationStep<MyVersion, JsonDocument> {}

    public class ConfigMigration_1_4_3_to_1_5_0 : ConfigMigrationStep
    {
        public override MyVersion from => 1;
        public override MyVersion to => 2;
        public override bool Run(JsonDocument input)
        {
            return true;
        }
    }

    class ConfigDataMigrator 
    {
    }

    class Program
    {
        static void Main(string[] args)
        {
            new ConfigDataMigrator().Run();
        }
    }
}
