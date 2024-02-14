using System.Collections.Generic;

namespace AurumGames.CompositeRoot
{
	public interface IMultipleRegistrable
	{
		
	}

	public interface IMultipleDependency<out T> : IReadOnlyList<T> 
		where T : IMultipleRegistrable
	{
		
	}

	internal class MultipleRegistrableList<T> : List<T>, IMultipleDependency<T>
		where T : IMultipleRegistrable
	{
		
	}
}