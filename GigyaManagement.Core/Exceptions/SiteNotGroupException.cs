namespace GigyaManagement.Core.Exceptions;

[Serializable]
public class ResourceLoadException : Exception
{
	public ResourceLoadException(string resource, string path) : base($"Failed to load {resource} at {path}") { }

	protected ResourceLoadException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

[Serializable]
public class SiteNotGroupException : Exception
{
	public SiteNotGroupException(string message) : base(message) { }
	protected SiteNotGroupException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
