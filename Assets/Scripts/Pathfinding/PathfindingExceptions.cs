using System;

namespace Pathfinding
{
	[Serializable]
	public class SmallPathException : Exception
	{
		public SmallPathException()
		{
		}

		public SmallPathException(string message) : base(message)
		{
		}

		public SmallPathException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	[Serializable]
	public class IncompletePathException : Exception
	{
		public readonly int nodesNum;

		public IncompletePathException()
		{
		}

		public IncompletePathException(string message) : base(message)
		{
		}

		public IncompletePathException(string message, int nodesNum) : base(
			$"{message} On node [{nodesNum.ToString()}].")
		{
			this.nodesNum = nodesNum;
		}

		public IncompletePathException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
	
	[Serializable]
	public class InvalidCircleInfoException : Exception
	{
		public InvalidCircleInfoException()
		{
		}

		public InvalidCircleInfoException(string message) : base(message)
		{
		}

		public InvalidCircleInfoException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}