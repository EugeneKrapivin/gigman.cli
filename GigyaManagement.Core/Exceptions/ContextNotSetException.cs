using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigyaManagement.Core.Exceptions;

[Serializable]
public class ContextNotSetException : Exception
{
	public ContextNotSetException() 
		: base("Context is not set, please use the cli tool to set a context")
	{ }
	protected ContextNotSetException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}