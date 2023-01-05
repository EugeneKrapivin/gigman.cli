using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigyaManagement.Core.Exceptions;

[Serializable]
public class ContextNotSetException : Exception
{
	public ContextNotSetException() { }
	public ContextNotSetException(string message) : base(message) { }
	public ContextNotSetException(string message, Exception inner) : base(message, inner) { }
	protected ContextNotSetException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}