using System.Configuration;

namespace Zinc.WebServices.Journalling
{
    public partial class JournallingConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty( "methods", IsRequired = true, IsDefaultCollection = true )]
        public ConfigurationElementCollection<JournallingConfig> Methods
        {
            get { return (ConfigurationElementCollection<JournallingConfig>) this[ "methods" ]; }
            set { this[ "methods" ] = value; }
        }
    }



    public partial class JournallingConfig : ConfigurationElement
    {
        [ConfigurationProperty( "action", IsKey = true, IsRequired = true )]
        public string Action
        {
            get { return (string) base[ "action" ]; }
            set { base[ "action" ] = value; }
        }

        [ConfigurationProperty( "enabled", DefaultValue = true )]
        public bool Enabled
        {
            get { return (bool) base[ "enabled" ]; }
            set { base[ "enabled" ] = value; }
        }

        [ConfigurationProperty( "request" )]
        public JournallingMessageConfig Request
        {
            get { return (JournallingMessageConfig) base[ "request" ]; }
            set { base[ "request" ] = value; }
        }

        [ConfigurationProperty( "response" )]
        public JournallingMessageConfig Response
        {
            get { return (JournallingMessageConfig) base[ "response" ]; }
            set { base[ "response" ] = value; }
        }
    }



    public partial class JournallingMessageConfig : ConfigurationElement
    {
        [ConfigurationProperty( "journal", DefaultValue = true )]
        public bool Journal
        {
            get { return (bool) base[ "journal" ]; }
            set { base[ "journal" ] = value; }
        }


        [ConfigurationProperty( "", IsRequired = true, IsDefaultCollection = true )]
        public ConfigurationElementCollection<JournallingMessageSecretConfig> Secrets
        {
            get { return (ConfigurationElementCollection<JournallingMessageSecretConfig>) this[ "" ]; }
            set { this[ "" ] = value; }
        }
    }



    public partial class JournallingMessageSecretConfig : ConfigurationElement
    {
        [ConfigurationProperty( "secret", IsRequired = true )]
        public string Expression
        {
            get { return (string) base[ "secret" ]; }
            set { base[ "secret" ] = value; }
        }
    }
}
