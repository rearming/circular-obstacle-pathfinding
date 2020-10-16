using System;
using System.Collections.Generic;

namespace Pathfinding
{         
	public interface IGraph<T> where T : IEquatable<T>
	{
		int Cost(Node<T> current, Node<T> next);
		List<Node<T>> Neighbors(Node<T> current);
	}
}