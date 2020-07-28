using System.Configuration;

public class Config : ConfigurationSection
{
    private static Config settings = ConfigurationManager.GetSection("Config") as Config;

    [ConfigurationProperty("params")]
    public ConfigCollection Settings
    {
        get { return ((ConfigCollection)(base["params"])); }
    }
}

public class ConfigElement : ConfigurationElement
{
    [ConfigurationProperty("path")]
    public string Path
    {
        get { return (string)(base["path"]); }
        set { base["path"] = value; }
    }

    [ConfigurationProperty("limit")]
    public int Limit
    {
        get { return ((int)(base["limit"])); }
        set { base["limit"] = value; }
    }

    [ConfigurationProperty("interval")]
    public int Interval
    {
        get { return ((int)(base["interval"])); }
        set { base["interval"] = value; }
    }

    [ConfigurationProperty("type")]
    public string Type
    {
        get { return ((string)(base["type"])); }
        set { base["type"] = value; }
    }
}


[ConfigurationCollection(typeof(ConfigElement))]
public class ConfigCollection : ConfigurationElementCollection
{
    protected override ConfigurationElement CreateNewElement()
    {
        return new ConfigElement();
    }
    protected override object GetElementKey(ConfigurationElement element)
    {
        return ((ConfigElement)element).Path;
    }
    public ConfigElement this[int idx]
    {
        get { return (ConfigElement)BaseGet(idx); }
    }
}


